using Microsoft.VisualBasic.CompilerServices;
using SkiaSharp;
using SKYNET.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Resources;
using ValveResourceFormat;
using ValveResourceFormat.ResourceTypes;

internal class VPKManager
{
    public static Bitmap ExtractAndGenerate(string filePath)
    {
        try
        {
            filePath = "panorama/images/" + filePath + "_png.vtex_c";
            string vPKLocation = modCommon.VPKLocation;
            using VpkFile vpkFile = new VpkFile(vPKLocation);
            vpkFile.Open();
            VpkNode file = vpkFile.GetFile(filePath);

            if (file == null)
            {
                VpkNode[] allFilesInDirectoryAndSubdirectories =
                    vpkFile.GetAllFilesInDirectoryAndSubdirectories(filePath);

                if (allFilesInDirectoryAndSubdirectories.Count() != 0)
                {
                    VpkNode node = allFilesInDirectoryAndSubdirectories[0];
                    return ExtractBitmap(vPKLocation, node);
                }

                // Opción A: si tenés Resources
                return SKYNET.Properties.Resources.default_item;

                // Opción B: si no tenés Resources, usá esto en su lugar:
                // return GetDefaultBitmap();
            }

            return ExtractBitmap(vPKLocation, file);
        }
        catch (Exception)
        {
            // Opción A:
            return SKYNET.Properties.Resources.default_item;

            // Opción B:
            // return GetDefaultBitmap();
        }
    }
    private static Bitmap SKBitmapToBitmap(SKBitmap skBitmap)
    {
        using SKImage image = SKImage.FromBitmap(skBitmap);
        using SKData data = image.Encode(SKEncodedImageFormat.Png, 100);
        using MemoryStream ms = new MemoryStream(data.ToArray());
        using Bitmap temp = new Bitmap(ms);
        return new Bitmap(temp);
    }
    private static Bitmap ExtractBitmap(string vpkDirFileName, VpkNode node)
    {
        string text = Path.Combine("data", node.FilePath.Replace("_png.vtex_c", ".png"));
        EnsureDirectoryExists(Path.GetDirectoryName(text));

        Resource resource = new Resource();
        using Stream input = GetInputStream(vpkDirFileName, node);
        resource.Read(input);

        Texture texture = resource.DataBlock as Texture
            ?? throw new InvalidOperationException("DataBlock no es Texture");

        using Bitmap bitmap = SKBitmapToBitmap(texture.GenerateBitmap());

        if (bitmap.Size.Width == 256 && bitmap.Size.Height == 256)
        {
            using Bitmap bitmap2 = new Bitmap(256, 170);
            using (Graphics graphics = Graphics.FromImage(bitmap2))
            {
                graphics.DrawImage(
                    bitmap,
                    new Rectangle(0, 0, bitmap2.Width, bitmap2.Height),
                    new Rectangle(0, 0, bitmap2.Width, bitmap2.Height),
                    GraphicsUnit.Pixel);
                graphics.Save();
            }
            bitmap2.Save(text, ImageFormat.Png);
            return new Bitmap(bitmap2); // devolvemos copia porque bitmap2 se dispone
        }

        bitmap.Save(text, ImageFormat.Png);
        return new Bitmap(bitmap); // devolvemos copia porque bitmap se dispone
    }

    private static void DoExtractFile(string vpkDirFileName, VpkNode node)
	{
		if ((long)node.ArchiveIndex == short.MaxValue)
		{
			modCommon.Show("Found entry: " + node.FilePath);
		}
		else
		{
			modCommon.Show("Found entry: " + node.FilePath + " in VPK " + node.ArchiveIndex);
		}
		ExtractFile(vpkDirFileName, node);
	}

	private static void ExtractFile(string vpkDirFileName, VpkNode node)
	{
		using Stream stream = GetInputStream(vpkDirFileName, node);
		string[] array = node.FilePath.Split('/');
		IEnumerable<string> source = array.Take(array.Count() - 1);
		string text = array.Last();
		EnsureDirectoryExists(Path.Combine(source.ToArray()));
		using FileStream fileStream = File.OpenWrite(Path.Combine(array));
		byte[] array2 = new byte[1024];
		int num = (int)node.EntryLength;
		int num2;
		while ((num2 = stream.Read(array2, 0, array2.Length)) > 0 && num > 0)
		{
			fileStream.Write(array2, 0, Math.Min(num, num2));
			num -= num2;
		}
	}

