namespace SKYNET;

/// <summary>
/// Implementación de IUIState.
/// Delega al singleton frmMain.frm para acceder a los BundleBox.
/// </summary>
public class UIState : IUIState
{
    private static frmMain Frm => frmMain.frm;

    // ════════════════════════════════════════
    // SETTERS DE ITEMS
    // ════════════════════════════════════════
    public void SetTerrainItem(Items item)
    {
        if (Frm?.CurrentTerrain != null) Frm.CurrentTerrain.item = item;
    }

    public void SetWeatherEffectItem(Items item)
    {
        if (Frm?.CurrentWeatherEffect != null) Frm.CurrentWeatherEffect.item = item;
    }

    public void SetRadiantTowersItem(Items item)
    {
        if (Frm?.CurrentRadiantTowers != null) Frm.CurrentRadiantTowers.item = item;
    }

    public void SetDireTowersItem(Items item)
    {
        if (Frm?.CurrentDireTowers != null) Frm.CurrentDireTowers.item = item;
    }

    public void SetRadiantCreepsItem(Items item)
    {
        if (Frm?.CurrentRadiantCreeps != null) Frm.CurrentRadiantCreeps.item = item;
    }

    public void SetDireCreepsItem(Items item)
    {
        if (Frm?.CurrentDireCreeps != null) Frm.CurrentDireCreeps.item = item;
    }

    public void SetVersusScreenItem(Items item)
    {
        if (Frm?.CurrentVersusScreen != null) Frm.CurrentVersusScreen.item = item;
    }

    public void SetCursorPackItem(Items item)
    {
        if (Frm?.CurrentCursorPack != null) Frm.CurrentCursorPack.item = item;
    }

    public void SetMultikillBannerItem(Items item)
    {
        if (Frm?.CurrentMultikillBanner != null) Frm.CurrentMultikillBanner.item = item;
    }

    public void SetEmblemItem(Items item)
    {
        if (Frm?.CurrentEmblem != null) Frm.CurrentEmblem.item = item;
    }

    public void SetEmoticonsItem(Items item)
    {
        if (Frm?.CurrentEmoticons != null) Frm.CurrentEmoticons.item = item;
    }

    public void SetHUDSkinItem(Items item)
    {
        if (Frm?.CurrentHUDSkin != null) Frm.CurrentHUDSkin.item = item;
    }

    public void SetCourierItem(Items item)
    {
        if (Frm?.CurrentCourier != null) Frm.CurrentCourier.item = item;
    }

    public void SetWardItem(Items item)
    {
        if (Frm?.CurrentWard != null) Frm.CurrentWard.item = item;
    }

    public void SetLoadingScreenItem(Items item)
    {
        if (Frm?.CurrentLoadingScreen != null) Frm.CurrentLoadingScreen.item = item;
    }

    public void SetMusicPackItem(Items item)
    {
        if (Frm?.CurrentMusicPack != null) Frm.CurrentMusicPack.item = item;
    }

    public void SetAnnouncerItem(Items item)
    {
        if (Frm?.CurrentAnnouncer != null) Frm.CurrentAnnouncer.item = item;
    }

    public void SetMegaKillItem(Items item)
    {
        if (Frm?.CurrentMegaKill != null) Frm.CurrentMegaKill.item = item;
    }

    public void SetPetsItem(Items item)
    {
        if (Frm?.CurrentPets != null) Frm.CurrentPets.item = item;
    }

    public void SetStreakEffectItem(Items item)
    {
        if (Frm?.CurrentStreakEffect != null) Frm.CurrentStreakEffect.item = item;
    }

