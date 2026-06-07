namespace SKYNET;

/// <summary>
/// Interfaz para el catálogo centralizado de items del juego.
/// Permite buscar, filtrar y agregar items por categoría.
/// </summary>
public interface IItemCatalog
{
    // ════════════════════════════════════════
    // AGREGAR
    // ════════════════════════════════════════

    /// <summary>
    /// Agrega un item al catálogo en la categoría especificada.
    /// </summary>
    void Add(ItemCategory category, Items item);

    // ════════════════════════════════════════
    // BÚSQUEDAS RÁPIDAS (O(1))
    // ════════════════════════════════════════

    /// <summary>
    /// Busca un item por ID. Devuelve null si no existe.
    /// </summary>
    Items GetById(string itemId);

    /// <summary>
    /// Busca un item por nombre (case-insensitive). Devuelve null si no existe.
    /// </summary>
    Items GetByName(string name);

    // ════════════════════════════════════════
    // OBTENER POR CATEGORÍA
    // ════════════════════════════════════════

    /// <summary>
    /// Devuelve una COPIA de la lista de items de una categoría.
    /// </summary>
    List<Items> GetByCategory(ItemCategory category);

    /// <summary>
    /// Devuelve la referencia directa a la lista de una categoría.
    /// Más rápido pero NO MODIFICAR.
    /// </summary>
    List<Items> GetByCategoryFast(ItemCategory category);

    // ════════════════════════════════════════
    // FILTROS COMUNES
    // ════════════════════════════════════════

    /// <summary>
    /// Devuelve todos los items que pertenecen a un héroe específico.
    /// </summary>
    List<Items> GetByHero(string heroName);

    /// <summary>
    /// Devuelve items de una categoría específica para un héroe.
    /// </summary>
    List<Items> GetByHero(ItemCategory category, string heroName);

    /// <summary>
    /// Devuelve items de un héroe en un slot específico.
    /// </summary>
    List<Items> GetByHeroAndSlot(string heroName, string slot);

    // ════════════════════════════════════════
    // BÚSQUEDAS AVANZADAS
    // ════════════════════════════════════════

    /// <summary>
    /// Devuelve todos los items que tienen el mismo ID en TODAS las categorías.
    /// </summary>
    List<Items> FindAllById(string itemId);

    // ════════════════════════════════════════
    // UTILIDADES
    // ════════════════════════════════════════

    /// <summary>
    /// Cantidad total de items en el catálogo.
    /// </summary>
    int TotalCount { get; }

    /// <summary>
    /// Cantidad de items en una categoría específica.
    /// </summary>
    int CountByCategory(ItemCategory category);

    /// <summary>
    /// Limpia todo el catálogo.
    /// </summary>
    void Clear();
}