namespace SKYNET;

public class CourierHandler : BundleHandlerBase
{
    public CourierHandler(IAudioService audio, IModConfigurationService modConfig, IUIState uiState)
        : base(audio, modConfig, uiState) { }

    public override string Prefab => "courier";

    protected override void ApplyItem(BundleBox box)
    {
        _uiState.SetCourierItem(box.item);
        _modConfig.SetCourier(box.item);
    }
}