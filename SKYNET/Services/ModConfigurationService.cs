namespace SKYNET;

/// <summary>
/// Implementación del servicio de configuración del mod.
/// Por ahora delega a frmMain.manager (singleton) hasta que se refactorice más adelante.
/// </summary>
public class ModConfigurationService : IModConfigurationService
{
    private static modManager Manager => frmMain.manager;

    // ════════════════════════════════════════
    // GETTERS
    // ════════════════════════════════════════
    public Items Terrain => Manager?.Terrain;
    public Items WeatherEffect => Manager?.WeatherEffect;
    public Items RadiantTowers => Manager?.RadiantTowers;
    public Items DireTowers => Manager?.DireTowers;
    public Items RadiantCreeps => Manager?.RadiantCreeps;
    public Items DireCreeps => Manager?.DireCreeps;
    public Items VersusScreen => Manager?.VersusScreen;
    public Items CursorPack => Manager?.CursorPack;
    public Items MultikillBanner => Manager?.MultikillBanner;
    public Items Emblem => Manager?.Emblem;
    public Items Emoticons => Manager?.Emoticons;
    public Items HUDSkin => Manager?.HUDSkin;
    public Items Courier => Manager?.Courier;
    public Items Ward => Manager?.Ward;
    public Items LoadingScreen => Manager?.LoadingScreen;
    public Items MusicPack => Manager?.MusicPack;
    public Items Announcer => Manager?.Announcer;
    public Items MegaKillAnnouncer => Manager?.MegaKillAnnouncer;
    public Items Pets => Manager?.Pets;
    public Items StreakEffect => Manager?.StreakEffect;

    // ════════════════════════════════════════
    // SETTERS
    // ════════════════════════════════════════
    public void SetTerrain(Items item) { if (Manager != null) Manager.Terrain = item; }
    public void SetWeatherEffect(Items item) { if (Manager != null) Manager.WeatherEffect = item; }
    public void SetRadiantTowers(Items item) { if (Manager != null) Manager.RadiantTowers = item; }
    public void SetDireTowers(Items item) { if (Manager != null) Manager.DireTowers = item; }
    public void SetRadiantCreeps(Items item) { if (Manager != null) Manager.RadiantCreeps = item; }
    public void SetDireCreeps(Items item) { if (Manager != null) Manager.DireCreeps = item; }
    public void SetVersusScreen(Items item) { if (Manager != null) Manager.VersusScreen = item; }
    public void SetCursorPack(Items item) { if (Manager != null) Manager.CursorPack = item; }
    public void SetMultikillBanner(Items item) { if (Manager != null) Manager.MultikillBanner = item; }
    public void SetEmblem(Items item) { if (Manager != null) Manager.Emblem = item; }
    public void SetEmoticons(Items item) { if (Manager != null) Manager.Emoticons = item; }
    public void SetHUDSkin(Items item) { if (Manager != null) Manager.HUDSkin = item; }
    public void SetCourier(Items item) { if (Manager != null) Manager.Courier = item; }
    public void SetWard(Items item) { if (Manager != null) Manager.Ward = item; }
    public void SetLoadingScreen(Items item) { if (Manager != null) Manager.LoadingScreen = item; }
    public void SetMusicPack(Items item) { if (Manager != null) Manager.MusicPack = item; }
    public void SetAnnouncer(Items item) { if (Manager != null) Manager.Announcer = item; }
    public void SetMegaKillAnnouncer(Items item) { if (Manager != null) Manager.MegaKillAnnouncer = item; }
    public void SetPets(Items item) { if (Manager != null) Manager.Pets = item; }
    public void SetStreakEffect(Items item) { if (Manager != null) Manager.StreakEffect = item; }

    // ════════════════════════════════════════
    // RESET
    // ════════════════════════════════════════
    public void ResetCategory(ItemCategory category)
    {
        if (Manager == null) return;

        switch (category)
        {
            case ItemCategory.Announcer: Manager.Announcer = null; break;
            case ItemCategory.MegaKillAnnouncer: Manager.MegaKillAnnouncer = null; break;
            case ItemCategory.MultikillBanner: Manager.MultikillBanner = null; break;
            case ItemCategory.Courier: Manager.Courier = null; break;
            case ItemCategory.CursorPack: Manager.CursorPack = null; break;
            case ItemCategory.DireCreeps: Manager.DireCreeps = null; break;
            case ItemCategory.DireTowers: Manager.DireTowers = null; break;
            case ItemCategory.RadiantCreeps: Manager.RadiantCreeps = null; break;
            case ItemCategory.RadiantTowers: Manager.RadiantTowers = null; break;
            case ItemCategory.MusicPack: Manager.MusicPack = null; break;
            case ItemCategory.Pets: Manager.Pets = null; break;
            case ItemCategory.VersusScreen: Manager.VersusScreen = null; break;
            case ItemCategory.Ward: Manager.Ward = null; break;
            case ItemCategory.WeatherEffect: Manager.WeatherEffect = null; break;
            case ItemCategory.LoadingScreen: Manager.LoadingScreen = null; break;
            case ItemCategory.Emblem: Manager.Emblem = null; break;
            case ItemCategory.Emoticons: Manager.Emoticons = null; break;
            case ItemCategory.HUDSkin: Manager.HUDSkin = null; break;
            case ItemCategory.Terrain: Manager.Terrain = null; break;
            case ItemCategory.StreakEffect: Manager.StreakEffect = null; break;
        }
    }
    public void ResetAll()
    {
        if (Manager == null) return;

        Manager.Announcer = null;
        Manager.MegaKillAnnouncer = null;
        Manager.MultikillBanner = null;
        Manager.Courier = null;
        Manager.CursorPack = null;
        Manager.DireCreeps = null;
        Manager.DireTowers = null;
        Manager.RadiantCreeps = null;
        Manager.RadiantTowers = null;
        Manager.MusicPack = null;
        Manager.Pets = null;
        Manager.VersusScreen = null;
        Manager.Ward = null;
        Manager.WeatherEffect = null;
        Manager.LoadingScreen = null;
        Manager.Emblem = null;
        Manager.Emoticons = null;
        Manager.HUDSkin = null;
        Manager.Terrain = null;
        Manager.StreakEffect = null;
    }

    // ════════════════════════════════════════
    // PERSISTENCIA
    // ════════════════════════════════════════
    public void Save()
    {
        Manager?.Save();
    }
}