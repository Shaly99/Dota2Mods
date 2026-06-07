namespace SKYNET;

public class StreakEffectHandler : BundleHandlerBase
{
    public StreakEffectHandler(IAudioService audio, IModConfigurationService modConfig, IUIState uiState)
        : base(audio, modConfig, uiState) { }

    public override string Prefab => "streak_effect";

    protected override void ApplyItem(BundleBox box)
    {
        _uiState.SetStreakEffectItem(box.item);
        _modConfig.SetStreakEffect(box.item);
    }
}