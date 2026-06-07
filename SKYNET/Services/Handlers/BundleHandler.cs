namespace SKYNET;

public class BundleHandler : BundleHandlerBase
{
    private readonly IItemCatalog _catalog;

    public BundleHandler(IAudioService audio, IModConfigurationService modConfig, IUIState uiState, IItemCatalog catalog)
        : base(audio, modConfig, uiState)
    {
        _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
    }

    public override string Prefab => "bundle";

    public override void Handle(BundleBox box)
    {
        // 🔑 Si el box fue reseteado, abrir el selector
        if (box.WasReset)
        {
            ShowToolSelector(box);
            return;
        }

        if (box?.item?.bundles == null) return;

        var bundleItems = box.item.bundles
            .Select(bundleName => _catalog.GetByName(bundleName))
            .Where(item => item != null)
            .ToList();

        if (bundleItems.Count == 0) return;

        for (int i = 0; i < frmMain.frm.SlotContainer.Controls.Count; i++)
        {
            if (!(frmMain.frm.SlotContainer.Controls[i] is BoxItems boxItem))
                continue;

            try
            {
                string slotName = boxItem.Name?.ToLower() ?? "";

                Items matchingItem = bundleItems.Find(x =>
                    x?.item_slot != null &&
                    x.item_slot.ToLower() == slotName);

                if (matchingItem != null)
                {
                    boxItem.Item = matchingItem;
                }
            }
            catch (Exception ex)
            {
                modCommon.Save(ex);
            }
        }
    }

    protected override void ApplyItem(BundleBox box)
    {
        // No se usa
    }
}