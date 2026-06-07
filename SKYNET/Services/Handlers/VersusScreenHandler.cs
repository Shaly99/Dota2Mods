namespace SKYNET;

public class VersusScreenHandler : BundleHandlerBase
{
    public VersusScreenHandler(IAudioService audio, IModConfigurationService modConfig, IUIState uiState)
        : base(audio, modConfig, uiState) { }

    public override string Prefab => "versus_screen";

    protected override void ApplyItem(BundleBox box)
    {
        _uiState.SetVersusScreenItem(box.item);
        _modConfig.SetVersusScreen(box.item);
    }
}