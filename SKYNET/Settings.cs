using System.Text.Json;

namespace SKYNET;

public class Settings
{
    private static GlobalSettings instance;

    public static string Dota2Path
    {
        get
        {
            return instance.Dota2Path;
        }
        set
        {
            instance.Dota2Path = value;
        }
    }

    public static bool GenerateOnStart
    {
        get
        {
            return instance.GenerateOnStart;
        }
        set
        {
            instance.GenerateOnStart = value;
        }
    }

    public static bool OpenDota
    {
        get
        {
            return instance.OpenDota;
        }
        set
        {
            instance.OpenDota = value;
        }
    }

    public static bool ActiveSounds
    {
        get
        {
            return instance.ActiveSounds;
        }
        set
        {
            instance.ActiveSounds = value;
        }
    }

    public static string ClientVersion
    {
        get
        {
            return instance.ClientVersion;
        }
        set
        {
            instance.ClientVersion = value;
        }
    }

    static Settings()
    {
        instance = new GlobalSettings();
    }

    public static void Save()
    {
        string text = modCommon.CurrentDirectory + "/Data/Settings.ini";
        string contents = JsonSerializer.Serialize(instance);
        EnsureFile(text);
        File.WriteAllText(text, contents);
    }

    private static void EnsureFile(string fileName)
    {
        string directoryName = Path.GetDirectoryName(fileName);
        if (!Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }
        if (!File.Exists(fileName))
        {
            File.WriteAllText(fileName, "");
        }
    }

    public static void Load()
    {
        try
        {
            string path = modCommon.CurrentDirectory + "/Data/Settings.ini";
            string input = File.ReadAllText(path);
            instance = JsonSerializer.Deserialize<GlobalSettings>(input) ?? new GlobalSettings();
            Dota2Path = instance.Dota2Path;
            GenerateOnStart = instance.GenerateOnStart;
            OpenDota = instance.OpenDota;
            ActiveSounds = instance.ActiveSounds;
            ClientVersion = instance.ClientVersion;
        }
        catch (Exception)
        {
            modCommon.Show("Error reading settings file. Please erase it.");
            Environment.Exit(1);
        }
    }

}
