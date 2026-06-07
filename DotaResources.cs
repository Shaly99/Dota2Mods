using SkiaSharp;
using SKYNET;
using ValveResourceFormat;
using ValveResourceFormat.ResourceTypes;

public class DotaResources
{
    private static IVpkService Vpk => ServiceContainer.Get<IVpkService>();
    private static IResourceLoaderService Loader => ServiceContainer.Get<IResourceLoaderService>();

    /// <summary>
    /// Acceso al KValue principal (items_game.txt).
    /// Wrapper que delega al IResourceLoaderService.
    /// </summary>
    public static KValue kValue => Loader?.MainData;

    // ════════════════════════════════════════════════════════════
    // CONVERSIÓN SKBITMAP → BITMAP
    // ════════════════════════════════════════════════════════════

    private static Bitmap SKBitmapToBitmap(SKBitmap skBitmap)
    {
        using SKImage image = SKImage.FromBitmap(skBitmap);
        using SKData data = image.Encode(SKEncodedImageFormat.Png, 100);
        using MemoryStream ms = new MemoryStream(data.ToArray());
        using Bitmap temp = new Bitmap(ms);
        return new Bitmap(temp);
    }

    // ════════════════════════════════════════════════════════════
    // EXTRACCIÓN DE RECURSOS (sin cambios, ya están bien)
    // ════════════════════════════════════════════════════════════

    public static void ExtractResources(string ruta, string ToFolder)
    {
        if (!Vpk.IsLoaded) return;

        string text = Path.Combine(modCommon.DataDirectory);
        Paths.EnsureDirectory(text);

        var entries = Vpk.GetEntriesByExtension("vtex_c")
            .Where((PackageEntry e) => e.DirectoryName != null && e.DirectoryName.Contains(ruta))
            .ToList();

        if (entries.Count == 0) return;

        var progress = ServiceContainer.TryGet<IProgressReporter>();
        progress?.SetCurrentTask($"📁 {ruta}");

        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };

