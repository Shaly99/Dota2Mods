using System.Security.Cryptography;
using System.Text;

namespace SKYNET;

/// <summary>
/// Implementación del escritor de VPK.
/// Esta versión genera un VPK v2 con todo el contenido dentro de pak01_dir.vpk
/// (sin dividir en archivos _001, _002, etc.).
/// </summary>
public sealed class VpkWriterService : IVpkWriterService
{
    private readonly IUIService _ui;

    public VpkWriterService(IUIService ui)
    {
        _ui = ui ?? throw new ArgumentNullException(nameof(ui));
    }

    public string BuildFromFolder(string sourceFolder)
    {
        if (string.IsNullOrWhiteSpace(sourceFolder))
        {
            throw new ArgumentException("La carpeta de origen no puede estar vacía.", nameof(sourceFolder));
        }

        sourceFolder = Path.GetFullPath(sourceFolder);

        if (!Directory.Exists(sourceFolder))
        {
            throw new DirectoryNotFoundException($"No existe la carpeta de origen: {sourceFolder}");
        }

        string outputDirectory = Path.GetDirectoryName(sourceFolder)
            ?? throw new InvalidOperationException("No se pudo resolver el directorio de salida del VPK.");

        string outputFileName = Path.GetFileName(sourceFolder) + ".vpk";
        string outputPath = Path.Combine(outputDirectory, outputFileName);

        try
        {
            List<VpkSourceEntry> entries = CollectEntries(sourceFolder);

            if (entries.Count == 0)
            {
                throw new InvalidOperationException("No hay archivos para empaquetar en el VPK.");
            }

            byte[] treeBytes = BuildTreeBytes(entries);
            byte[] treeChecksum = MD5.HashData(treeBytes);

            // No generamos archivos externos, así que la sección de MD5 de archivos externos
            // queda vacía y su checksum es el MD5 de un buffer vacío.
            byte[] archiveMd5EntriesChecksum = MD5.HashData(Array.Empty<byte>());

            uint fileDataSectionSize = checked((uint)entries.Sum(x => (long)x.Length));
            byte[] headerBytes = BuildHeader((uint)treeBytes.Length, fileDataSectionSize);

            // Hash del archivo completo:
            // header + tree + data + archiveMD5Section + primeros 32 bytes de OtherMD5Section
            using IncrementalHash wholeFileHash = IncrementalHash.CreateHash(HashAlgorithmName.MD5);
            wholeFileHash.AppendData(headerBytes);
            wholeFileHash.AppendData(treeBytes);

            Directory.CreateDirectory(outputDirectory);

            using (FileStream output = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                // Header + tree
                output.Write(headerBytes, 0, headerBytes.Length);
                output.Write(treeBytes, 0, treeBytes.Length);

                // Data section
                byte[] buffer = new byte[1024 * 128];

                foreach (VpkSourceEntry entry in entries)
                {
                    using FileStream input = new FileStream(entry.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read);

                    int read;
                    while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        output.Write(buffer, 0, read);
                        wholeFileHash.AppendData(buffer, 0, read);
                    }
                }

                // Los primeros 32 bytes del OtherMD5Section forman parte del hash global
                byte[] otherMd5Prefix = new byte[32];
                Buffer.BlockCopy(treeChecksum, 0, otherMd5Prefix, 0, 16);
                Buffer.BlockCopy(archiveMd5EntriesChecksum, 0, otherMd5Prefix, 16, 16);
                wholeFileHash.AppendData(otherMd5Prefix);

                byte[] wholeFileChecksum = wholeFileHash.GetHashAndReset();

                // OtherMD5Section (48 bytes)
                output.Write(treeChecksum, 0, treeChecksum.Length);
                output.Write(archiveMd5EntriesChecksum, 0, archiveMd5EntriesChecksum.Length);
                output.Write(wholeFileChecksum, 0, wholeFileChecksum.Length);

                output.Flush();
            }

            return outputPath;
        }
        catch (Exception ex)
        {
            try
            {
                _ui.LogException("Error generando el VPK", ex);
            }
            catch
            {
                // Si el logger también falla, no bloqueamos la excepción original.
            }

            throw;
        }
    }

    // ════════════════════════════════════════════════════════════
    // RECOLECCIÓN DE ARCHIVOS
    // ════════════════════════════════════════════════════════════

    private static List<VpkSourceEntry> CollectEntries(string sourceFolder)
    {
        List<VpkSourceEntry> entries = new List<VpkSourceEntry>();

        foreach (string filePath in Directory.EnumerateFiles(sourceFolder, "*", SearchOption.AllDirectories))
        {
            string relativePath = Path.GetRelativePath(sourceFolder, filePath).Replace('\\', '/');
            string fileName = Path.GetFileNameWithoutExtension(relativePath);
            string extension = Path.GetExtension(relativePath).TrimStart('.');
            string? directoryName = Path.GetDirectoryName(relativePath);

            if (!string.IsNullOrWhiteSpace(directoryName))
            {
                directoryName = directoryName.Replace('\\', '/');
            }
            else
            {
                directoryName = null;
            }

            FileInfo fileInfo = new FileInfo(filePath);
            if (fileInfo.Length > uint.MaxValue)
            {
                throw new InvalidOperationException(
                    $"El archivo '{relativePath}' excede el tamaño máximo soportado por VPK.");
            }

            entries.Add(new VpkSourceEntry
            {
                FullPath = filePath,
                Extension = extension ?? string.Empty,
                DirectoryName = directoryName,
                FileName = fileName,
                Length = checked((uint)fileInfo.Length),
                Crc32 = ComputeCrc32(filePath)
            });
        }

        entries.Sort(VpkSourceEntryComparer.Instance);

        uint currentOffset = 0;
        foreach (VpkSourceEntry entry in entries)
        {
            entry.Offset = currentOffset;
            currentOffset = checked(currentOffset + entry.Length);
        }

        return entries;
    }

    // ════════════════════════════════════════════════════════════
    // BUILD DEL TREE VPK
    // ════════════════════════════════════════════════════════════

    private static byte[] BuildTreeBytes(IReadOnlyList<VpkSourceEntry> entries)
    {
        using MemoryStream ms = new MemoryStream();
        using BinaryWriter writer = new BinaryWriter(ms, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false), leaveOpen: true);

        string? currentExtension = null;
        string? currentDirectory = null;
        bool extensionOpen = false;
        bool directoryOpen = false;

        foreach (VpkSourceEntry entry in entries)
        {
            if (!string.Equals(currentExtension, entry.Extension, StringComparison.Ordinal))
            {
                if (directoryOpen)
                {
                    WriteNullTermString(writer, string.Empty);
                    directoryOpen = false;
                }

                if (extensionOpen)
                {
                    WriteNullTermString(writer, string.Empty);
                }

                currentExtension = entry.Extension;
                WriteNullTermString(writer, currentExtension);
                extensionOpen = true;
                currentDirectory = null;
            }

            string directoryKey = entry.DirectoryName ?? string.Empty;
            if (!string.Equals(currentDirectory, directoryKey, StringComparison.Ordinal))
            {
                if (directoryOpen)
                {
                    WriteNullTermString(writer, string.Empty);
                }

                currentDirectory = directoryKey;
                WriteNullTermString(writer, string.IsNullOrEmpty(directoryKey) ? " " : directoryKey);
                directoryOpen = true;
            }

            WriteNullTermString(writer, entry.FileName);

            writer.Write(entry.Crc32);
            writer.Write((ushort)0); // preload bytes = 0
            writer.Write((ushort)short.MaxValue); // 32767 = archivo principal (_dir.vpk)
            writer.Write(entry.Offset);
            writer.Write(entry.Length);
            writer.Write(ushort.MaxValue); // terminador de entrada
        }

        if (directoryOpen)
        {
            WriteNullTermString(writer, string.Empty);
        }

        if (extensionOpen)
        {
            WriteNullTermString(writer, string.Empty);
        }

        WriteNullTermString(writer, string.Empty); // terminador final

        writer.Flush();
        return ms.ToArray();
    }

    private static byte[] BuildHeader(uint treeSize, uint fileDataSectionSize)
    {
        using MemoryStream ms = new MemoryStream(28);
        using BinaryWriter writer = new BinaryWriter(ms, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false), leaveOpen: true);

        writer.Write(global::Package.MAGIC);
        writer.Write((uint)2);
        writer.Write(treeSize);
        writer.Write(fileDataSectionSize);
        writer.Write((uint)0);  // ArchiveMD5SectionSize
        writer.Write((uint)48); // OtherMD5SectionSize
        writer.Write((uint)0);  // SignatureSectionSize

        writer.Flush();
        return ms.ToArray();
    }

    private static void WriteNullTermString(BinaryWriter writer, string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            writer.Write(bytes);
        }

        writer.Write((byte)0);
    }

    // ════════════════════════════════════════════════════════════
    // CRC32
    // ════════════════════════════════════════════════════════════

    private static uint ComputeCrc32(string filePath)
    {
        uint crc = 0xFFFFFFFFu;
        byte[] buffer = new byte[1024 * 64];

        using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        int read;
        while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
        {
            for (int i = 0; i < read; i++)
            {
                crc = (crc >> 8) ^ Crc32Table[(crc ^ buffer[i]) & 0xFF];
            }
        }

        return ~crc;
    }

    private static uint[] CreateCrc32Table()
    {
        const uint polynomial = 0xEDB88320u;
        uint[] table = new uint[256];

        for (uint i = 0; i < table.Length; i++)
        {
            uint value = i;

            for (int j = 0; j < 8; j++)
            {
                if ((value & 1) != 0)
                {
                    value = (value >> 1) ^ polynomial;
                }
                else
                {
                    value >>= 1;
                }
            }

            table[i] = value;
        }

        return table;
    }

    private static readonly uint[] Crc32Table = CreateCrc32Table();

    // ════════════════════════════════════════════════════════════
    // MODELOS PRIVADOS
    // ════════════════════════════════════════════════════════════

    private sealed class VpkSourceEntry
    {
        public string FullPath { get; init; } = string.Empty;
        public string Extension { get; init; } = string.Empty;
        public string? DirectoryName { get; init; }
        public string FileName { get; init; } = string.Empty;
        public uint Crc32 { get; init; }
        public uint Length { get; init; }
        public uint Offset { get; set; }
    }

    private sealed class VpkSourceEntryComparer : IComparer<VpkSourceEntry>
    {
        public static readonly VpkSourceEntryComparer Instance = new VpkSourceEntryComparer();

        public int Compare(VpkSourceEntry? x, VpkSourceEntry? y)
        {
            if (ReferenceEquals(x, y))
            {
                return 0;
            }

            if (x is null)
            {
                return -1;
            }

            if (y is null)
            {
                return 1;
            }

            int result = string.Compare(x.Extension, y.Extension, StringComparison.Ordinal);
            if (result != 0)
            {
                return result;
            }

            string xDirectory = x.DirectoryName ?? string.Empty;
            string yDirectory = y.DirectoryName ?? string.Empty;

            result = string.Compare(xDirectory, yDirectory, StringComparison.Ordinal);
            if (result != 0)
            {
                return result;
            }

            return string.Compare(x.FileName, y.FileName, StringComparison.Ordinal);
        }
    }
}