    // ════════════════════════════════════════
    // RESET ALL
    // ════════════════════════════════════════
    public void ResetCategory(ItemCategory category)
    {
        if (Frm == null) return;

        switch (category)
        {
            case ItemCategory.Announcer: Frm.CurrentAnnouncer?.Reset(); break;
            case ItemCategory.MegaKillAnnouncer: Frm.CurrentMegaKill?.Reset(); break;
            case ItemCategory.MultikillBanner: Frm.CurrentMultikillBanner?.Reset(); break;
            case ItemCategory.Courier: Frm.CurrentCourier?.Reset(); break;
            case ItemCategory.CursorPack: Frm.CurrentCursorPack?.Reset(); break;
            case ItemCategory.DireCreeps: Frm.CurrentDireCreeps?.Reset(); break;
            case ItemCategory.DireTowers: Frm.CurrentDireTowers?.Reset(); break;
            case ItemCategory.RadiantCreeps: Frm.CurrentRadiantCreeps?.Reset(); break;
            case ItemCategory.RadiantTowers: Frm.CurrentRadiantTowers?.Reset(); break;
            case ItemCategory.MusicPack: Frm.CurrentMusicPack?.Reset(); break;
            case ItemCategory.Pets: Frm.CurrentPets?.Reset(); break;
            case ItemCategory.VersusScreen: Frm.CurrentVersusScreen?.Reset(); break;
            case ItemCategory.Ward: Frm.CurrentWard?.Reset(); break;
            case ItemCategory.WeatherEffect: Frm.CurrentWeatherEffect?.Reset(); break;
            case ItemCategory.LoadingScreen: Frm.CurrentLoadingScreen?.Reset(); break;
            case ItemCategory.Emblem: Frm.CurrentEmblem?.Reset(); break;
            case ItemCategory.Emoticons: Frm.CurrentEmoticons?.Reset(); break;
            case ItemCategory.HUDSkin: Frm.CurrentHUDSkin?.Reset(); break;
            case ItemCategory.Terrain: Frm.CurrentTerrain?.Reset(); break;
            case ItemCategory.StreakEffect: Frm.CurrentStreakEffect?.Reset(); break;
        }
    }
    public void LoadDefaultForCategory(ItemCategory category)
    {
        if (Frm == null) return;

        // Obtener el catálogo para buscar el item "default"
        var catalog = ServiceContainer.TryGet<IItemCatalog>();
        if (catalog == null) return;

        // Buscar el item con "default" en el nombre (case-insensitive)
        var defaultItem = catalog.GetByCategoryFast(category)
            .Find(x => x.name != null && x.name.ToLower().Contains("default"));

        // Casos especiales que no siguen la convención "Default XXX"
        if (defaultItem == null)
        {
            // Pets: busca "DefaultPet" en item_name
            if (category == ItemCategory.Pets)
            {
                defaultItem = catalog.GetByCategoryFast(ItemCategory.DefaultItems)
                    .Find(x => x.item_name != null && x.item_name.ToLower().Contains("defaultpet"));
            }
            // StreakEffect: busca "No Kill" en item_name
            else if (category == ItemCategory.StreakEffect)
            {
                defaultItem = catalog.GetByCategoryFast(ItemCategory.StreakEffect)
                    .Find(x => x.item_name != null && x.item_name.ToLower().Contains("no kill"));
            }
        }

        // Si no hay item default real, crear uno mínimo con prefab/slot apropiados
        if (defaultItem == null)
        {
            defaultItem = CreateFallbackDefault(category);
        }

        // Asignar al BundleBox correspondiente
        switch (category)
        {
            case ItemCategory.Terrain: SetTerrainItem(defaultItem); break;
            case ItemCategory.WeatherEffect: SetWeatherEffectItem(defaultItem); break;
            case ItemCategory.RadiantTowers: SetRadiantTowersItem(defaultItem); break;
            case ItemCategory.DireTowers: SetDireTowersItem(defaultItem); break;
            case ItemCategory.RadiantCreeps: SetRadiantCreepsItem(defaultItem); break;
            case ItemCategory.DireCreeps: SetDireCreepsItem(defaultItem); break;
            case ItemCategory.VersusScreen: SetVersusScreenItem(defaultItem); break;
            case ItemCategory.CursorPack: SetCursorPackItem(defaultItem); break;
            case ItemCategory.MultikillBanner: SetMultikillBannerItem(defaultItem); break;
            case ItemCategory.Emblem: SetEmblemItem(defaultItem); break;
            case ItemCategory.Emoticons: SetEmoticonsItem(defaultItem); break;
            case ItemCategory.HUDSkin: SetHUDSkinItem(defaultItem); break;
            case ItemCategory.Courier: SetCourierItem(defaultItem); break;
            case ItemCategory.Ward: SetWardItem(defaultItem); break;
            case ItemCategory.LoadingScreen: SetLoadingScreenItem(defaultItem); break;
            case ItemCategory.MusicPack: SetMusicPackItem(defaultItem); break;
            case ItemCategory.Announcer: SetAnnouncerItem(defaultItem); break;
            case ItemCategory.MegaKillAnnouncer: SetMegaKillItem(defaultItem); break;
            case ItemCategory.Pets: SetPetsItem(defaultItem); break;
            case ItemCategory.StreakEffect: SetStreakEffectItem(defaultItem); break;
        }
    }

