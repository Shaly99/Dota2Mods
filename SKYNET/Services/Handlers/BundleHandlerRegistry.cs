namespace SKYNET;

/// <summary>
/// Registro de todos los handlers de bundles.
/// Permite encontrar el handler correcto para un BundleBox dado.
/// </summary>
public class BundleHandlerRegistry
{
    private readonly List<IBundleHandler> _handlers = new List<IBundleHandler>();

    /// <summary>
    /// Registra un handler.
    /// </summary>
    public void Register(IBundleHandler handler)
    {
        if (handler == null) return;
        _handlers.Add(handler);
    }

    /// <summary>
    /// Busca el handler apropiado para un BundleBox.
    /// Retorna null si no hay handler registrado.
    /// </summary>
    public IBundleHandler GetHandler(BundleBox box)
    {
        if (box?.item?.prefab == null) return null;

        // Buscar el primer handler que pueda manejar este box
        foreach (var handler in _handlers)
        {
            if (handler.CanHandle(box))
            {
                return handler;
            }
        }

        return null;
    }

    /// <summary>
    /// Cantidad de handlers registrados.
    /// </summary>
    public int Count => _handlers.Count;
}