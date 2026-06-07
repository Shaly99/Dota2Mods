namespace SKYNET;

public class WardHandler : BundleHandlerBase
{
    public WardHandler(IAudioService audio, IModConfigurationService modConfig, IUIState uiState)
        : base(audio, modConfig, uiState) { }

    public override string Prefab => "ward";

    protected override void ApplyItem(BundleBox box)
    {
        _uiState.SetWardItem(box.item);
        _modConfig.SetWard(box.item);
    }
}