namespace SKYNET;

public class TerrainHandler : BundleHandlerBase
{
    public TerrainHandler(IAudioService audio, IModConfigurationService modConfig, IUIState uiState)
        : base(audio, modConfig, uiState) { }

    public override string Prefab => "terrain";

    protected override void ApplyItem(BundleBox box)
    {
        _uiState.SetTerrainItem(box.item);
        _modConfig.SetTerrain(box.item);
    }
}