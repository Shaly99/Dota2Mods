namespace SKYNET;

public class HUDSkinHandler : BundleHandlerBase
{
    public HUDSkinHandler(IAudioService audio, IModConfigurationService modConfig, IUIState uiState)
        : base(audio, modConfig, uiState) { }

    public override string Prefab => "hud_skin";

    protected override void ApplyItem(BundleBox box)
    {
        _uiState.SetHUDSkinItem(box.item);
        _modConfig.SetHUDSkin(box.item);
    }
}