using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SKYNET;

/// <summary>
/// Herramienta de diagnóstico para explorar el contenido del VPK y items_game.txt.
/// </summary>
public static class VpkExplorer
{
    public static void GenerateReport(string vpkPath)
    {
        string reportPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            "vpk_explorer_report.txt");

        var lines = new List<string>();

        lines.Add("═══════════════════════════════════════════════════════");
        lines.Add($" ASSET MODIFIER ANALYSIS - {DateTime.Now:yyyy-MM-dd HH:mm}");
        lines.Add("═══════════════════════════════════════════════════════");
        lines.Add("");

        string itemsGamePath = Path.Combine("data", "db", "items_game.txt");
        if (!File.Exists(itemsGamePath))
        {
            lines.Add("ERROR: items_game.txt no encontrado");
            File.WriteAllLines(reportPath, lines);
            return;
        }

        KValue itemsGame = KValue.LoadAsText(itemsGamePath);
        if (itemsGame == null || !itemsGame.ContainsKey("items"))
        {
            lines.Add("ERROR: No se pudo parsear items_game.txt");
            File.WriteAllLines(reportPath, lines);
            return;
        }

        // ════════════════════════════════════════════════════════════
        // SECCIÓN 1: TODOS LOS TIPOS DE ASSET_MODIFIER
        // ════════════════════════════════════════════════════════════
        lines.Add("╔══════════════════════════════════════════════════════╗");
        lines.Add("║  SECCIÓN 1: TIPOS DE ASSET_MODIFIER                ║");
        lines.Add("╚══════════════════════════════════════════════════════╝");
        lines.Add("");

        var allModifiers = new List<ModifierInfo>();

        foreach (KValue item in itemsGame["items"].Children)
        {
            if (!item.ContainsKey("visuals")) continue;

            string itemName = item["name"]?.AsString() ?? "";
            string itemPrefab = item["prefab"]?.AsString() ?? "";
            string itemSlot = item["item_slot"]?.AsString() ?? "";
            string heroUse = "";

            if (item.ContainsKey("used_by_heroes"))
            {
                var heroes = item["used_by_heroes"].Children;
                if (heroes.Count > 0)
                    heroUse = heroes[0].Name;
            }

            foreach (KValue visual in item["visuals"].Children)
            {
                if (!visual.Name.StartsWith("asset_modifier")) continue;

                string type = visual["type"]?.AsString() ?? "UNKNOWN";
                string asset = visual["asset"]?.AsString() ?? "";
                string modifier = visual["modifier"]?.AsString() ?? "";
                string style = visual["style"]?.AsString() ?? "";
                string frequency = visual["frequency"]?.AsString() ?? "";

                allModifiers.Add(new ModifierInfo
                {
                    ItemId = item.Name,
                    ItemName = itemName,
                    Prefab = itemPrefab,
                    Slot = itemSlot,
                    Hero = heroUse,
                    Type = type,
                    Asset = asset,
                    Modifier = modifier,
                    Style = style,
                    Frequency = frequency
                });
            }
        }

        // Resumen por tipo
        var byType = allModifiers
            .GroupBy(m => m.Type)
            .OrderByDescending(g => g.Count())
            .ToList();

        lines.Add($"  Total asset_modifiers encontrados: {allModifiers.Count:N0}");
        lines.Add($"  Tipos únicos: {byType.Count}");
        lines.Add("");

        foreach (var group in byType)
        {
            lines.Add($"  {group.Key,-35} → {group.Count(),8:N0}");
        }
        lines.Add("");

        // ════════════════════════════════════════════════════════════
        // SECCIÓN 2: DETALLE POR TIPO CON EJEMPLOS
        // ════════════════════════════════════════════════════════════
        lines.Add("╔══════════════════════════════════════════════════════╗");
        lines.Add("║  SECCIÓN 2: DETALLE POR TIPO (con ejemplos)        ║");
        lines.Add("╚══════════════════════════════════════════════════════╝");
        lines.Add("");

        foreach (var group in byType)
        {
            lines.Add($"  ══ {group.Key} ({group.Count():N0}) ══");
            lines.Add("");

            // Subcategorizar por prefab
            var byPrefab = group
                .GroupBy(m => m.Prefab)
                .OrderByDescending(g => g.Count())
                .Take(10)
                .ToList();

            lines.Add("    Por prefab:");
            foreach (var pg in byPrefab)
            {
                lines.Add($"      {(string.IsNullOrEmpty(pg.Key) ? "(vacío)" : pg.Key),-25} → {pg.Count(),6:N0}");
            }
            lines.Add("");

            // Ejemplos concretos (3 por tipo)
            lines.Add("    Ejemplos:");
            foreach (var example in group.Take(3))
            {
                lines.Add($"      Item #{example.ItemId}: {example.ItemName}");
                lines.Add($"        prefab={example.Prefab} slot={example.Slot} hero={example.Hero}");
                lines.Add($"        asset   = {example.Asset}");
                lines.Add($"        modifier= {example.Modifier}");
                if (!string.IsNullOrEmpty(example.Style))
                    lines.Add($"        style   = {example.Style}");
                lines.Add("");
            }

            lines.Add("");
        }

