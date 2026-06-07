using System.Text.Json;

namespace SKYNET;

public class modManager
{
    public List<Hero> Heroes { get; set; } = new List<Hero>();

    public Items MusicPack { get; set; }

    public Items Announcer { get; set; }

    public Items MegaKillAnnouncer { get; set; }

    public Items Terrain { get; set; }

    public Items WeatherEffect { get; set; }

    public Items RadiantCreeps { get; set; }

    public Items DireCreeps { get; set; }

    public Items RadiantTowers { get; set; }

    public Items DireTowers { get; set; }

    public Items CursorPack { get; set; }

    public Items MultikillBanner { get; set; }

    public Items Emblem { get; set; }

    public Items LoadingScreen { get; set; }

    public Items VersusScreen { get; set; }

    public Items Emoticons { get; set; }

    public Items Courier { get; set; }

    public Items Ward { get; set; }

    public Items HUDSkin { get; set; }

    public Items Pets { get; set; }

    public Items StreakEffect { get; set; }

    internal void Save()
    {
        string path = modCommon.DataDirectory + "/[SKYNET] Dota2 Mods.bin";
        string contents = JsonSerializer.Serialize(this);
        File.WriteAllText(path, contents);
    }

    internal void Load()
    {
        modManager manager = new modManager();
        try
        {
            string path = modCommon.DataDirectory + "/[SKYNET] Dota2 Mods.bin";
            string input = File.ReadAllText(path);
            manager = JsonSerializer.Deserialize<modManager>(input) ?? new modManager();
        }
        catch
        {
            frmMain.manager = new modManager();
        }
        frmMain.manager = manager;
    }

    public void CreateMod()
    {
        ThreadManager.RunThread(modHelpers.ModWorker_DoWork);
    }

    public static void SetHeroesToList()
    {
        KValue kValue = KValue.LoadAsText(Path.Combine("data", "db", "activelist.txt"));
        if (kValue == null)
        {
            return;
        }

        // 🔑 Usar el servicio para gestionar héroes
        var heroesService = ServiceContainer.TryGet<IHeroesService>();
        if (heroesService == null) return;

        foreach (KValue hero in kValue.Children)
        {
            heroesService.AddIfMissing(hero.Name);
        }
    }
}
