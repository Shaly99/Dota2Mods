namespace SKYNET;

/// <summary>
/// Catálogo centralizado de todos los items del juego.
/// Reemplaza las 24 listas dispersas en frmMain con un solo lugar.
/// 
/// Ventajas:
/// - Búsqueda por ID: O(1) en lugar de O(24n)
/// - Búsqueda por nombre: O(1)
/// - Filtros por categoría/héroe/slot eficientes
/// - Una sola fuente de verdad
/// </summary>
public class ItemCatalog : IItemCatalog
{
    // ════════════════════════════════════════
    // ESTRUCTURAS INTERNAS
    // ════════════════════════════════════════

    /// <summary>
    /// Items organizados por categoría.
    /// Útil para mostrar todos los items de un tipo (ej: todos los bundles).
    /// </summary>
    private readonly Dictionary<ItemCategory, List<Items>> _byCategory;

    /// <summary>
    /// Items indexados por ID para búsqueda O(1).
    /// Guarda TODOS los items que tienen ID.
    /// Si hay duplicados (raro), se queda con el primero.
    /// </summary>
    private readonly Dictionary<string, Items> _byId;

    /// <summary>
    /// Items indexados por nombre (en lowercase) para búsqueda O(1).
    /// </summary>
    private readonly Dictionary<string, Items> _byName;

    /// <summary>
    /// Lock para acceso thread-safe en caso de carga paralela.
    /// </summary>
    private readonly object _lock = new();

    // ════════════════════════════════════════
    // CONSTRUCTOR
    // ════════════════════════════════════════

    public ItemCatalog()
    {
        _byCategory = new Dictionary<ItemCategory, List<Items>>();
        _byId = new Dictionary<string, Items>();
        _byName = new Dictionary<string, Items>();

        // Inicializar todas las categorías con listas vacías
        foreach (ItemCategory category in Enum.GetValues(typeof(ItemCategory)))
        {
            _byCategory[category] = new List<Items>();
        }
    }

    // ════════════════════════════════════════
    // AGREGAR ITEMS
    // ════════════════════════════════════════

    /// <summary>
    /// Agrega un item al catálogo en la categoría especificada.
    /// El item se indexa automáticamente por ID y nombre.
    /// </summary>
    public void Add(ItemCategory category, Items item)
    {
        if (item == null) return;

        lock (_lock)
        {
            // Agregar a la categoría
            _byCategory[category].Add(item);

            // Indexar por ID (si tiene)
            if (!string.IsNullOrEmpty(item.ItemID) && !_byId.ContainsKey(item.ItemID))
            {
                _byId[item.ItemID] = item;
            }

            // Indexar por nombre (si tiene)
            if (!string.IsNullOrEmpty(item.name))
            {
                string nameKey = item.name.ToLowerInvariant();
                if (!_byName.ContainsKey(nameKey))
                {
                    _byName[nameKey] = item;
                }
            }
        }
    }

    // ════════════════════════════════════════
    // BÚSQUEDAS RÁPIDAS (O(1))
    // ════════════════════════════════════════

    /// <summary>
    /// Busca un item por su ID. O(1).
    /// </summary>
    public Items GetById(string itemId)
    {
        if (string.IsNullOrEmpty(itemId)) return null;

        lock (_lock)
        {
            return _byId.TryGetValue(itemId, out Items item) ? item : null;
        }
    }

    /// <summary>
    /// Busca un item por su nombre (case-insensitive). O(1).
    /// </summary>
    public Items GetByName(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;

        lock (_lock)
        {
            return _byName.TryGetValue(name.ToLowerInvariant(), out Items item) ? item : null;
        }
    }

    // ════════════════════════════════════════
    // OBTENER POR CATEGORÍA
    // ════════════════════════════════════════

    /// <summary>
    /// Devuelve todos los items de una categoría.
    /// Devuelve una COPIA para evitar modificaciones externas.
    /// </summary>
    public List<Items> GetByCategory(ItemCategory category)
    {
        lock (_lock)
        {
            return new List<Items>(_byCategory[category]);
        }
    }

