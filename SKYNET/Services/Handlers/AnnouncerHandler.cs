namespace SKYNET;

public class AnnouncerHandler : BundleHandlerBase
{
    public AnnouncerHandler(IAudioService audio, IModConfigurationService modConfig, IUIState uiState)
        : base(audio, modConfig, uiState) { }

    public override string Prefab => "announcer";

    public override bool CanHandle(BundleBox box)
    {
        return base.CanHandle(box) && box.item?.item_slot == "announcer";
    }

    protected override void ApplyItem(BundleBox box)
    {
        _uiState.SetAnnouncerItem(box.item);
        _modConfig.SetAnnouncer(box.item);
        PlayPreview(box.item);
    }

    private void PlayPreview(Items item)
    {
        if (string.IsNullOrEmpty(item.asset))
        {
            ItemsManager.GetSoundAssetName(item.ItemID, out var patch);

            if (!string.IsNullOrEmpty(patch))
            {
                string heroFolder = patch.Replace("npc_dota_hero_", "");
                heroFolder = "sounds/vo/" + heroFolder;

                string fileName = patch.Replace("npc_dota_hero_announcer_dlc_", "");
                fileName += "_ann_welcome_01";

                _audio.StopSounds();
                _audio.PlaySound(heroFolder + "/" + fileName + ".vsnd", toStop: true, 186);
            }
            else
            {
                string fallback = item.item_description?.Replace("#DOTA_Item_Desc_Announcer_", "") ?? "";
                string heroFolder = "sounds/vo/announcer_dlc_" + fallback;
                string fileName = "_ann_welcome_01";

                _audio.StopSounds();
                _audio.PlaySound(heroFolder + "/" + fileName + ".vsnd", toStop: true, 186);
            }
        }
        else
        {
            _audio.StopSounds();
            _audio.PlaySound(item.asset, toStop: true, 186);
        }
    }
}