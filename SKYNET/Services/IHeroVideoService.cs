namespace SKYNET;

/// <summary>
/// Servicio para obtener las rutas de los videos de héroes.
/// </summary>
public interface IHeroVideoService
{
    /// <summary>
    /// Indica si hay videos disponibles.
    /// </summary>
    bool HasVideos { get; }

    /// <summary>
    /// Obtiene la ruta completa al video del héroe.
    /// Devuelve null si el video no existe.
    /// </summary>
    string GetVideoPath(string heroName);

    /// <summary>
    /// Pre-carga las rutas de todos los videos disponibles.
    /// </summary>
    void Load(string videoFolder);
}