    /// <summary>
    /// Devuelve la referencia directa a la lista de una categoría.
    /// Más rápido que GetByCategory pero NO MODIFICAR la lista devuelta.
    /// </summary>
    public List<Items> GetByCategoryFast(ItemCategory category)
    {
        return _byCategory[category];
    }

    // ════════════════════════════════════════
    // FILTROS COMUNES
    // ════════════════════════════════════════

    /// <summary>
    /// Devuelve todos los items que pertenecen a un héroe específico.
    /// Busca en TODAS las categorías.
    /// </summary>
    public List<Items> GetByHero(string heroName)
    {
        if (string.IsNullOrEmpty(heroName)) return new List<Items>();

        lock (_lock)
        {
            var result = new List<Items>();
            foreach (var list in _byCategory.Values)
            {
                result.AddRange(list.Where(i => i.used_by_heroes == heroName));
            }
            return result;
        }
    }

    /// <summary>
    /// Devuelve items de una categoría específica para un héroe.
    /// </summary>
    public List<Items> GetByHero(ItemCategory category, string heroName)
    {
        if (string.IsNullOrEmpty(heroName)) return new List<Items>();

        lock (_lock)
        {
            return _byCategory[category]
                .Where(i => i.used_by_heroes == heroName)
                .ToList();
        }
    }

    /// <summary>
    /// Devuelve items de un héroe en un slot específico.
    /// </summary>
    public List<Items> GetByHeroAndSlot(string heroName, string slot)
    {
        if (string.IsNullOrEmpty(heroName) || string.IsNullOrEmpty(slot))
            return new List<Items>();

        lock (_lock)
        {
            return _byCategory[ItemCategory.Items]
                .Where(i => i.used_by_heroes == heroName && i.item_slot == slot)
                .ToList();
        }
    }

    // ════════════════════════════════════════
    // BÚSQUEDAS QUE INCLUYEN TODAS LAS LISTAS
    // (compatible con el comportamiento viejo de ItemsManager)
    // ════════════════════════════════════════

    /// <summary>
    /// Busca un item en TODAS las categorías por su nombre.
    /// Equivalente al viejo ItemsManager.GetItemFromName().
    /// </summary>
    public Items FindByNameInAllCategories(string name)
    {
        // Es lo mismo que GetByName porque _byName indexa todas las categorías
        return GetByName(name);
    }

    /// <summary>
    /// Devuelve TODOS los items que tienen el mismo ID en TODAS las categorías.
    /// (En la práctica, un ID debería ser único, pero el método viejo permitía duplicados).
    /// </summary>
    public List<Items> FindAllById(string itemId)
    {
        if (string.IsNullOrEmpty(itemId)) return new List<Items>();

        lock (_lock)
        {
            var result = new List<Items>();
            foreach (var list in _byCategory.Values)
            {
                result.AddRange(list.Where(i => i.ItemID == itemId));
            }
            return result;
        }
    }

    // ════════════════════════════════════════
    // UTILIDADES
    // ════════════════════════════════════════

    /// <summary>
    /// Cantidad total de items en el catálogo.
    /// </summary>
    public int TotalCount
    {
        get
        {
            lock (_lock)
            {
                return _byCategory.Values.Sum(l => l.Count);
            }
        }
    }

    /// <summary>
    /// Cantidad de items en una categoría específica.
    /// </summary>
    public int CountByCategory(ItemCategory category)
    {
        lock (_lock)
        {
            return _byCategory[category].Count;
        }
    }

    /// <summary>
    /// Limpia todo el catálogo. Útil para recargar desde cero.
    /// </summary>
    public void Clear()
    {
        lock (_lock)
        {
            foreach (var list in _byCategory.Values)
            {
                list.Clear();
            }
            _byId.Clear();
            _byName.Clear();
        }
    }
}