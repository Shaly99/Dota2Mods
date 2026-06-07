namespace SKYNET;

/// <summary>
/// Implementación de ISettingsService que delega a la clase estática Settings.
/// </summary>
public class SettingsService : ISettingsService
{
    public string Dota2Path
    {
        get => Settings.Dota2Path;
        set => Settings.Dota2Path = value;
    }

    public bool GenerateOnStart
    {
        get => Settings.GenerateOnStart;
        set => Settings.GenerateOnStart = value;
    }

    public bool OpenDota
    {
        get => Settings.OpenDota;
        set => Settings.OpenDota = value;
    }

    public bool ActiveSounds
    {
        get => Settings.ActiveSounds;
        set => Settings.ActiveSounds = value;
    }

    public string ClientVersion
    {
        get => Settings.ClientVersion;
        set => Settings.ClientVersion = value;
    }

    public void Save()
    {
        Settings.Save();
    }

    public void Load()
    {
        Settings.Load();
    }
}