using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;

public class Package : IDisposable
{
    public const int MAGIC = 1437209140;

    public const char DirectorySeparatorChar = '/';

    private BinaryReader Reader;

    private bool IsDirVPK;

    private uint HeaderSize;

    // 🔑 Lock para operaciones sobre el stream principal
    private readonly object _mainStreamLock = new object();

    // 🔑 Pool de streams para archivos externos (uno por thread)
    private readonly ConcurrentDictionary<int, ThreadLocal<FileStream>> _externalStreamPools
        = new ConcurrentDictionary<int, ThreadLocal<FileStream>>();

    // 🔑 Lista de todos los ThreadLocal para liberar al dispose
    private readonly List<ThreadLocal<FileStream>> _allThreadLocals = new List<ThreadLocal<FileStream>>();

    public string FileName { get; private set; }

    public uint Version { get; private set; }

    public uint TreeSize { get; private set; }

    public uint FileDataSectionSize { get; private set; }

    public uint ArchiveMD5SectionSize { get; private set; }

    public uint OtherMD5SectionSize { get; private set; }

    public uint SignatureSectionSize { get; private set; }

    public byte[] TreeChecksum { get; private set; }

    public byte[] ArchiveMD5EntriesChecksum { get; private set; }

    public byte[] WholeFileChecksum { get; private set; }

    public byte[] PublicKey { get; private set; }

    public byte[] Signature { get; private set; }

    public Dictionary<string, List<PackageEntry>> Entries { get; private set; }

    public List<ArchiveMD5SectionEntry> ArchiveMD5Entries { get; private set; }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (Reader != null)
            {
                Reader.Dispose();
                Reader = null;
            }

