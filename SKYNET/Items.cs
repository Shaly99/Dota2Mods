namespace SKYNET;

[Serializable]
public class Items
{
    public string ItemID { get; set; }

    public List<string> bundles { get; set; }

    public List<Style> styles { get; set; } = new List<Style>();

    public string name { get; set; }

    public string prefab { get; set; }

    public string image_inventory { get; set; }

    public string item_description { get; set; }

    public string item_name { get; set; }

    public string item_rarity { get; set; }

    public string item_slot { get; set; }

    public string item_type_name { get; set; }

    public Style style { get; set; }

    public string used_by_heroes { get; set; }

    public string asset { get; set; }
}
