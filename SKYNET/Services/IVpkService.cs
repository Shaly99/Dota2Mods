namespace SKYNET;

/// <summary>
/// Servicio para gestionar el archivo VPK de Dota 2.
/// Encapsula el acceso al Package global.
/// </summary>
public interface IVpkService
{
    /// <summary>
    /// Indica si el VPK está cargado.
    /// </summary>
    bool IsLoaded { get; }

    /// <summary>
    /// Carga el archivo VPK desde la ruta especificada.
    /// </summary>
    void Load(string vpkPath);

    /// <summary>
    /// Descarga el VPK actual y libera recursos.
    /// </summary>
    void Unload();

    /// <summary>
    /// Obtiene una entrada por ruta completa (puede ser null).
    /// </summary>
    PackageEntry FindEntry(string filePath);

    /// <summary>
    /// Obtiene una entrada por directorio y nombre (puede ser null).
    /// </summary>
    PackageEntry FindEntry(string directory, string fileName);

    /// <summary>
    /// Obtiene todas las entradas de una extensión específica.
    /// Devuelve lista vacía si no existen.
    /// </summary>
    List<PackageEntry> GetEntriesByExtension(string extension);

    /// <summary>
    /// Lee el contenido binario de una entrada.
    /// </summary>
    void ReadEntry(PackageEntry entry, out byte[] output, bool validateCrc = true);

    /// <summary>
    /// Acceso directo al Package subyacente.
    /// Usar SOLO si los métodos de arriba no cubren tu caso.
    /// Puede ser null si no está cargado.
    /// </summary>
    Package GetUnderlyingPackage();
}