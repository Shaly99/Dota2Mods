namespace SKYNET;

public class RadiantCreepsHandler : BundleHandlerBase
{
    public RadiantCreepsHandler(IAudioService audio, IModConfigurationService modConfig, IUIState uiState)
        : base(audio, modConfig, uiState) { }

    public override string Prefab => "radiantcreeps";

    protected override void ApplyItem(BundleBox box)
    {
        _uiState.SetRadiantCreepsItem(box.item);
        _modConfig.SetRadiantCreeps(box.item);
    }
}