    /// <summary>
    /// Crea un Items fallback cuando no se encuentra un "default" real en el catálogo.
    /// </summary>
    private Items CreateFallbackDefault(ItemCategory category)
    {
        switch (category)
        {
            case ItemCategory.Terrain: return new Items { prefab = "terrain" };
            case ItemCategory.WeatherEffect: return new Items { prefab = "misc", item_slot = "weather" };
            case ItemCategory.RadiantTowers: return new Items { prefab = "radianttowers" };
            case ItemCategory.DireTowers: return new Items { prefab = "diretowers" };
            case ItemCategory.RadiantCreeps: return new Items { prefab = "radiantcreeps" };
            case ItemCategory.DireCreeps: return new Items { prefab = "direcreeps" };
            case ItemCategory.VersusScreen: return new Items { prefab = "versus_screen" };
            case ItemCategory.CursorPack: return new Items { prefab = "cursor_pack" };
            case ItemCategory.MultikillBanner:
                return new Items
                {
                    prefab = "misc",
                    item_slot = "multikill_banner",
                    name = "Default Multikill-Banner",
                    item_rarity = ""
                };
            case ItemCategory.Emblem: return new Items { prefab = "emblem" };
            case ItemCategory.Emoticons: return new Items { prefab = "emoticon_tool" };
            case ItemCategory.HUDSkin: return new Items { prefab = "hud_skin" };
            case ItemCategory.Courier: return new Items { prefab = "courier" };
            case ItemCategory.Ward: return new Items { prefab = "ward" };
            case ItemCategory.LoadingScreen: return new Items { prefab = "loading_screen" };
            case ItemCategory.MusicPack: return new Items { prefab = "music" };
            case ItemCategory.Announcer: return new Items { prefab = "announcer", item_slot = "announcer" };
            case ItemCategory.MegaKillAnnouncer: return new Items { prefab = "announcer", item_slot = "mega_kills" };
            case ItemCategory.Pets:
                return new Items
                {
                    prefab = "wearable",
                    item_slot = "summon",
                    item_name = "#DOTA_Wearable_All_Heroes_DefaultPet"
                };
            case ItemCategory.StreakEffect:
                return new Items
                {
                    name = "No Kill Streak Effect",
                    prefab = "streak_effect",
                    image_inventory = "econ/default_no_item",
                    item_name = "#DOTA_Item_No_Kill_Streak_Effect"
                };
            default: return new Items();
        }
    }
    public void ResetAll()
    {
        if (Frm == null) return;

        Frm.CurrentAnnouncer?.Reset();
        Frm.CurrentMegaKill?.Reset();
        Frm.CurrentMultikillBanner?.Reset();
        Frm.CurrentCourier?.Reset();
        Frm.CurrentCursorPack?.Reset();
        Frm.CurrentDireCreeps?.Reset();
        Frm.CurrentDireTowers?.Reset();
        Frm.CurrentRadiantCreeps?.Reset();
        Frm.CurrentRadiantTowers?.Reset();
        Frm.CurrentMusicPack?.Reset();
        Frm.CurrentPets?.Reset();
        Frm.CurrentVersusScreen?.Reset();
        Frm.CurrentWard?.Reset();
        Frm.CurrentWeatherEffect?.Reset();
        Frm.CurrentLoadingScreen?.Reset();
        Frm.CurrentEmblem?.Reset();
        Frm.CurrentEmoticons?.Reset();
        Frm.CurrentHUDSkin?.Reset();
        Frm.CurrentTerrain?.Reset();
        Frm.CurrentStreakEffect?.Reset();
    }
}