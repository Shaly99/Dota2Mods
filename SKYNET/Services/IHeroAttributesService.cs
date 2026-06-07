namespace SKYNET;

/// <summary>
/// Servicio para obtener atributos primarios de héroes desde npc_heroes.txt.
/// Permite agrupar héroes por Fuerza/Agilidad/Inteligencia/Universal.
/// </summary>
public interface IHeroAttributesService
{
    /// <summary>
    /// Carga los atributos de todos los héroes desde el archivo VPK extraído.
    /// </summary>
    void Load();

    /// <summary>
    /// Obtiene el atributo primario de un héroe.
    /// Devuelve Universal si no se encuentra (fallback seguro).
    /// </summary>
    HeroAttribute GetAttribute(string heroName);

    /// <summary>
    /// Indica si los datos ya fueron cargados.
    /// </summary>
    bool IsLoaded { get; }
}

/// <summary>
/// Atributo primario de un héroe en Dota 2.
/// </summary>
public enum HeroAttribute
{
    Strength,
    Agility,
    Intelligence,
    Universal
}