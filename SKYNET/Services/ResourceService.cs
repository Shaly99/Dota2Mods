namespace SKYNET;

/// <summary>
/// Implementación del servicio de recursos.
/// Por ahora delega a los métodos estáticos de DotaResources para no romper nada.
/// En el futuro se puede mover la lógica acá.
/// </summary>
public class ResourceService : IResourceService
{
    private readonly IItemCatalog _catalog;
    private readonly IUIService _ui;

    public ResourceService(IItemCatalog catalog, IUIService ui)
    {
        _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
        _ui = ui ?? throw new ArgumentNullException(nameof(ui));
    }

    public void ExtractResources(string ruta, string toFolder)
    {
        DotaResources.ExtractResources(ruta, toFolder);
    }

    public void ExtractFile(string file, string folder)
    {
        DotaResources.ExtractFile(file, folder);
    }

    public void LoadResources()
    {
        DotaResources.LoadResources();
    }

    public void LoadItemsDescriptions()
    {
        DotaResources.LoadItemsDescriptions();
    }

    public void LoadRichPresence()
    {
        DotaResources.LoadRichPresence();
    }

    public Bitmap ExtractAndGenerate(string ruta)
    {
        return DotaResources.ExtractAndGenerate(ruta);
    }
}