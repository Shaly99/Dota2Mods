using SkiaSharp;
using SKYNET;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using ValveResourceFormat;
using ValveResourceFormat.ResourceTypes;

internal class LoadingScreenManager
{
    private static IVpkService Vpk => ServiceContainer.Get<IVpkService>();
    internal static void Extract()
    {
        frmExportLS frmExportLS2 = new frmExportLS();
        frmExportLS2.ShowDialog();
    }

    internal static void Extract(Items itemScreen, bool Desktop = false, bool open = false)
    {
        // 🔑 Validar parámetros
        if (itemScreen == null)
        {
            modCommon.Show("No hay loading screen seleccionado");
            return;
        }

        if (string.IsNullOrEmpty(itemScreen.name))
        {
            modCommon.Show("El loading screen seleccionado no tiene nombre");
            return;
        }
        // 🔑 Detectar si es el "Default" (no es un archivo real del VPK)
        if (itemScreen.name.Trim().Equals("Default Loading Screen", StringComparison.OrdinalIgnoreCase)
            || itemScreen.name.Trim().Equals("Default", StringComparison.OrdinalIgnoreCase))
        {
            modCommon.Show(
                "El 'Default Loading Screen' es la pantalla genérica del juego.\n\n" +
                "No es un archivo del VPK, sino que se renderiza directamente por Dota 2.\n\n" +
                "Para exportar, seleccioná otro loading screen de la lista.");
            return;
        }

        if (!Vpk.IsLoaded)
        {
            modCommon.Show("El VPK no está cargado");
            return;
        }

        // 🔑 Obtener dimensiones del screen (esto SÍ está bien porque se llama desde el UI thread)
        Rectangle workingArea;
        try
        {
            workingArea = Screen.FromHandle(frmMain.frm.Handle).WorkingArea;
        }
        catch
        {
            // Fallback: dimensiones HD
            workingArea = new Rectangle(0, 0, 1920, 1080);
        }

        int width = workingArea.Width;
        int height = workingArea.Height;
        string dataDirectory = modCommon.DataDirectory;
        Paths.EnsureDirectory(dataDirectory);

        try
        {
            List<PackageEntry> list = Vpk.GetEntriesByExtension("vtex_c")
                .Where(l => l.DirectoryName != null && l.DirectoryName.Contains("panorama/images/loadingscreen"))
                .ToList();

            if (!list.Any())
            {
                modCommon.Show("No se encontraron loading screens en el VPK");
                return;
            }

            string targetName = itemScreen.name.Trim();
            bool found = false;

            foreach (PackageEntry item in list)
            {
                string fileName = item.GetFullPath().Replace("panorama/images/", "");
                string assetName = ItemsManager.NameFromAsset(fileName);

                // 🔑 Comparación tolerante
                string normalizedFound = assetName?.Trim() ?? "";

                if (!normalizedFound.Equals(targetName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                found = true;

                // ════════════════════════════════════════
                // CASO 1: Set Desktop wallpaper
                // ════════════════════════════════════════
                if (Desktop)
                {
                    string folderPath = modCommon.DataDirectory + "/Loading Screen/";
                    Paths.EnsureDirectory(folderPath);
                    string filePath = folderPath + assetName + ".png";

                    using Bitmap texture = GetTexture(Vpk.GetUnderlyingPackage(), item, new Rectangle(0, 0, width, height));
                    texture.Save(filePath, ImageFormat.Png);
                    Wallpaper.Set(filePath, Wallpaper.Style.Stretched);
                    return;
                }

                // ════════════════════════════════════════
                // CASO 2: Open file
                // ════════════════════════════════════════
                if (open)
                {
                    string folderPath = modCommon.DataDirectory + "/Loading Screen/";
                    Paths.EnsureDirectory(folderPath);
                    string filePath = folderPath + assetName + ".png";

                    if (!File.Exists(filePath))
                    {
                        using Bitmap texture = GetTexture(Vpk.GetUnderlyingPackage(), item, new Rectangle(0, 0, width, height));
                        texture.Save(filePath, ImageFormat.Png);
                    }

                    // Abrir el archivo
                    var processStartInfo = new ProcessStartInfo
                    {
                        FileName = filePath,
                        UseShellExecute = true
                    };
                    Process.Start(processStartInfo);
                    return;
                }

                // ════════════════════════════════════════
                // CASO 3: Export (SaveFileDialog)
                // ════════════════════════════════════════
                using SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    FileName = assetName + ".png",
                    Title = "Save loading screen file",
                    Filter = "PNG file | *.png"
                };

                DialogResult dialogResult = saveFileDialog.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    using Bitmap texture = GetTexture(Vpk.GetUnderlyingPackage(), item, new Rectangle(0, 0, width, height));
                    texture.Save(saveFileDialog.FileName, ImageFormat.Png);
                    modCommon.Show($"Loading screen exportado a:\n{saveFileDialog.FileName}");
                }

                return;
            }

            // Si llegamos acá, no se encontró el item
            if (!found)
            {
                modCommon.Show(
                    $"No se encontró la imagen para '{targetName}' en el VPK.\n\n" +
                    "Esto puede pasar si el loading screen es un placeholder " +
                    "o si fue removido del juego en una actualización reciente.");
            }
        }
        catch (Exception ex)
        {
            // 🔑 NO silenciar errores - mostrar y loguear
            modCommon.Save("Error exportando loading screen", ex);
            modCommon.Show($"Error: {ex.Message}");
        }
    }
    private static Bitmap ConvertSKBitmapToBitmap(SKBitmap skBitmap)
    {
        // Convertimos a SKImage primero
        using SKImage skImage = SKImage.FromBitmap(skBitmap);

        // Codificamos como PNG en memoria
        using SKData skData = skImage.Encode(SKEncodedImageFormat.Png, 100);

        // Creamos el Bitmap desde el stream
        using MemoryStream ms = new MemoryStream(skData.ToArray());

        // Importante: devolvemos una COPIA porque ms se cierra con using
        return new Bitmap(Image.FromStream(ms));
    }
    public static Bitmap GetTexture(Package apk, PackageEntry entry, Rectangle? resizeTo = null)
    {
        apk.ReadEntry(entry, out var output);
        Resource resource = new Resource();
        resource.Read(new MemoryStream(output));
        // Línea 163
        Texture texture = resource.DataBlock as Texture
            ?? throw new InvalidOperationException("El DataBlock no es un Texture.");

        // Línea 164
        SKBitmap skBitmap = texture.GenerateBitmap();
        Bitmap bitmap = ConvertSKBitmapToBitmap(skBitmap);
        if (resizeTo.HasValue)
        {
            Bitmap bitmap2 = bitmap;
            bitmap = ResizeImage(bitmap2, resizeTo.Value.Width, resizeTo.Value.Height);
            bitmap2.Dispose();
        }
        return bitmap;
    }

    private static Bitmap ResizeImage(Image image, int width, int height)
    {
        Rectangle destRect = new Rectangle(0, 0, width, height);
        Bitmap bitmap = new Bitmap(width, height);
        bitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);
        using (Graphics graphics = Graphics.FromImage(bitmap))
        {
            graphics.CompositingMode = CompositingMode.SourceCopy;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            using ImageAttributes imageAttributes = new ImageAttributes();
            imageAttributes.SetWrapMode(WrapMode.TileFlipXY);
            graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
        }
        return bitmap;
    }
}
