namespace SKYNET;

public class WeatherEffectHandler : BundleHandlerBase
{
    public WeatherEffectHandler(IAudioService audio, IModConfigurationService modConfig, IUIState uiState)
        : base(audio, modConfig, uiState) { }

    public override string Prefab => "misc";

    public override bool CanHandle(BundleBox box)
    {
        return base.CanHandle(box) && box.item?.item_slot == "weather";
    }

    protected override void ApplyItem(BundleBox box)
    {
        _uiState.SetWeatherEffectItem(box.item);
        _modConfig.SetWeatherEffect(box.item);
    }
}