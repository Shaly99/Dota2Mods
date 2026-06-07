namespace SKYNET;

/// <summary>
/// Implementación de IHeroesService.
/// Por ahora delega a frmMain.manager (singleton) hasta migración completa.
/// </summary>
public class HeroesService : IHeroesService
{
    private static modManager Manager => frmMain.manager;

    // ════════════════════════════════════════
    // LECTURA
    // ════════════════════════════════════════

    public List<Hero> GetAll()
    {
        if (Manager?.Heroes == null) return new List<Hero>();
        return new List<Hero>(Manager.Heroes);
    }

    public int Count => Manager?.Heroes?.Count ?? 0;

    public Hero GetByName(string heroName)
    {
        if (Manager?.Heroes == null || string.IsNullOrEmpty(heroName))
            return null;

        return Manager.Heroes.Find(h => h?.Name == heroName);
    }

    public Hero GetAt(int index)
    {
        if (Manager?.Heroes == null) return null;
        if (index < 0 || index >= Manager.Heroes.Count) return null;

        return Manager.Heroes[index];
    }

    public List<Items> GetBundles(string heroName)
    {
        var hero = GetByName(heroName);
        if (hero?.Bundles == null) return new List<Items>();
        return new List<Items>(hero.Bundles);
    }

    public Items GetBundleBySlot(string heroName, string itemSlot)
    {
        var hero = GetByName(heroName);
        if (hero?.Bundles == null || string.IsNullOrEmpty(itemSlot)) return null;

        return hero.Bundles.Find(b => b?.item_slot == itemSlot);
    }

    // ════════════════════════════════════════
    // MODIFICACIÓN
    // ════════════════════════════════════════

    public void AddIfMissing(string heroName)
    {
        if (Manager?.Heroes == null || string.IsNullOrEmpty(heroName)) return;

        var existing = GetByName(heroName);
        if (existing != null) return;

        Manager.Heroes.Add(new Hero
        {
            Name = heroName,
            Bundles = new List<Items>()
        });
    }

    public void SetBundle(string heroName, Items item)
    {
        if (Manager?.Heroes == null || string.IsNullOrEmpty(heroName) || item == null) return;

        // Asegurar que el héroe existe
        AddIfMissing(heroName);

        var hero = GetByName(heroName);
        if (hero?.Bundles == null) return;

        // Buscar bundle existente con el mismo slot
        var existingBundle = hero.Bundles.Find(b => b?.item_slot == item.item_slot);

        if (existingBundle != null)
        {
            // Si el item nuevo es "default" (ID 0000), eliminar el bundle
            if (item.ItemID == "0000")
            {
                hero.Bundles.Remove(existingBundle);
            }
            else
            {
                // Reemplazar
                int index = hero.Bundles.IndexOf(existingBundle);
                hero.Bundles[index] = item;
            }
        }
        else
        {
            // No existe → agregar (solo si no es default)
            if (item.ItemID != "0000")
            {
                hero.Bundles.Add(item);
            }
        }
    }

    public void RemoveBundle(string heroName, string itemSlot)
    {
        if (Manager?.Heroes == null || string.IsNullOrEmpty(heroName) || string.IsNullOrEmpty(itemSlot))
            return;

        var hero = GetByName(heroName);
        if (hero?.Bundles == null) return;

        var bundle = hero.Bundles.Find(b => b?.item_slot == itemSlot);
        if (bundle != null)
        {
            hero.Bundles.Remove(bundle);
        }
    }
}