            // 🔑 Liberar todos los streams thread-local
            lock (_allThreadLocals)
            {
                foreach (var tl in _allThreadLocals)
                {
                    try
                    {
                        foreach (var fs in tl.Values)
                        {
                            fs?.Dispose();
                        }
                        tl.Dispose();
                    }
                    catch { }
                }
                _allThreadLocals.Clear();
            }
            _externalStreamPools.Clear();
        }
    }

    public void SetFileName(string fileName)
    {
        if (fileName.EndsWith(".vpk", StringComparison.Ordinal))
        {
            fileName = fileName.Substring(0, fileName.Length - 4);
        }
        if (fileName.EndsWith("_dir", StringComparison.Ordinal))
        {
            IsDirVPK = true;
            fileName = fileName.Substring(0, fileName.Length - 4);
        }
        FileName = fileName;
    }

    public void Read(string filename)
    {
        SetFileName(filename);
        FileStream input = new FileStream(
            string.Format("{0}{1}.vpk", FileName, IsDirVPK ? "_dir" : string.Empty),
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read);
        Read(input);
    }

    public void Read(Stream input)
    {
        if (FileName == null)
        {
            throw new InvalidOperationException("If you call Read() directly with a stream, you must call SetFileName() first.");
        }
        Reader = new BinaryReader(input);
        if (Reader.ReadUInt32() != 1437209140)
        {
            throw new InvalidDataException("Given file is not a VPK.");
        }
        Version = Reader.ReadUInt32();
        TreeSize = Reader.ReadUInt32();
        if (Version != 1)
        {
            if (Version != 2)
            {
                throw new InvalidDataException($"Bad VPK version. ({Version})");
            }
            FileDataSectionSize = Reader.ReadUInt32();
            ArchiveMD5SectionSize = Reader.ReadUInt32();
            OtherMD5SectionSize = Reader.ReadUInt32();
            SignatureSectionSize = Reader.ReadUInt32();
        }
        HeaderSize = (uint)input.Position;
        ReadEntries();
        if (Version == 2)
        {
            input.Position += FileDataSectionSize;
            ReadArchiveMD5Section();
            ReadOtherMD5Section();
            ReadSignatureSection();
        }
    }

    public PackageEntry FindEntry(string filePath)
    {
        filePath = filePath?.Replace('\\', '/');
        return FindEntry(Path.GetDirectoryName(filePath), filePath);
    }

    public PackageEntry FindEntry(string directory, string fileName)
    {
        fileName = fileName?.Replace('\\', '/');
        return FindEntry(directory, Path.GetFileNameWithoutExtension(fileName), Path.GetExtension(fileName)?.TrimStart('.'));
    }

    public PackageEntry FindEntry(string directory, string fileName, string extension)
    {
        if (extension == null)
        {
            extension = string.Empty;
        }
        if (!Entries.ContainsKey(extension))
        {
            return null;
        }
        directory = directory?.Replace('\\', '/').Trim('/');
        if (directory == string.Empty)
        {
            directory = null;
        }
        return Entries[extension].FirstOrDefault((PackageEntry x) => x.DirectoryName == directory && x.FileName == fileName);
    }

    // ════════════════════════════════════════════════════════════
    // 🚀 ReadEntry thread-safe
    // ════════════════════════════════════════════════════════════

    public void ReadEntry(PackageEntry entry, out byte[] output, bool validateCrc = true)
    {
        output = new byte[entry.SmallData.Length + entry.Length];

        if (entry.SmallData.Length != 0)
        {
            entry.SmallData.CopyTo(output, 0);
        }

        if (entry.Length != 0)
        {
            if (entry.ArchiveIndex != 32767)
            {
                // ════════════════════════════════════════════════
                // Archivo externo (pak01_001.vpk, etc.)
                // ════════════════════════════════════════════════
                if (!IsDirVPK)
                {
                    throw new InvalidOperationException("Given VPK is not a _dir, but entry is referencing an external archive.");
                }

                // 🔑 Cada thread tiene su propio FileStream para este archivo
                var stream = GetOrCreateExternalStream(entry.ArchiveIndex);

                lock (stream)
                {
                    stream.Seek(entry.Offset, SeekOrigin.Begin);
                    stream.Read(output, entry.SmallData.Length, (int)entry.Length);
                }
            }
            else
            {
                // ════════════════════════════════════════════════
                // Archivo principal (pak01_dir.vpk) - usa lock
                // ════════════════════════════════════════════════
                lock (_mainStreamLock)
                {
                    var stream = Reader.BaseStream;
                    uint num = entry.Offset + HeaderSize + TreeSize;
                    stream.Seek(num, SeekOrigin.Begin);
                    stream.Read(output, entry.SmallData.Length, (int)entry.Length);
                }
            }
        }

        if (validateCrc && entry.CRC32 != Crc32.Compute(output))
        {
            throw new InvalidDataException("CRC32 mismatch for read data.");
        }
    }

    /// <summary>
    /// Obtiene o crea un FileStream thread-local para un archivo VPK externo.
    /// Cada thread tiene su propio stream, evitando conflictos de Seek/Read.
    /// </summary>
    private FileStream GetOrCreateExternalStream(ushort archiveIndex)
    {
        var threadLocal = _externalStreamPools.GetOrAdd(archiveIndex, idx =>
        {
            var tl = new ThreadLocal<FileStream>(() =>
            {
                return new FileStream(
                    $"{FileName}_{idx:D3}.vpk",
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read);
            }, trackAllValues: true);

            // Registrar para dispose
            lock (_allThreadLocals)
            {
                _allThreadLocals.Add(tl);
            }

            return tl;
        });

        return threadLocal.Value;
    }

    private void ReadEntries()
    {
        Dictionary<string, List<PackageEntry>> dictionary = new Dictionary<string, List<PackageEntry>>();

        while (true)
        {
            string typeName = Reader.ReadNullTermString(Encoding.UTF8);
            if (typeName == string.Empty)
            {
                break;
            }

            List<PackageEntry> list = new List<PackageEntry>();
            while (true)
            {
                string directoryName = Reader.ReadNullTermString(Encoding.UTF8);
                if (directoryName == string.Empty)
                {
                    break;
                }
                if (directoryName == " ")
                {
                    directoryName = null;
                }

                while (true)
                {
                    string fileName = Reader.ReadNullTermString(Encoding.UTF8);
                    if (fileName == string.Empty)
                    {
                        break;
                    }

                    PackageEntry packageEntry = new PackageEntry
                    {
                        FileName = fileName,
                        DirectoryName = directoryName,
                        TypeName = typeName,
                        CRC32 = Reader.ReadUInt32(),
                        SmallData = new byte[Reader.ReadUInt16()],
                        ArchiveIndex = Reader.ReadUInt16(),
                        Offset = Reader.ReadUInt32(),
                        Length = Reader.ReadUInt32()
                    };
                    if (Reader.ReadUInt16() != ushort.MaxValue)
                    {
                        throw new FormatException("Invalid terminator.");
                    }
                    if (packageEntry.SmallData.Length != 0)
                    {
                        Reader.Read(packageEntry.SmallData, 0, packageEntry.SmallData.Length);
                    }
                    list.Add(packageEntry);
                }
            }
            dictionary.Add(typeName, list);
        }

        Entries = dictionary;
    }

    public void VerifyHashes()
    {
        if (Version != 2)
        {
            throw new InvalidDataException("Only version 2 is supported.");
        }
        using (MD5 mD = MD5.Create())
        {
            Reader.BaseStream.Position = 0L;
            byte[] array = mD.ComputeHash(Reader.ReadBytes((int)(HeaderSize + TreeSize + FileDataSectionSize + ArchiveMD5SectionSize + 32)));
            if (!array.SequenceEqual(WholeFileChecksum))
            {
                throw new InvalidDataException("Package checksum mismatch (" + BitConverter.ToString(array) + " != expected " + BitConverter.ToString(WholeFileChecksum) + ")");
            }
            Reader.BaseStream.Position = HeaderSize;
            array = mD.ComputeHash(Reader.ReadBytes((int)TreeSize));
            if (!array.SequenceEqual(TreeChecksum))
            {
                throw new InvalidDataException("File tree checksum mismatch (" + BitConverter.ToString(array) + " != expected " + BitConverter.ToString(TreeChecksum) + ")");
            }
            Reader.BaseStream.Position = HeaderSize + TreeSize + FileDataSectionSize;
            array = mD.ComputeHash(Reader.ReadBytes((int)ArchiveMD5SectionSize));
            if (!array.SequenceEqual(ArchiveMD5EntriesChecksum))
            {
                throw new InvalidDataException("Archive MD5 entries checksum mismatch (" + BitConverter.ToString(array) + " != expected " + BitConverter.ToString(ArchiveMD5EntriesChecksum) + ")");
            }
        }
        if (PublicKey == null || Signature == null || IsSignatureValid())
        {
            return;
        }
        throw new InvalidDataException("VPK signature is not valid.");
    }

    public bool IsSignatureValid()
    {
        Reader.BaseStream.Position = 0L;
        AsnKeyParser asnKeyParser = new AsnKeyParser(PublicKey);
        RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
        rSACryptoServiceProvider.ImportParameters(asnKeyParser.ParseRSAPublicKey());
        RSAPKCS1SignatureDeformatter rSAPKCS1SignatureDeformatter = new RSAPKCS1SignatureDeformatter(rSACryptoServiceProvider);
        rSAPKCS1SignatureDeformatter.SetHashAlgorithm("SHA256");
        byte[] rgbHash = new SHA256Managed().ComputeHash(Reader.ReadBytes((int)(HeaderSize + TreeSize + FileDataSectionSize + ArchiveMD5SectionSize + OtherMD5SectionSize)));
        return rSAPKCS1SignatureDeformatter.VerifySignature(rgbHash, Signature);
    }

    private void ReadArchiveMD5Section()
    {
        ArchiveMD5Entries = new List<ArchiveMD5SectionEntry>();
        if (ArchiveMD5SectionSize != 0)
        {
            uint num = ArchiveMD5SectionSize / 28;
            for (int i = 0; i < num; i++)
            {
                ArchiveMD5Entries.Add(new ArchiveMD5SectionEntry
                {
                    ArchiveIndex = Reader.ReadUInt32(),
                    Offset = Reader.ReadUInt32(),
                    Length = Reader.ReadUInt32(),
                    Checksum = Reader.ReadBytes(16)
                });
            }
        }
    }

    private void ReadOtherMD5Section()
    {
        if (OtherMD5SectionSize != 48)
        {
            throw new InvalidDataException($"Encountered OtherMD5Section with size of {OtherMD5SectionSize} (should be 48)");
        }
        TreeChecksum = Reader.ReadBytes(16);
        ArchiveMD5EntriesChecksum = Reader.ReadBytes(16);
        WholeFileChecksum = Reader.ReadBytes(16);
    }

    private void ReadSignatureSection()
    {
        if (SignatureSectionSize != 0)
        {
            long position = Reader.BaseStream.Position;
            if (Reader.ReadUInt32() == 1437209140)
            {
                Reader.ReadUInt32();
                Reader.ReadBytes(12);
                PublicKey = new byte[0];
                Signature = new byte[0];
            }
            else
            {
                Reader.BaseStream.Position = position;
                int count = Reader.ReadInt32();
                PublicKey = Reader.ReadBytes(count);
                int count2 = Reader.ReadInt32();
                Signature = Reader.ReadBytes(count2);
            }
        }
    }
}