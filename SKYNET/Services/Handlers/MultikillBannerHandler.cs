namespace SKYNET;

public class MultikillBannerHandler : BundleHandlerBase
{
    public MultikillBannerHandler(IAudioService audio, IModConfigurationService modConfig, IUIState uiState)
        : base(audio, modConfig, uiState) { }

    public override string Prefab => "misc";

    public override bool CanHandle(BundleBox box)
    {
        return base.CanHandle(box) && box.item?.item_slot == "multikill_banner";
    }

    protected override void ApplyItem(BundleBox box)
    {
        _uiState.SetMultikillBannerItem(box.item);
        _modConfig.SetMultikillBanner(box.item);
    }
}