        // ════════════════════════════════════════════════════════════
        // SECCIÓN 3: ACTIVIDADES/ANIMACIONES
        // ════════════════════════════════════════════════════════════
        lines.Add("╔══════════════════════════════════════════════════════╗");
        lines.Add("║  SECCIÓN 3: ACTIVIDADES/ANIMACIONES (activity)     ║");
        lines.Add("╚══════════════════════════════════════════════════════╝");
        lines.Add("");

        var activityModifiers = allModifiers
            .Where(m => m.Type == "activity")
            .ToList();

        lines.Add($"  Total modifiers de tipo 'activity': {activityModifiers.Count}");
        lines.Add("");

        // Assets únicos (qué actividades se reemplazan)
        var uniqueAssets = activityModifiers
            .Select(m => m.Asset)
            .Distinct()
            .OrderBy(a => a)
            .ToList();

        lines.Add($"  Actividades únicas reemplazadas: {uniqueAssets.Count}");
        lines.Add("");

        foreach (string asset in uniqueAssets.Take(50))
        {
            int count = activityModifiers.Count(m => m.Asset == asset);
            lines.Add($"    {asset,-55} → {count,4} items");
        }

        if (uniqueAssets.Count > 50)
        {
            lines.Add($"    ... y {uniqueAssets.Count - 50} más");
        }
        lines.Add("");

        // Modifiers únicos (qué actividades nuevas se usan)
        var uniqueModifiers = activityModifiers
            .Select(m => m.Modifier)
            .Distinct()
            .OrderBy(m => m)
            .ToList();

        lines.Add($"  Actividades de reemplazo únicas: {uniqueModifiers.Count}");
        lines.Add("");

        foreach (string mod in uniqueModifiers.Take(50))
        {
            int count = activityModifiers.Count(m => m.Modifier == mod);
            lines.Add($"    {mod,-55} → {count,4} items");
        }

        if (uniqueModifiers.Count > 50)
        {
            lines.Add($"    ... y {uniqueModifiers.Count - 50} más");
        }
        lines.Add("");

        // Ejemplos completos de activity por héroe
        lines.Add("  ── Ejemplos de items con múltiples activity modifiers ──");
        lines.Add("");

        var itemsWithMultipleActivities = activityModifiers
            .GroupBy(m => m.ItemId)
            .Where(g => g.Count() >= 3)
            .OrderByDescending(g => g.Count())
            .Take(10)
            .ToList();

        foreach (var itemGroup in itemsWithMultipleActivities)
        {
            var first = itemGroup.First();
            lines.Add($"    Item #{first.ItemId}: {first.ItemName} (hero={first.Hero})");
            lines.Add($"      prefab={first.Prefab} slot={first.Slot}");

            foreach (var mod in itemGroup)
            {
                lines.Add($"      {mod.Asset,-40} → {mod.Modifier}");
            }
            lines.Add("");
        }

        // ════════════════════════════════════════════════════════════
        // SECCIÓN 4: PARTÍCULAS
        // ════════════════════════════════════════════════════════════
        lines.Add("╔══════════════════════════════════════════════════════╗");
        lines.Add("║  SECCIÓN 4: PARTÍCULAS (particle*)                 ║");
        lines.Add("╚══════════════════════════════════════════════════════╝");
        lines.Add("");

        var particleModifiers = allModifiers
            .Where(m => m.Type.StartsWith("particle"))
            .ToList();

        var particleSubTypes = particleModifiers
            .GroupBy(m => m.Type)
            .OrderByDescending(g => g.Count())
            .ToList();

        lines.Add($"  Total modifiers de partículas: {particleModifiers.Count}");
        lines.Add("");

        foreach (var group in particleSubTypes)
        {
            lines.Add($"    {group.Key,-30} → {group.Count(),6:N0}");
        }
        lines.Add("");

        // Ejemplos de items con muchas partículas
        lines.Add("  ── Items con más partículas ──");
        lines.Add("");

        var itemsWithManyParticles = particleModifiers
            .GroupBy(m => m.ItemId)
            .OrderByDescending(g => g.Count())
            .Take(10)
            .ToList();

