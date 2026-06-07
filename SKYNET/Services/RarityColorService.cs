namespace SKYNET;

/// <summary>
/// Implementación del servicio de colores por rareza.
/// </summary>
public class RarityColorService : IRarityColorService
{
    private readonly Dictionary<string, Colors> _colorsByName = new(StringComparer.OrdinalIgnoreCase);
    private readonly IItemsService _items;

    public RarityColorService(IItemsService items)
    {
        _items = items ?? throw new ArgumentNullException(nameof(items));
    }

    public void RegisterColor(Colors color)
    {
        if (color == null || string.IsNullOrEmpty(color.Name)) return;

        // Normalizar: quitar "desc_" si está al principio
        string key = color.Name.Replace("desc_", "");
        _colorsByName[key] = color;
    }

    public Color GetColorForRarity(string rarity)
    {
        try
        {
            string key = string.IsNullOrEmpty(rarity) ? "common" : rarity;

            if (_colorsByName.TryGetValue(key, out var colorEntry))
            {
                return ColorTranslator.FromHtml(colorEntry.hex_color);
            }

            // Fallback a common si la rareza no está registrada
            if (_colorsByName.TryGetValue("common", out var common))
            {
                return ColorTranslator.FromHtml(common.hex_color);
            }
        }
        catch
        {
            // Color por defecto si algo falla
        }

        return Color.Gray;
    }

    public Color GetColorForItemId(string itemId)
    {
        var item = _items.GetItem(itemId);
        if (item == null) return Color.Gray;

        return GetColorForRarity(item.item_rarity);
    }
}