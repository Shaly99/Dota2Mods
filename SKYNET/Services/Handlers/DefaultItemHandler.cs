namespace SKYNET;

public class DefaultItemHandler : BundleHandlerBase
{
    public DefaultItemHandler(IAudioService audio, IModConfigurationService modConfig, IUIState uiState)
        : base(audio, modConfig, uiState) { }

    public override string Prefab => "default_item";

    protected override void ApplyItem(BundleBox box)
    {
        // No hace nada al aplicar (es solo display)
    }

    public override void Handle(BundleBox box)
    {
        // 🔑 Si el box fue reseteado o no es tool, abrir el selector
        if (box.WasReset || !box.isTool)
        {
            ShowToolSelector(box);
        }
    }
}