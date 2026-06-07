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

            string reaverPath = Path.Combine(basePath, "fonts", "reaver-bold.otf");
            string radiancePath = Path.Combine(basePath, "fonts", "radiance-bold.otf");

            if (File.Exists(reaverPath))
            {
                _fonts.AddFontFile(reaverPath);
            }

            if (File.Exists(radiancePath))
            {
                _fonts.AddFontFile(radiancePath);
            }

            // Buscar las familias por nombre
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
}