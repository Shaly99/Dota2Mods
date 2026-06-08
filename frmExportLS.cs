using SkiaSharp;
using SKYNET;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using ValveResourceFormat;
using ValveResourceFormat.ResourceTypes;

public class frmExportLS : Form
{
    private bool mouseDown;

    // Variables cacheadas antes del thread (deben capturarse en el UI thread)
    private int _screenWidth;
    private int _screenHeight;

    private Point lastLocation;

    private Items item;

    private Thread ExportThread;

    private string lastscreen = "";

    private IContainer components = null;

    private Button acceptBtn;

    private Button ok;

    private Panel panel1;

    private FlatButton cancelBtn;

    private PictureBox LoadScreen;

    private Panel panel3;

    private Label txtMessage;

    private Panel panel2;
    private static IVpkService Vpk => ServiceContainer.Get<IVpkService>();
    public frmExportLS()
    {
        InitializeComponent();
        ExportThread = new Thread(ExportWorker_DoWork)
        {
            IsBackground = true
        };
    }

    public frmExportLS(Items item)
    {
        InitializeComponent();
        this.item = item;
    }

    public void MostrarLabel(string v)
    {
        this.InvokeIfRequired(() =>
        {
            if (LoadScreen == null || LoadScreen.IsDisposed) return;
            if (txtMessage == null || txtMessage.IsDisposed) return;

            if (File.Exists(lastscreen))
            {
                // Usar el cache de imágenes en lugar de Image.FromFile (que bloquea el archivo)
                LoadScreen.Image = ImageCache.GetOrLoad(lastscreen);
            }
            txtMessage.Text = v;
        });
    }

    private void Event_MouseMove(object sender, MouseEventArgs e)
    {
        if (mouseDown)
        {
            base.Location = new Point(base.Location.X - lastLocation.X + e.X, base.Location.Y - lastLocation.Y + e.Y);
            Update();
            base.Opacity = 0.93;
        }
    }

    private void Event_MouseDown(object sender, MouseEventArgs e)
    {
        mouseDown = true;
        lastLocation = e.Location;
    }

    private void Event_MouseUp(object sender, MouseEventArgs e)
    {
        mouseDown = false;
        base.Opacity = 100.0;
    }

    private void cancelBtn_Click(object sender, EventArgs e)
    {
        ExportThread.Interrupt();
        Close();
    }