        foreach (var itemGroup in itemsWithManyParticles)
        {
            var first = itemGroup.First();
            lines.Add($"    Item #{first.ItemId}: {first.ItemName} ({itemGroup.Count()} partículas)");
            lines.Add($"      hero={first.Hero} prefab={first.Prefab} slot={first.Slot}");

            foreach (var mod in itemGroup.Take(5))
            {
                lines.Add($"      [{mod.Type}]");
                lines.Add($"        {mod.Asset}");
                lines.Add($"        → {mod.Modifier}");
            }

            if (itemGroup.Count() > 5)
            {
                lines.Add($"      ... y {itemGroup.Count() - 5} más");
            }
            lines.Add("");
        }

        // ════════════════════════════════════════════════════════════
        // SECCIÓN 5: ENTITY MODELS
        // ════════════════════════════════════════════════════════════
        lines.Add("╔══════════════════════════════════════════════════════╗");
        lines.Add("║  SECCIÓN 5: ENTITY MODELS                          ║");
        lines.Add("╚══════════════════════════════════════════════════════╝");
        lines.Add("");

        var entityModels = allModifiers
            .Where(m => m.Type.Contains("model"))
            .ToList();

        var modelSubTypes = entityModels
            .GroupBy(m => m.Type)
            .OrderByDescending(g => g.Count())
            .ToList();

        lines.Add($"  Total modifiers de modelos: {entityModels.Count}");
        lines.Add("");

        foreach (var group in modelSubTypes)
        {
            lines.Add($"    {group.Key,-35} → {group.Count(),6:N0}");
        }
        lines.Add("");

        // ════════════════════════════════════════════════════════════
        // SECCIÓN 6: OTROS TIPOS INTERESANTES
        // ════════════════════════════════════════════════════════════
        lines.Add("╔══════════════════════════════════════════════════════╗");
        lines.Add("║  SECCIÓN 6: ABILITY/SOUND/SPEECH/ICON/MISC         ║");
        lines.Add("╚══════════════════════════════════════════════════════╝");
        lines.Add("");

        var otherTypes = allModifiers
            .Where(m => !m.Type.StartsWith("particle")
                && !m.Type.Contains("model")
                && m.Type != "activity")
            .GroupBy(m => m.Type)
            .OrderByDescending(g => g.Count())
            .ToList();

        foreach (var group in otherTypes)
        {
            lines.Add($"  ══ {group.Key} ({group.Count():N0}) ══");

            foreach (var example in group.Take(3))
            {
                lines.Add($"    Item #{example.ItemId}: {example.ItemName}");
                lines.Add($"      asset   = {example.Asset}");
                lines.Add($"      modifier= {example.Modifier}");
                lines.Add("");
            }
        }

        // ════════════════════════════════════════════════════════════
        // SECCIÓN 7: RESUMEN POR HÉROE - TOP 10 CON MÁS MODIFIERS
        // ════════════════════════════════════════════════════════════
        lines.Add("╔══════════════════════════════════════════════════════╗");
        lines.Add("║  SECCIÓN 7: TOP HÉROES CON MÁS ASSET_MODIFIERS    ║");
        lines.Add("╚══════════════════════════════════════════════════════╝");
        lines.Add("");

        var byHero = allModifiers
            .Where(m => !string.IsNullOrEmpty(m.Hero))
            .GroupBy(m => m.Hero)
            .OrderByDescending(g => g.Count())
            .Take(20)
            .ToList();

        foreach (var heroGroup in byHero)
        {
            var heroTypes = heroGroup
                .GroupBy(m => m.Type)
                .OrderByDescending(g => g.Count())
                .ToList();

            lines.Add($"  {heroGroup.Key,-40} → {heroGroup.Count(),5:N0} total");

            foreach (var typeGroup in heroTypes)
            {
                lines.Add($"    {typeGroup.Key,-30} → {typeGroup.Count(),4}");
            }
            lines.Add("");
        }

        // ════════════════════════════════════════════════════════════
        // GUARDAR
        // ════════════════════════════════════════════════════════════
        File.WriteAllLines(reportPath, lines);
    }

    private class ModifierInfo
    {
        public string ItemId { get; init; } = "";
        public string ItemName { get; init; } = "";
        public string Prefab { get; init; } = "";
        public string Slot { get; init; } = "";
        public string Hero { get; init; } = "";
        public string Type { get; init; } = "";
        public string Asset { get; init; } = "";
        public string Modifier { get; init; } = "";
        public string Style { get; init; } = "";
        public string Frequency { get; init; } = "";
    }
}