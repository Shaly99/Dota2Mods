namespace SKYNET;

public class ExtractRichPresenceCommand
{
    private bool bool_0;

    public string Description { get; }

    public string Usage { get; }

    public string Name { get; }

    public string Module { get; }

    private static IVpkService Vpk => ServiceContainer.Get<IVpkService>();

    public void ProcessCommand()
    {
        if (bool_0)
        {
            modCommon.WriteLine("Already extracting rich presence tokens...");
            return;
        }
        string richPresencePath = Paths.GetRichPresencePath(570u);
        Paths.EnsureDirectory(richPresencePath);
        if (Vpk.IsLoaded)
        {
            modCommon.WriteLine("Extracting rich presence tokens");
            bool_0 = true;
            try
            {
                try
                {
                    List<PackageEntry> list = Vpk.GetEntriesByExtension("txt").Where((PackageEntry e) => e.FileName.Contains("richpresence_spanish") || e.FileName.Contains("richpresence_english")).ToList();
                    if (list.Any())
                    {
                        modCommon.WriteLine($"{list.Count} rich presence languages found...");
                        int num = 0;
                        foreach (PackageEntry item in list)
                        {
                            string path = Path.Combine(richPresencePath, item.FileName.Replace("richpresence_", string.Empty) + ".vdf");
                            num++;
                            modCommon.Write($"{(float)num / ((float)list.Count * 1f) * 100f:0}% exported.\r\n");
                            using FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate);
                            byte[] output = null;
                            Vpk.ReadEntry(item, out output);
                            fileStream.Write(output, 0, output.Length);
                        }
                    }
                }
                finally
                {
                }
            }
            catch (Exception ex)
            {
                bool_0 = false;
                modCommon.WriteLine("Error extracting rich presence tokens.\r\n" + ex.Message + "\r\n" + ex.StackTrace);
                return;
            }
            modCommon.WriteLine("Rich presence tokens extracted successfully.");
        }
        bool_0 = false;
    }
}
