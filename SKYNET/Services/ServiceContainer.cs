namespace SKYNET;

/// <summary>
/// Contenedor simple de servicios (Dependency Injection casero).
/// Permite registrar y obtener servicios sin librerías externas.
/// 
/// USO:
///   // Registrar:
///   ServiceContainer.Register&lt;IItemCatalog&gt;(new ItemCatalog());
///   
///   // Obtener:
///   var catalog = ServiceContainer.Get&lt;IItemCatalog&gt;();
/// </summary>
public static class ServiceContainer
{
    private static readonly Dictionary<Type, object> _services = new();
    private static readonly object _lock = new();

    /// <summary>
    /// Registra una instancia de servicio para una interfaz.
    /// Si ya existe, la reemplaza.
    /// </summary>
    public static void Register<TInterface>(TInterface implementation)
        where TInterface : class
    {
        if (implementation == null)
            throw new ArgumentNullException(nameof(implementation));

        lock (_lock)
        {
            _services[typeof(TInterface)] = implementation;
        }
    }

    /// <summary>
    /// Obtiene un servicio registrado.
    /// Lanza InvalidOperationException si no está registrado.
    /// </summary>
    public static TInterface Get<TInterface>()
        where TInterface : class
    {
        lock (_lock)
        {
            if (_services.TryGetValue(typeof(TInterface), out object service))
            {
                return (TInterface)service;
            }

            throw new InvalidOperationException(
                $"Servicio '{typeof(TInterface).Name}' no está registrado. " +
                $"Asegúrate de llamar a ServiceContainer.Register antes de usarlo.");
        }
    }

    /// <summary>
    /// Intenta obtener un servicio. Devuelve null si no está registrado.
    /// </summary>
    public static TInterface TryGet<TInterface>()
        where TInterface : class
    {
        lock (_lock)
        {
            if (_services.TryGetValue(typeof(TInterface), out object service))
            {
                return (TInterface)service;
            }
            return null;
        }
    }

    /// <summary>
    /// Verifica si un servicio está registrado.
    /// </summary>
    public static bool IsRegistered<TInterface>()
        where TInterface : class
    {
        lock (_lock)
        {
            return _services.ContainsKey(typeof(TInterface));
        }
    }

    /// <summary>
    /// Limpia todos los servicios registrados.
    /// Útil para tests o reinicio limpio.
    /// </summary>
    public static void Clear()
    {
        lock (_lock)
        {
            _services.Clear();
        }
    }
}