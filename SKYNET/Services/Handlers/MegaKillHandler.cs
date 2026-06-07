namespace SKYNET;

public class MegaKillHandler : BundleHandlerBase
{
    public MegaKillHandler(IAudioService audio, IModConfigurationService modConfig, IUIState uiState)
        : base(audio, modConfig, uiState) { }

    public override string Prefab => "announcer";

    public override bool CanHandle(BundleBox box)
    {
        return base.CanHandle(box) && box.item?.item_slot == "mega_kills";
    }

    protected override void ApplyItem(BundleBox box)
    {
        _uiState.SetMegaKillItem(box.item);
        _modConfig.SetMegaKillAnnouncer(box.item);
        PlayPreview(box.item);
    }

    private void PlayPreview(Items item)
    {
        if (string.IsNullOrEmpty(item.asset))
        {
            ItemsManager.GetSoundAssetName(item.ItemID, out var patch);

            string heroFolder = patch.Replace("npc_dota_hero_", "");
            heroFolder = "sounds/vo/" + heroFolder;

            string fileName = patch.Replace("npc_dota_hero_announcer_dlc_", "");
            fileName += "_ann_welcome_01";

            _audio.StopSounds();
            _audio.PlaySound(heroFolder + "/" + fileName + ".vsnd", toStop: true, 341);
        }
        else
        {
            _audio.StopSounds();
            _audio.PlaySound(item.asset, toStop: true, 341);
        }
    }
}