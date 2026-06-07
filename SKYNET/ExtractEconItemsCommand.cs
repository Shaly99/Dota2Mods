using SkiaSharp;
using System.Drawing.Imaging;
using ValveResourceFormat;
using ValveResourceFormat.ResourceTypes;

namespace SKYNET;

public class ExtractEconItemsCommand
{
    private static System.Drawing.Bitmap SKBitmapToBitmap(SKBitmap skBitmap)
    {
        using SKImage image = SKImage.FromBitmap(skBitmap);
        using SKData data = image.Encode(SKEncodedImageFormat.Png, 100);
        using MemoryStream ms = new MemoryStream(data.ToArray());
        using System.Drawing.Bitmap temp = new System.Drawing.Bitmap(ms);
        return new System.Drawing.Bitmap(temp);
    }
    public string Description { get; }

    public string Usage { get; }

    public string Name { get; }

    public string Module { get; }

    public void ProcessCommand()
    {

        string text = Path.Combine("D:\\Juegos\\Steam\\Steam\\steamapps\\common\\dota 2 beta\\game\\dota");
        string text2 = Path.Combine(text, "pak01_dir.vpk");
        string text3 = Path.Combine(Paths.WwwBasePath, "cdn.dota2.com", "apps", "570", "images");
        Paths.EnsureDirectory(text3);
        if (Directory.Exists(text) && File.Exists(text2))
        {
            modCommon.WriteLine("Extracting econ images from dota");
            try
            {
                using Package package = new Package();
                package.Read(text2);
                List<PackageEntry> list = package.Entries["vtex_c"].Where((PackageEntry e) => e.DirectoryName.Contains("panorama/images/econ")).ToList();
                if (list.Any())
                {
                    modCommon.WriteLine($"{list.Count} Econ items found.");
                    int num = 0;
                    foreach (PackageEntry item in list)
                    {
                        string text4 = Path.Combine(text3, item.DirectoryName.Replace("panorama/images/", string.Empty));
                        string text5 = Path.Combine(text4, item.FileName.Replace("_png", string.Empty) + ".png");
                        num++;
                        if (num % 1000 == 0)
                        {
                            modCommon.Write($"{(float)num / ((float)list.Count * 1f) * 100f:0}% generated.\r\n");
                        }
                        if (File.Exists(text5))
                        {
                            continue;
                        }
                        Paths.EnsureDirectory(text4);
                        byte[] output = null;
                        package.ReadEntry(item, out output, validateCrc: false);
                        using Resource resource = new Resource();
                        using MemoryStream input = RecyclableStreams.Create(output);
                        resource.Read(input);
                        try
                        {
                            Texture texture = resource.DataBlock as Texture
                                ?? throw new InvalidOperationException("DataBlock no es Texture");

                            using Bitmap bitmap = SKBitmapToBitmap(texture.GenerateBitmap());

                            if (bitmap.Size.Width == 256 && bitmap.Size.Height == 256)
                            {
                                using Bitmap bitmap2 = new Bitmap(256, 170);
                                using (Graphics graphics = Graphics.FromImage(bitmap2))
                                {
                                    graphics.DrawImage(bitmap,
                                        new Rectangle(0, 0, bitmap2.Width, bitmap2.Height),
                                        new Rectangle(0, 0, bitmap2.Width, bitmap2.Height),
                                        GraphicsUnit.Pixel);
                                    graphics.Save();
                                }
                                bitmap2.Save(text5, ImageFormat.Png);
                            }
                            else
                            {
                                bitmap.Save(text5, ImageFormat.Png);
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                    modCommon.Write($"{(float)num / ((float)list.Count * 1f) * 100f:0}% generated.\r\n");
                }
            }
            catch (Exception ex2)
            {
                modCommon.WriteLine("Error extracting econ images.\r\n" + ex2.Message + "\r\n" + ex2.StackTrace);
                return;
            }
            modCommon.WriteLine("Econ images was extracted successfully.");
        }
        else
        {
            modCommon.WriteLine("Dota 2 was not found on content folders " + text + "...");
        }
    }
}
