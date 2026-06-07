namespace SKYNET;

public class MusicHandler : BundleHandlerBase
{
    public MusicHandler(IAudioService audio, IModConfigurationService modConfig, IUIState uiState)
        : base(audio, modConfig, uiState) { }

    public override string Prefab => "music";

    protected override void ApplyItem(BundleBox box)
    {
        _uiState.SetMusicPackItem(box.item);
        _modConfig.SetMusicPack(box.item);

        PlayPreview(box.item);
    }

    private void PlayPreview(Items item)
    {
        if (string.IsNullOrEmpty(item.asset))
        {
            if (item.prefab == "music" && item.item_name != null
                && item.item_name.Contains("The_International"))
            {
                string itemName = item.item_name;
                itemName = itemName.Replace("#DOTA_Item_The_International_201", "");
                itemName = itemName.Replace("_Music_Pack", "");
                string filePath = "sounds/music/valve_ti" + itemName + "/music/ui_hero_select.vsnd";

                _audio.StopSounds();
                _audio.PlaySound(filePath, toStop: true, 34);
            }
        }
        else
        {
            _audio.StopSounds();
            _audio.PlaySound(item.asset, toStop: true, 34);
        }
    }
}