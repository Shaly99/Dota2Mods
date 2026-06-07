namespace SKYNET;

/// <summary>
/// Interfaz para el servicio de gestión de items.
/// Provee búsquedas, filtros y obtención de imágenes para items.
/// </summary>
public interface IItemsService
{
    /// <summary>
    /// Busca un item por su ID en todas las categorías.
    /// </summary>
    Items GetItem(string itemID);

    /// <summary>
    /// Busca todos los items con un ID específico.
    /// </summary>
    List<Items> GetItems(string itemID);

    /// <summary>
    /// Busca un item por su nombre.
    /// </summary>
    Items GetItemFromName(string itemname);

    /// <summary>
    /// Busca un bundle por su nombre.
    /// </summary>
    Items GetBundleFromName(string itemname);

    /// <summary>
    /// Devuelve los items de la misma categoría que el item dado.
    /// </summary>
    List<Items> GetItemsFromPrefab(Items item);

    /// <summary>
    /// Devuelve la descripción de un item desde el archivo de idioma.
    /// </summary>
    string GetDescription(string name);

    /// <summary>
    /// Devuelve la imagen por defecto para un slot/héroe.
    /// </summary>
    Image GetDefaultImageItem(string slot, string hero);

    /// <summary>
    /// Devuelve el item guardado por el usuario para un héroe en un slot.
    /// </summary>
    Items GetSavedItem(string used_by_heroes, string item_slot);

    /// <summary>
    /// Devuelve el nombre legible de un héroe.
    /// </summary>
    string GetHeroName(string name);

    /// <summary>
    /// Devuelve el nombre original (npc_dota_hero_X) desde el nombre legible.
    /// </summary>
    string GetHeroOriginalName(string name);

    /// <summary>
    /// Devuelve la imagen de un item desde su path.
    /// </summary>
    Image GetImage(string path);

    /// <summary>
    /// Devuelve el nombre de un loading screen desde su asset path.
    /// </summary>
    string NameFromAsset(string v);

    /// <summary>
    /// Devuelve el path del sonido asociado a un item.
    /// </summary>
    void GetSoundAssetName(string itemID, out string patch);
}