        Parallel.ForEach(entries, parallelOptions, (item) =>
        {
            try
            {
                string text2 = Path.Combine(text, item.DirectoryName);
                string path = item.FileName.Replace("_png", "") + ".png";
                string text3 = Path.Combine(text2, path);

                if (File.Exists(text3))
                {
                    progress?.IncrementProgress();
                    return;
                }

                byte[] output;
                Vpk.ReadEntry(item, out output);

                try
                {
                    using MemoryStream input = new MemoryStream(output);
                    Resource resource = new Resource();
                    resource.Read(input);

                    Texture obj = resource.DataBlock as Texture
                        ?? throw new InvalidOperationException("DataBlock no es Texture");

                    Paths.EnsureDirectory(text2);

                    using Bitmap bmp = SKBitmapToBitmap(obj.GenerateBitmap());
                    bmp.Save(text3);
                }
                catch (Exception)
                {
                    try
                    {
                        string fileName = item.FileName.Replace("_png", "") + ".png";
                        PackageEntry packageEntry = Vpk.FindEntry(item.DirectoryName, fileName);

                        if (packageEntry != null)
                        {
                            Vpk.ReadEntry(packageEntry, out var output2);
                            Paths.EnsureDirectory(text2);
                            File.WriteAllBytes(text3, output2);
                        }
                    }
                    catch { }
                }
            }
            catch (Exception) { }
            finally
            {
                progress?.IncrementProgress();
            }
        });
    }

    public static void ExtractFile(string file, string folder)
    {
        try
        {
            Paths.EnsureDirectory(folder);
            string extension = Path.GetExtension(file);
            extension = extension.Replace(".", "");
            string fullPath = Path.GetFullPath(file);

            if (!Vpk.IsLoaded || extension.Contains("vmap")) return;

            PackageEntry packageEntry = Vpk.GetEntriesByExtension(extension + "_c")
                .Find((PackageEntry x) => file.Contains(x.DirectoryName) && file.Contains(x.FileName));

            if (packageEntry == null) return;

            string text = Path.Combine(folder, packageEntry.DirectoryName);
            string path = Path.Combine(text, packageEntry.FileName + "." + extension + "_c");

            if (File.Exists(path)) return;

            try
            {
                Paths.EnsureDirectory(text);
                Vpk.ReadEntry(packageEntry, out var output);
                File.WriteAllBytes(path, output);
            }
            catch (Exception ex)
            {
                modCommon.Save(ex);
            }
        }
        catch (Exception ex2)
        {
            modCommon.Save(ex2);
        }
    }

    public static Bitmap ExtractAndGenerate(string ruta)
    {
        string filePath = "panorama/images/" + ruta + "_png.vtex_c";
        string text = Path.Combine(modCommon.DataDirectory);
        Paths.EnsureDirectory(text);

        if (Vpk.IsLoaded)
        {
            PackageEntry packageEntry = Vpk.FindEntry(filePath);
            if (packageEntry != null)
            {
                string text2 = Path.Combine(text, packageEntry.DirectoryName);
                string text3 = Path.Combine(text2, packageEntry.FileName.Replace("_png", string.Empty) + ".png");

                if (!File.Exists(text3))
                {
                    try
                    {
                        Vpk.ReadEntry(packageEntry, out var output);
                        Resource resource = new Resource();
                        resource.Read(new MemoryStream(output));

                        Texture texture = resource.DataBlock as Texture
                            ?? throw new InvalidOperationException("DataBlock no es Texture");

                        Paths.EnsureDirectory(text2);

                        Bitmap result = SKBitmapToBitmap(texture.GenerateBitmap());
                        result.Save(text3);
                        return result;
                    }
                    catch (Exception ex)
                    {
                        modCommon.Save(ex);
                    }
                }
                else
                {
                    try
                    {
                        Image cached = ImageCache.GetOrLoad(text3);
                        return cached as Bitmap ?? new Bitmap(cached);
                    }
                    catch { }
                }
            }
        }
        return Resources.default_item;
    }

    // ════════════════════════════════════════════════════════════
    // CARGAS DE DATOS (delegan al servicio)
    // ════════════════════════════════════════════════════════════

    /// <summary>
    /// Carga items_game.txt y popula prefabs, colors, items.
    /// Delega al ResourceLoaderService.
    /// </summary>
    internal static void LoadResources()
    {
        Loader?.LoadMainData();
    }

    /// <summary>
    /// Carga descripciones de items desde el archivo de idioma.
    /// </summary>
    internal static void LoadItemsDescriptions()
    {
        Loader?.LoadItemDescriptions();
    }

    /// <summary>
    /// Carga la rich presence (nombres de héroes).
    /// </summary>
    internal static void LoadRichPresence()
    {
        Loader?.LoadRichPresence();
    }

    // ════════════════════════════════════════════════════════════
    // PROCESAMIENTO DE ITEMS (llamado desde ResourceLoaderService)
    // ════════════════════════════════════════════════════════════

    /// <summary>
    /// Procesa una entrada del nodo "items" de items_game.txt.
    /// Crea el objeto Items correspondiente y lo agrega al catálogo
    /// según su prefab/item_slot/etc.
    /// 
    /// Es público porque lo llama ResourceLoaderService.LoadMainData().
    /// </summary>
    public static void ProcessItemEntry(KValue child3)
    {
        // Obtener héroe asociado
        string used_by_heroes = "";
        if (child3.ContainsKey("used_by_heroes"))
        {
            try
            {
                foreach (var current4 in child3["used_by_heroes"].Children)
                {
                    used_by_heroes = current4.Name;
                }
            }
            catch (Exception ex)
            {
                modCommon.Save(ex);
            }
        }

        // Obtener visuals (estilos y assets)
        string text = "";
        string text2 = "";
        string asset = "";
        string asset2 = "";
        List<Style> list2 = new List<Style>();

        if (child3.ContainsKey("visuals"))
        {
            try
            {
                foreach (var current5 in child3["visuals"].Children)
                {
                    if (current5.Name == "styles")
                    {
                        try
                        {
                            foreach (var styleChild in child3["visuals"]["styles"].Children)
                            {
                                text = styleChild.Name;
                                text2 = child3["visuals"]["alternate_icons"][styleChild.Name]["icon_path"].AsString();
                                list2.Add(new Style { ID = text, icon_path = text2 });
                            }
                        }
                        catch (Exception ex2) { modCommon.Save(ex2); }
                    }

                    text2 = "";
                    asset2 = "";

                    if (!current5.Name.StartsWith("asset_modifier")) continue;

                    try
                    {
                        bool foundSound = false;
                        foreach (var assetChild in child3["visuals"][current5.Name].Children)
                        {
                            if (assetChild.Name == "asset")
                            {
                                asset = assetChild.Value.ToString();
                                if (assetChild.Value.ToString().Contains(".vsnd"))
                                {
                                    asset2 = assetChild.Value.ToString();
                                    foundSound = true;
                                    break;
                                }
                            }
                        }
                        if (foundSound) break;
                    }
                    catch (Exception ex3) { modCommon.Save(ex3); }
                }
            }
            catch (Exception ex4) { modCommon.Save(ex4); }
        }

        string prefab = child3["prefab"].AsString();

        // ════════════════════════════════════════
        // WEARABLE (items normales y pets)
        // ════════════════════════════════════════
        if (prefab == "wearable")
        {
            Items item3 = new Items
            {
                ItemID = child3.Name,
                name = child3["name"].AsString(),
                styles = list2,
                prefab = prefab,
                image_inventory = child3["image_inventory"].AsString(),
                item_description = child3["item_description"].AsString(),
                item_name = child3["item_name"].AsString(),
                item_rarity = modCommon.GetRarityFromPrefabs(child3["item_rarity"].AsString(), "wearable"),
                item_slot = modCommon.GetSlotFromPrefabs(child3["item_slot"].AsString(), "wearable"),
                item_type_name = child3["item_type_name"].AsString(),
                used_by_heroes = used_by_heroes
            };

            if (child3["item_type_name"].AsString().ToLower().Contains("_pet"))
            {
                frmMain.frm.Catalog.Add(ItemCategory.Pets, item3);

                var existingDefault = frmMain.frm.Catalog.GetByCategoryFast(ItemCategory.Pets)
                    .Find(x => x.ItemID == "0000");

                if (existingDefault == null)
                {
                    var defaultPet = new Items
                    {
                        item_slot = "summon",
                        prefab = "wearable",
                        image_inventory = "/pets/",
                        name = "All Heroes' Default Pet",
                        ItemID = "0000",
                        item_rarity = ""
                    };
                    frmMain.frm.Catalog.Add(ItemCategory.Pets, defaultPet);
                }
            }
            else
            {
                frmMain.frm.Catalog.Add(ItemCategory.Items, item3);
            }
        }

        // ════════════════════════════════════════
        // DEFAULT ITEMS
        // ════════════════════════════════════════
        if (child3["name"].AsString().StartsWith("Default "))
        {
            Items item4 = new Items
            {
                ItemID = child3.Name,
                name = child3["name"].AsString(),
                prefab = prefab,
                image_inventory = child3["image_inventory"].AsString(),
                item_description = child3["item_description"].AsString(),
                item_name = child3["item_name"].AsString(),
                item_rarity = modCommon.GetRarityFromPrefabs(child3["item_rarity"].AsString(), "default_item"),
                item_slot = modCommon.GetSlotFromPrefabs(child3["item_slot"].AsString(), "default_item"),
                item_type_name = child3["item_type_name"].AsString()
            };
            frmMain.frm.Catalog.Add(ItemCategory.DefaultItems, item4);
        }

        if (prefab == "default_item")
        {
            Items item5 = new Items
            {
                ItemID = child3.Name,
                name = child3["name"].AsString(),
                prefab = prefab,
                image_inventory = child3["image_inventory"].AsString(),
                item_description = child3["item_description"].AsString(),
                item_name = child3["item_name"].AsString(),
                item_rarity = modCommon.GetRarityFromPrefabs(child3["item_rarity"].AsString(), "default_item"),
                item_slot = modCommon.GetSlotFromPrefabs(child3["item_slot"].AsString(), "default_item"),
                item_type_name = child3["item_type_name"].AsString(),
                used_by_heroes = used_by_heroes
            };
            frmMain.frm.Catalog.Add(ItemCategory.DefaultItems, item5);
        }

        // ════════════════════════════════════════
        // BUNDLE LIST
        // ════════════════════════════════════════
        List<string> list3 = new List<string>();
        if (child3.ContainsKey("bundle"))
        {
            try
            {
                foreach (var current6 in child3["bundle"].Children)
                {
                    list3.Add(current6.Name);
                }
            }
            catch (Exception ex5) { modCommon.Save(ex5); }
        }

        // ════════════════════════════════════════
        // BUNDLE
        // ════════════════════════════════════════
        if (prefab == "bundle")
        {
            Items item6 = new Items
            {
                ItemID = child3.Name,
                bundles = list3,
                name = child3["name"].AsString(),
                prefab = prefab,
                image_inventory = child3["image_inventory"].AsString(),
                item_description = child3["item_description"].AsString(),
                item_name = child3["item_name"].AsString(),
                item_rarity = modCommon.GetRarityFromPrefabs(child3["item_rarity"].AsString(), "default_item"),
                item_type_name = child3["item_type_name"].AsString(),
                used_by_heroes = used_by_heroes
            };
            frmMain.frm.Catalog.Add(ItemCategory.Bundles, item6);
        }

        // ════════════════════════════════════════
        // TAUNT
        // ════════════════════════════════════════
        if (prefab == "taunt")
        {
            Items item7 = new Items
            {
                ItemID = child3.Name,
                styles = list2,
                prefab = prefab,
                name = child3["name"].AsString(),
                item_slot = "taunt",
                image_inventory = child3["image_inventory"].AsString(),
                item_description = child3["item_description"].AsString(),
                item_name = child3["item_name"].AsString(),
                item_rarity = modCommon.GetRarityFromPrefabs(child3["item_rarity"].AsString(), "default_item"),
                used_by_heroes = used_by_heroes
            };
            frmMain.frm.Catalog.Add(ItemCategory.Taunts, item7);
        }

        // ════════════════════════════════════════
        // MUSIC
        // ════════════════════════════════════════
        if (prefab == "music")
        {
            Items item8 = new Items
            {
                ItemID = child3.Name,
                styles = list2,
                prefab = prefab,
                name = child3["name"].AsString(),
                image_inventory = child3["image_inventory"].AsString(),
                item_description = child3["item_description"].AsString(),
                item_name = child3["item_name"].AsString(),
                item_rarity = modCommon.GetRarityFromPrefabs(child3["item_rarity"].AsString(), "default_item"),
                asset = asset2,
                used_by_heroes = used_by_heroes
            };
            frmMain.frm.Catalog.Add(ItemCategory.MusicPack, item8);
        }

        // ════════════════════════════════════════
        // ANNOUNCER y MEGA-KILL ANNOUNCER
        // ════════════════════════════════════════
        if (prefab == "announcer")
        {
            string itemSlot = child3["item_slot"].AsString();

            if (itemSlot == "announcer")
            {
                Items item9 = new Items
                {
                    ItemID = child3.Name,
                    styles = list2,
                    prefab = prefab,
                    name = child3["name"].AsString(),
                    image_inventory = child3["image_inventory"].AsString(),
                    item_description = child3["item_description"].AsString(),
                    item_name = child3["item_name"].AsString(),
                    item_slot = itemSlot,
                    item_rarity = modCommon.GetRarityFromPrefabs(child3["item_rarity"].AsString(), "default_item"),
                    asset = asset2,
                    used_by_heroes = used_by_heroes
                };
                frmMain.frm.Catalog.Add(ItemCategory.Announcer, item9);
            }

            if (itemSlot == "mega_kills")
            {
                Items item10 = new Items
                {
                    ItemID = child3.Name,
                    styles = list2,
                    prefab = prefab,
                    name = child3["name"].AsString(),
                    image_inventory = child3["image_inventory"].AsString(),
                    item_description = child3["item_description"].AsString(),
                    item_name = child3["item_name"].AsString(),
                    item_slot = itemSlot,
                    item_rarity = modCommon.GetRarityFromPrefabs(child3["item_rarity"].AsString(), "default_item"),
                    asset = asset2,
                    used_by_heroes = used_by_heroes
                };
                frmMain.frm.Catalog.Add(ItemCategory.MegaKillAnnouncer, item10);
            }
        }

        // ════════════════════════════════════════
        // TERRAIN
        // ════════════════════════════════════════
        if (prefab == "terrain")
        {
            Items item11 = new Items
            {
                ItemID = child3.Name,
                styles = list2,
                prefab = prefab,
                name = child3["name"].AsString(),
                image_inventory = child3["image_inventory"].AsString(),
                item_description = child3["item_description"].AsString(),
                item_name = child3["item_name"].AsString(),
                item_rarity = modCommon.GetRarityFromPrefabs(child3["item_rarity"].AsString(), "default_item"),
                used_by_heroes = used_by_heroes
            };
            frmMain.frm.Catalog.Add(ItemCategory.Terrain, item11);
        }

        // ════════════════════════════════════════
        // MISC (weather y multikill_banner)
        // ════════════════════════════════════════
        if (prefab == "misc")
        {
            string itemSlot = child3["item_slot"].AsString();

            if (itemSlot == "weather")
            {
                Items item12 = new Items
                {
                    ItemID = child3.Name,
                    styles = list2,
                    prefab = "misc",
                    item_slot = itemSlot,
                    name = child3["name"].AsString(),
                    image_inventory = child3["image_inventory"].AsString(),
                    item_description = child3["item_description"].AsString(),
                    item_name = child3["item_name"].AsString(),
                    item_rarity = modCommon.GetRarityFromPrefabs(child3["item_rarity"].AsString(), "default_item"),
                    used_by_heroes = used_by_heroes
                };
                frmMain.frm.Catalog.Add(ItemCategory.WeatherEffect, item12);
            }

            if (itemSlot == "multikill_banner")
            {
                Items item13 = new Items
                {
                    ItemID = child3.Name,
                    styles = list2,
                    prefab = "misc",
                    item_slot = itemSlot,
                    name = child3["name"].AsString(),
                    image_inventory = child3["image_inventory"].AsString(),
                    item_description = child3["item_description"].AsString(),
                    item_name = child3["item_name"].AsString(),
                    item_rarity = modCommon.GetRarityFromPrefabs(child3["item_rarity"].AsString(), "default_item"),
                    used_by_heroes = used_by_heroes
                };
                frmMain.frm.Catalog.Add(ItemCategory.MultikillBanner, item13);

                var existingDefault = frmMain.frm.Catalog.GetByCategoryFast(ItemCategory.MultikillBanner)
                    .Find(x => x.ItemID == "0000");

                if (existingDefault == null)
                {
                    var defaultBanner = new Items
                    {
                        prefab = "misc",
                        image_inventory = "econ/testitem_slot_empty",
                        name = "Default Multikill-Banner",
                        ItemID = "0000",
                        item_rarity = "",
                        item_slot = "multikill_banner"
                    };
                    frmMain.frm.Catalog.Add(ItemCategory.MultikillBanner, defaultBanner);
                }
            }
        }

        // ════════════════════════════════════════
        // WEATHER (prefab alternativo)
        // ════════════════════════════════════════
        if (prefab == "weather")
        {
            Items item14 = new Items
            {
                ItemID = child3.Name,
                styles = list2,
                prefab = "misc",
                item_slot = "weather",
                name = child3["name"].AsString(),
                image_inventory = child3["image_inventory"].AsString(),
                item_description = child3["item_description"].AsString(),
                item_name = child3["item_name"].AsString(),
                item_rarity = modCommon.GetRarityFromPrefabs(child3["item_rarity"].AsString(), "default_item"),
                used_by_heroes = used_by_heroes
            };
            frmMain.frm.Catalog.Add(ItemCategory.WeatherEffect, item14);
        }

        // ════════════════════════════════════════
        // CREEPS y TOWERS
        // ════════════════════════════════════════
        if (prefab == "radiantcreeps")
        {
            Items item15 = BuildSimpleItem(child3, prefab, list2, used_by_heroes);
            frmMain.frm.Catalog.Add(ItemCategory.RadiantCreeps, item15);
        }

        if (prefab == "direcreeps")
        {
            Items item16 = BuildSimpleItem(child3, prefab, list2, used_by_heroes);
            frmMain.frm.Catalog.Add(ItemCategory.DireCreeps, item16);
        }

        if (prefab == "radianttowers")
        {
            Items item17 = BuildSimpleItem(child3, prefab, list2, used_by_heroes);
            frmMain.frm.Catalog.Add(ItemCategory.RadiantTowers, item17);
        }

        if (prefab == "diretowers")
        {
            Items item18 = BuildSimpleItem(child3, prefab, list2, used_by_heroes);
            frmMain.frm.Catalog.Add(ItemCategory.DireTowers, item18);
        }

        if (prefab == "versus_screen")
        {
            Items item19 = BuildSimpleItem(child3, prefab, list2, used_by_heroes);
            frmMain.frm.Catalog.Add(ItemCategory.VersusScreen, item19);
        }

        if (prefab == "cursor_pack")
        {
            Items item20 = BuildSimpleItem(child3, prefab, list2, used_by_heroes);
            frmMain.frm.Catalog.Add(ItemCategory.CursorPack, item20);
        }

        if (prefab == "emblem")
        {
            Items item21 = BuildSimpleItem(child3, prefab, list2, used_by_heroes);
            frmMain.frm.Catalog.Add(ItemCategory.Emblem, item21);

            var existingDefault = frmMain.frm.Catalog.GetByCategoryFast(ItemCategory.Emblem)
                .Find(x => x.ItemID == "0000");

            if (existingDefault == null)
            {
                var defaultEmblem = new Items
                {
                    prefab = "emblem",
                    image_inventory = "econ/testitem_slot_empty",
                    name = "Default Emblem",
                    ItemID = "0000",
                    item_rarity = ""
                };
                frmMain.frm.Catalog.Add(ItemCategory.Emblem, defaultEmblem);
            }
        }

        if (prefab == "emoticon_tool")
        {
            Items item22 = BuildSimpleItem(child3, prefab, list2, used_by_heroes);
            frmMain.frm.Catalog.Add(ItemCategory.Emoticons, item22);
        }

        if (prefab == "hud_skin")
        {
            Items item23 = BuildSimpleItem(child3, prefab, list2, used_by_heroes);
            frmMain.frm.Catalog.Add(ItemCategory.HUDSkin, item23);
        }

        if (prefab == "courier")
        {
            Items item24 = BuildSimpleItem(child3, prefab, list2, used_by_heroes);
            frmMain.frm.Catalog.Add(ItemCategory.Courier, item24);
        }

        if (prefab == "ward")
        {
            Items item25 = BuildSimpleItem(child3, prefab, list2, used_by_heroes);
            frmMain.frm.Catalog.Add(ItemCategory.Ward, item25);
        }

        if (prefab == "loading_screen")
        {
            Items item26 = new Items
            {
                ItemID = child3.Name,
                styles = list2,
                prefab = prefab,
                name = child3["name"].AsString(),
                asset = asset,
                image_inventory = child3["image_inventory"].AsString(),
                item_description = child3["item_description"].AsString(),
                item_name = child3["item_name"].AsString(),
                item_rarity = modCommon.GetRarityFromPrefabs(child3["item_rarity"].AsString(), "default_item"),
                used_by_heroes = used_by_heroes
            };
            frmMain.frm.Catalog.Add(ItemCategory.LoadingScreen, item26);
        }

        if (prefab == "streak_effect")
        {
            Items item27 = new Items
            {
                ItemID = child3.Name,
                styles = list2,
                prefab = prefab,
                name = child3["name"].AsString(),
                asset = asset,
                image_inventory = child3["image_inventory"].AsString(),
                item_description = child3["item_description"].AsString(),
                item_name = child3["item_name"].AsString(),
                item_rarity = modCommon.GetRarityFromPrefabs(child3["item_rarity"].AsString(), "default_item"),
                used_by_heroes = used_by_heroes
            };
            frmMain.frm.Catalog.Add(ItemCategory.StreakEffect, item27);
        }
    }

    /// <summary>
    /// Helper para construir items "simples" (que tienen los mismos campos).
    /// Reduce duplicación en categorías como creeps, towers, ward, courier, etc.
    /// </summary>
    private static Items BuildSimpleItem(KValue child, string prefab, List<Style> styles, string usedByHeroes)
    {
        return new Items
        {
            ItemID = child.Name,
            styles = styles,
            prefab = prefab,
            name = child["name"].AsString(),
            image_inventory = child["image_inventory"].AsString(),
            item_description = child["item_description"].AsString(),
            item_name = child["item_name"].AsString(),
            item_rarity = modCommon.GetRarityFromPrefabs(child["item_rarity"].AsString(), "default_item"),
            used_by_heroes = usedByHeroes
        };
    }
}