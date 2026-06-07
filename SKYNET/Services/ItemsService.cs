namespace SKYNET;

/// <summary>
/// Servicio para gestión de items del juego.
/// Reemplaza la versión estática (ItemsManager) que estaba acoplada a frmMain.
/// </summary>
public class ItemsService : IItemsService
{
    private readonly IItemCatalog _catalog;

    /// <summary>
    /// Crea una nueva instancia con las dependencias inyectadas.
    /// </summary>
    public ItemsService(IItemCatalog catalog)
    {
        _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
    }

    // ════════════════════════════════════════════════════════════════
    // BÚSQUEDAS PRINCIPALES (O(1) gracias al catálogo)
    // ════════════════════════════════════════════════════════════════

    public Items GetItem(string itemID)
    {
        if (string.IsNullOrEmpty(itemID)) return null;
        return _catalog.GetById(itemID);
    }

    public List<Items> GetItems(string itemID)
    {
        if (string.IsNullOrEmpty(itemID)) return new List<Items>();
        return _catalog.FindAllById(itemID);
    }

    public Items GetItemFromName(string itemname)
    {
        if (string.IsNullOrEmpty(itemname)) return null;
        return _catalog.GetByName(itemname);
    }

    public Items GetBundleFromName(string itemname)
    {
        if (string.IsNullOrEmpty(itemname)) return null;

        string searchName = itemname.ToLower();
        return _catalog.GetByCategoryFast(ItemCategory.Bundles)
            .Find(b => b?.name != null && b.name.ToLower() == searchName);
    }

    public List<Items> GetItemsFromPrefab(Items item)
    {
        if (item == null) return new List<Items>();

        ItemCategory? category = MapPrefabToCategory(item);
        if (category == null) return new List<Items>();

        return _catalog.GetByCategory(category.Value);
    }

    private static ItemCategory? MapPrefabToCategory(Items item)
    {
        if (item?.prefab == null) return null;

        switch (item.prefab)
        {
            case "wearable":
                if (item.image_inventory != null && item.image_inventory.ToLower().Contains("/pets/"))
                    return ItemCategory.Pets;
                return ItemCategory.Items;

            case "default_item":
                if (item.item_name != null && item.item_name.ToLower().Contains("pet"))
                    return ItemCategory.Pets;
                return ItemCategory.DefaultItems;

            case "bundle": return ItemCategory.Bundles;
            case "taunt": return ItemCategory.Taunts;
            case "music": return ItemCategory.MusicPack;

            case "announcer":
                return item.item_slot == "announcer"
                    ? ItemCategory.Announcer
                    : ItemCategory.MegaKillAnnouncer;

            case "terrain": return ItemCategory.Terrain;

            case "misc":
                return item.item_slot == "weather"
                    ? ItemCategory.WeatherEffect
                    : ItemCategory.MultikillBanner;

            case "radiantcreeps": return ItemCategory.RadiantCreeps;
            case "direcreeps": return ItemCategory.DireCreeps;
            case "radianttowers": return ItemCategory.RadiantTowers;
            case "diretowers": return ItemCategory.DireTowers;
            case "versus_screen": return ItemCategory.VersusScreen;
            case "cursor_pack": return ItemCategory.CursorPack;
            case "emblem": return ItemCategory.Emblem;
            case "emoticon_tool": return ItemCategory.Emoticons;
            case "hud_skin": return ItemCategory.HUDSkin;
            case "courier": return ItemCategory.Courier;
            case "ward": return ItemCategory.Ward;
            case "multikill_banner": return ItemCategory.MultikillBanner;
            case "loading_screen": return ItemCategory.LoadingScreen;
            case "streak_effect": return ItemCategory.StreakEffect;

            default: return null;
        }
    }

    // ════════════════════════════════════════════════════════════════
    // DESCRIPCIONES
    // ════════════════════════════════════════════════════════════════

    public string GetDescription(string name)
    {
        if (string.IsNullOrEmpty(name)) return name ?? "";

        // Por ahora aún usamos frmMain.frm para itemDescription
        // En sesiones futuras lo migraremos también
        if (frmMain.frm?.itemDescription == null) return name;

        foreach (ItemDescription item in frmMain.frm.itemDescription)
        {
            if (item?.item_description != null &&
                item.item_description.ToLower() == name.ToLower())
            {
                return item.description ?? "";
            }
        }

        return name;
    }

    // ════════════════════════════════════════════════════════════════
    // ASSETS DE LOADING SCREEN
    // ════════════════════════════════════════════════════════════════

    public string NameFromAsset(string v)
    {
        if (string.IsNullOrEmpty(v)) return "";

        var loadingScreens = _catalog.GetByCategoryFast(ItemCategory.LoadingScreen);
        foreach (Items item in loadingScreens)
        {
            if (item == null) continue;

            if (item.asset == v.Replace(".vtex", ""))
                return item.name;
            if (item.asset == v.Replace("_tga.vtex_c", ""))
                return item.name;
            if (item.asset == v.Replace(".vtex_c", ".vtex"))
                return item.name;
        }

        string parent = Paths.GetParent(v);
        parent = parent.Replace("loadingscreens", "");
        parent = parent.Replace("/", "");
        parent = parent.Replace("_", " ");
        parent = parent.Replace("loadingscreens", " ");
        parent = parent.Replace("loading", " ");
        return parent.Replace("screens", " ");
    }