    private void ExportWorker_DoWork()
    {
        // 🔑 Usar las dimensiones cacheadas (no acceder a Handle desde background thread)
        int width = _screenWidth;
        int height = _screenHeight;

        string text2 = modCommon.DataDirectory + "/Loading Screen";
        Paths.EnsureDirectory(text2);
        string DirectoryName = "panorama/images/loadingscreen";

        if (!Vpk.IsLoaded)
        {
            MostrarLabel("VPK no está cargado");
            return;
        }

        try
        {
            List<PackageEntry> list = Vpk.GetEntriesByExtension("vtex_c")
                .Where((PackageEntry l) => l.DirectoryName != null && l.DirectoryName.Contains(DirectoryName))
                .ToList();

            if (!list.Any())
            {
                MostrarLabel("No se encontraron loading screens");
                return;
            }

            // ════════════════════════════════════════
            // 🚀 PARALELIZACIÓN
            // ════════════════════════════════════════
            int totalCount = list.Count;
            int processedCount = 0;
            int lastReportedPercent = -1;
            object reportLock = new object();

            MostrarLabel($"Exportando {totalCount} loading screens...");

            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount
            };

            Parallel.ForEach(list, parallelOptions, (item) =>
            {
                try
                {
                    string fileName = item.GetFullPath().Replace("panorama/images/", "");
                    fileName = ItemsManager.NameFromAsset(fileName);
                    string fullPath = Path.Combine(text2, fileName + ".png").Replace(" .png", ".png");

                    // Si ya existe, saltamos
                    if (File.Exists(fullPath))
                    {
                        UpdateProgress(ref processedCount, totalCount, ref lastReportedPercent, reportLock);
                        return;
                    }

                    // Extraer y guardar
                    using Bitmap texture = GetTexture(
                        Vpk.GetUnderlyingPackage(),
                        item,
                        new Rectangle(0, 0, width, height));

                    texture.Save(fullPath, ImageFormat.Png);

                    // Guardar el último extraído para mostrar preview
                    lastscreen = fullPath.Replace("\\", "/").Replace("\\\\", "/");

                    UpdateProgress(ref processedCount, totalCount, ref lastReportedPercent, reportLock);
                }
                catch
                {
                    // Ignorar errores individuales
                    UpdateProgress(ref processedCount, totalCount, ref lastReportedPercent, reportLock);
                }
            });

            MostrarLabel("Export finalizado ✅");

            // Abrir el explorador con la carpeta
            try
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = text2,
                    WindowStyle = ProcessWindowStyle.Normal
                };
                Process.Start(processStartInfo);
            }
            catch { }
        }
        catch (Exception ex)
        {
            modCommon.Save("Error exportando loading screens", ex);
        }
    }

    /// <summary>
    /// Actualiza el progreso de forma thread-safe con throttling
    /// (solo actualiza la UI cuando cambia el porcentaje entero).
    /// </summary>
    private void UpdateProgress(ref int processedCount, int total, ref int lastReportedPercent, object reportLock)
    {
        int newCount = Interlocked.Increment(ref processedCount);
        int percent = (int)((double)newCount / total * 100);

        // Throttling: solo actualizar UI cuando cambia el porcentaje
        lock (reportLock)
        {
            if (percent != lastReportedPercent)
            {
                lastReportedPercent = percent;
                MostrarLabel($"Exportando: {newCount}/{total} ({percent}%)");
            }
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
        byte[] output = new byte[256];
        apk.ReadEntry(entry, out output);
        Resource resource = new Resource();
        using MemoryStream input = new MemoryStream(output);
        resource.Read(input);
        if (resource.ResourceType != ResourceType.Texture)
        {
            throw new NotSupportedException();
        }
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

    private void FrmExportLS_Shown(object sender, EventArgs e)
    {
        // 🔑 Capturar dimensiones en el UI thread ANTES de lanzar el background thread
        Rectangle workingArea = Screen.FromHandle(base.Handle).WorkingArea;
        _screenWidth = workingArea.Width;
        _screenHeight = workingArea.Height;

        ExportThread.Start();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmExportLS));
        this.acceptBtn = new System.Windows.Forms.Button();
        this.ok = new System.Windows.Forms.Button();
        this.panel1 = new System.Windows.Forms.Panel();
        this.panel2 = new System.Windows.Forms.Panel();
        this.txtMessage = new System.Windows.Forms.Label();
        this.panel3 = new System.Windows.Forms.Panel();
        this.LoadScreen = new System.Windows.Forms.PictureBox();
        this.cancelBtn = new FlatButton();
        this.panel1.SuspendLayout();
        this.panel3.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)this.LoadScreen).BeginInit();
        base.SuspendLayout();
        this.acceptBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
        this.acceptBtn.Location = new System.Drawing.Point(485, 375);
        this.acceptBtn.Name = "acceptBtn";
        this.acceptBtn.Size = new System.Drawing.Size(75, 23);
        this.acceptBtn.TabIndex = 16;
        this.acceptBtn.Text = "button1";
        this.acceptBtn.UseVisualStyleBackColor = true;
        this.ok.DialogResult = System.Windows.Forms.DialogResult.OK;
        this.ok.Location = new System.Drawing.Point(483, 145);
        this.ok.Name = "ok";
        this.ok.Size = new System.Drawing.Size(18, 23);
        this.ok.TabIndex = 24;
        this.ok.Text = "ok";
        this.ok.UseVisualStyleBackColor = true;
        this.panel1.BackColor = System.Drawing.Color.FromArgb(28, 29, 32);
        this.panel1.Controls.Add(this.panel2);
        this.panel1.Controls.Add(this.txtMessage);
        this.panel1.Controls.Add(this.panel3);
        this.panel1.Controls.Add(this.cancelBtn);
        this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.panel1.Location = new System.Drawing.Point(1, 1);
        this.panel1.Name = "panel1";
        this.panel1.Size = new System.Drawing.Size(400, 308);
        this.panel1.TabIndex = 25;
        this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(Event_MouseDown);
        this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(Event_MouseMove);
        this.panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(Event_MouseUp);
        this.panel2.Location = new System.Drawing.Point(380, 246);
        this.panel2.Name = "panel2";
        this.panel2.Size = new System.Drawing.Size(19, 20);
        this.panel2.TabIndex = 30;
        this.txtMessage.AutoSize = true;
        this.txtMessage.ForeColor = System.Drawing.Color.Gray;
        this.txtMessage.Location = new System.Drawing.Point(6, 245);
        this.txtMessage.Name = "txtMessage";
        this.txtMessage.Size = new System.Drawing.Size(0, 16);
        this.txtMessage.TabIndex = 29;
        this.panel3.Controls.Add(this.LoadScreen);
        this.panel3.Location = new System.Drawing.Point(7, 8);
        this.panel3.Name = "panel3";
        this.panel3.Padding = new System.Windows.Forms.Padding(2);
        this.panel3.Size = new System.Drawing.Size(386, 234);
        this.panel3.TabIndex = 28;
        this.LoadScreen.Dock = System.Windows.Forms.DockStyle.Fill;
        this.LoadScreen.Location = new System.Drawing.Point(2, 2);
        this.LoadScreen.Name = "LoadScreen";
        this.LoadScreen.Size = new System.Drawing.Size(382, 230);
        this.LoadScreen.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
        this.LoadScreen.TabIndex = 27;
        this.LoadScreen.TabStop = false;
        this.cancelBtn.BackColor = System.Drawing.Color.FromArgb(43, 47, 48);
        this.cancelBtn.BackColorMouseOver = System.Drawing.Color.FromArgb(57, 62, 63);
        this.cancelBtn.Cursor = System.Windows.Forms.Cursors.Hand;
        this.cancelBtn.Font = FontService.GetRadiance(10f);
        this.cancelBtn.ForeColor = System.Drawing.Color.FromArgb(147, 157, 160);
        this.cancelBtn.ForeColorMouseOver = System.Drawing.Color.Empty;
        this.cancelBtn.ImageAlignment = FlatButton._ImgAlign.Left;
        this.cancelBtn.ImageIcon = null;
        this.cancelBtn.Location = new System.Drawing.Point(148, 276);
        this.cancelBtn.Name = "cancelBtn";
        this.cancelBtn.Rounded = false;
        this.cancelBtn.Size = new System.Drawing.Size(93, 29);
        this.cancelBtn.Style = FlatButton._Style.TextOnly;
        this.cancelBtn.TabIndex = 26;
        this.cancelBtn.Text = "Cancelar";
        this.cancelBtn.Click += new System.EventHandler(cancelBtn_Click);
        base.AutoScaleDimensions = new System.Drawing.SizeF(7f, 16f);
        base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
        this.BackColor = System.Drawing.Color.FromArgb(7, 164, 245);
        base.ClientSize = new System.Drawing.Size(402, 310);
        base.Controls.Add(this.panel1);
        base.Controls.Add(this.ok);
        base.Controls.Add(this.acceptBtn);
        this.Font = new System.Drawing.Font("Segoe UI Emoji", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
        base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        base.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
        base.Name = "frmExportLS";
        base.Padding = new System.Windows.Forms.Padding(1);
        base.ShowIcon = false;
        base.ShowInTaskbar = false;
        base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "Message";
        base.Shown += new System.EventHandler(FrmExportLS_Shown);
        this.panel1.ResumeLayout(false);
        this.panel1.PerformLayout();
        this.panel3.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)this.LoadScreen).EndInit();
        base.ResumeLayout(false);
    }
}
