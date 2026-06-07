namespace SKYNET;

/// <summary>
/// Servicio para extraer y cargar recursos del juego desde el VPK.
/// </summary>
public interface IResourceService
{
    /// <summary>
    /// Extrae todos los recursos (imágenes) de una ruta del VPK al disco.
    /// </summary>
    void ExtractResources(string ruta, string toFolder);

    /// <summary>
    /// Extrae un archivo específico del VPK.
    /// </summary>
    void ExtractFile(string file, string folder);

    /// <summary>
    /// Carga los items desde items_game.txt al catálogo.
    /// </summary>
    void LoadResources();

    /// <summary>
    /// Carga las descripciones de items desde el archivo de idioma.
    /// </summary>
    void LoadItemsDescriptions();

    /// <summary>
    /// Carga la rich presence (nombres de héroes) desde el archivo de idioma.
    /// </summary>
    void LoadRichPresence();

    /// <summary>
    /// Extrae una imagen específica del VPK y devuelve el bitmap.
    /// </summary>
    Bitmap ExtractAndGenerate(string ruta);
}