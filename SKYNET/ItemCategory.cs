namespace SKYNET;

/// <summary>
/// Categorías de items disponibles en el juego.
/// Cada categoría corresponde a una de las listas que antes estaban dispersas en frmMain.
/// </summary>
public enum ItemCategory
{
    Items,              // Items normales de héroes (armas, armaduras, etc.)
    DefaultItems,       // Items por defecto
    Bundles,            // Bundles (sets completos)
    Towers,             // Torres genéricas
    Taunts,             // Taunts/burlas
    MusicPack,          // Packs de música
    Announcer,          // Locutores
    MegaKillAnnouncer,  // Locutores de mega-kills
    Pets,               // Mascotas
    Terrain,            // Terrenos del mapa
    WeatherEffect,      // Efectos climáticos
    RadiantCreeps,      // Creeps del Radiant
    DireCreeps,         // Creeps del Dire
    RadiantTowers,      // Torres del Radiant
    DireTowers,         // Torres del Dire
    CursorPack,         // Packs de cursores
    MultikillBanner,    // Banners de multi-kill
    Emblem,             // Emblemas
    LoadingScreen,      // Loading screens
    VersusScreen,       // Pantallas versus
    Emoticons,          // Emoticones
    Courier,            // Mensajeros
    Ward,               // Wards
    HUDSkin,            // Skins de HUD
    StreakEffect        // Efectos de racha de kills
}