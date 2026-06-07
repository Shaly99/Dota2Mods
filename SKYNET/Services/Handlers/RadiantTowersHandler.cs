namespace SKYNET;

public class RadiantTowersHandler : BundleHandlerBase
{
    public RadiantTowersHandler(IAudioService audio, IModConfigurationService modConfig, IUIState uiState)
        : base(audio, modConfig, uiState) { }

    public override string Prefab => "radianttowers";

    protected override void ApplyItem(BundleBox box)
    {
        _uiState.SetRadiantTowersItem(box.item);
        _modConfig.SetRadiantTowers(box.item);
    }
}