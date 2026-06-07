using Microsoft.Win32;
using SKYNET;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

public class modCommon
{
    public enum Language
    {
        Spanish,
        English
    }

    public static bool NeedUpdate;

    public static string DataDirectory;

    public static string TempDirectory;

    public static string DetectedDota2Path;

    public static Language CurrentLanguage;

    public static string VPKGeneratorLocation;

    public static Image DefaultIcon;

    private static ILog log;

    public static string VPKLocation { get; set; }

    public static Items ItemIDSelected { get; set; }

    public static bool ExtractFiles { get; set; }

    public static string CurrentDirectory
    {
        get
        {
            using var process = Process.GetCurrentProcess();
            return new FileInfo(process.MainModule.FileName).Directory?.FullName ?? "";
        }
    }

    // ════════════════════════════════════════════════════════════
    // CONSTRUCTOR ESTÁTICO
    // ════════════════════════════════════════════════════════════

    static modCommon()
    {
        NeedUpdate = false;
        DataDirectory = GetDataDirectory();
        TempDirectory = DataDirectory + "/TEMP";
        DetectedDota2Path = Paths.GetParent(Paths.GetParent(Win32Path())) + "dota";
        CurrentLanguage = Language.Spanish;
        VPKGeneratorLocation = DataDirectory + "/VPKGenerator";
        DefaultIcon = Resources.testitem_slot_empty;
        log = new ILog();
    }

    private static string GetDataDirectory()
    {
        using var process = Process.GetCurrentProcess();
        return (new FileInfo(process.MainModule.FileName).Directory?.FullName ?? "") + "\\Data";
    }

    public static string Win32Path()
    {
        try
        {
            using var registryKey = Registry.ClassesRoot.OpenSubKey("dota2\\Shell\\Open\\Command", writable: true);
            if (registryKey == null) return "";

            string path = registryKey.GetValue(null).ToString().Split('"')[1];
            return Directory.GetParent(path)?.ToString() ?? "";
        }
        catch
        {
            return "";
        }
    }

    public static string GetClientVersion()
    {
        try
        {
            string path = SKYNET.Settings.Dota2Path + "\\steam.inf";
            if (!File.Exists(path)) return "";

            string[] lines = File.ReadAllLines(path, Encoding.Default);
            foreach (string line in lines)
            {
                if (line.StartsWith("ClientVersion"))
                {
                    string[] parts = line.Split('=');
                    if (parts.Length >= 2)
                    {
                        return parts[1].TrimStart(' ');
                    }
                }
            }
        }
        catch { }
        return "";
    }

    // ════════════════════════════════════════════════════════════
    // HELPERS DE STRING
    // ════════════════════════════════════════════════════════════

    internal static string FirstUpper(string text)
    {
        if (string.IsNullOrEmpty(text)) return "";
        return char.ToUpper(text[0]) + text.Substring(1).ToLower();
    }

    internal static string PreparePrefab(string prefab)
    {
        string text = prefab switch
        {
            "default_item" => "Default Item",
            "multikill_banner" => "multikill banner",
            "radiantcreeps" => "creeps",
            "direcreeps" => "creeps",
            "radianttowers" => "towers",
            "diretowers" => "towers",
            "versus_screen" => "versus screen",
            "cursor_pack" => "cursors",
            "emoticon_tool" => "emoticons",
            "hud_skin" => "hud skin",
            "loading_screen" => "loading",
            _ => prefab
        };
        return FirstUpper(text);
    }

    internal static bool GetBool(string boolean)
    {
        return boolean?.ToLower() switch
        {
            "1" => true,
            "true" => true,
            _ => false
        };
    }

    // ════════════════════════════════════════════════════════════
    // CURSOR
    // ════════════════════════════════════════════════════════════

    internal static void SetCursor(Control control)
    {
        if (control == null) return;

        Icon cursor = Resources.cursor;
        control.Cursor = new Cursor(cursor.Handle);

        foreach (Control child in control.Controls)
        {
            SetCursor(child);
        }
    }

