namespace SKYNET;

/// <summary>
/// Implementación del servicio de VPK.
/// Thread-safe: maneja correctamente el acceso concurrente al package.
/// </summary>
public class VpkService : IVpkService
{
    private Package _package;
    private readonly object _lock = new object();

    public bool IsLoaded
    {
        get
        {
            lock (_lock)
            {
                return _package != null;
            }
        }
    }

    public void Load(string vpkPath)
    {
        if (string.IsNullOrEmpty(vpkPath))
            throw new ArgumentException("La ruta del VPK no puede estar vacía", nameof(vpkPath));

        lock (_lock)
        {
            // Descargar el anterior si existe
            UnloadInternal();

            // Cargar el nuevo
            _package = new Package();
            _package.Read(vpkPath);
        }
    }

    public void Unload()
    {
        lock (_lock)
        {
            UnloadInternal();
        }
    }

    private void UnloadInternal()
    {
        try
        {
            _package?.Dispose();
        }
        catch
        {
            // No dejar que un error de dispose rompa la app
        }
        finally
        {
            _package = null;
        }
    }

    public PackageEntry FindEntry(string filePath)
    {
        if (string.IsNullOrEmpty(filePath)) return null;

        lock (_lock)
        {
            return _package?.FindEntry(filePath);
        }
    }

    public PackageEntry FindEntry(string directory, string fileName)
    {
        lock (_lock)
        {
            return _package?.FindEntry(directory, fileName);
        }
    }

    public List<PackageEntry> GetEntriesByExtension(string extension)
    {
        lock (_lock)
        {
            if (_package?.Entries == null || string.IsNullOrEmpty(extension))
                return new List<PackageEntry>();

            return _package.Entries.TryGetValue(extension, out var entries)
                ? entries
                : new List<PackageEntry>();
        }
    }

    public void ReadEntry(PackageEntry entry, out byte[] output, bool validateCrc = true)
    {
        output = null;
        if (entry == null) return;

        // No usamos lock acá porque Package.ReadEntry ya es thread-safe internamente
        // (lo hicimos thread-safe en Fase 4 con el pool de streams)
        var pkg = GetUnderlyingPackage();
        pkg?.ReadEntry(entry, out output, validateCrc);
    }

    public Package GetUnderlyingPackage()
    {
        lock (_lock)
        {
            return _package;
        }
    }
}