    // ════════════════════════════════════════════════════════════════
    // IMÁGENES POR DEFECTO
    // ════════════════════════════════════════════════════════════════

    public Image GetDefaultImageItem(string slot, string hero)
    {
        if (slot == "taunt") return Resources.default_taunt;

        try
        {
            List<Items> list = new List<Items>();
            var defaultItems = _catalog.GetByCategoryFast(ItemCategory.DefaultItems);

            foreach (Items defaultItem in defaultItems)
            {
                if (defaultItem?.used_by_heroes == hero)
                {
                    list.Add(defaultItem);
                }
            }

            Items items = new Items();
            foreach (Items item in list)
            {
                items = item;
                if (item?.item_slot != null && slot != null &&
                    item.item_slot.ToLower() == slot.ToLower())
                {
                    return GetImage(items.image_inventory);
                }
            }

            if (string.IsNullOrEmpty(items.image_inventory))
                return Resources.default_item;

            string[] array = items.image_inventory.Split('/');
            string text = "";
            for (int i = 0; i < array.Length; i++)
            {
                if (i != array.Length - 1)
                {
                    text = text + array[i] + "/";
                }
            }

            string fullDir = "data/panorama/images/" + text;
            if (!Directory.Exists(fullDir)) return Resources.default_item;

            string[] files = Directory.GetFiles(fullDir);
            bool flag = true;
            int num = 0;
            while (flag)
            {
                foreach (string text2 in files)
                {
                    for (int k = 0; k < list.Count; k++)
                    {
                        Items items2 = list[k];
                        if (items2 != null && text2.Contains(items2.image_inventory))
                        {
                            list.Remove(items2);
                        }
                        if (list.Count == 1)
                        {
                            return GetImage(list[0].image_inventory);
                        }
                    }
                }
                num++;
                if (num > 10) flag = false;
            }
        }
        catch (Exception)
        {
            return Resources.default_item;
        }

        return Resources.default_item;
    }

    // ════════════════════════════════════════════════════════════════
    // ITEMS GUARDADOS POR EL USUARIO
    // ════════════════════════════════════════════════════════════════

    public Items GetSavedItem(string used_by_heroes, string item_slot)
    {
        var heroesService = ServiceContainer.TryGet<IHeroesService>();
        if (heroesService == null) return null;

        return heroesService.GetBundleBySlot(used_by_heroes, item_slot);
    }

    // ════════════════════════════════════════════════════════════════
    // HEROES
    // ════════════════════════════════════════════════════════════════

    public string GetHeroName(string name)
    {
        if (string.IsNullOrEmpty(name)) return "";

        string text = "";
        if (frmMain.frm?.rich_presence != null)
        {
            foreach (ItemDescription item in frmMain.frm.rich_presence)
            {
                if (item?.item_description != null &&
                    item.item_description.ToLower() == "#" + name.ToLower())
                {
                    text = item.description ?? "";
                }
            }
        }

        if (text == "")
        {
            text = name.Replace("npc_dota_hero_", "").Replace("_", " ");
        }
        return text.ToUpper();
    }

    public string GetHeroOriginalName(string name)
    {
        if (string.IsNullOrEmpty(name)) return "";

        string text = "";
        if (frmMain.frm?.rich_presence != null)
        {
            foreach (ItemDescription item in frmMain.frm.rich_presence)
            {
                if (item?.description != null &&
                    item.description.ToLower() == name.ToLower())
                {
                    text = item.item_description ?? "";
                }
            }
        }

        if (text == "")
        {
            text = "npc_dota_hero_" + name.Replace(" ", "_").ToLower();
        }
        return text.Replace("#", "").ToLower();
    }

    // ════════════════════════════════════════════════════════════════
    // IMÁGENES (USA CACHE)
    // ════════════════════════════════════════════════════════════════

    public Image GetImage(string path)
    {
        if (string.IsNullOrEmpty(path)) return Resources.default_item;

        string fullPath = "data/panorama/images/" + path + ".png";

        Image cached = ImageCache.GetOrLoad(fullPath);
        if (cached != null) return cached;

        try
        {
            Image extracted = DotaResources.ExtractAndGenerate(path);
            if (extracted != null)
            {
                ImageCache.Add(fullPath, extracted);
            }
            return extracted ?? Resources.default_item;
        }
        catch (Exception)
        {
            return Resources.default_item;
        }
    }

    // ════════════════════════════════════════════════════════════════
    // SONIDOS
    // ════════════════════════════════════════════════════════════════

    public void GetSoundAssetName(string itemID, out string patch)
    {
        patch = "";

        if (string.IsNullOrEmpty(itemID) || DotaResources.kValue == null)
            return;

        try
        {
            KValue kValue = new KValue();
            kValue.Children = DotaResources.kValue["Items"][itemID].Children;
            if (!kValue.ContainsKey("visuals"))
                return;

            foreach (var current in kValue["visuals"].Children)
            {
                if (!current.Name.StartsWith("asset_modifier"))
                    continue;

                foreach (var assetChild in kValue["visuals"][current.Name].Children)
                {
                    if (assetChild.Name == "asset")
                    {
                        patch = assetChild.Value.ToString();
                    }
                }
            }
        }
        catch
        {
            // Ignorar errores
        }
    }
}