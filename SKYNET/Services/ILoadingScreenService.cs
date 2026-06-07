namespace SKYNET;

/// <summary>
/// Servicio para manejar la extracción y exportación de loading screens.
/// </summary>
public interface ILoadingScreenService
{
    /// <summary>
    /// Extrae todas las loading screens al disco.
    /// </summary>
    void ExtractAll();

    /// <summary>
    /// Extrae una loading screen específica.
    /// </summary>
    /// <param name="itemScreen">El item de loading screen</param>
    /// <param name="desktop">Si es true, también la pone como fondo de escritorio</param>
    /// <param name="open">Si es true, abre el archivo después de extraerlo</param>
    void Extract(Items itemScreen, bool desktop = false, bool open = false);
}