    // ════════════════════════════════════════════════════════════
    // UI / MENSAJES (wrappers de IUIService)
    // ════════════════════════════════════════════════════════════

    public static void WriteLine(string v, bool loading = false)
    {
        if (loading && frmMain.frm != null)
        {
            frmMain.frm.MostrarLabel(v);
        }
    }

    public static void Write(string v)
    {
        WriteLine(v);
    }

    internal static void Show(object v = null)
    {
        string message = v?.ToString() ?? "La variable enviada para mostrar el mensaje es nulo";
        using var frmMessage = new frmMessage(message);
        frmMessage.ShowDialog();
    }

    internal static void Save(Exception ex)
    {
        ILog.Save(ex);
    }

    internal static void Save(string message, Exception ex)
    {
        ILog.Save(message, ex);
    }

    public static void InvokeAction(Control control, Action action)
    {
        control?.InvokeIfRequired(action);
    }

    // ════════════════════════════════════════════════════════════
    // COLORS (wrappers de IRarityColorService)
    // ════════════════════════════════════════════════════════════

    internal static Color ColorItemsFromRarity(string ItemId)
    {
        var colorService = ServiceContainer.TryGet<IRarityColorService>();
        return colorService?.GetColorForItemId(ItemId) ?? Color.Gray;
    }

    internal static Color ColorBundleFromRarity(string ItemId)
    {
        var colorService = ServiceContainer.TryGet<IRarityColorService>();
        return colorService?.GetColorForItemId(ItemId) ?? Color.Empty;
    }

    internal static Color ColorFromRarity(string rarity)
    {
        var colorService = ServiceContainer.TryGet<IRarityColorService>();
        return colorService?.GetColorForRarity(rarity) ?? Color.Empty;
    }

    // ════════════════════════════════════════════════════════════
    // PREFABS (wrappers de IPrefabsService)
    // ════════════════════════════════════════════════════════════

    internal static string GetSlotFromPrefabs(string slot, string type)
    {
        var prefabsService = ServiceContainer.TryGet<IPrefabsService>();
        return prefabsService?.GetSlotForPrefab(slot, type) ?? slot ?? "";
    }

    internal static string GetRarityFromPrefabs(string rarity, string type)
    {
        var prefabsService = ServiceContainer.TryGet<IPrefabsService>();
        return prefabsService?.GetRarityForPrefab(rarity, type) ?? rarity ?? "";
    }

    // ════════════════════════════════════════════════════════════
    // ARCHIVOS
    // ════════════════════════════════════════════════════════════

    /// <summary>
    /// Lee todas las líneas de un archivo usando codificación Default.
    /// Reemplazo limpio del método viejo que usaba TextBox para parsear.
    /// </summary>
    internal static List<string> ReadAllLines(string FilePath)
    {
        if (!File.Exists(FilePath)) return new List<string>();
        return File.ReadAllLines(FilePath, Encoding.Default).ToList();
    }

    /// <summary>
    /// Escribe todas las líneas a un archivo usando codificación Default.
    /// </summary>
    internal static void WriteAllLines(string FilePath, List<string> lines)
    {
        if (lines == null) return;
        File.WriteAllLines(FilePath, lines, Encoding.Default);
    }

    // ════════════════════════════════════════════════════════════
    // TEXTO Y MEDICIÓN
    // ════════════════════════════════════════════════════════════

    public static SizeF GetTextSize(string text, Font font)
    {
        using Bitmap bitmap = new Bitmap(1, 1);
        using Graphics graphics = Graphics.FromImage(bitmap);
        return graphics.MeasureString(text, font);
    }

    public static string GetVideoFileName(string HeroName)
    {
        if (string.IsNullOrEmpty(HeroName)) return "";
        return (HeroName + ".webm").Replace(" ", "_");
    }

    // ════════════════════════════════════════════════════════════
    // CONVERSIÓN HTML A TEXTO
    // ════════════════════════════════════════════════════════════

