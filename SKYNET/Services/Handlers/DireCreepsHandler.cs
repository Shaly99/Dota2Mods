namespace SKYNET;

public class DireCreepsHandler : BundleHandlerBase
{
    public DireCreepsHandler(IAudioService audio, IModConfigurationService modConfig, IUIState uiState)
        : base(audio, modConfig, uiState) { }

    public override string Prefab => "direcreeps";

    protected override void ApplyItem(BundleBox box)
    {
        _uiState.SetDireCreepsItem(box.item);
        _modConfig.SetDireCreeps(box.item);
    }
}