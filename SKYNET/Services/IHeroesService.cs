namespace SKYNET;

/// <summary>
/// Servicio para gestionar la lista de héroes y sus bundles guardados por el usuario.
/// Encapsula el acceso a frmMain.manager.Heroes.
/// </summary>
public interface IHeroesService
{
    // ════════════════════════════════════════
    // LECTURA
    // ════════════════════════════════════════

    /// <summary>
    /// Lista completa de héroes (solo lectura, devuelve una copia).
    /// </summary>
    List<Hero> GetAll();

    /// <summary>
    /// Cantidad total de héroes.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Busca un héroe por nombre.
    /// </summary>
    Hero GetByName(string heroName);

    /// <summary>
    /// Devuelve el héroe en una posición específica (útil para iteración).
    /// </summary>
    Hero GetAt(int index);

    /// <summary>
    /// Devuelve los bundles guardados de un héroe.
    /// </summary>
    List<Items> GetBundles(string heroName);

    /// <summary>
    /// Devuelve un bundle específico (por slot) de un héroe.
    /// </summary>
    Items GetBundleBySlot(string heroName, string itemSlot);

    // ════════════════════════════════════════
    // MODIFICACIÓN
    // ════════════════════════════════════════

    /// <summary>
    /// Agrega un héroe si no existe.
    /// </summary>
    void AddIfMissing(string heroName);

    /// <summary>
    /// Establece o reemplaza un bundle para un héroe.
    /// Si ya existe un bundle con el mismo item_slot, lo reemplaza.
    /// Si no existe, lo agrega.
    /// </summary>
    void SetBundle(string heroName, Items item);

    /// <summary>
    /// Remueve un bundle de un héroe.
    /// </summary>
    void RemoveBundle(string heroName, string itemSlot);
}