    public static string ConvertHtmlTotext(string source)
    {
        try
        {
            string text = source.Replace("\r", " ");
            text = text.Replace("\n", " ");
            text = text.Replace("\t", string.Empty);
            text = Regex.Replace(text, "( )+", " ");
            text = Regex.Replace(text, "<( )*head([^>])*>", "<head>", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "(<( )*(/)( )*head( )*>)", "</head>", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "(<head>).*(</head>)", string.Empty, RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "<( )*script([^>])*>", "<script>", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "(<( )*(/)( )*script( )*>)", "</script>", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "(<script>).*(</script>)", string.Empty, RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "<( )*style([^>])*>", "<style>", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "(<( )*(/)( )*style( )*>)", "</style>", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "(<style>).*(</style>)", string.Empty, RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "<( )*td([^>])*>", "\t", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "<( )*br( )*>", "\r", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "<( )*li( )*>", "\r", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "<( )*div([^>])*>", "\r\r", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "<( )*tr([^>])*>", "\r\r", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "<( )*p([^>])*>", "\r\r", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "<[^>]*>", string.Empty, RegexOptions.IgnoreCase);
            text = Regex.Replace(text, " ", " ", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "&bull;", " * ", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "&lsaquo;", "<", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "&rsaquo;", ">", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "&trade;", "(tm)", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "&frasl;", "/", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "&lt;", "<", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "&gt;", ">", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "&copy;", "(c)", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "&reg;", "(r)", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "&(.{2,6});", string.Empty, RegexOptions.IgnoreCase);
            text = text.Replace("\n", "\r");
            text = Regex.Replace(text, "(\r)( )+(\r)", "\r\r", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "(\t)( )+(\t)", "\t\t", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "(\t)( )+(\r)", "\t\r", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "(\r)( )+(\t)", "\r\t", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "(\r)(\t)+(\r)", "\r\r", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "(\r)(\t)+", "\r\t", RegexOptions.IgnoreCase);
            string text2 = "\r\r\r";
            string text3 = "\t\t\t\t\t";
            for (int i = 0; i < text.Length; i++)
            {
                text = text.Replace(text2, "\r\r");
                text = text.Replace(text3, "\t\t\t\t");
                text2 += "\r";
                text3 += "\t";
            }
            return text;
        }
        catch
        {
            return source;
        }
    }

    // ════════════════════════════════════════════════════════════
    // MEMORIA
    // ════════════════════════════════════════════════════════════

    /// <summary>
    /// Solicita al SO que libere la memoria del proceso (working set).
    /// </summary>
    public static void ReleaseMemory()
    {
        try
        {
            SetProcessWorkingSetSize(GetCurrentProcess(), -1, -1);
        }
        catch
        {
            // Ignorar si falla (no es crítico)
        }
    }

    [DllImport("KERNEL32.DLL", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    private static extern bool SetProcessWorkingSetSize(IntPtr pProcess, int dwMinimumWorkingSetSize, int dwMaximumWorkingSetSize);

    [DllImport("KERNEL32.DLL", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    private static extern IntPtr GetCurrentProcess();

    // ════════════════════════════════════════════════════════════
    // IMÁGENES
    // ════════════════════════════════════════════════════════════

    internal static Image GetHeroImage(string name)
    {
        string path = DataDirectory + "/panorama/images/heroes/selection/" + name + ".png";
        return ImageCache.GetOrLoad(path, Resources.default_item);
    }

    public static Bitmap ChangeOpacity(Image img, float opacityvalue)
    {
        if (img == null) return null;

        try
        {
            Bitmap bitmap = new Bitmap(img.Width, img.Height);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            using (ImageAttributes imageAttributes = new ImageAttributes())
            {
                ColorMatrix newColorMatrix = new ColorMatrix { Matrix33 = opacityvalue };
                imageAttributes.SetColorMatrix(newColorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                graphics.DrawImage(img, new Rectangle(0, 0, bitmap.Width, bitmap.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imageAttributes);
            }

            return bitmap;
        }
        catch (Exception ex)
        {
            Save(ex);
            return null;
        }
    }
}