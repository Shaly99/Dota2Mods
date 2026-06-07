namespace SKYNET;

public class PetsHandler : BundleHandlerBase
{
    public PetsHandler(IAudioService audio, IModConfigurationService modConfig, IUIState uiState)
        : base(audio, modConfig, uiState) { }

    public override string Prefab => "wearable";

    public override bool CanHandle(BundleBox box)
    {
        if (!base.CanHandle(box)) return false;

        var image = box.item?.image_inventory?.ToLower() ?? "";
        return image.Contains("/pets/");
    }

    protected override void ApplyItem(BundleBox box)
    {
        _uiState.SetPetsItem(box.item);
        _modConfig.SetPets(box.item);
    }
}