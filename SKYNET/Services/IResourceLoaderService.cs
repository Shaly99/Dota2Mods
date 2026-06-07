namespace SKYNET;

/// <summary>
/// Servicio para cargar datos desde archivos del juego (items_game.txt, lang files, etc.).
/// Encapsula el estado mutable que antes estaba en DotaResources (kValue).
/// </summary>
public interface IResourceLoaderService
{
    /// <summary>
    /// Indica si los datos principales (items_game.txt) ya fueron cargados.
    /// </summary>
    bool IsLoaded { get; }

    /// <summary>
    /// Acceso al KValue principal (items_game.txt).
    /// Devuelve null si todavía no se cargó.
    /// </summary>
    KValue MainData { get; }

    /// <summary>
    /// Carga el archivo items_game.txt y popula:
    /// - Prefabs Service
    /// - Color Service
    /// - Item Catalog (todas las categorías)
    /// </summary>
    void LoadMainData();

    /// <summary>
    /// Carga las descripciones de items desde el archivo de idioma actual.
    /// </summary>
    void LoadItemDescriptions();

    /// <summary>
    /// Carga la rich presence (nombres de héroes) desde el archivo de idioma.
    /// </summary>
    void LoadRichPresence();
}