namespace SKYNET;

/// <summary>
/// Implementación del servicio de loading screens.
/// Wrapper sobre LoadingScreenManager para mantener compatibilidad.
/// </summary>
public class LoadingScreenService : ILoadingScreenService
{
    private readonly IUIService _ui;

    public LoadingScreenService(IUIService ui)
    {
        _ui = ui ?? throw new ArgumentNullException(nameof(ui));
    }

    public void ExtractAll()
    {
        LoadingScreenManager.Extract();
    }

    public void Extract(Items itemScreen, bool desktop = false, bool open = false)
    {
        LoadingScreenManager.Extract(itemScreen, desktop, open);
    }
}