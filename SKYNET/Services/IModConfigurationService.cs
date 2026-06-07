namespace SKYNET;

/// <summary>
/// Servicio para gestionar la configuración de items del mod del usuario.
/// Encapsula el acceso a frmMain.manager.
/// </summary>
public interface IModConfigurationService
{
    // ════════════════════════════════════════
    // GETTERS (leer estado actual)
    // ════════════════════════════════════════
    Items Terrain { get; }
    Items WeatherEffect { get; }
    Items RadiantTowers { get; }
    Items DireTowers { get; }
    Items RadiantCreeps { get; }
    Items DireCreeps { get; }
    Items VersusScreen { get; }
    Items CursorPack { get; }
    Items MultikillBanner { get; }
    Items Emblem { get; }
    Items Emoticons { get; }
    Items HUDSkin { get; }
    Items Courier { get; }
    Items Ward { get; }
    Items LoadingScreen { get; }
    Items MusicPack { get; }
    Items Announcer { get; }
    Items MegaKillAnnouncer { get; }
    Items Pets { get; }
    Items StreakEffect { get; }

    // ════════════════════════════════════════
    // SETTERS (modificar estado)
    // ════════════════════════════════════════
    void SetTerrain(Items item);
    void SetWeatherEffect(Items item);
    void SetRadiantTowers(Items item);
    void SetDireTowers(Items item);
    void SetRadiantCreeps(Items item);
    void SetDireCreeps(Items item);
    void SetVersusScreen(Items item);
    void SetCursorPack(Items item);
    void SetMultikillBanner(Items item);
    void SetEmblem(Items item);
    void SetEmoticons(Items item);
    void SetHUDSkin(Items item);
    void SetCourier(Items item);
    void SetWard(Items item);
    void SetLoadingScreen(Items item);
    void SetMusicPack(Items item);
    void SetAnnouncer(Items item);
    void SetMegaKillAnnouncer(Items item);
    void SetPets(Items item);
    void SetStreakEffect(Items item);

    // ════════════════════════════════════════
    // RESET (volver a default)
    // ════════════════════════════════════════
    /// <summary>
    /// Resetea (pone en null) la configuración de una categoría específica.
    /// </summary>
    void ResetCategory(ItemCategory category);
    void ResetAll();
    // ════════════════════════════════════════
    // PERSISTENCIA
    // ════════════════════════════════════════
    void Save();
}