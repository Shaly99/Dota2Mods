namespace SKYNET;

/// <summary>
/// Implementación del servicio de prefabs.
/// </summary>
public class PrefabsService : IPrefabsService
{
    private readonly Dictionary<string, prefabs> _prefabsByName = new(StringComparer.OrdinalIgnoreCase);

    public void RegisterPrefab(prefabs prefab)
    {
        if (prefab == null || string.IsNullOrEmpty(prefab.Name)) return;
        _prefabsByName[prefab.Name] = prefab;
    }

    public string GetSlotForPrefab(string slot, string prefabType)
    {
        // Si ya tiene slot, devolverlo
        if (!string.IsNullOrEmpty(slot)) return slot;

        // Buscar el slot del prefab por defecto
        if (string.IsNullOrEmpty(prefabType)) return "";

        if (_prefabsByName.TryGetValue(prefabType, out var prefab))
        {
            return prefab.item_slot ?? "";
        }

        return "";
    }

    public string GetRarityForPrefab(string rarity, string prefabType)
    {
        // Si ya tiene rareza, devolverla
        if (!string.IsNullOrEmpty(rarity)) return rarity;

        // Buscar la rareza del prefab por defecto
        if (string.IsNullOrEmpty(prefabType)) return "";

        if (_prefabsByName.TryGetValue(prefabType, out var prefab))
        {
            return prefab.item_rarity ?? "";
        }

        return "";
    }
}