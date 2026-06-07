namespace SKYNET;

/// <summary>
/// Servicio responsable de generar el mod (.vpk) de Dota 2.
/// Toma la configuración del usuario y produce el archivo final.
/// </summary>
public interface IModBuilderService
{
    /// <summary>
    /// Construye el mod completo:
    /// 1. Limpia mods anteriores
    /// 2. Copia mods externos
    /// 3. Aplica todos los items seleccionados (héroes, world, misc)
    /// 4. Genera el archivo .vpk
    /// 5. Lo coloca en la carpeta SkynetMods
    /// 6. Actualiza gameinfo.gi
    /// 7. Opcionalmente abre Dota2
    /// </summary>
    void BuildMod();

    /// <summary>
    /// Limpia las entradas de SkynetMods del gameinfo.gi.
    /// Útil para restaurar el estado original.
    /// </summary>
    void ClearGameInfo();
}