using System.Diagnostics;

namespace SKYNET;

/// <summary>
/// Implementación del servicio de construcción de mods.
/// Reemplaza la clase estática modHelpers.
/// </summary>
public class ModBuilderService : IModBuilderService
{
    private readonly IItemCatalog _catalog;
    private readonly IUIService _ui;
    private readonly IAudioService _audio;
    private readonly ISettingsService _settings;
    private readonly IItemsService _items;
    private readonly IHeroesService _heroes;
    private readonly IVpkWriterService _vpkWriter;
    private readonly IVpkService _vpkReader;

    public ModBuilderService(
        IItemCatalog catalog,
        IUIService ui,
        IAudioService audio,
        ISettingsService settings,
        IItemsService items,
        IHeroesService heroes,
        IVpkWriterService vpkWriter,
        IVpkService vpkReader) // 🔑 NUEVO
    {
        _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
        _ui = ui ?? throw new ArgumentNullException(nameof(ui));
        _audio = audio ?? throw new ArgumentNullException(nameof(audio));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _items = items ?? throw new ArgumentNullException(nameof(items));
        _heroes = heroes ?? throw new ArgumentNullException(nameof(heroes));
        _vpkWriter = vpkWriter ?? throw new ArgumentNullException(nameof(vpkWriter));
        _vpkReader = vpkReader ?? throw new ArgumentNullException(nameof(vpkReader)); // 🔑 NUEVO
    }

    // ════════════════════════════════════════════════════════════════
    // API PÚBLICA
    // ════════════════════════════════════════════════════════════════

    public void BuildMod()
    {
        try
        {
            // 🔑 Sigue usando frmMain por ahora (lo migramos en futuras sesiones)
            frmMain.frm.SelectTab(frmMain.frm.tabPage6);

            CloseDota();
            CleanPreviousMods();

            var kValue = LoadItemsDatabase();
            if (kValue == null) return;

            CopyExternalMods();
            ConfigureWorldItems(kValue);
            ConfigureMiscItems(kValue);
            ConfigureSpecialCases(kValue);
            ConfigureTaunts(kValue);
            ProcessAllHeroes(kValue);

            SaveAndPackage(kValue);
            UpdateGameInfo();

            modCommon.ReleaseMemory();
            OnBuildCompleted();
        }
        catch (Exception ex)
        {
            _ui.LogException("Error building mod", ex);
            _ui.ShowError($"Error generating mod: {ex.Message}");
        }
    }