	private static void EnsureDirectoryExists(string directory)
	{
		if (!Directory.Exists(directory))
		{
			Directory.CreateDirectory(directory);
		}
	}

	private static Stream GetInputStream(string vpkDirFileName, VpkNode node)
	{
		if (node.EntryLength == 0 && node.PreloadBytes > 0)
		{
			return new MemoryStream(node.PreloadData);
		}
		if (node.PreloadBytes != 0)
		{
			throw new NotSupportedException("Unable to get entry data: Both EntryLength and PreloadBytes specified.");
		}
		string text = new string(Enumerable.Repeat('0', 3 - node.ArchiveIndex.ToString().Length).ToArray());
		string path = vpkDirFileName.Replace("_dir.vpk", "_" + text + node.ArchiveIndex + ".vpk");
		FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
		fileStream.Seek(node.EntryOffset, SeekOrigin.Begin);
		return fileStream;
	}

	public static void Vpk_ExtractFile(string Vpk_Location, string FileInVPK, string Destination_FileName)
	{
		Package package = new Package();
		package.Read(Vpk_Location);
		package.ReadEntry(package.FindEntry(FileInVPK), out var output);
		FileStream fileStream = new FileStream(Destination_FileName, FileMode.OpenOrCreate);
		fileStream.Write(output, 0, output.Length);
		fileStream.Close();
		package.Dispose();
		GC.Collect();
	}

	public static void ExtractScripts()
	{
		try
		{
			string text = modCommon.DataDirectory + "/db";
			Paths.EnsureDirectory(text);
			Vpk_ExtractFile(modCommon.VPKLocation, "scripts/items/items_game.txt", text + "/items_game.txt");
			Vpk_ExtractFile(modCommon.VPKLocation, "scripts/npc/portraits.txt", text + "/portraits.txt");
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			ProjectData.ClearProjectError();
		}
	}

	public static void ExtractThisLoadingScreen(string filePath)
	{
		try
		{
			filePath = "panorama/images/" + filePath + "_png.vtex_c";
			string vPKLocation = modCommon.VPKLocation;
			using VpkFile vpkFile = new VpkFile(vPKLocation);
			vpkFile.Open();
			VpkNode file = vpkFile.GetFile(filePath);
			if (file != null)
			{
				return;
			}
			VpkNode[] allFilesInDirectoryAndSubdirectories = vpkFile.GetAllFilesInDirectoryAndSubdirectories(filePath);
			if (allFilesInDirectoryAndSubdirectories.Count() == 0)
			{
				modCommon.Show("Load screen not found: " + filePath);
				return;
			}
			VpkNode[] array = allFilesInDirectoryAndSubdirectories;
			VpkNode[] array2 = array;
			foreach (VpkNode node in array2)
			{
				ExtractLoadScreen(vPKLocation, node);
			}
		}
		catch (Exception)
		{
		}
	}

    private static void ExtractLoadScreen(string vpkDirFileName, VpkNode node)
    {
        string text = Path.Combine("data", node.FilePath.Replace("_png.vtex_c", ".png"));
        EnsureDirectoryExists(Path.GetDirectoryName(text));

        Resource resource = new Resource();
        using Stream input = GetInputStream(vpkDirFileName, node);
        resource.Read(input);

        Texture texture = resource.DataBlock as Texture
            ?? throw new InvalidOperationException("DataBlock no es Texture");

        using Bitmap image = SKBitmapToBitmap(texture.GenerateBitmap());
        using Bitmap bitmap = new Bitmap(1360, 768);

        using (Graphics graphics = Graphics.FromImage(bitmap))
        {
            graphics.DrawImage(
                image,
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                GraphicsUnit.Pixel);
            graphics.Save();
        }

        modCommon.Show(text);
        bitmap.Save(text, ImageFormat.Png);
    }
}
