namespace SKYNET;

public class EmoticonsHandler : BundleHandlerBase
{
    public EmoticonsHandler(IAudioService audio, IModConfigurationService modConfig, IUIState uiState)
        : base(audio, modConfig, uiState) { }

    public override string Prefab => "emoticon_tool";

    protected override void ApplyItem(BundleBox box)
    {
        _uiState.SetEmoticonsItem(box.item);
        _modConfig.SetEmoticons(box.item);
    }
}