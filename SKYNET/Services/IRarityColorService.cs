namespace SKYNET;

/// <summary>
/// Servicio para obtener colores asociados a rarezas de items.
/// Encapsula el acceso a la lista de colores cargada del VPK.
/// </summary>
public interface IRarityColorService
{
    /// <summary>
    /// Obtiene el color para una rareza específica.
    /// Si la rareza está vacía, usa "common".
    /// </summary>
    Color GetColorForRarity(string rarity);

    /// <summary>
    /// Obtiene el color del item por su ID (busca el item y usa su rareza).
    /// </summary>
    Color GetColorForItemId(string itemId);

    /// <summary>
    /// Registra un nuevo color (usado durante la carga inicial).
    /// </summary>
    void RegisterColor(Colors color);
}