namespace SKYNET;

public class CursorPackHandler : BundleHandlerBase
{
    public CursorPackHandler(IAudioService audio, IModConfigurationService modConfig, IUIState uiState)
        : base(audio, modConfig, uiState) { }

    public override string Prefab => "cursor_pack";

    protected override void ApplyItem(BundleBox box)
    {
        _uiState.SetCursorPackItem(box.item);
        _modConfig.SetCursorPack(box.item);
    }
}