namespace SKYNET;

/// <summary>
/// Implementación del validador de cache del VPK.
/// Usa tamaño + fecha de modificación como firma rápida.
/// </summary>
public class VpkCacheValidator : IVpkCacheValidator
{
    private readonly ISettingsService _settings;
    private readonly IUIService _ui;

    private const string CACHE_HASH_FILE = "vpk.hash";

    public VpkCacheValidator(ISettingsService settings, IUIService ui)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _ui = ui ?? throw new ArgumentNullException(nameof(ui));
    }

    // ════════════════════════════════════════════════════════
    // API PÚBLICA
    // ════════════════════════════════════════════════════════

    public bool IsCacheValid()
    {
        try
        {
            string vpkPath = GetVpkPath();
            string hashPath = GetHashFilePath();

            // Si no hay VPK, no podemos validar
            if (!File.Exists(vpkPath))
            {
                return false;
            }

            // Si no hay hash guardado, es la primera vez
            if (!File.Exists(hashPath))
            {
                return false;
            }

            // Verificar que los datos extraídos existen
            if (!ExtractedDataExists())
            {
                return false;
            }

            // Calcular firma actual del VPK
            string currentSignature = ComputeSignature(vpkPath);

            // Comparar con la guardada
            string savedSignature = File.ReadAllText(hashPath).Trim();

            bool isValid = currentSignature == savedSignature;

            if (isValid)
            {
                _ui.WriteLine("VPK no ha cambiado, usando cache existente.", loading: true);
            }
            else
            {
                _ui.WriteLine("VPK ha cambiado, re-extrayendo recursos...", loading: true);
            }

            return isValid;
        }
        catch (Exception ex)
        {
            _ui.LogException("Error validando cache del VPK", ex);
            return false; // En caso de error, mejor re-extraer
        }
    }

    public void SaveCacheSignature()
    {
        try
        {
            string vpkPath = GetVpkPath();
            string hashPath = GetHashFilePath();

            if (!File.Exists(vpkPath))
            {
                _ui.LogException(new FileNotFoundException(
                    "No se puede guardar firma: VPK no encontrado", vpkPath));
                return;
            }

            string signature = ComputeSignature(vpkPath);

            // Asegurar que el directorio existe
            string directory = Path.GetDirectoryName(hashPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(hashPath, signature);
        }
        catch (Exception ex)
        {
            _ui.LogException("Error guardando firma del cache", ex);
        }
    }

    public void InvalidateCache()
    {
        try
        {
            string hashPath = GetHashFilePath();
            if (File.Exists(hashPath))
            {
                File.Delete(hashPath);
            }
        }
        catch (Exception ex)
        {
            _ui.LogException("Error invalidando cache", ex);
        }
    }

    public bool ExtractedDataExists()
    {
        try
        {
            // Verificar que existen los directorios clave de la extracción
            string dataDir = modCommon.DataDirectory;
            string dbDir = Path.Combine(dataDir, "db");
            string imagesDir = Path.Combine(dataDir, "panorama", "images");
            string itemsGame = Path.Combine(dbDir, "items_game.txt");

            return Directory.Exists(dbDir)
                && Directory.Exists(imagesDir)
                && File.Exists(itemsGame);
        }
        catch
        {
            return false;
        }
    }

    // ════════════════════════════════════════════════════════
    // HELPERS PRIVADOS
    // ════════════════════════════════════════════════════════

    /// <summary>
    /// Calcula una firma rápida del VPK basada en tamaño + fecha de modificación.
    /// Formato: "tamaño_ticks"
    /// </summary>
    private string ComputeSignature(string vpkPath)
    {
        var fileInfo = new FileInfo(vpkPath);
        return $"{fileInfo.Length}_{fileInfo.LastWriteTimeUtc.Ticks}";
    }

    private string GetVpkPath()
    {
        return modCommon.VPKLocation;
    }

    private string GetHashFilePath()
    {
        return Path.Combine(modCommon.DataDirectory, CACHE_HASH_FILE);
    }
}