namespace SKYNET;

public class EmblemHandler : BundleHandlerBase
{
    public EmblemHandler(IAudioService audio, IModConfigurationService modConfig, IUIState uiState)
        : base(audio, modConfig, uiState) { }

    public override string Prefab => "emblem";

    protected override void ApplyItem(BundleBox box)
    {
        _uiState.SetEmblemItem(box.item);
        _modConfig.SetEmblem(box.item);
    }
}