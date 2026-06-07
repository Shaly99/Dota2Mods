namespace SKYNET;

/// <summary>
/// Interfaz para manejar el click sobre un BundleBox de una categoría específica.
/// Cada categoría (music, announcer, terrain, etc.) implementa esta interfaz.
/// </summary>
public interface IBundleHandler
{
    /// <summary>
    /// El prefab que este handler maneja.
    /// Ej: "music", "announcer", "terrain", etc.
    /// </summary>
    string Prefab { get; }

    /// <summary>
    /// Determina si este handler aplica para el BundleBox dado.
    /// Útil para casos donde el prefab no es suficiente (ej: "misc" tiene weather y multikill_banner).
    /// </summary>
    bool CanHandle(BundleBox box);

    /// <summary>
    /// Procesa el click sobre el BundleBox.
    /// </summary>
    void Handle(BundleBox box);
}