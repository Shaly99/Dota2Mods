using System;
using System.Collections.Generic;
using System.IO;

namespace SKYNET;

/// <summary>
/// Implementación del servicio de atributos de héroes.
/// Lee npc_heroes.txt extraído del VPK y mapea cada héroe a su atributo primario.
/// </summary>
public class HeroAttributesService : IHeroAttributesService
{
    private readonly Dictionary<string, HeroAttribute> _attributes
        = new Dictionary<string, HeroAttribute>(StringComparer.OrdinalIgnoreCase);

    private readonly IUIService _ui;
    private readonly IVpkService _vpkReader;

    public bool IsLoaded { get; private set; }

    public HeroAttributesService(IUIService ui, IVpkService vpkReader)
    {
        _ui = ui ?? throw new ArgumentNullException(nameof(ui));
        _vpkReader = vpkReader ?? throw new ArgumentNullException(nameof(vpkReader));
    }

    // ════════════════════════════════════════════════════════════
    // CARGA
    // ════════════════════════════════════════════════════════════

    public void Load()
    {
        try
        {
            // Primero intentamos leer del disco (si ya fue extraído)
            string localPath = Path.Combine(modCommon.DataDirectory, "db", "npc_heroes.txt");

            if (File.Exists(localPath))
            {
                LoadFromFile(localPath);
                IsLoaded = true;
                return;
            }

            // Si no está en disco, lo leemos directamente del VPK
            LoadFromVpk();
            IsLoaded = true;
        }
        catch (Exception ex)
        {
            _ui.LogException("Error cargando atributos de héroes", ex);
        }
    }

    private void LoadFromFile(string path)
    {
        KValue kValue = KValue.LoadAsText(path);
        if (kValue == null) return;

        ParseHeroes(kValue);
    }

    private void LoadFromVpk()
    {
        var entry = _vpkReader.FindEntry("scripts/npc/npc_heroes.txt");
        if (entry == null)
        {
            _ui.LogException(new FileNotFoundException(
                "No se encontró scripts/npc/npc_heroes.txt en el VPK"));
            return;
        }

        _vpkReader.ReadEntry(entry, out byte[] data, validateCrc: false);
        if (data == null || data.Length == 0) return;

        // Guardar en disco para uso futuro
        string targetPath = Path.Combine(modCommon.DataDirectory, "db", "npc_heroes.txt");
        Paths.EnsureDirectory(Path.GetDirectoryName(targetPath));
        File.WriteAllBytes(targetPath, data);

        // Y parsear
        LoadFromFile(targetPath);
    }

    // ════════════════════════════════════════════════════════════
    // PARSING
    // ════════════════════════════════════════════════════════════

    private void ParseHeroes(KValue kValue)
    {
        foreach (KValue heroEntry in kValue.Children)
        {
            // Saltear entradas que no son héroes (como "Version", base templates, etc.)
            if (!heroEntry.Name.StartsWith("npc_dota_hero_", StringComparison.OrdinalIgnoreCase))
                continue;

            string attributeRaw = heroEntry["AttributePrimary"]?.AsString() ?? "";

            HeroAttribute attribute = attributeRaw.ToUpperInvariant() switch
            {
                "DOTA_ATTRIBUTE_STRENGTH" => HeroAttribute.Strength,
                "DOTA_ATTRIBUTE_AGILITY" => HeroAttribute.Agility,
                "DOTA_ATTRIBUTE_INTELLECT" => HeroAttribute.Intelligence,
                "DOTA_ATTRIBUTE_ALL" => HeroAttribute.Universal,
                _ => HeroAttribute.Universal // fallback seguro
            };

            _attributes[heroEntry.Name] = attribute;
        }
    }

    // ════════════════════════════════════════════════════════════
    // CONSULTA
    // ════════════════════════════════════════════════════════════

    public HeroAttribute GetAttribute(string heroName)
    {
        if (string.IsNullOrEmpty(heroName))
            return HeroAttribute.Universal;

        return _attributes.TryGetValue(heroName, out var attr)
            ? attr
            : HeroAttribute.Universal;
    }
}