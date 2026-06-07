namespace SKYNET;

/// <summary>
/// Clase base abstracta para handlers de bundles.
/// Implementa la lógica común: "si isTool aplicar, sino mostrar selector".
/// </summary>
public abstract class BundleHandlerBase : IBundleHandler
{
    protected readonly IAudioService _audio;
    protected readonly IModConfigurationService _modConfig;
    protected readonly IUIState _uiState;

    protected BundleHandlerBase(IAudioService audio, IModConfigurationService modConfig, IUIState uiState)
    {
        _audio = audio ?? throw new ArgumentNullException(nameof(audio));
        _modConfig = modConfig ?? throw new ArgumentNullException(nameof(modConfig));
        _uiState = uiState ?? throw new ArgumentNullException(nameof(uiState));
    }

    /// <summary>
    /// El prefab que este handler maneja (cada subclase lo define).
    /// </summary>
    public abstract string Prefab { get; }

    /// <summary>
    /// Por defecto, maneja todos los items que coincidan con su prefab.
    /// Las subclases pueden sobrescribir para lógica más específica.
    /// </summary>
    public virtual bool CanHandle(BundleBox box)
    {
        if (box?.item?.prefab == null) return false;
        return box.item.prefab.Equals(Prefab, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Procesa el click. Decide entre aplicar el item o mostrar el selector.
    /// </summary>
    public virtual void Handle(BundleBox box)
    {
        // 🔑 Si el box fue reseteado, abrir directo el selector (el item está vacío)
        if (box.WasReset)
        {
            ShowToolSelector(box);
            return;
        }

        if (box.isTool)
        {
            ApplyItem(box);
        }
        else
        {
            ShowToolSelector(box);
        }
    }

    /// <summary>
    /// Aplica el item seleccionado (se ejecuta cuando isTool=true).
    /// Cada subclase implementa su lógica específica.
    /// </summary>
    protected abstract void ApplyItem(BundleBox box);

    /// <summary>
    /// Muestra el selector de items (frmTools).
    /// Por defecto abre frmLoading que abre frmTools.
    /// </summary>
    protected virtual void ShowToolSelector(BundleBox box)
    {
        var frmLoading = new frmLoading(box.item);
        frmLoading.ShowDialog();
    }
}