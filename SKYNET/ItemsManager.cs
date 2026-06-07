namespace SKYNET;

/// <summary>
/// Facade (wrapper) que delega al IItemsService registrado en ServiceContainer.
/// Mantiene la API estática vieja para que el código existente siga funcionando.
/// 
/// USO RECOMENDADO PARA CÓDIGO NUEVO:
///   Inyectar IItemsService por constructor en lugar de usar esta clase estática.
/// </summary>
public class ItemsManager
{
    /// <summary>
    /// Acceso al servicio real. Si no está registrado, lanza una excepción clara.
    /// </summary>
    private static IItemsService Service => ServiceContainer.Get<IItemsService>();

    public static Items GetItem(string itemID) => Service.GetItem(itemID);

    public static List<Items> GetItems(string itemID) => Service.GetItems(itemID);

    internal static Items GetItemFromName(string itemname) => Service.GetItemFromName(itemname);

    internal static Items GetBundleFromName(string itemname) => Service.GetBundleFromName(itemname);

    public static List<Items> GetItemsFromPrefab(Items item) => Service.GetItemsFromPrefab(item);

    public static string GetDescription(string name) => Service.GetDescription(name);

    internal static string NameFromAsset(string v) => Service.NameFromAsset(v);

    internal static Image GetDefaultImageItem(string slot, string hero) => Service.GetDefaultImageItem(slot, hero);

    internal static Items GetSavedItem(string used_by_heroes, string item_slot) => Service.GetSavedItem(used_by_heroes, item_slot);

    public static string GetHeroName(string name) => Service.GetHeroName(name);

    internal static string GetHeroOriginalName(string name) => Service.GetHeroOriginalName(name);

    internal static Image GetImage(string path) => Service.GetImage(path);

    internal static void GetSoundAssetName(string itemID, out string patch)
    {
        Service.GetSoundAssetName(itemID, out patch);
    }
}