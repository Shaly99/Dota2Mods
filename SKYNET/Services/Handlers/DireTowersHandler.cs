namespace SKYNET;

public class DireTowersHandler : BundleHandlerBase
{
    public DireTowersHandler(IAudioService audio, IModConfigurationService modConfig, IUIState uiState)
        : base(audio, modConfig, uiState) { }

    public override string Prefab => "diretowers";

    protected override void ApplyItem(BundleBox box)
    {
        _uiState.SetDireTowersItem(box.item);
        _modConfig.SetDireTowers(box.item);
    }
}