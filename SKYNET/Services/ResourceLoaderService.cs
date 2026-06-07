namespace SKYNET;

/// <summary>
/// Implementación del servicio de carga de recursos.
/// Encapsula el estado de kValue (antes estaba como static en DotaResources).
/// </summary>
public class ResourceLoaderService : IResourceLoaderService
{
    private readonly IItemCatalog _catalog;
    private readonly IPrefabsService _prefabs;
    private readonly IRarityColorService _colors;
    private readonly IUIService _ui;

    private KValue _mainData;

    public bool IsLoaded => _mainData != null;
    public KValue MainData => _mainData;

    public ResourceLoaderService(
        IItemCatalog catalog,
        IPrefabsService prefabs,
        IRarityColorService colors,
        IUIService ui)
    {
        _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
        _prefabs = prefabs ?? throw new ArgumentNullException(nameof(prefabs));
        _colors = colors ?? throw new ArgumentNullException(nameof(colors));
        _ui = ui ?? throw new ArgumentNullException(nameof(ui));
    }

    // ════════════════════════════════════════════════════════════
    // CARGA PRINCIPAL
    // ════════════════════════════════════════════════════════════

    public void LoadMainData()
    {
        _mainData = KValue.LoadAsText(Path.Combine("data", "db", "items_game.txt"));
        if (_mainData == null)
        {
            _ui.LogException(new InvalidOperationException("No se pudo cargar items_game.txt"));
            return;
        }

        LoadPrefabs();
        LoadColors();
        LoadItems();
    }

    private void LoadPrefabs()
    {
        if (!_mainData.ContainsKey("prefabs")) return;

        foreach (KValue child in _mainData["prefabs"].Children)
        {
            var item = new prefabs
            {
                Name = child.Name,
                item_quality = child["item_quality"].AsString(),
                item_rarity = child["item_rarity"].AsString(),
                item_slot = child["item_slot"].AsString()
            };

            // Registrar en lista vieja (compatibilidad)
            frmMain.frm.prefabs.Add(item);

            // Registrar en servicio
            _prefabs.RegisterPrefab(item);
        }
    }

    private void LoadColors()
    {
        if (!_mainData.ContainsKey("colors")) return;

        foreach (KValue child in _mainData["colors"].Children)
        {
            var item = new Colors
            {
                Name = child.Name,
                color_name = child["color_name"].AsString(),
                hex_color = child["hex_color"].AsString()
            };

            // Registrar en lista vieja (compatibilidad)
            frmMain.frm.Colors.Add(item);

            // Registrar en servicio
            _colors.RegisterColor(item);
        }
    }

    private void LoadItems()
    {
        if (!_mainData.ContainsKey("items")) return;

        foreach (KValue child in _mainData["items"].Children)
        {
            // Toda la lógica de carga de items la mantenemos en DotaResources
            // por ahora (es muy grande y específica)
            DotaResources.ProcessItemEntry(child);
        }
    }

    // ════════════════════════════════════════════════════════════
    // CARGA DE TEXTOS
    // ════════════════════════════════════════════════════════════

    public void LoadItemDescriptions()
    {
        string langFile = modCommon.CurrentLanguage == modCommon.Language.Spanish
            ? "lang_spanish.txt"
            : "lang_english.txt";

        KValue kValue = KValue.LoadAsText(Path.Combine("data", "db", langFile));
        if (kValue == null || !kValue.ContainsKey("Tokens")) return;

        foreach (KValue child in kValue["Tokens"].Children)
        {
            frmMain.frm.itemDescription.Add(new ItemDescription
            {
                item_description = "#" + child.Name,
                description = child.Value.ToString()
            });
        }
    }

    public void LoadRichPresence()
    {
        KValue kValue = KValue.LoadAsText(
            Path.Combine(modCommon.DataDirectory, "rich_presence", "570", "spanish.vdf"));

        if (kValue == null || !kValue.ContainsKey("Tokens")) return;

        foreach (KValue child in kValue["Tokens"].Children)
        {
            frmMain.frm.rich_presence.Add(new ItemDescription
            {
                item_description = child.Name,
                description = child.Value.ToString()
            });
        }
    }
}