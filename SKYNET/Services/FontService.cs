using System.Drawing.Text;

namespace SKYNET;

public static class FontService
{
    // ════════════════════════════════════════════════════════════
    // COLECCIÓN PRIVADA DE FONTS
    // ════════════════════════════════════════════════════════════

    private static readonly PrivateFontCollection _fonts = new PrivateFontCollection();

    private static FontFamily? _reaverFamily;
    private static FontFamily? _radianceFamily;

    private static bool _initialized = false;

    // ════════════════════════════════════════════════════════════
    // INICIALIZACIÓN
    // ════════════════════════════════════════════════════════════

    public static void Initialize()
    {
        if (_initialized) return;

        try
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            string reaverPath = Path.Combine(basePath, "fonts", "reaver-semibold.otf");
            string radiancePath = Path.Combine(basePath, "fonts", "radiance-semibold.otf");

            if (File.Exists(reaverPath))
                _fonts.AddFontFile(reaverPath);

            if (File.Exists(radiancePath))
                _fonts.AddFontFile(radiancePath);

            foreach (FontFamily family in _fonts.Families)
            {
                if (family.Name.Contains("Reaver", StringComparison.OrdinalIgnoreCase))
                    _reaverFamily = family;

                if (family.Name.Contains("Radiance", StringComparison.OrdinalIgnoreCase))
                    _radianceFamily = family;
            }

            _initialized = true;
        }
        catch (Exception ex)
        {
            modCommon.Save(ex);
        }
    }

    // ════════════════════════════════════════════════════════════
    // ACCESO A FONTS
    // ════════════════════════════════════════════════════════════

    /// <summary>
    /// Font Reaver Bold para el banner principal y tabs superiores.
    /// </summary>
    public static Font GetReaver(float size) =>
        _reaverFamily is not null
            ? new Font(_reaverFamily, size, FontStyle.Regular, GraphicsUnit.Point)
            : new Font("Arial", size, FontStyle.Bold, GraphicsUnit.Point);

    /// <summary>
    /// Font Radiance Bold para el resto de la app.
    /// </summary>
    public static Font GetRadiance(float size) =>
        _radianceFamily is not null
            ? new Font(_radianceFamily, size, FontStyle.Regular, GraphicsUnit.Point)
            : new Font("Arial", size, FontStyle.Regular, GraphicsUnit.Point);
    // ════════════════════════════════════════════════════════════
    // SUAVIZADO DE TEXTO GLOBAL
    // ════════════════════════════════════════════════════════════

    /// <summary>
    /// Aplica el suavizado de texto estándar de la app a un Graphics.
    /// Usar al principio del OnPaint en controles que dibujan texto.
    /// </summary>
    public static void ApplyTextSmoothing(System.Drawing.Graphics g)
    {
        if (g == null) return;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
    }
}