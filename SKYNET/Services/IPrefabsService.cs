namespace SKYNET;

/// <summary>
/// Servicio para obtener información de prefabs (tipos de items).
/// Encapsula el acceso a la lista de prefabs cargada del VPK.
/// </summary>
public interface IPrefabsService
{
    /// <summary>
    /// Obtiene el slot para un prefab.
    /// Si el slot ya está definido (no vacío), lo devuelve.
    /// Si está vacío, busca el slot default del prefab.
    /// </summary>
    string GetSlotForPrefab(string slot, string prefabType);

    /// <summary>
    /// Obtiene la rareza para un prefab.
    /// Si la rareza ya está definida (no vacía), la devuelve.
    /// Si está vacía, busca la rareza default del prefab.
    /// </summary>
    string GetRarityForPrefab(string rarity, string prefabType);

    /// <summary>
    /// Registra un nuevo prefab (usado durante la carga inicial).
    /// </summary>
    void RegisterPrefab(prefabs prefab);
}