    public void ClearGameInfo()
    {
        try
        {
            string text = _settings.Dota2Path + "\\gameinfo.gi";
            if (!File.Exists(text)) return;

            List<string> list = modCommon.ReadAllLines(text);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].ToLower().Contains(" game ") || list[i].ToLower().Contains(" mod ")
                    || list[i].ToLower().Contains("\tgame\t") || list[i].ToLower().Contains("\tmod\t"))
                {
                    string text2 = ClearWhiteSpaces(list[i]);
                    if (text2.ToLower().StartsWith("mod") && !text2.ToLower().Contains("core") && !text2.ToLower().Contains("dota"))
                    {
                        list.RemoveAt(i);
                        i--;
                    }
                    else if (text2.ToLower().StartsWith("game") && !text2.ToLower().Contains("core") && !text2.ToLower().Contains("dota"))
                    {
                        list.RemoveAt(i);
                        i--;
                    }
                }
            }
            File.WriteAllLines(text, list);
        }
        catch (Exception ex)
        {
            _ui.ShowError(ex.Message + ": " + ex.StackTrace);
        }
    }

    // ════════════════════════════════════════════════════════════════
    // PASOS DEL BUILD (privados)
    // ════════════════════════════════════════════════════════════════

    private void CloseDota()
    {
        try
        {
            var dotaProcess = Process.GetProcessesByName("dota2").FirstOrDefault();
            dotaProcess?.Kill();
        }
        catch { }
    }

    private void CleanPreviousMods()
    {
        _ui.WriteLine("Deleting Last Mods", loading: true);
        FileManager.DeleteDirectory(modCommon.VPKGeneratorLocation + "/pak01_dir/");
        FileManager.DeleteDirectory(Paths.GetParent(_settings.Dota2Path).ToString() + "/SkynetMod");

        string path = Path.Combine(modCommon.VPKGeneratorLocation, "pak01_dir");
        Paths.CleanDirectory(path);
    }

    private KValue LoadItemsDatabase()
    {
        _ui.WriteLine("Loading Items Database", loading: true);
        return DotaResources.kValue?.Clone();
    }

    private void CopyExternalMods()
    {
        _ui.WriteLine("Preparing external mods", loading: true);
        string externalPath = Path.Combine(modCommon.DataDirectory, "ExternalItems");

        if (!Directory.Exists(externalPath)) return;

        string[] files = Directory.GetFiles(externalPath, "*", SearchOption.AllDirectories);

        foreach (string sourceFile in files)
        {
            string destFile = modCommon.VPKGeneratorLocation + "/pak01_dir" + sourceFile.Replace(externalPath, "");
            if (!File.Exists(destFile))
            {
                try
                {
                    Paths.EnsureDirectory(Path.GetDirectoryName(destFile));
                    File.Copy(sourceFile, destFile, overwrite: true);
                }
                catch { }
            }
        }
    }

    private void ConfigureWorldItems(KValue kValue)
    {
        ConfigureItem(kValue, "Radiant Towers", "radianttowers", null, frmMain.manager.RadiantTowers);
        ConfigureItem(kValue, "Dire Towers", "diretowers", null, frmMain.manager.DireTowers);
        ConfigureItem(kValue, "Terrain", "terrain", null, frmMain.manager.Terrain);
        ConfigureItem(kValue, "Weather Effect", "weather", null, frmMain.manager.WeatherEffect);
        ConfigureItem(kValue, "Radiant Creeps", "radiantcreeps", null, frmMain.manager.RadiantCreeps);
        ConfigureItem(kValue, "Dire Creeps", "direcreeps", null, frmMain.manager.DireCreeps);
        ConfigureItem(kValue, "Cursor Pack", "cursor_pack", null, frmMain.manager.CursorPack);
        ConfigureItem(kValue, "Loading Screen", "loading_screen", null, frmMain.manager.LoadingScreen);
        ConfigureItem(kValue, "Versus Screen", "versus_screen", null, frmMain.manager.VersusScreen);
        ConfigureItem(kValue, "Courier", "courier", null, frmMain.manager.Courier);
        ConfigureItem(kValue, "Ward", "ward", null, frmMain.manager.Ward);
        ConfigureItem(kValue, "HUD Skin", "hud_skin", null, frmMain.manager.HUDSkin);
    }

    private void ConfigureMiscItems(KValue kValue)
    {
        ConfigureItem(kValue, "Music Pack", "music", null, frmMain.manager.MusicPack);
        ConfigureItem(kValue, "Announcer", "announcer", "announcer", frmMain.manager.Announcer);
        ConfigureItem(kValue, "Megakill Announcer", "announcer", "mega_kills", frmMain.manager.MegaKillAnnouncer);
        ConfigureItem(kValue, "Emoticons", "emoticon_tool", null, frmMain.manager.Emoticons);
    }

    private void ConfigureSpecialCases(KValue kValue)
    {
        // Multikill Banner (usa su propio ID, no busca en DefaultItems)
        _ui.WriteLine("Configuring Multikill Banner", loading: true);
        if (!string.IsNullOrEmpty(frmMain.manager.MultikillBanner?.ItemID))
        {
            SetItem(kValue, frmMain.manager.MultikillBanner.ItemID, frmMain.manager.MultikillBanner);
        }

        // Emblem (usa su propio ID, no busca en DefaultItems)
        _ui.WriteLine("Configuring Emblem", loading: true);
        if (!string.IsNullOrEmpty(frmMain.manager.Emblem?.ItemID))
        {
            SetItem(kValue, frmMain.manager.Emblem.ItemID, frmMain.manager.Emblem);
        }

        // Pets (busca por "DefaultPet" en lugar de "Default ")
        _ui.WriteLine("Configuring hero pets", loading: true);
        if (!string.IsNullOrEmpty(frmMain.manager.Pets?.ItemID))
        {
            string petDefaultID = _catalog.GetByCategoryFast(ItemCategory.DefaultItems)
                .Find(x => x.item_name != null && x.item_name.Contains("DefaultPet"))?.ItemID;
            SetItem(kValue, petDefaultID, frmMain.manager.Pets);
        }
    }

    private void ConfigureTaunts(KValue kValue)
    {
        _ui.WriteLine("Configuring Taunts", loading: true);
        Items defaultTaunt = _catalog.GetByCategoryFast(ItemCategory.DefaultItems)
            .Find(x => x.item_slot == "taunt");

        if (!string.IsNullOrEmpty(defaultTaunt?.ItemID))
        {
            KValue kValue2 = new KValue();
            kValue2.Children = kValue["Items"][defaultTaunt.ItemID].Children;
            kValue2["prefab"].Value = "wearable";
            kValue["Items"][defaultTaunt.ItemID].Children = kValue2.Children;
        }
    }

    private void ProcessAllHeroes(KValue kValue)
    {
        var taunts = _catalog.GetByCategoryFast(ItemCategory.Taunts);
        var defaultItems = _catalog.GetByCategoryFast(ItemCategory.DefaultItems);

        var heroesService = ServiceContainer.Get<IHeroesService>();
        var allHeroes = heroesService.GetAll();

        for (int num = 0; num < allHeroes.Count; num++)
        {
            string heroName = allHeroes[num].Name;
            _ui.WriteLine("PROCESSING " + _items.GetHeroName(heroName).ToUpper(), loading: true);

            var heroBundles = heroesService.GetBundles(heroName);
            foreach (Items bundle in heroBundles)
            {
                if (bundle.ItemID == "0000") continue;

                if (bundle.item_slot == "taunt")
                {
                    ProcessTauntForHero(kValue, taunts, heroName, bundle);
                    continue;
                }

                foreach (Items defaultItem in defaultItems)
                {
                    if (defaultItem.used_by_heroes == heroName
                        && defaultItem.item_slot == bundle.item_slot
                        && bundle.ItemID != "0000")
                    {
                        SetItem(kValue, defaultItem.ItemID, bundle);
                    }
                }
            }
        }
    }

    private void SaveAndPackage(KValue kValue)
    {
        string itemsScriptsPath = Path.Combine(
            modCommon.VPKGeneratorLocation, "pak01_dir", "scripts", "items");
        Directory.CreateDirectory(itemsScriptsPath);

        _ui.WriteLine("Saving Custom Items Database", loading: true);
        kValue.SaveToFile(Path.Combine(itemsScriptsPath, "items_game.txt"));

        Paths.EnsureDirectory(modCommon.VPKGeneratorLocation + "/pak01_dir/scripts/npc");
        FileManager.FileCopy(
            modCommon.DataDirectory + "/db/portraits.txt",
            modCommon.VPKGeneratorLocation + "/pak01_dir/scripts/npc/portraits.txt");

        _ui.WriteLine("Creating vpk file", loading: true);
        RunVpkPacker();

        string skynetModsPath = Paths.GetParent(_settings.Dota2Path).ToString() + "/SkynetMods";
        Paths.EnsureDirectory(skynetModsPath);
        FileManager.MakeFolderAllUsersAccessible(modCommon.VPKGeneratorLocation);
        FileManager.MakeFolderAllUsersAccessible(skynetModsPath);
        FileManager.FileMove(
            modCommon.VPKGeneratorLocation + "/pak01_dir.vpk",
            skynetModsPath + "/pak01_dir.vpk");
    }

    private void RunVpkPacker()
    {
        string sourceFolder = Path.Combine(modCommon.VPKGeneratorLocation, "pak01_dir");
        _vpkWriter.BuildFromFolder(sourceFolder);
    }

    private void UpdateGameInfo()
    {
        _ui.WriteLine("Verifing gameinfo", loading: true);
        if (!PrepareGameInfo())
        {
            _ui.ShowError("Error processing gameinfo.gi file");
        }
    }

    private void OnBuildCompleted()
    {
        if (_settings.OpenDota)
        {
            CloseDota();

            try
            {
                _ui.WriteLine("Opening Dota2", loading: true);
                string fileName = Path.Combine(Paths.GetParent(_settings.Dota2Path), "bin", "win32") + "/dota2.exe";
                Process.Start(fileName);
            }
            catch { }

            Thread.Sleep(1000);
        }

        _ui.WriteLine("", loading: true);
        frmMain.frm.SelectTab(frmMain.CurrentTab);
        _audio.PlaySound("sounds/ui/bonus_level.vsnd", toStop: false);
    }

    // ════════════════════════════════════════════════════════════════
    // HELPERS PRIVADOS
    // ════════════════════════════════════════════════════════════════

    private void ConfigureItem(KValue kValue, string displayName, string prefab, string itemSlot, Items managerItem)
    {
        _ui.WriteLine($"Configuring {displayName}", loading: true);

        if (string.IsNullOrEmpty(managerItem?.ItemID))
            return;

        var defaultItem = _catalog.GetByCategoryFast(ItemCategory.DefaultItems)
            .Find(x =>
                x.prefab == prefab
                && (itemSlot == null || x.item_slot == itemSlot)
                && x.name != null
                && x.name.StartsWith("Default "));

        if (defaultItem?.ItemID != null)
        {
            SetItem(kValue, defaultItem.ItemID, managerItem);
        }
    }

    private void ProcessTauntForHero(KValue kValue, List<Items> taunts, string heroName, Items bundle)
    {
        foreach (Items taunt in taunts)
        {
            if (taunt.used_by_heroes != heroName) continue;

            if (taunt.ItemID == bundle.ItemID)
            {
                if (kValue["Items"][bundle.ItemID].ContainsKey("baseitem"))
                {
                    kValue["Items"][bundle.ItemID]["baseitem"].Value = "1";
                }
                else
                {
                    kValue["Items"][bundle.ItemID].Children.Add(new KValue("baseitem", "1"));
                }
            }
            else if (kValue["Items"][bundle.ItemID].ContainsKey("baseitem"))
            {
                kValue["Items"][bundle.ItemID].RemoveKey("baseitem");
            }
        }
    }

    private void SetItem(KValue kValue, string defaultItem, Items currentItem)
    {
        if (currentItem.ItemID == "0000") return;

        int num = 1;
        KValue kValue2 = new KValue();
        kValue2.Children = kValue["Items"][currentItem.ItemID].Clone().Children;

        if (currentItem.style != null && !string.IsNullOrEmpty(currentItem.style.ID) && currentItem.style.ID != "0")
        {
            if (kValue2.ContainsKey("visuals"))
            {
                foreach (var current in kValue2["visuals"].Children)
                {
                    if (!current.Name.StartsWith("asset_modifier") || !current.ContainsKey("type"))
                        continue;

                    if (current.ContainsKey("style"))
                    {
                        if (current["style"].Value.ToString() == "0")
                        {
                            current["style"].Value = currentItem.style.ID;
                        }
                        else if (current["style"].Value.ToString() == currentItem.style.ID)
                        {
                            current.RemoveKey("style");
                        }
                    }
                    if (current["type"].Value.ToString() == "activity" && current.ContainsKey("style"))
                    {
                        current.RemoveKey("style");
                    }
                }
            }

            Items item = _items.GetItem(currentItem.ItemID);
            for (int i = 0; i < item?.styles.Count; i++)
            {
                if (item.styles[i].ID != currentItem.style.ID)
                {
                    kValue2["visuals"]["styles"].RemoveKey(item.styles[i].ID);
                }
            }
        }

        if (!kValue2.ContainsKey("baseitem") && num == 1)
        {
            kValue2.Children.Add(new KValue("baseitem", "1"));
            num++;
        }

        if (kValue2.ContainsKey("prefab"))
        {
            kValue2["prefab"].Value = kValue["Items"][defaultItem]["prefab"]?.Value;
        }

        kValue["Items"][defaultItem].Children = kValue2.Children;
        modCommon.ExtractFiles = false;

        if (!modCommon.ExtractFiles) return;

        string text = Path.Combine(modCommon.VPKGeneratorLocation, "pak01_dir");
        Paths.EnsureDirectory(text);

        if (kValue2.ContainsKey("model_player"))
        {
            DotaResources.ExtractFile(kValue2["model_player"].Value?.ToString(), text);
        }

        if (!kValue2.ContainsKey("visuals")) return;

        foreach (var current2 in kValue2["visuals"].Children)
        {
            if (!current2.Name.StartsWith("asset_modifier")) continue;

            foreach (var current3 in current2.Children)
            {
                if (current3.Value != null && current3.Value.ToString().Contains("/"))
                {
                    string[] source = current3.Value.ToString().Split('/');
                    if (source.Count() > 2)
                    {
                        DotaResources.ExtractFile(current3.Value.ToString(), text);
                    }
                }
            }
        }
    }

    private bool PrepareGameInfo()
    {
        ClearGameInfo();
        try
        {
            string text = _settings.Dota2Path + "\\gameinfo.gi";
            if (!File.Exists(text)) return false;

            List<string> list = modCommon.ReadAllLines(text);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Contains("Game_Language") && !list.Contains("\t\t\tGame\t\t\t\tSkynetMods"))
                {
                    list[i] = "\t\t\tGame\t\t\t\tSkynetMods" + Environment.NewLine + list[i];
                }
                if (list[i].Contains("Game\t\t\t\tdota") && !list.Contains("\t\t\tGame\t\t\t\tSkynetMods"))
                {
                    list[i] = "\t\t\tGame\t\t\t\tSkynetMods" + Environment.NewLine + list[i];
                }
                if (list[i].Contains("Mod\t\t\t\t\tdota") && !list.Contains("\t\t\tMod\t\t\t\tSkynetMods"))
                {
                    list[i] = "\t\t\tMod\t\t\t\t    SkynetMods" + Environment.NewLine + list[i];
                }
            }
            modCommon.WriteAllLines(text, list);
            return true;
        }
        catch (Exception ex)
        {
            _ui.LogException(ex);
            return false;
        }
    }

    private static string ClearWhiteSpaces(string v)
    {
        string text = "";
        for (int i = 0; i < v.Length; i++)
        {
            char c = v[i];
            if (!char.IsWhiteSpace(c))
            {
                text += c;
            }
        }
        return text;
    }
}