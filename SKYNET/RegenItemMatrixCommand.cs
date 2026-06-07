namespace SKYNET;

public class RegenItemMatrixCommand
{
    public string Description { get; }

    public string Usage { get; }

    public string Name { get; }

    public string Module { get; }

    public void ProcessCommand(Package package)
    {
        string text = Path.Combine("data", "db");
        string path = Path.Combine(text, "items_game.txt");
        string path2 = Path.Combine(text, "lang_spanish.txt");
        string path3 = Path.Combine(text, "lang_english.txt");
        string path4 = Path.Combine(text, "portraits.txt");
        string path5 = Path.Combine(text, "activelist.txt");
        string path6 = Path.Combine(text, "items.txt");
        if (Settings.GenerateOnStart || !File.Exists(path))
        {
            Paths.EnsureDirectory(text);
            if (package != null)
            {
                SaveFile(package, "txt", "items_game", "", path);
                SaveFile(package, "txt", "items_spanish", "", path2);
                SaveFile(package, "txt", "items_english", "", path3);
                SaveFile(package, "txt", "portraits", "scripts/npc", path4);
                SaveFile(package, "txt", "activelist", "scripts/npc", path5);
                SaveFile(package, "txt", "items", "scripts/npc", path6);
            }
            else
            {
                modCommon.WriteLine("Dota 2 was not found on content folders...");
            }
            if (File.Exists(path))
            {
                CreateItemMatrix("data");
            }
        }
    }

    private void SaveFile(Package package, string type, string FileName, string DirectoryName, string path)
    {
        try
        {
            PackageEntry packageEntry = package.Entries[type].Find((PackageEntry p) => p.FileName == FileName && p.DirectoryName.Contains(DirectoryName));
            if (packageEntry != null)
            {
                using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate))
                {
                    byte[] output = null;
                    package.ReadEntry(packageEntry, out output);
                    fileStream.Write(output, 0, output.Length);
                    return;
                }
            }
        }
        catch (Exception)
        {
        }
    }

    public void CreateItemMatrix(string basePath)
    {
        Dictionary<string, uint> dictionary = new Dictionary<string, uint>();
        Dictionary<string, uint> dictionary2 = new Dictionary<string, uint>();
        Dictionary<string, uint> dictionary3 = new Dictionary<string, uint>();
        string text = Path.Combine("data", "db", "items_game.txt");
        string[] files = Directory.GetFiles(Path.Combine("data", "db"), "lang_*");
        List<KValue> list = new List<KValue>();
        string[] array = files;
        for (int i = 0; i < array.Length; i++)
        {
            KValue kValue = KValue.LoadAsText(array[i]);
            if (kValue != null)
            {
                list.Add(kValue);
            }
        }
    }
}
