namespace SKYNET;

/// <summary>
/// Validador de cache del VPK.
/// Determina si el archivo pak01_dir.vpk cambió desde la última extracción.
/// </summary>
public interface IVpkCacheValidator
{
    /// <summary>
    /// Verifica si el cache es válido (el VPK no cambió).
    /// </summary>
    /// <returns>true si el cache es válido y podemos saltar la extracción</returns>
    bool IsCacheValid();

    /// <summary>
    /// Guarda el "sello" del VPK actual para validar en el próximo arranque.
    /// Llamar después de una extracción exitosa.
    /// </summary>
    void SaveCacheSignature();

    /// <summary>
    /// Invalida manualmente el cache (forzar re-extracción en el próximo arranque).
    /// </summary>
    void InvalidateCache();

    /// <summary>
    /// Indica si los datos extraídos existen en disco.
    /// </summary>
    bool ExtractedDataExists();
}