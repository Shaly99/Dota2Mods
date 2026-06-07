using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO.Compression;

namespace SKYNET;

public class frmMain : Form
{
    // P/Invoke para suspender repintado y eliminar parpadeo
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern int SendMessage(IntPtr hWnd, int wMsg, bool wParam, int lParam);
    private const int WM_SETREDRAW = 0x000B;

    public static frmMain frm;

    private bool FirstLaunch = false;

    private bool mouseDown;

    private Point lastLocation;

    private Media media;

    private Keyboard hook;

    private string VideoFolder;

    public static modManager manager;

    // ════════════════════════════════════════
    // NUEVO: Catálogo centralizado de items
    // (reemplazará progresivamente a las 24 listas de abajo)
    // ════════════════════════════════════════
    public ItemCatalog Catalog { get; } = new ItemCatalog();

    public List<ItemDescription> itemDescription = new List<ItemDescription>();

    public List<ItemDescription> rich_presence = new List<ItemDescription>();

    public List<prefabs> prefabs = new List<prefabs>();

    // 🎬 Overlay flotante para hover animado de héroes
    private HeroHoverOverlay _heroHoverOverlay;
    private HeroPortrait _currentHoveredPortrait;
    private System.Windows.Forms.Timer _hoverPollTimer;

    private bool tabPage3Loaded;

    private bool tabPage4Loaded;

    private List<Control> Menus = new List<Control>();

    private IContainer components = null;

    private Panel panel1;

    private PictureBox banner;

    private Label labelHeroes;

    private Label labelMisc;

    private Label labelWord;

    private TabPage tabPage1;

    private TabPage tabPage2;

    private PictureBox CloseBtn;

    private PictureBox HeroAvatar;

    private Label HeroName;

    private TabPage tabPage3;

    private TabPage tabPage4;

    private Label label1;

    private Label label2;

    public BundleBox CurrentMusicPack;

    public BundleBox CurrentAnnouncer;

    private Panel WordContainer;

    private Label label3;

    private Label label4;

    private Label label5;

    private Label label6;

    private Label label8;

    private Label label7;

    private Label label9;

    private Label label10;

    private Label label11;

    private Label label12;

    private Label label13;

    private Label label14;

    private Label label15;

    private Label label16;

    private PictureBox OpenSettings;

    private Label label17;

    private SkynListView LoadingScreenView;

    private Panel LoadingScreenContainer;

    private PictureBox loadScreen;

    private ColumnHeader columnHeader1;

    private ColumnHeader columnHeader2;

    private ColumnHeader columnHeader3;

    private ColumnHeader columnHeader4;

    private Label label18;

    private Label labelCreateMod;

    private TabPage tabPage5;

    public TabPage tabPage6;

    private PictureBox logo;

    private Label load;

    public BundleBox CurrentRadiantTowers;

    public BundleBox CurrentTerrain;

    public BundleBox CurrentWeatherEffect;

    public BundleBox CurrentDireTowers;

    public BundleBox CurrentDireCreeps;

    public BundleBox CurrentRadiantCreeps;

    public BundleBox CurrentVersusScreen;

    public BundleBox CurrentEmoticons;

    public BundleBox CurrentEmblem;

    public BundleBox CurrentMultikillBanner;

    public BundleBox CurrentCursorPack;

    public BundleBox CurrentWard;

    public BundleBox CurrentCourier;

    public BundleBox CurrentHUDSkin;

    public FlatTabControl tabControl1;

    private PictureBox picRight;

    private PictureBox picLeft;

    private Label labelLeft;

    private Label labelRight;

    private Panel OpenDota;

    private MetroPanel BundleControl;

    public SlotsPanel SlotContainer;

    public BoxInfo boxInfo;

    private Label label19;

    private Panel panel2;

    private Panel panel3;

    private FlatButton importBtn;

    private FlatButton flatButton1;

    private FlatButton ExportAll;

    private FlatButton ExportThis;

    private FlatButton SetDesktop;

    private Panel panel4;

    private Label label20;

    private Label label22;

    private Label label21;

    private Panel panelHeroes;

    private Panel panelCreateMod;

    private Panel panelMisc;

    private Panel panelWord;

    private Label label23;

    private Label label24;

    private TextBox keyPress;

    private Label keyPressed;

    private System.Windows.Forms.Timer keyTimer;

    public FlatButton stopSounds;

    public BundleBox CurrentPets;

    public BundleBox CurrentMegaKill;

    public BundleBox CurrentLoadingScreen;

    private TextBox KeyBind;

    private FlatContextMenuStrip ImageMenu;

    private ToolStripMenuItem OpenImageMenu;

    private ToolStripMenuItem SaveImageMenu;

    private SkynListView ExternalModView;

    private ColumnHeader columnHeader5;

    public Label createdBy;

    public Label ClientVersion;

    public BundleBox CurrentStreakEffect;

    private Label label25;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public List<Colors> Colors { get; set; } = new List<Colors>();
    public bool LoadVideo { get; }

    public bool Updating { get; private set; }

    public bool Started { get; private set; }

    public static TabPage CurrentTab { get; set; }

    public bool Ready { get; private set; }

    public frmMain()
    {
        InitializeComponent();
        base.AutoScaleMode = AutoScaleMode.Inherit;
        frm = this;

        // ════════════════════════════════════════════════════════════
        // Tamaño dinámico al 80% del escritorio (centrado)
        // ════════════════════════════════════════════════════════════
        var screen = Screen.PrimaryScreen.WorkingArea;
        int newWidth = (int)(screen.Width * 0.80);
        int newHeight = (int)(screen.Height * 0.85);
        base.Size = new Size(newWidth, newHeight);
        base.StartPosition = FormStartPosition.CenterScreen;

        // 🔑 Registrar servicios en el contenedor con manejo de errores
        try
        {
            RegisterServices();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Error crítico al inicializar servicios:\n\n{ex.Message}\n\n{ex.StackTrace}",
                "Error de inicialización",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            Environment.Exit(1);
        }

        // Hacer que tabControl ocupe todo el espacio disponible debajo del banner
        // Extendemos un poco a la derecha para compensar el recorte de los tabs
        tabControl1.Dock = DockStyle.None;
        tabControl1.Location = new Point(0, 53);
        tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom
                            | AnchorStyles.Left | AnchorStyles.Right;
        tabControl1.Size = new Size(
            this.ClientSize.Width + tabControl1.ItemSize.Width + 4,
            this.ClientSize.Height - 53 - 25);

        // Esconder los tabs visuales recortando solo lo necesario
        tabControl1.SizeChanged += (s, e) =>
        {
            if (tabControl1.Width <= 0 || tabControl1.Height <= 0) return;

            // Recorta el ancho del tab header (Alignment=Right + ItemSize.Width=80)
            int tabHeaderWidth = tabControl1.ItemSize.Width + 4;
            int visibleWidth = tabControl1.Width - tabHeaderWidth;
            int visibleHeight = tabControl1.Height;

            tabControl1.Region = new Region(new Rectangle(
                0, 0, visibleWidth, visibleHeight));
        };

        SetParents();

        CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
        if (currentCulture.ToString().Contains("es"))
        {
            modCommon.CurrentLanguage = modCommon.Language.Spanish;
        }
        else
        {
            modCommon.CurrentLanguage = modCommon.Language.English;
        }
        if (!Directory.Exists(modCommon.DataDirectory))
        {
            FirstLaunch = true;
        }
        if (!File.Exists(modCommon.DataDirectory + "/Settings.ini"))
        {
            FirstLaunch = true;
        }
        manager = new modManager();
        manager.Load();
        SelectTab(tabPage6);
        if (FirstLaunch)
        {
            if (Directory.Exists(modCommon.DetectedDota2Path))
            {
                Settings.Dota2Path = modCommon.DetectedDota2Path;
                modCommon.VPKLocation = Settings.Dota2Path + "/pak01_dir.vpk";
            }
            else
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Title = "Select pak01_dir.vpk in \"dota 2 beta\\game\\dota\"",
                    Filter = "Valve File | pak01_dir.vpk"
                };
                DialogResult dialogResult = openFileDialog.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    Settings.Dota2Path = Path.GetDirectoryName(openFileDialog.FileName);
                    modCommon.VPKLocation = openFileDialog.FileName;
                }
            }
            Settings.GenerateOnStart = true;
            Settings.OpenDota = false;
            Settings.ActiveSounds = true;
            Settings.ClientVersion = modCommon.GetClientVersion();
            Settings.Save();
        }
        else
        {
            Settings.Load();
            modCommon.VPKLocation = Settings.Dota2Path + "/pak01_dir.vpk";
            string text = "0";
            if (File.Exists(Settings.Dota2Path + "\\steam.inf"))
            {
                text = Settings.ClientVersion;
            }
            if (text != "0" && modCommon.GetClientVersion() != text)
            {
                FileManager.DeleteDirectory(Path.Combine(modCommon.DataDirectory, "db"));
                modCommon.NeedUpdate = true;
                Settings.ClientVersion = text;
            }
        }
        if (!File.Exists(modCommon.VPKLocation))
        {
            modCommon.Show("File " + modCommon.VPKLocation + " not found");
            modCommon.Show(Settings.Dota2Path);
            if (Directory.Exists(modCommon.DetectedDota2Path))
            {
                Settings.Dota2Path = modCommon.DetectedDota2Path;
                modCommon.VPKLocation = Settings.Dota2Path + "/pak01_dir.vpk";
            }
            else
            {
                OpenFileDialog openFileDialog2 = new OpenFileDialog
                {
                    Title = "Select pak01_dir.vpk in \"dota 2 beta\\game\\dota\"",
                    Filter = "Valve File | pak01_dir.vpk"
                };
                DialogResult dialogResult2 = openFileDialog2.ShowDialog();
                if (dialogResult2 == DialogResult.OK)
                {
                    Settings.Dota2Path = Path.GetDirectoryName(openFileDialog2.FileName);
                    modCommon.VPKLocation = openFileDialog2.FileName;
                }
            }
            if (!File.Exists(modCommon.VPKLocation))
            {
                hook?.Remove();
                Environment.Exit(0);
            }
        }
        Paths.EnsureDirectory(modCommon.DataDirectory + "/ExternalItems");
        if (Directory.Exists(Settings.Dota2Path))
        {
            VideoFolder = Settings.Dota2Path + "\\panorama\\videos\\heroes";
            if (Directory.Exists(VideoFolder) && Directory.GetFiles(VideoFolder).Length > 1)
            {
                LoadVideo = true;

                // Pre-cachear las rutas de videos para el hover overlay
                ServiceContainer.Get<IHeroVideoService>().Load(VideoFolder);
            }
        }
        GenerateCache();
        media = new Media();
        ClientVersion.Text = "Client version: " + Settings.ClientVersion;
        hook = new Keyboard();
        hook.Install(base.Handle);

        // Agrandar el form al 95% del escritorio
        this.Load += (s, e) =>
        {
            var screen = Screen.FromControl(this).WorkingArea;
            int newWidth = (int)(screen.Width * 0.95);
            int newHeight = (int)(screen.Height * 0.95);
            this.Size = new Size(newWidth, newHeight);
            this.Location = new Point(
                screen.X + (screen.Width - newWidth) / 2,
                screen.Y + (screen.Height - newHeight) / 2);
        };
    }

    internal async void GenerateCache()
    {
        await GenerateResources();
        AddHeroes();
    }

    private Task GenerateResources()
    {
        return Task.Run(delegate
        {
            Updating = true;
            SelectTab(tabPage6);
            modCommon.WriteLine("Starting...", loading: true);

            // ════════════════════════════════════════════════════════
            // Verificar cache antes de extraer
            // ════════════════════════════════════════════════════════
            var cacheValidator = ServiceContainer.Get<IVpkCacheValidator>();
            bool useCache = cacheValidator.IsCacheValid();

            // 🔑 Usar el servicio para cargar el VPK
            var vpkService = ServiceContainer.Get<IVpkService>();
            vpkService.Load(modCommon.VPKLocation);

            try
            {
                VpkExplorer.GenerateReport(modCommon.VPKLocation);
                modCommon.WriteLine("📋 VPK Explorer report generado en el escritorio", loading: true);
            }
            catch (Exception ex)
            {
                modCommon.Save(ex);
            }

            if (!useCache)
            {
                // ════════════════════════════════════════════════════
                // 🔑 NUEVO: Calcular total de items a procesar
                // ════════════════════════════════════════════════════
                string[] rutasParaExtraer = new[]
                {
                "panorama/images/heroes",
                "panorama/images/econ/heroes",
                "panorama/images/econ/items",
                "panorama/images/econ/pets",
                "panorama/images/econ/sets",
                "panorama/images/econ/loading_screen",
                "panorama/images/econ/taunts",
                "panorama/images/econ/courier",
                "panorama/images/econ/music",
                "panorama/images/econ/announcer",
                "panorama/images/econ/terrain",
                "panorama/images/econ/tools",
                "panorama/images/econ/cursor_pack",
                "panorama/images/econ/huds",
                "panorama/images/items"
            };

                int totalItems = 0;
                try
                {
                    var allEntries = vpkService.GetEntriesByExtension("vtex_c");
                    foreach (var ruta in rutasParaExtraer)
                    {
                        totalItems += allEntries.Count(e =>
                            e.DirectoryName != null && e.DirectoryName.Contains(ruta));
                    }
                }
                catch { totalItems = 21539; } // fallback aproximado

                // Iniciar reporte de progreso
                var progress = ServiceContainer.Get<IProgressReporter>();
                progress.Start("⚡ Extrayendo recursos", totalItems);

                // Extraer items database (sin progreso, es rápido)
                modCommon.WriteLine("Extracting Items Database", loading: true);
                RegenItemMatrixCommand regenItemMatrixCommand = new RegenItemMatrixCommand();
                regenItemMatrixCommand.ProcessCommand(vpkService.GetUnderlyingPackage());

                // Extraer todas las rutas (con progreso automático)
                foreach (var ruta in rutasParaExtraer)
                {
                    DotaResources.ExtractResources(ruta, "Items");
                }

                // Completar progreso
                progress.Complete();

                // Guardar firma del cache
                cacheValidator.SaveCacheSignature();
                modCommon.WriteLine("✅ Cache actualizado", loading: true);
            }
            else
            {
                modCommon.WriteLine("⚡ Usando cache existente (extracción omitida)", loading: true);
            }

            // Cargas finales (siempre se hacen, son rápidas)
            if (!Started)
            {
                modCommon.WriteLine("Generating program resources", loading: true);
                DotaResources.LoadResources();

                modCommon.WriteLine("Loading Rich Presence", loading: true);
                DotaResources.LoadRichPresence();

                modCommon.WriteLine("Loading items descriptions", loading: true);
                DotaResources.LoadItemsDescriptions();

                modCommon.WriteLine("Loading hero attributes", loading: true);
                ServiceContainer.Get<IHeroAttributesService>().Load();

                Started = true;
            }
        });
    }

    private void AddHeroes()
    {
        modManager.SetHeroesToList();
        manager.Heroes.Sort((Hero s1, Hero s2) =>
            s1.Name.ToString().CompareTo(s2.Name.ToString()));

        // 🚀 Activar double buffering en tabPage1 para evitar parpadeo
        EnableDoubleBuffering(tabPage1);

        var attrService = ServiceContainer.Get<IHeroAttributesService>();

        // ════════════════════════════════════════════════════════════
        // CONFIGURACIÓN DE LAYOUT
        // STR/AGI/INT: 6 héroes por fila | UNIVERSAL: 4 héroes por fila
        // ════════════════════════════════════════════════════════════
        int[] columnsPerAttributeArray = { 6, 6, 6, 4 };

        const int portraitSpacing = 3;
        const int headerHeight = 25;
        const int columnSpacing = 25;
        const int topMargin = 15;
        const int sideMargin = 15;

        int totalSlots = columnsPerAttributeArray.Sum();

        int availableWidth = this.ClientSize.Width - (sideMargin * 2);
        int totalSpacingBetweenColumns = columnSpacing * 3;
        int totalSpacingBetweenPortraits = portraitSpacing * (totalSlots - 4);
        int availableForPortraits = availableWidth - totalSpacingBetweenColumns - totalSpacingBetweenPortraits;

        int portraitWidth = availableForPortraits / totalSlots;
        int portraitHeight = (int)(portraitWidth * 1.37);

        int[] columnWidths = new int[4];
        for (int i = 0; i < 4; i++)
        {
            columnWidths[i] = (portraitWidth * columnsPerAttributeArray[i])
                + (portraitSpacing * (columnsPerAttributeArray[i] - 1));
        }

        int totalGridWidth = columnWidths.Sum() + (columnSpacing * 3);
        int startX = (this.ClientSize.Width - totalGridWidth) / 2;
        if (startX < sideMargin) startX = sideMargin;

        // ════════════════════════════════════════════════════════════
        // AGRUPAR HÉROES POR ATRIBUTO
        // ════════════════════════════════════════════════════════════
        var byAttribute = new Dictionary<HeroAttribute, List<Hero>>
        {
            { HeroAttribute.Strength, new List<Hero>() },
            { HeroAttribute.Agility, new List<Hero>() },
            { HeroAttribute.Intelligence, new List<Hero>() },
            { HeroAttribute.Universal, new List<Hero>() }
        };

        foreach (var hero in manager.Heroes)
        {
            var attr = attrService.GetAttribute(hero.Name);
            byAttribute[attr].Add(hero);
        }

        var columnOrder = new[]
        {
            (HeroAttribute.Strength, "STRENGTH", Color.FromArgb(220, 70, 70)),
            (HeroAttribute.Agility, "AGILITY", Color.FromArgb(80, 200, 100)),
            (HeroAttribute.Intelligence, "INTELLIGENCE", Color.FromArgb(80, 160, 230)),
            (HeroAttribute.Universal, "UNIVERSAL", Color.FromArgb(180, 130, 220))
        };

        // ════════════════════════════════════════════════════════════
        // CREAR LAS 4 COLUMNAS
        // ════════════════════════════════════════════════════════════
        int currentX = startX;
        for (int col = 0; col < columnOrder.Length; col++)
        {
            var (attribute, title, color) = columnOrder[col];
            var heroes = byAttribute[attribute];
            int heroesPerRow = columnsPerAttributeArray[col];
            int thisColumnWidth = columnWidths[col];

            // Título de la columna
            var headerLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI Emoji", 9F, FontStyle.Bold),
                ForeColor = color,
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(currentX, topMargin),
                Size = new Size(thisColumnWidth, headerHeight)
            };

            modCommon.InvokeAction(tabPage1, delegate
            {
                tabPage1.Controls.Add(headerLabel);
            });

            // Crear los retratos de la columna
            int currentRow = 0;
            int currentColInRow = 0;

            foreach (var hero in heroes)
            {
                try
                {
                    Image image = ItemsManager.GetImage("heroes/selection/" + hero.Name);
                    if (image == null) continue;

                    int x = currentX + (currentColInRow * (portraitWidth + portraitSpacing));
                    int y = topMargin + headerHeight + (currentRow * (portraitHeight + portraitSpacing));

                    var portrait = new HeroPortrait
                    {
                        Name = hero.Name,
                        HeroName = hero.Name,
                        HeroDisplayName = ItemsManager.GetHeroName(hero.Name),
                        HeroImage = image,
                        Size = new Size(portraitWidth, portraitHeight),
                        Location = new Point(x, y)
                    };

                    portrait.HeroClicked += Portrait_Clicked;

                    modCommon.InvokeAction(tabPage1, delegate
                    {
                        tabPage1.Controls.Add(portrait);
                    });

                    currentColInRow++;
                    if (currentColInRow >= heroesPerRow)
                    {
                        currentColInRow = 0;
                        currentRow++;
                    }
                }
                catch (Exception ex)
                {
                    modCommon.Save(ex);
                }
            }

            currentX += thisColumnWidth + columnSpacing;
        }

        // ════════════════════════════════════════════════════════════
        // CREAR EL OVERLAY DE HOVER + TIMER DE POLLING
        // ════════════════════════════════════════════════════════════
        modCommon.InvokeAction(tabPage1, delegate
        {
            _heroHoverOverlay = new HeroHoverOverlay();
            tabPage1.Controls.Add(_heroHoverOverlay);
            _heroHoverOverlay.BringToFront();

            _hoverPollTimer = new System.Windows.Forms.Timer { Interval = 50 };
            _hoverPollTimer.Tick += HoverPollTimer_Tick;
            _hoverPollTimer.Start();
        });

        Updating = false;
        SelectTab(tabPage1);
        Ready = true;
        modCommon.InvokeAction(ClientVersion, delegate
        {
            ClientVersion.Visible = true;
        });
        modCommon.InvokeAction(createdBy, delegate
        {
            createdBy.Visible = true;
        });

        // Re-layout con debounce
        var resizeDebounce = new System.Windows.Forms.Timer { Interval = 100 };
        resizeDebounce.Tick += (s, e) =>
        {
            resizeDebounce.Stop();
            RelayoutHeroGrid();
        };

        this.Resize += (s, e) =>
        {
            resizeDebounce.Stop();
            resizeDebounce.Start();
        };
    }

    /// <summary>
    /// Reposiciona el grid de héroes (header labels y PictureBoxes) según el ancho actual.
    /// </summary>
    /// <summary>
    /// Reposiciona el grid de héroes según el ancho actual.
    /// </summary>
    private void RelayoutHeroGrid()
    {
        if (manager?.Heroes == null || manager.Heroes.Count == 0) return;

        var attrService = ServiceContainer.TryGet<IHeroAttributesService>();
        if (attrService == null) return;

        int[] columnsPerAttributeArray = { 6, 6, 6, 4 };

        const int portraitSpacing = 3;
        const int headerHeight = 25;
        const int columnSpacing = 25;
        const int topMargin = 15;
        const int sideMargin = 15;

        int totalSlots = columnsPerAttributeArray.Sum();

        int availableWidth = this.ClientSize.Width - (sideMargin * 2);
        int totalSpacingBetweenColumns = columnSpacing * 3;
        int totalSpacingBetweenPortraits = portraitSpacing * (totalSlots - 4);
        int availableForPortraits = availableWidth - totalSpacingBetweenColumns - totalSpacingBetweenPortraits;

        int portraitWidth = availableForPortraits / totalSlots;
        int portraitHeight = (int)(portraitWidth * 1.37);

        int[] columnWidths = new int[4];
        for (int i = 0; i < 4; i++)
        {
            columnWidths[i] = (portraitWidth * columnsPerAttributeArray[i])
                + (portraitSpacing * (columnsPerAttributeArray[i] - 1));
        }

        int totalGridWidth = columnWidths.Sum() + (columnSpacing * 3);
        int startX = (this.ClientSize.Width - totalGridWidth) / 2;
        if (startX < sideMargin) startX = sideMargin;

        var byAttribute = new Dictionary<HeroAttribute, List<Hero>>
        {
            { HeroAttribute.Strength, new List<Hero>() },
            { HeroAttribute.Agility, new List<Hero>() },
            { HeroAttribute.Intelligence, new List<Hero>() },
            { HeroAttribute.Universal, new List<Hero>() }
        };

        foreach (var hero in manager.Heroes)
        {
            var attr = attrService.GetAttribute(hero.Name);
            byAttribute[attr].Add(hero);
        }

        var columnOrder = new[]
        {
            (HeroAttribute.Strength, "STRENGTH"),
            (HeroAttribute.Agility, "AGILITY"),
            (HeroAttribute.Intelligence, "INTELLIGENCE"),
            (HeroAttribute.Universal, "UNIVERSAL")
        };

        SendMessage(tabPage1.Handle, WM_SETREDRAW, false, 0);
        tabPage1.SuspendLayout();
        try
        {
            int currentX = startX;
            for (int col = 0; col < columnOrder.Length; col++)
            {
                var (attribute, headerText) = columnOrder[col];
                var heroes = byAttribute[attribute];
                int heroesPerRow = columnsPerAttributeArray[col];
                int thisColumnWidth = columnWidths[col];

                foreach (Control c in tabPage1.Controls)
                {
                    if (c is Label lbl && lbl.Text == headerText)
                    {
                        lbl.Location = new Point(currentX, topMargin);
                        lbl.Size = new Size(thisColumnWidth, headerHeight);
                        break;
                    }
                }

                int currentRow = 0;
                int currentColInRow = 0;

                foreach (var hero in heroes)
                {
                    foreach (Control c in tabPage1.Controls)
                    {
                        if (c is HeroPortrait hp && hp.HeroName == hero.Name)
                        {
                            int x = currentX + (currentColInRow * (portraitWidth + portraitSpacing));
                            int y = topMargin + headerHeight + (currentRow * (portraitHeight + portraitSpacing));
                            hp.Location = new Point(x, y);
                            hp.Size = new Size(portraitWidth, portraitHeight);
                            break;
                        }
                    }

                    currentColInRow++;
                    if (currentColInRow >= heroesPerRow)
                    {
                        currentColInRow = 0;
                        currentRow++;
                    }
                }

                currentX += thisColumnWidth + columnSpacing;
            }
        }
        finally
        {
            tabPage1.ResumeLayout();
            SendMessage(tabPage1.Handle, WM_SETREDRAW, true, 0);
            tabPage1.Invalidate(true);
        }
    }

    /// <summary>
    /// Activa double buffering en un control usando reflection.
    /// Esto elimina el parpadeo al repintar.
    /// </summary>
    private static void EnableDoubleBuffering(Control control)
    {
        if (SystemInformation.TerminalServerSession) return;

        var prop = typeof(Control).GetProperty(
            "DoubleBuffered",
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance);

        prop?.SetValue(control, true, null);
    }

    private void SetParents()
    {
        Menus.Add(labelHeroes);
        Menus.Add(labelWord);
        Menus.Add(labelMisc);
        Menus.Add(labelCreateMod);
        Menus.Add(panelHeroes);
        Menus.Add(panelWord);
        Menus.Add(panelMisc);
        Menus.Add(panelCreateMod);
        foreach (Control menu in Menus)
        {
            if (menu is Panel)
            {
                menu.Parent = banner;
            }
        }
        CloseBtn.Parent = banner;
        OpenSettings.Parent = banner;
        OpenDota.Parent = banner;
    }

    // ════════════════════════════════════════════════════════════
    // HEROPORTRAIT HANDLERS
    // El hover se detecta por polling desde el form (no por eventos del control),
    // así el overlay puede tapar al portrait sin romper la detección.
    // ════════════════════════════════════════════════════════════

    private void Portrait_Clicked(object sender, HeroPortrait portrait)
    {
        ProcessHero(portrait.HeroName);
    }

    private void HoverPollTimer_Tick(object sender, EventArgs e)
    {
        // Solo activo en el tab de héroes
        if (tabControl1.SelectedTab != tabPage1)
        {
            if (_currentHoveredPortrait != null)
            {
                _currentHoveredPortrait = null;
                _heroHoverOverlay?.Hide();
            }
            return;
        }

        // Obtener posición del cursor en coordenadas de tabPage1
        Point mousePos = tabPage1.PointToClient(Cursor.Position);

        // Buscar qué HeroPortrait está debajo del cursor
        HeroPortrait foundPortrait = null;
        foreach (Control c in tabPage1.Controls)
        {
            if (c is HeroPortrait hp && hp.Bounds.Contains(mousePos))
            {
                foundPortrait = hp;
                break;
            }
        }

        // Si cambió el portrait bajo el cursor
        if (foundPortrait != _currentHoveredPortrait)
        {
            _currentHoveredPortrait = foundPortrait;

            if (foundPortrait != null)
            {
                // Mostrar overlay sobre el nuevo portrait
                var videoService = ServiceContainer.TryGet<IHeroVideoService>();
                string videoPath = videoService?.GetVideoPath(foundPortrait.HeroName);

                var rect = new Rectangle(foundPortrait.Location, foundPortrait.Size);
                _heroHoverOverlay.ShowOver(
                    rect,
                    foundPortrait.HeroImage,
                    foundPortrait.HeroDisplayName,
                    videoPath);
            }
            else
            {
                // El cursor salió de todos los portraits
                _heroHoverOverlay?.Hide();
            }
        }
    }

    // ════════════════════════════════════════════════════════════
    // HANDLERS LEGACY (mantenemos por compatibilidad con tabPage1)
    // ════════════════════════════════════════════════════════════

    private void Hero_MouseLeave(object sender, EventArgs e) { }
    private void Hero_MouseMove(object sender, MouseEventArgs e) { }
    private void Hero_Click(object sender, EventArgs e) { }

    public void ProcessHero(string Name)
    {
        CleanTabItems();
        modCommon.WriteLine("Cargando", loading: true);
        SetLeftRightHero(Name);
        string videoFileName = modCommon.GetVideoFileName(Name);
        if (File.Exists(VideoFolder + "\\" + videoFileName))
        {
            media.Open(VideoFolder + "\\" + videoFileName, HeroAvatar);
            media.Repeat = true;
            media.Play();
        }
        else
        {
            HeroAvatar.Image = modCommon.GetHeroImage(Name);
        }
        // Obtener items del héroe desde el catálogo
        var heroItems = Catalog.GetByCategoryFast(ItemCategory.Items)
            .Where(i => i.used_by_heroes == Name)
            .OrderByDescending(i => i.item_slot)
            .ToList();

        List<string> addedSlots = new List<string>();

        // Agregar slots normales del héroe
        foreach (Items item in heroItems)
        {
            if (!addedSlots.Contains(item.item_slot))
            {
                Items savedItem = ItemsManager.GetSavedItem(item.used_by_heroes, item.item_slot);
                if (savedItem == null)
                {
                    savedItem = new Items
                    {
                        ItemID = "0000",
                        used_by_heroes = Name,
                        item_slot = item.item_slot
                    };
                }

                BoxItems slotBox = new BoxItems
                {
                    Name = item.item_slot,
                    Item = savedItem,
                    Type = item.item_slot.ToUpper(),
                    Hero = Name,
                    Size = new Size(108, 97),
                    MaximumSize = new Size(108, 97),
                    MinimumSize = new Size(108, 97)
                };

                SlotContainer.AddSlot(slotBox);
                addedSlots.Add(item.item_slot);
            }
        }

        // Agregar slot de taunt si el héroe tiene
        List<Items> tauntList = Catalog.GetByCategoryFast(ItemCategory.Taunts)
            .FindAll(t => t.used_by_heroes == Name);
        if (tauntList.Any())
        {
            Items savedTaunt = ItemsManager.GetSavedItem(Name, "taunt");
            if (savedTaunt == null)
            {
                savedTaunt = new Items
                {
                    ItemID = "0000",
                    used_by_heroes = Name,
                    prefab = "taunt",
                    item_slot = "taunt"
                };
            }

            BoxItems tauntBox = new BoxItems
            {
                Name = "TAUNT",
                Item = savedTaunt,
                Type = "taunt",
                Hero = Name,
                Size = new Size(108, 97),
                MaximumSize = new Size(108, 97),
                MinimumSize = new Size(108, 97)
            };

            SlotContainer.AddSlot(tauntBox);
        }
        BundleControl = new MetroPanel();
        BundleControl.AutoScroll = true;
        BundleControl.HorizontalScrollbar = true;
        BundleControl.HorizontalScrollbarBarColor = true;
        BundleControl.HorizontalScrollbarHighlightOnWheel = false;
        BundleControl.HorizontalScrollbarSize = 10;
        BundleControl.Location = new Point(12, 321);
        BundleControl.Name = "BundleControl";
        BundleControl.Size = new Size(1112, 154);
        BundleControl.TabIndex = 59;
        BundleControl.UseSelectable = false;
        BundleControl.VerticalScrollbar = true;
        BundleControl.VerticalScrollbarBarColor = true;
        BundleControl.VerticalScrollbarHighlightOnWheel = false;
        BundleControl.VerticalScrollbarSize = 10;
        int num3 = 5;
        int num4 = 7;
        var heroBundles = Catalog.GetByCategoryFast(ItemCategory.Bundles)
            .Where(b => b.used_by_heroes == Name)
            .OrderByDescending(b => b.ItemID)
            .ToList();

        foreach (Items bundle in heroBundles)
        {
            BundleControl.Controls.Add(new BundleBox
            {
                item = bundle,
                ShowToolTip = true,
                Location = new Point(num3, num4)
            });
            num3 += 135;

        }
        if (!tabPage2.Controls.Contains(BundleControl))
        {
            tabPage2.Controls.Add(BundleControl);
        }
        HeroName.Text = ItemsManager.GetHeroName(Name);
        AudioPlayer.PlayHeroName(Name);
        SelectTab(tabPage2);
    }

    private void SetLeftRightHero(string name)
    {
        for (int i = 0; i < manager.Heroes.Count; i++)
        {
            if (manager.Heroes[i].Name == name)
            {
                if (i == 0)
                {
                    labelLeft.Text = ItemsManager.GetHeroName(manager.Heroes[manager.Heroes.Count - 1].Name);
                    labelRight.Text = ItemsManager.GetHeroName(manager.Heroes[i + 1].Name);
                }
                else if (i == manager.Heroes.Count - 1)
                {
                    labelLeft.Text = ItemsManager.GetHeroName(manager.Heroes[i - 1].Name);
                    labelRight.Text = ItemsManager.GetHeroName(manager.Heroes[0].Name);
                }
                else
                {
                    labelLeft.Text = ItemsManager.GetHeroName(manager.Heroes[i - 1].Name);
                    labelRight.Text = ItemsManager.GetHeroName(manager.Heroes[i + 1].Name);
                }
            }
        }
    }

    /// <summary>
    /// Procesa el click sobre un BundleBox.
    /// Delega a un handler específico según el prefab del item.
    /// Si no hay handler registrado, no hace nada (logueado para debug).
    /// </summary>
    public void Bundle_Click(BundleBox box)
    {
        if (box?.item == null)
        {
            return;
        }

        var registry = ServiceContainer.TryGet<BundleHandlerRegistry>();
        if (registry == null)
        {
            modCommon.Save(new InvalidOperationException(
                "BundleHandlerRegistry no está registrado en el ServiceContainer"));
            return;
        }

        var handler = registry.GetHandler(box);
        if (handler == null)
        {
            // No hay handler para este prefab - puede ser un tipo nuevo de item
            modCommon.Save(new NotSupportedException(
                $"No hay handler para prefab='{box.item.prefab}' " +
                $"item_slot='{box.item.item_slot}' " +
                $"name='{box.item.name}'"));
            return;
        }

        handler.Handle(box);
    }

    private void CleanTabItems()
    {
        SlotContainer.Clear();
        if (BundleControl != null)
        {
            tabPage2.Controls.Remove(BundleControl);
            BundleControl.Dispose();
            BundleControl = null;
        }
    }

    private void FrmMain_MouseMove(object sender, MouseEventArgs e)
    {
        for (int i = 0; i < tabPage1.Controls.Count; i++)
        {
            if (tabPage1.Controls[i] is PictureBox && tabPage1.Controls[i].Name.Contains("npc_dota_hero"))
            {
                PictureBox pictureBox = (PictureBox)tabPage1.Controls[i];
                pictureBox.BorderStyle = BorderStyle.None;
            }
        }
    }

    public void MostrarLabel(string v)
    {
        this.InvokeIfRequired(() =>
        {
            if (load == null || load.IsDisposed) return;

            load.Visible = true;
            int num = base.Width / 2;
            int num2 = Convert.ToInt32(modCommon.GetTextSize(v, load.Font).Width) / 2;
            int num3 = num - num2;
            int num4 = base.Height - 300;
            load.Location = new Point(num3, num4);
            load.Text = v;
        });
    }

    public void MostrarKey(string v)
    {
        this.InvokeIfRequired(() =>
        {
            if (keyPressed == null || keyPressed.IsDisposed) return;

            int num = base.Width / 2;
            int num2 = Convert.ToInt32(modCommon.GetTextSize(v, keyPressed.Font).Width) / 2;
            int num3 = num - num2;
            int num4 = base.Height - 133;
            keyPressed.Location = new Point(num3, num4);
            keyPressed.Text = v;
        });
    }

    private void Menu_Click(object sender, EventArgs e)
    {
        if (Updating)
        {
            return;
        }
        Label label = new Label();
        if (sender is Label)
        {
            label = (Label)sender;
        }
        else if (sender is Panel)
        {
            foreach (object control in ((Panel)sender).Controls)
            {
                if (control is Label)
                {
                    label = (Label)control;
                }
            }
        }
        switch (label.Text)
        {
            case "HEROES":
                CleanTabItems();
                foreach (object control2 in tabPage1.Controls)
                {
                    if (control2 is PictureBox)
                    {
                        ((PictureBox)control2).Image = modCommon.GetHeroImage(((PictureBox)control2).Name);
                        ((PictureBox)control2).Tag = "1";
                    }
                }
                SelectTab(tabPage1);
                break;
            case "MISC":
                if (tabPage3Loaded)
                {
                    SelectTab(tabPage3);
                }
                else
                {
                    ProcessMISCTab();
                }
                break;
            case "WORLD":
                if (tabPage4Loaded)
                {
                    SelectTab(tabPage4);
                }
                else
                {
                    ProcessWORDTab();
                }
                break;
            case "CREATE MOD":
                manager.CreateMod();
                break;
        }
    }

    private void ProcessWORDTab()
    {
        AssignBundleItem(CurrentTerrain, manager.Terrain, ItemCategory.Terrain,
            new Items { prefab = "terrain" });

        AssignBundleItem(CurrentWeatherEffect, manager.WeatherEffect, ItemCategory.WeatherEffect,
            new Items { prefab = "misc", item_slot = "weather" });

        AssignBundleItem(CurrentRadiantTowers, manager.RadiantTowers, ItemCategory.RadiantTowers,
            new Items { prefab = "radianttowers" });

        AssignBundleItem(CurrentDireTowers, manager.DireTowers, ItemCategory.DireTowers,
            new Items { prefab = "diretowers" });

        AssignBundleItem(CurrentRadiantCreeps, manager.RadiantCreeps, ItemCategory.RadiantCreeps,
            new Items { prefab = "radiantcreeps" });

        AssignBundleItem(CurrentDireCreeps, manager.DireCreeps, ItemCategory.DireCreeps,
            new Items { prefab = "direcreeps" });

        AssignBundleItem(CurrentVersusScreen, manager.VersusScreen, ItemCategory.VersusScreen,
            new Items { prefab = "versus_screen" });

        AssignBundleItem(CurrentCursorPack, manager.CursorPack, ItemCategory.CursorPack,
            new Items { prefab = "cursor_pack" });

        AssignBundleItem(CurrentMultikillBanner, manager.MultikillBanner, ItemCategory.MultikillBanner,
            new Items
            {
                prefab = "misc",
                item_slot = "multikill_banner",
                item_rarity = "",
                name = "Default Multikill-Banner"
            });

        AssignBundleItem(CurrentEmblem, manager.Emblem, ItemCategory.Emblem,
            new Items { prefab = "emblem" });

        AssignBundleItem(CurrentEmoticons, manager.Emoticons, ItemCategory.Emoticons,
            new Items { prefab = "emoticon_tool" });

        AssignBundleItem(CurrentHUDSkin, manager.HUDSkin, ItemCategory.HUDSkin,
            new Items { prefab = "hud_skin" });

        AssignBundleItem(CurrentCourier, manager.Courier, ItemCategory.Courier,
            new Items { prefab = "courier" });

        AssignBundleItem(CurrentWard, manager.Ward, ItemCategory.Ward,
            new Items { prefab = "ward" });

        tabPage4Loaded = true;
        SelectTab(tabPage4);
    }

    private void Effect_Click(object sender, EventArgs e)
    {
    }

    private void ProcessMISCTab()
    {
        DetectExternalMods();

        AssignBundleItem(CurrentMusicPack, manager.MusicPack, ItemCategory.MusicPack,
            new Items { prefab = "music" });

        AssignBundleItem(CurrentAnnouncer, manager.Announcer, ItemCategory.Announcer,
            new Items { prefab = "announcer", item_slot = "announcer" });

        AssignBundleItem(CurrentMegaKill, manager.MegaKillAnnouncer, ItemCategory.MegaKillAnnouncer,
            new Items { prefab = "announcer", item_slot = "mega_kills" });

        // Pets tiene lógica especial: busca en DefaultItems con criterio diferente
        if (manager.Pets != null)
        {
            CurrentPets.item = manager.Pets;
        }
        else
        {
            CurrentPets.item = Catalog.GetByCategoryFast(ItemCategory.DefaultItems)
                .Find(x => x.item_name != null && x.item_name.ToLower().Contains("defaultpet"));

            if (CurrentPets.item == null)
            {
                CurrentPets.item = new Items
                {
                    prefab = "wearable",
                    item_slot = "summon",
                    item_name = "#DOTA_Wearable_All_Heroes_DefaultPet"
                };
            }
        }

        // StreakEffect tiene lógica especial: busca por item_name en vez de name
        if (manager.StreakEffect != null)
        {
            CurrentStreakEffect.item = manager.StreakEffect;
        }
        else
        {
            CurrentStreakEffect.item = Catalog.GetByCategoryFast(ItemCategory.StreakEffect)
                .Find(x => x.item_name != null && x.item_name.ToLower().Contains("no kill"));

            if (CurrentStreakEffect.item == null)
            {
                CurrentStreakEffect.item = new Items
                {
                    name = "No Kill Streak Effect",
                    prefab = "streak_effect",
                    image_inventory = "econ/default_no_item",
                    item_name = "#DOTA_Item_No_Kill_Streak_Effect"
                };
            }
        }

        // LoadingScreen: iterar y agregar a la lista
        // 🚀 OPTIMIZACIÓN: Suspender repintes del ListView
        LoadingScreenView.BeginUpdate();
        try
        {
            // Pre-cachear nombres existentes para evitar búsquedas O(n) en AddItem
            var existingNames = new HashSet<string>(
                LoadingScreenView.Items.Cast<ListViewItem>()
                    .Where(i => i.SubItems.Count > 1)
                    .Select(i => i.SubItems[1].Text),
                StringComparer.OrdinalIgnoreCase);

            foreach (Items item in Catalog.GetByCategoryFast(ItemCategory.LoadingScreen))
            {
                // Buscar el default
                if (item.name != null && item.name.ToLower().Contains("default"))
                {
                    CurrentLoadingScreen.item = item;
                    LoadingScreenContainer.BackColor = modCommon.ColorFromRarity(item.item_rarity);
                    loadScreen.Image = ItemsManager.GetImage(item.image_inventory);
                    loadScreen.Tag = item.name;
                    SelectIndex(item.name);
                }

                // Agregar al ListView solo si no existe
                AddItemFast(item, existingNames);
            }
        }
        finally
        {
            LoadingScreenView.EndUpdate();
        }

        if (manager.LoadingScreen != null)
        {
            CurrentLoadingScreen.item = manager.LoadingScreen;
            LoadingScreenContainer.BackColor = modCommon.ColorFromRarity(manager.LoadingScreen.item_rarity);
            loadScreen.Image = ItemsManager.GetImage(manager.LoadingScreen.image_inventory);
            loadScreen.Tag = manager.LoadingScreen.name;
            SelectIndex(manager.LoadingScreen.name);
        }

        tabPage3Loaded = true;
        SelectTab(tabPage3);
    }

    private void SelectIndex(string name)
    {
    }

    private void AddItem(Items screen)
    {
        ListViewItem listViewItem = LoadingScreenView.FindItemWithText(screen.name);
        if (listViewItem == null)
        {
            ListViewItem listViewItem2 = new ListViewItem();
            ListViewItem.ListViewSubItem item = new ListViewItem.ListViewSubItem();
            ListViewItem.ListViewSubItem item2 = new ListViewItem.ListViewSubItem();
            ListViewItem.ListViewSubItem item3 = new ListViewItem.ListViewSubItem();
            ListViewItem.ListViewSubItem item4 = new ListViewItem.ListViewSubItem();
            ListViewItem.ListViewSubItem item5 = new ListViewItem.ListViewSubItem();
            listViewItem2.SubItems.Add(item);
            listViewItem2.SubItems.Add(item2);
            listViewItem2.SubItems.Add(item3);
            listViewItem2.SubItems.Add(item4);
            listViewItem2.SubItems.Add(item5);
            listViewItem2.SubItems[0].Text = "";
            listViewItem2.SubItems[0].Tag = screen.image_inventory;
            listViewItem2.SubItems[1].Text = screen.name;
            listViewItem2.SubItems[2].Text = screen.ItemID;
            listViewItem2.SubItems[3].Text = screen.item_rarity;
            listViewItem2.SubItems[3].ForeColor = modCommon.ColorFromRarity(screen.item_rarity);
            if (!LoadingScreenView.Items.Contains(listViewItem2))
            {
                LoadingScreenView.Items.Add(listViewItem2);
            }
        }
    }

    /// <summary>
    /// Versión optimizada de AddItem que usa un HashSet para chequeo O(1).
    /// Útil cuando se agregan muchos items en batch.
    /// </summary>
    private void AddItemFast(Items screen, HashSet<string> existingNames)
    {
        if (screen?.name == null) return;
        if (existingNames.Contains(screen.name)) return;

        var listItem = new ListViewItem();
        listItem.SubItems.Add(new ListViewItem.ListViewSubItem());
        listItem.SubItems.Add(new ListViewItem.ListViewSubItem());
        listItem.SubItems.Add(new ListViewItem.ListViewSubItem());
        listItem.SubItems.Add(new ListViewItem.ListViewSubItem());
        listItem.SubItems.Add(new ListViewItem.ListViewSubItem());

        listItem.SubItems[0].Text = "";
        listItem.SubItems[0].Tag = screen.image_inventory;
        listItem.SubItems[1].Text = screen.name;
        listItem.SubItems[2].Text = screen.ItemID;
        listItem.SubItems[3].Text = screen.item_rarity;
        listItem.SubItems[3].ForeColor = modCommon.ColorFromRarity(screen.item_rarity);

        LoadingScreenView.Items.Add(listItem);
        existingNames.Add(screen.name); // Actualizar el cache
    }
    private void LoadingScreenView_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            string path = LoadingScreenView.SelectedItems[0].SubItems[0].Tag.ToString();
            string rarity = LoadingScreenView.SelectedItems[0].SubItems[3].Text;
            LoadingScreenContainer.BackColor = modCommon.ColorFromRarity(rarity);
            loadScreen.Image = ItemsManager.GetImage(path);
            loadScreen.Tag = LoadingScreenView.SelectedItems[0].SubItems[1].Text;
        }
        catch (Exception ex)
        {
            if (ex.Message != "El valor de '0' no es válido para 'index'")
            {
                modCommon.Save(ex);
            }
        }
    }

    private void LoadingScreenView_DoubleClick(object sender, EventArgs e)
    {
        try
        {
            Items item = ItemsManager.GetItem(LoadingScreenView.SelectedItems[0].SubItems[2].Text);
            CurrentLoadingScreen.item = item;
            manager.LoadingScreen = item;
        }
        catch (Exception ex)
        {
            modCommon.Save(ex);
        }
    }

    private void Event_MouseMove(object sender, MouseEventArgs e)
    {
        if (mouseDown)
        {
            base.Location = new Point(base.Location.X - lastLocation.X + e.X, base.Location.Y - lastLocation.Y + e.Y);
            Update();
            base.Opacity = 0.93;
        }
        for (int i = 0; i < tabPage1.Controls.Count; i++)
        {
            if (tabPage1.Controls[i] is PictureBox && tabPage1.Controls[i].Name.Contains("npc_dota_hero"))
            {
                PictureBox pictureBox = (PictureBox)tabPage1.Controls[i];
                pictureBox.BorderStyle = BorderStyle.None;
            }
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

    private void CloseBtn_MouseMove(object sender, MouseEventArgs e)
    {
        CloseBtn.Image = Resources.Close2;
    }

    private void CloseBtn_MouseLeave(object sender, EventArgs e)
    {
        CloseBtn.Image = Resources.Close_1;
    }

    private void CloseBtn_Click(object sender, EventArgs e)
    {
        // El cierre limpio dispara FormClosing automáticamente,
        // que ya guarda settings y remueve el hook
        this.Close();
    }

    private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
    {
        try
        {
            Settings.Save();
            manager.Save();
            hook?.Remove();
            ImageCache.Clear();  // ← Liberar todas las imágenes
        }
        catch (Exception ex)
        {
            modCommon.Save(ex);
        }
    }

    private void Settings_MouseMove(object sender, MouseEventArgs e)
    {
        OpenSettings.Image = Resources.Settings2;
    }

    private void Settings_MouseLeave(object sender, EventArgs e)
    {
        OpenSettings.Image = Resources.Settings1;
    }

    private void Settings_Click(object sender, EventArgs e)
    {
        if (Ready)
        {
            frmSettings frmSettings2 = new frmSettings();
            frmSettings2.Show();
        }
    }

    private void TabPage3_Click(object sender, EventArgs e)
    {
    }

    private void Logo_Click(object sender, EventArgs e)
    {
        frm.tabControl1.SelectTab("tabPage3");
    }

    private void HeroLeft_MouseMove(object sender, MouseEventArgs e)
    {
        picLeft.Image = Resources.Left2;
        labelLeft.ForeColor = Color.FromArgb(114, 129, 134);
    }

    private void HeroLeft_MouseLeave(object sender, EventArgs e)
    {
        picLeft.Image = Resources.Left1;
        labelLeft.ForeColor = Color.FromArgb(55, 64, 69);
    }

    private void HeroRight_MouseMove(object sender, MouseEventArgs e)
    {
        picRight.Image = Resources.Rigth2;
        labelRight.ForeColor = Color.FromArgb(114, 129, 134);
    }

    private void HeroRight_MouseLeave(object sender, EventArgs e)
    {
        picRight.Image = Resources.Rigth1;
        labelRight.ForeColor = Color.FromArgb(55, 64, 69);
    }

    private void HeroLeft_Click(object sender, EventArgs e)
    {
        AudioPlayer.PlaySound("sounds/ui/panorama/panorama_topmenu_select_01.vsnd", toStop: false);
        SelectTab(tabPage6);
        string heroOriginalName = ItemsManager.GetHeroOriginalName(labelLeft.Text);
        ProcessHero(heroOriginalName);
    }

    private void HeroRight_Click(object sender, EventArgs e)
    {
        AudioPlayer.PlaySound("sounds/ui/panorama/panorama_topmenu_select_01.vsnd", toStop: false);
        SelectTab(tabPage6);
        string heroOriginalName = ItemsManager.GetHeroOriginalName(labelRight.Text);
        ProcessHero(heroOriginalName);
    }

    private void LabelRight_TextChanged(object sender, EventArgs e)
    {
        int num = Convert.ToInt32(modCommon.GetTextSize("IO", load.Font).Width);
        int num2 = Convert.ToInt32(modCommon.GetTextSize(labelRight.Text, load.Font).Width);
        int num3 = 1085;
        int num4 = num3 - (num2 - num);
        labelRight.Location = new Point(num4, labelRight.Location.Y);
    }

    private void OpenDota_Click(object sender, EventArgs e)
    {
        try
        {
            Process.GetProcessesByName("dota2")[0].Kill();
        }
        catch
        {
        }
        try
        {
            string fileName = Path.Combine(Paths.GetParent(Settings.Dota2Path), "bin", "win32") + "/dota2.exe";
            Process.Start(fileName);
        }
        catch (Exception ex)
        {
            modCommon.Save(ex);
        }
    }

    private void HeroName_Click(object sender, EventArgs e)
    {
    }

    public static void SetBoxInfo(BundleBox bundleBox)
    {
        Control control = bundleBox.Parent;
        frm.boxInfo.item = bundleBox.item;
        frm.boxInfo.BringToFront();
        int num = control.Location.X + bundleBox.Location.X + (bundleBox.Width + 10);
        if (num > 980)
        {
            num -= bundleBox.Width * 2 + 32;
        }
        frm.boxInfo.Location = new Point(num, control.Location.Y + bundleBox.Location.Y + bundleBox.Height - 63);
        if (frm.boxInfo.SetVisible)
        {
            frm.boxInfo.Visible = true;
        }
    }

    private void CreatedBy_Click(object sender, EventArgs e)
    {
        if (Environment.UserName == "Hackerprod")
        {
        }
        frmAbout frmAbout2 = new frmAbout();
        frmAbout2.Show();
    }

    private void ImportBtn_Click(object sender, EventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Title = "Select zip file to import",
            Filter = "Zip file | *.zip",
            Multiselect = true
        };
        DialogResult dialogResult = openFileDialog.ShowDialog();
        if (dialogResult == DialogResult.OK)
        {
            string[] fileNames = openFileDialog.FileNames;
            string[] array = fileNames;
            foreach (string sourceArchiveFileName in array)
            {
                try
                {
                    Paths.EnsureDirectory(modCommon.TempDirectory);
                    if (Directory.Exists(modCommon.TempDirectory))
                    {
                        Paths.CleanDirectory(modCommon.TempDirectory);
                    }
                    ZipFile.ExtractToDirectory(sourceArchiveFileName, modCommon.TempDirectory);
                    string[] files = Directory.GetFiles(modCommon.TempDirectory, "*.*", SearchOption.AllDirectories);
                    for (int j = 0; j < files.Length; j++)
                    {
                        string sourceFileName = files[j];
                        string text = files[j].Replace(modCommon.TempDirectory, modCommon.DataDirectory + "/ExternalItems");
                        Paths.EnsureDirectory(Path.GetDirectoryName(text));
                        try
                        {
                            File.Copy(sourceFileName, text, overwrite: true);
                        }
                        catch
                        {
                        }
                    }
                    if (Directory.Exists(modCommon.TempDirectory))
                    {
                        Paths.CleanDirectory(modCommon.TempDirectory);
                    }
                    Directory.Delete(modCommon.TempDirectory, recursive: true);
                }
                catch (Exception ex)
                {
                    modCommon.Save(ex);
                }
            }
        }
        DetectExternalMods();
    }

    private void DetectExternalMods()
    {
        ExternalModView.Items.Clear();
        ExternalModView.UseSelectable = true;
        string text = "";
        string[] files = Directory.GetFiles(modCommon.DataDirectory + "/ExternalItems", "*.vpcf_c", SearchOption.AllDirectories);
        string[] array = files;
        foreach (string path in array)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
            text = text + "Detected " + fileNameWithoutExtension.Replace("_", " ") + " particle" + Environment.NewLine;
            AddExternalMod("Detected " + fileNameWithoutExtension.Replace("_", " ") + " particle");
        }
    }

    private void AddExternalMod(string externalmod)
    {
        ListViewItem.ListViewSubItem item = new ListViewItem.ListViewSubItem();
        ListViewItem listViewItem = new ListViewItem();
        listViewItem.SubItems.Add(item);
        listViewItem.SubItems[0].Text = externalmod;
        ExternalModView.Items.Add(listViewItem);
    }

    private void FlatButton1_Click(object sender, EventArgs e)
    {
        Paths.CleanDirectory(modCommon.DataDirectory + "/ExternalItems");
        DetectExternalMods();
    }

    private void ExportThis_Click(object sender, EventArgs e)
    {
        LoadingScreenManager.Extract(CurrentLoadingScreen.item);
    }

    private void ExportAll_Click(object sender, EventArgs e)
    {
        LoadingScreenManager.Extract();
    }

    private void SetDesktop_Click(object sender, EventArgs e)
    {
        try
        {
            Items item = CurrentLoadingScreen.item;
            string v = item.asset.Replace("panorama/images/", "");
            v = ItemsManager.NameFromAsset(v);
            string text = modCommon.DataDirectory + "/Loading Screen/" + v + ".png";
            if (File.Exists(text))
            {
                Wallpaper.Set(text, Wallpaper.Style.Stretched);
            }
            else
            {
                LoadingScreenManager.Extract(item, Desktop: true);
            }
        }
        catch (Exception ex)
        {
            modCommon.Save(ex);
        }
    }

    private void Menu_MouseMove(object sender, MouseEventArgs e)
    {
        if (Updating)
        {
            return;
        }
        if (sender is Label)
        {
            ((Label)sender).ForeColor = Color.FromArgb(224, 224, 224);
        }
        else if (sender is Panel)
        {
            foreach (Control control in ((Panel)sender).Controls)
            {
                if (control is Label)
                {
                    ((Label)control).ForeColor = Color.FromArgb(224, 224, 224);
                }
            }
        }
        if (!(sender is Label))
        {
        }
    }

    private void Menu_MouseLeave(object sender, EventArgs e)
    {
        if (Updating)
        {
            return;
        }
        if (sender is Label)
        {
            if (CurrentTab == tabPage1)
            {
                if ((Label)sender == labelHeroes)
                {
                    return;
                }
                ((Label)sender).ForeColor = Color.Gray;
            }
            else if (CurrentTab == tabPage2)
            {
                if ((Label)sender == labelHeroes)
                {
                    return;
                }
                ((Label)sender).ForeColor = Color.Gray;
            }
            else if (CurrentTab == tabPage3)
            {
                if ((Label)sender == labelMisc)
                {
                    return;
                }
                ((Label)sender).ForeColor = Color.Gray;
            }
            else if (CurrentTab == tabPage4)
            {
                if ((Label)sender == labelWord)
                {
                    return;
                }
                ((Label)sender).ForeColor = Color.Gray;
            }
            else
            {
                ((Label)sender).ForeColor = Color.Gray;
            }
        }
        else if (sender is Panel)
        {
            foreach (Control control in ((Panel)sender).Controls)
            {
                if (!(control is Label))
                {
                    continue;
                }
                if (CurrentTab == tabPage1)
                {
                    if ((Label)control == labelHeroes)
                    {
                        return;
                    }
                    ((Label)control).ForeColor = Color.Gray;
                }
                else if (CurrentTab == tabPage2)
                {
                    if ((Label)control == labelHeroes)
                    {
                        return;
                    }
                    ((Label)control).ForeColor = Color.Gray;
                }
                else if (CurrentTab == tabPage3)
                {
                    if ((Label)control == labelMisc)
                    {
                        return;
                    }
                    ((Label)control).ForeColor = Color.Gray;
                }
                else if (CurrentTab == tabPage4)
                {
                    if ((Label)control == labelWord)
                    {
                        return;
                    }
                    ((Label)control).ForeColor = Color.Gray;
                }
                else
                {
                    ((Label)control).ForeColor = Color.Gray;
                }
            }
        }
        if (sender is Label && (Label)sender == createdBy)
        {
            boxInfo.Visible = false;
        }
    }

    public void SelectTab(TabPage tabPage)
    {
        this.InvokeIfRequired(() =>
        {
            if (tabPage != tabPage6)
            {
                CurrentTab = tabPage;
            }
            frm.tabControl1.SelectTab(tabPage);
            frm.SetMenuLabelColor(tabPage);
            if (tabPage != tabPage6 && tabPage != tabPage2)
            {
                AudioPlayer.PlaySound("sounds/ui/panorama/panorama_topmenu_select_03.vsnd", toStop: false);
            }
            if (tabPage == tabPage1)
            {
                frm.banner.Image = Resources.M1;
            }
            else if (tabPage == tabPage2)
            {
                frm.banner.Image = Resources.M1;
                frm.KeyBind.Focus();
            }
            else if (tabPage == tabPage3)
            {
                frm.banner.Image = Resources.M3;
            }
            else if (tabPage == tabPage4)
            {
                frm.banner.Image = Resources.M2;
            }
            else if (tabPage == tabPage6)
            {
                frm.logo.Visible = true;
                frm.logo.BringToFront();
            }
            else
            {
                frm.banner.Image = Resources.Banner;
            }
            modCommon.ReleaseMemory();
        });
    }

    private void SetMenuLabelColor(TabPage tabPage)
    {
        Label label = new Label();
        switch (tabPage.Name)
        {
            case "tabPage1":
                label = labelHeroes;
                break;
            case "tabPage2":
                label = labelHeroes;
                break;
            case "tabPage3":
                label = labelMisc;
                break;
            case "tabPage4":
                label = labelWord;
                break;
        }
        foreach (Control menu in Menus)
        {
            if (menu is Label)
            {
                if (menu.Text == label.Text)
                {
                    menu.ForeColor = Color.FromArgb(224, 224, 224);
                }
                else
                {
                    menu.ForeColor = Color.Gray;
                }
            }
        }
    }

    private void StopSounds_Click(object sender, EventArgs e)
    {
        AudioPlayer.soundPlayer.Stop();
    }

    private void KeyPress_MouseDown(object sender, MouseEventArgs e)
    {
    }

    private void KeyPress_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyData == Keys.Delete || e.KeyData == Keys.Back || e.KeyData == Keys.Return || e.KeyData == Keys.Escape)
        {
            foreach (object control in tabPage1.Controls)
            {
                if (control is PictureBox)
                {
                    ((PictureBox)control).Image = modCommon.GetHeroImage(((PictureBox)control).Name);
                    ((PictureBox)control).Tag = "1";
                }
            }
        }
        else
        {
            string text = e.KeyData.ToString().ToLower();
            if (text.Length > 1)
            {
                return;
            }
            MostrarKey(text.ToUpper());
            keyTimer.Enabled = true;
            foreach (object control2 in tabPage1.Controls)
            {
                if (control2 is PictureBox)
                {
                    PictureBox pictureBox = (PictureBox)control2;
                    string text2 = pictureBox.Name.Replace("npc_dota_hero_", "");
                    text2 = text2.Replace("_", " ");
                    if (text2.ToLower().StartsWith(text))
                    {
                        ((PictureBox)control2).Image = modCommon.GetHeroImage(((PictureBox)control2).Name);
                        ((PictureBox)control2).Tag = "1";
                    }
                    else if (pictureBox.Tag.ToString() == "1")
                    {
                        pictureBox.Image = modCommon.ChangeOpacity(pictureBox.Image, 0.2f);
                        pictureBox.Tag = "0";
                    }
                }
            }
        }
        keyPress.Text = "";
    }

    private void KeyTimer_Tick(object sender, EventArgs e)
    {
        MostrarKey("");
        keyTimer.Enabled = false;
    }

    private void LoadScreen_DoubleClick(object sender, EventArgs e)
    {
        try
        {
            string text = loadScreen.Tag.ToString();
            string text2 = modCommon.DataDirectory + "/Loading Screen/" + text + ".png";
            Items itemScreen = new Items
            {
                name = text
            };
            if (File.Exists(text2))
            {
                Process.Start(text2);
            }
            else
            {
                LoadingScreenManager.Extract(itemScreen, Desktop: false, open: true);
            }
        }
        catch (Exception ex)
        {
            modCommon.Save(ex);
        }
    }

    private void BoxInfo_Click(object sender, EventArgs e)
    {
    }

    private void KeyBind_KeyDown(object sender, KeyEventArgs e)
    {
    }

    public void ProcessKey(Keys keyData)
    {
        switch (keyData)
        {
            case Keys.Left:
                if (frm.tabControl1.SelectedTab == tabPage2)
                {
                    AudioPlayer.PlaySound("sounds/ui/panorama/panorama_topmenu_select_01.vsnd", toStop: false);
                    SelectTab(tabPage6);
                    string heroOriginalName = ItemsManager.GetHeroOriginalName(labelLeft.Text);
                    ProcessHero(heroOriginalName);
                }
                break;
            case Keys.Right:
                if (frm.tabControl1.SelectedTab == tabPage2)
                {
                    AudioPlayer.PlaySound("sounds/ui/panorama/panorama_topmenu_select_01.vsnd", toStop: false);
                    SelectTab(tabPage6);
                    string heroOriginalName2 = ItemsManager.GetHeroOriginalName(labelRight.Text);
                    ProcessHero(heroOriginalName2);
                }
                break;
            case Keys.F5:
                if (Ready)
                {
                    manager.CreateMod();
                }
                break;
            case Keys.F9:
                if (Ready)
                {
                    try
                    {
                        var dotaProcess = Process.GetProcessesByName("dota2").FirstOrDefault();
                        dotaProcess?.Kill();
                    }
                    catch
                    {
                    }
                    try
                    {
                        modCommon.WriteLine("Opening Dota2", loading: true);
                        string fileName = Path.Combine(Paths.GetParent(Settings.Dota2Path), "bin", "win32") + "/dota2.exe";
                        Process.Start(fileName);
                        break;
                    }
                    catch
                    {
                        break;
                    }
                }
                break;
        }
    }

    private void OpenImageMenu_Click(object sender, EventArgs e)
    {
        try
        {
            string text = loadScreen.Tag.ToString();
            string text2 = modCommon.DataDirectory + "/Loading Screen/" + text + ".png";
            Items itemScreen = new Items
            {
                name = text
            };
            if (File.Exists(text2))
            {
                Process.Start(text2);
            }
            else
            {
                LoadingScreenManager.Extract(itemScreen, Desktop: false, open: true);
            }
        }
        catch (Exception ex)
        {
            modCommon.Save(ex);
        }
    }

    private void SaveImageMenu_Click(object sender, EventArgs e)
    {
        try
        {
            string text = loadScreen.Tag.ToString();
            string text2 = modCommon.DataDirectory + "/Loading Screen/" + text + ".png";
            Items itemScreen = new Items
            {
                name = text
            };
            LoadingScreenManager.Extract(itemScreen);
        }
        catch (Exception ex)
        {
            modCommon.Save(ex);
        }
    }

    private void LoadScreen_MouseClick(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            int num = loadScreen.Width / 2;
            int num2 = OpenImageMenu.Width / 2;
            int num3 = num - num2;
            int num4 = loadScreen.Height / 2 - OpenImageMenu.Height;
            ImageMenu.Show(loadScreen, num3, num4);
        }
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
        components = new Container();
        panel1 = new Panel();
        panelWord = new Panel();
        labelWord = new Label();
        panelCreateMod = new Panel();
        labelCreateMod = new Label();
        panelMisc = new Panel();
        labelMisc = new Label();
        panelHeroes = new Panel();
        labelHeroes = new Label();
        OpenDota = new Panel();
        OpenSettings = new PictureBox();
        CloseBtn = new PictureBox();
        banner = new PictureBox();
        createdBy = new Label();
        keyTimer = new System.Windows.Forms.Timer(components);
        ClientVersion = new Label();
        boxInfo = new BoxInfo();
        tabControl1 = new FlatTabControl();
        tabPage1 = new TabPage();
        keyPressed = new Label();
        keyPress = new TextBox();
        tabPage2 = new TabPage();
        KeyBind = new TextBox();
        SlotContainer = new SlotsPanel();
        BundleControl = new MetroPanel();
        labelRight = new Label();
        labelLeft = new Label();
        HeroName = new Label();
        picRight = new PictureBox();
        picLeft = new PictureBox();
        HeroAvatar = new PictureBox();
        tabPage4 = new TabPage();
        WordContainer = new Panel();
        label14 = new Label();
        CurrentWard = new BundleBox();
        CurrentCourier = new BundleBox();
        label15 = new Label();
        CurrentHUDSkin = new BundleBox();
        label16 = new Label();
        label10 = new Label();
        CurrentEmoticons = new BundleBox();
        CurrentEmblem = new BundleBox();
        label11 = new Label();
        CurrentMultikillBanner = new BundleBox();
        label12 = new Label();
        CurrentCursorPack = new BundleBox();
        label13 = new Label();
        label9 = new Label();
        CurrentVersusScreen = new BundleBox();
        label8 = new Label();
        label7 = new Label();
        CurrentDireCreeps = new BundleBox();
        CurrentRadiantCreeps = new BundleBox();
        label6 = new Label();
        CurrentDireTowers = new BundleBox();
        CurrentRadiantTowers = new BundleBox();
        label5 = new Label();
        CurrentWeatherEffect = new BundleBox();
        label4 = new Label();
        CurrentTerrain = new BundleBox();
        label3 = new Label();
        tabPage3 = new TabPage();
        CurrentStreakEffect = new BundleBox();
        label25 = new Label();
        CurrentPets = new BundleBox();
        label24 = new Label();
        CurrentMegaKill = new BundleBox();
        label23 = new Label();
        stopSounds = new FlatButton();
        panel4 = new Panel();
        label22 = new Label();
        label21 = new Label();
        label20 = new Label();
        SetDesktop = new FlatButton();
        ExportAll = new FlatButton();
        ExportThis = new FlatButton();
        label19 = new Label();
        panel2 = new Panel();
        panel3 = new Panel();
        ExternalModView = new SkynListView();
        columnHeader5 = new ColumnHeader();
        flatButton1 = new FlatButton();
        importBtn = new FlatButton();
        label18 = new Label();
        LoadingScreenContainer = new Panel();
        loadScreen = new PictureBox();
        LoadingScreenView = new SkynListView();
        columnHeader1 = new ColumnHeader();
        columnHeader2 = new ColumnHeader();
        columnHeader3 = new ColumnHeader();
        columnHeader4 = new ColumnHeader();
        CurrentLoadingScreen = new BundleBox();
        label17 = new Label();
        CurrentAnnouncer = new BundleBox();
        CurrentMusicPack = new BundleBox();
        label2 = new Label();
        label1 = new Label();
        tabPage5 = new TabPage();
        tabPage6 = new TabPage();
        logo = new PictureBox();
        load = new Label();
        ImageMenu = new FlatContextMenuStrip();
        OpenImageMenu = new ToolStripMenuItem();
        SaveImageMenu = new ToolStripMenuItem();
        panel1.SuspendLayout();
        panelWord.SuspendLayout();
        panelCreateMod.SuspendLayout();
        panelMisc.SuspendLayout();
        panelHeroes.SuspendLayout();
        ((ISupportInitialize)OpenSettings).BeginInit();
        ((ISupportInitialize)CloseBtn).BeginInit();
        ((ISupportInitialize)banner).BeginInit();
        tabControl1.SuspendLayout();
        tabPage1.SuspendLayout();
        tabPage2.SuspendLayout();
        ((ISupportInitialize)picRight).BeginInit();
        ((ISupportInitialize)picLeft).BeginInit();
        ((ISupportInitialize)HeroAvatar).BeginInit();
        tabPage4.SuspendLayout();
        WordContainer.SuspendLayout();
        tabPage3.SuspendLayout();
        panel4.SuspendLayout();
        panel2.SuspendLayout();
        panel3.SuspendLayout();
        LoadingScreenContainer.SuspendLayout();
        ((ISupportInitialize)loadScreen).BeginInit();
        tabPage6.SuspendLayout();
        ((ISupportInitialize)logo).BeginInit();
        ImageMenu.SuspendLayout();
        SuspendLayout();
        // 
        // panel1
        // 
        panel1.Controls.Add(panelWord);
        panel1.Controls.Add(panelCreateMod);
        panel1.Controls.Add(panelMisc);
        panel1.Controls.Add(panelHeroes);
        panel1.Controls.Add(OpenDota);
        panel1.Controls.Add(OpenSettings);
        panel1.Controls.Add(CloseBtn);
        panel1.Controls.Add(banner);
        panel1.Dock = DockStyle.Top;
        panel1.Location = new Point(0, 0);
        panel1.Name = "panel1";
        panel1.Size = new Size(1152, 53);
        panel1.TabIndex = 0;
        // 
        // panelWord
        // 
        panelWord.BackColor = Color.Transparent;
        panelWord.Controls.Add(labelWord);
        panelWord.Location = new Point(344, 0);
        panelWord.Name = "panelWord";
        panelWord.Size = new Size(83, 40);
        panelWord.TabIndex = 58;
        panelWord.Click += Menu_Click;
        panelWord.MouseLeave += Menu_MouseLeave;
        panelWord.MouseMove += Menu_MouseMove;
        // 
        // labelWord
        // 
        labelWord.AutoSize = true;
        labelWord.BackColor = Color.Transparent;
        labelWord.Font = new Font("Segoe UI Emoji", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        labelWord.ForeColor = Color.Gray;
        labelWord.Location = new Point(18, 13);
        labelWord.Name = "labelWord";
        labelWord.Size = new Size(44, 15);
        labelWord.TabIndex = 52;
        labelWord.Text = "World";
        labelWord.Click += Menu_Click;
        labelWord.MouseLeave += Menu_MouseLeave;
        labelWord.MouseMove += Menu_MouseMove;
        // 
        // panelCreateMod
        // 
        panelCreateMod.BackColor = Color.Transparent;
        panelCreateMod.Controls.Add(labelCreateMod);
        panelCreateMod.Location = new Point(535, 0);
        panelCreateMod.Name = "panelCreateMod";
        panelCreateMod.Size = new Size(94, 40);
        panelCreateMod.TabIndex = 59;
        panelCreateMod.Click += Menu_Click;
        panelCreateMod.MouseLeave += Menu_MouseLeave;
        panelCreateMod.MouseMove += Menu_MouseMove;
        // 
        // labelCreateMod
        // 
        labelCreateMod.AutoSize = true;
        labelCreateMod.BackColor = Color.Transparent;
        labelCreateMod.Font = new Font("Segoe UI Emoji", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        labelCreateMod.ForeColor = Color.Gray;
        labelCreateMod.Location = new Point(5, 13);
        labelCreateMod.Name = "labelCreateMod";
        labelCreateMod.Size = new Size(77, 15);
        labelCreateMod.TabIndex = 56;
        labelCreateMod.Text = "Create Mod";
        labelCreateMod.Click += Menu_Click;
        labelCreateMod.MouseLeave += Menu_MouseLeave;
        labelCreateMod.MouseMove += Menu_MouseMove;
        // 
        // panelMisc
        // 
        panelMisc.BackColor = Color.Transparent;
        panelMisc.Controls.Add(labelMisc);
        panelMisc.Location = new Point(438, 0);
        panelMisc.Name = "panelMisc";
        panelMisc.Size = new Size(83, 40);
        panelMisc.TabIndex = 59;
        panelMisc.Click += Menu_Click;
        panelMisc.MouseLeave += Menu_MouseLeave;
        panelMisc.MouseMove += Menu_MouseMove;
        // 
        // labelMisc
        // 
        labelMisc.AutoSize = true;
        labelMisc.BackColor = Color.Transparent;
        labelMisc.Font = new Font("Segoe UI Emoji", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        labelMisc.ForeColor = Color.Gray;
        labelMisc.Location = new Point(24, 13);
        labelMisc.Name = "labelMisc";
        labelMisc.Size = new Size(29, 15);
        labelMisc.TabIndex = 51;
        labelMisc.Text = "HUI";
        labelMisc.Click += Menu_Click;
        labelMisc.MouseLeave += Menu_MouseLeave;
        labelMisc.MouseMove += Menu_MouseMove;
        // 
        // panelHeroes
        // 
        panelHeroes.BackColor = Color.Transparent;
        panelHeroes.Controls.Add(labelHeroes);
        panelHeroes.Location = new Point(247, 0);
        panelHeroes.Name = "panelHeroes";
        panelHeroes.Size = new Size(83, 40);
        panelHeroes.TabIndex = 0;
        panelHeroes.Click += Menu_Click;
        panelHeroes.MouseLeave += Menu_MouseLeave;
        panelHeroes.MouseMove += Menu_MouseMove;
        // 
        // labelHeroes
        // 
        labelHeroes.AutoSize = true;
        labelHeroes.BackColor = Color.Transparent;
        labelHeroes.Font = new Font("Segoe UI Emoji", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        labelHeroes.ForeColor = Color.FromArgb(224, 224, 224);
        labelHeroes.Location = new Point(16, 13);
        labelHeroes.Name = "labelHeroes";
        labelHeroes.Size = new Size(49, 15);
        labelHeroes.TabIndex = 50;
        labelHeroes.Text = "Heroes";
        labelHeroes.Click += Menu_Click;
        labelHeroes.MouseLeave += Menu_MouseLeave;
        labelHeroes.MouseMove += Menu_MouseMove;
        // 
        // OpenDota
        // 
        OpenDota.BackColor = Color.Transparent;
        OpenDota.Location = new Point(160, 3);
        OpenDota.Name = "OpenDota";
        OpenDota.Size = new Size(45, 42);
        OpenDota.TabIndex = 0;
        OpenDota.Click += OpenDota_Click;
        OpenDota.MouseDown += Event_MouseDown;
        OpenDota.MouseMove += Event_MouseMove;
        OpenDota.MouseUp += Event_MouseUp;
        // 
        // OpenSettings
        // 
        OpenSettings.BackColor = Color.Transparent;
        OpenSettings.Image = Resources.Settings1;
        OpenSettings.Location = new Point(12, 7);
        OpenSettings.Name = "OpenSettings";
        OpenSettings.Size = new Size(26, 26);
        OpenSettings.TabIndex = 55;
        OpenSettings.TabStop = false;
        OpenSettings.Click += Settings_Click;
        OpenSettings.MouseLeave += Settings_MouseLeave;
        OpenSettings.MouseMove += Settings_MouseMove;
        // 
        // CloseBtn
        // 
        CloseBtn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        CloseBtn.BackColor = Color.Transparent;
        CloseBtn.Image = Resources.Close_1;
        CloseBtn.Location = new Point(1097, 0);
        CloseBtn.Name = "CloseBtn";
        CloseBtn.Size = new Size(58, 38);
        CloseBtn.TabIndex = 54;
        CloseBtn.TabStop = false;
        CloseBtn.Click += CloseBtn_Click;
        CloseBtn.MouseLeave += CloseBtn_MouseLeave;
        CloseBtn.MouseMove += CloseBtn_MouseMove;
        // 
        // banner
        // 
        banner.BackColor = Color.FromArgb(31, 32, 35);
        banner.Dock = DockStyle.Fill;
        banner.Image = Resources.Banner;
        banner.Location = new Point(0, 0);
        banner.Name = "banner";
        banner.Size = new Size(1152, 53);
        banner.SizeMode = PictureBoxSizeMode.StretchImage;
        banner.TabIndex = 0;
        banner.TabStop = false;
        banner.MouseDown += Event_MouseDown;
        banner.MouseMove += Event_MouseMove;
        banner.MouseUp += Event_MouseUp;
        // 
        // createdBy
        // 
        createdBy.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        createdBy.AutoSize = true;
        createdBy.ForeColor = Color.Gray;
        createdBy.Location = new Point(1071, 551);
        createdBy.Name = "createdBy";
        createdBy.Size = new Size(85, 15);
        createdBy.TabIndex = 53;
        createdBy.Text = "by Hackerprod";
        createdBy.Visible = false;
        createdBy.Click += CreatedBy_Click;
        createdBy.MouseLeave += Menu_MouseLeave;
        createdBy.MouseMove += Menu_MouseMove;
        // 
        // keyTimer
        // 
        keyTimer.Interval = 1200;
        keyTimer.Tick += KeyTimer_Tick;
        // 
        // ClientVersion
        // 
        ClientVersion.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        ClientVersion.AutoSize = true;
        ClientVersion.Font = new Font("Segoe UI Emoji", 7.5F);
        ClientVersion.ForeColor = Color.Gray;
        ClientVersion.Location = new Point(3, 551);
        ClientVersion.Name = "ClientVersion";
        ClientVersion.Size = new Size(70, 14);
        ClientVersion.TabIndex = 62;
        ClientVersion.Text = "Client version: ";
        ClientVersion.Visible = false;
        // 
        // boxInfo
        // 
        boxInfo.BackColor = Color.FromArgb(31, 32, 35);
        boxInfo.Location = new Point(169, 551);
        boxInfo.Margin = new Padding(3, 4, 3, 4);
        boxInfo.MaximumSize = new Size(149, 162);
        boxInfo.MinimumSize = new Size(149, 162);
        boxInfo.Name = "boxInfo";
        boxInfo.Padding = new Padding(1);
        boxInfo.Size = new Size(149, 162);
        boxInfo.TabIndex = 61;
        boxInfo.Visible = false;
        boxInfo.Click += BoxInfo_Click;
        // 
        // tabControl1
        // 
        tabControl1.Alignment = TabAlignment.Right;
        tabControl1.Controls.Add(tabPage1);
        tabControl1.Controls.Add(tabPage2);
        tabControl1.Controls.Add(tabPage4);
        tabControl1.Controls.Add(tabPage3);
        tabControl1.Controls.Add(tabPage5);
        tabControl1.Controls.Add(tabPage6);
        tabControl1.Font = new Font("Segoe UI", 10F);
        tabControl1.ItemSize = new Size(80, 40);
        tabControl1.Location = new Point(0, 58);
        tabControl1.Multiline = true;
        tabControl1.Name = "tabControl1";
        tabControl1.SelectedIndex = 0;
        tabControl1.Size = new Size(1166, 486);
        tabControl1.SizeMode = TabSizeMode.Fixed;
        tabControl1.TabIndex = 51;
        // 
        // tabPage1
        // 
        tabPage1.BackColor = Color.FromArgb(31, 32, 35);
        tabPage1.Controls.Add(keyPressed);
        tabPage1.Controls.Add(keyPress);
        tabPage1.Location = new Point(4, 4);
        tabPage1.Name = "tabPage1";
        tabPage1.Padding = new Padding(3);
        tabPage1.Size = new Size(1118, 478);
        tabPage1.TabIndex = 0;
        tabPage1.Text = "First";
        tabPage1.MouseDown += Event_MouseDown;
        tabPage1.MouseMove += Event_MouseMove;
        tabPage1.MouseUp += Event_MouseUp;
        // 
        // keyPressed
        // 
        keyPressed.AutoSize = true;
        keyPressed.BackColor = Color.Transparent;
        keyPressed.Font = new Font("Segoe UI Emoji", 24F, FontStyle.Bold, GraphicsUnit.Point, 0);
        keyPressed.ForeColor = Color.Gray;
        keyPressed.Location = new Point(500, 431);
        keyPressed.Name = "keyPressed";
        keyPressed.Size = new Size(0, 43);
        keyPressed.TabIndex = 57;
        // 
        // keyPress
        // 
        keyPress.Location = new Point(1109, 447);
        keyPress.Name = "keyPress";
        keyPress.Size = new Size(0, 25);
        keyPress.TabIndex = 0;
        keyPress.KeyDown += KeyPress_KeyDown;
        // 
        // tabPage2
        // 
        tabPage2.BackColor = Color.FromArgb(31, 32, 35);
        tabPage2.Controls.Add(KeyBind);
        tabPage2.Controls.Add(SlotContainer);
        tabPage2.Controls.Add(BundleControl);
        tabPage2.Controls.Add(labelRight);
        tabPage2.Controls.Add(labelLeft);
        tabPage2.Controls.Add(HeroName);
        tabPage2.Controls.Add(picRight);
        tabPage2.Controls.Add(picLeft);
        tabPage2.Controls.Add(HeroAvatar);
        tabPage2.Location = new Point(4, 4);
        tabPage2.Name = "tabPage2";
        tabPage2.Padding = new Padding(3);
        tabPage2.Size = new Size(1118, 478);
        tabPage2.TabIndex = 1;
        tabPage2.Text = "Hero";
        // 
        // KeyBind
        // 
        KeyBind.Location = new Point(306, 282);
        KeyBind.Name = "KeyBind";
        KeyBind.Size = new Size(0, 25);
        KeyBind.TabIndex = 61;
        KeyBind.KeyDown += KeyBind_KeyDown;
        // 
        // SlotContainer
        // 
        SlotContainer.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        SlotContainer.AutoScroll = true;
        SlotContainer.BackColor = Color.FromArgb(31, 32, 35);
        SlotContainer.Location = new Point(148, 72);
        SlotContainer.Name = "SlotContainer";
        SlotContainer.Padding = new Padding(3);
        SlotContainer.Size = new Size(965, 235);
        SlotContainer.TabIndex = 60;
        // 
        // BundleControl
        // 
        BundleControl.AutoScroll = true;
        BundleControl.HorizontalScrollbar = true;
        BundleControl.Location = new Point(15, 318);
        BundleControl.Name = "BundleControl";
        BundleControl.Size = new Size(1112, 154);
        BundleControl.TabIndex = 59;
        BundleControl.UseSelectable = false;
        BundleControl.VerticalScrollbar = true;
        // 
        // labelRight
        // 
        labelRight.AutoSize = true;
        labelRight.BackColor = Color.Transparent;
        labelRight.Font = new Font("Segoe UI Emoji", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        labelRight.ForeColor = Color.FromArgb(75, 84, 89);
        labelRight.ImageAlign = ContentAlignment.MiddleRight;
        labelRight.Location = new Point(1085, 11);
        labelRight.Name = "labelRight";
        labelRight.Size = new Size(21, 15);
        labelRight.TabIndex = 58;
        labelRight.Text = "IO";
        labelRight.TextChanged += LabelRight_TextChanged;
        labelRight.Click += HeroRight_Click;
        labelRight.MouseLeave += HeroRight_MouseLeave;
        labelRight.MouseMove += HeroRight_MouseMove;
        // 
        // labelLeft
        // 
        labelLeft.AutoSize = true;
        labelLeft.BackColor = Color.Transparent;
        labelLeft.Font = new Font("Segoe UI Emoji", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        labelLeft.ForeColor = Color.FromArgb(75, 84, 89);
        labelLeft.Location = new Point(37, 11);
        labelLeft.Name = "labelLeft";
        labelLeft.Size = new Size(69, 15);
        labelLeft.TabIndex = 57;
        labelLeft.Text = "ANTIMAGE";
        labelLeft.Click += HeroLeft_Click;
        labelLeft.MouseLeave += HeroLeft_MouseLeave;
        labelLeft.MouseMove += HeroLeft_MouseMove;
        // 
        // HeroName
        // 
        HeroName.AutoSize = true;
        HeroName.BackColor = Color.Transparent;
        HeroName.Font = new Font("Segoe UI Emoji", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
        HeroName.ForeColor = Color.Gainsboro;
        HeroName.Location = new Point(9, 39);
        HeroName.Name = "HeroName";
        HeroName.Size = new Size(135, 32);
        HeroName.TabIndex = 53;
        HeroName.Text = "ABADDON";
        HeroName.Click += HeroName_Click;
        // 
        // picRight
        // 
        picRight.Image = Resources.Rigth1;
        picRight.Location = new Point(1110, 2);
        picRight.Name = "picRight";
        picRight.Size = new Size(31, 33);
        picRight.SizeMode = PictureBoxSizeMode.AutoSize;
        picRight.TabIndex = 56;
        picRight.TabStop = false;
        picRight.Click += HeroRight_Click;
        picRight.MouseLeave += HeroRight_MouseLeave;
        picRight.MouseMove += HeroRight_MouseMove;
        // 
        // picLeft
        // 
        picLeft.Image = Resources.Left1;
        picLeft.Location = new Point(2, 2);
        picLeft.Name = "picLeft";
        picLeft.Size = new Size(31, 33);
        picLeft.SizeMode = PictureBoxSizeMode.AutoSize;
        picLeft.TabIndex = 55;
        picLeft.TabStop = false;
        picLeft.Click += HeroLeft_Click;
        picLeft.MouseLeave += HeroLeft_MouseLeave;
        picLeft.MouseMove += HeroLeft_MouseMove;
        // 
        // HeroAvatar
        // 
        HeroAvatar.Location = new Point(15, 83);
        HeroAvatar.Name = "HeroAvatar";
        HeroAvatar.Size = new Size(130, 177);
        HeroAvatar.SizeMode = PictureBoxSizeMode.StretchImage;
        HeroAvatar.TabIndex = 0;
        HeroAvatar.TabStop = false;
        // 
        // tabPage4
        // 
        tabPage4.BackColor = Color.FromArgb(31, 32, 35);
        tabPage4.Controls.Add(WordContainer);
        tabPage4.Location = new Point(4, 4);
        tabPage4.Name = "tabPage4";
        tabPage4.Size = new Size(1118, 478);
        tabPage4.TabIndex = 3;
        tabPage4.Text = "word";
        // 
        // WordContainer
        // 
        WordContainer.AutoScroll = true;
        WordContainer.Controls.Add(label14);
        WordContainer.Controls.Add(CurrentWard);
        WordContainer.Controls.Add(CurrentCourier);
        WordContainer.Controls.Add(label15);
        WordContainer.Controls.Add(CurrentHUDSkin);
        WordContainer.Controls.Add(label16);
        WordContainer.Controls.Add(label10);
        WordContainer.Controls.Add(CurrentEmoticons);
        WordContainer.Controls.Add(CurrentEmblem);
        WordContainer.Controls.Add(label11);
        WordContainer.Controls.Add(CurrentMultikillBanner);
        WordContainer.Controls.Add(label12);
        WordContainer.Controls.Add(CurrentCursorPack);
        WordContainer.Controls.Add(label13);
        WordContainer.Controls.Add(label9);
        WordContainer.Controls.Add(CurrentVersusScreen);
        WordContainer.Controls.Add(label8);
        WordContainer.Controls.Add(label7);
        WordContainer.Controls.Add(CurrentDireCreeps);
        WordContainer.Controls.Add(CurrentRadiantCreeps);
        WordContainer.Controls.Add(label6);
        WordContainer.Controls.Add(CurrentDireTowers);
        WordContainer.Controls.Add(CurrentRadiantTowers);
        WordContainer.Controls.Add(label5);
        WordContainer.Controls.Add(CurrentWeatherEffect);
        WordContainer.Controls.Add(label4);
        WordContainer.Controls.Add(CurrentTerrain);
        WordContainer.Controls.Add(label3);
        WordContainer.Dock = DockStyle.Fill;
        WordContainer.Location = new Point(0, 0);
        WordContainer.Margin = new Padding(0);
        WordContainer.Name = "WordContainer";
        WordContainer.Size = new Size(1118, 478);
        WordContainer.TabIndex = 0;
        // 
        // label14
        // 
        label14.AutoSize = true;
        label14.BackColor = Color.Transparent;
        label14.Font = new Font("Segoe UI Emoji", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        label14.ForeColor = Color.Gray;
        label14.Location = new Point(948, 207);
        label14.Name = "label14";
        label14.Size = new Size(39, 15);
        label14.TabIndex = 87;
        label14.Text = "Ward";
        // 
        // CurrentWard
        // 
        CurrentWard.BackColor = Color.FromArgb(47, 48, 51);
        CurrentWard.Location = new Point(951, 225);
        CurrentWard.Margin = new Padding(3, 21, 3, 21);
        CurrentWard.MaximumSize = new Size(129, 126);
        CurrentWard.MinimumSize = new Size(129, 126);
        CurrentWard.Name = "CurrentWard";
        CurrentWard.ShowToolTip = true;
        CurrentWard.Size = new Size(129, 126);
        CurrentWard.Static = true;
        CurrentWard.TabIndex = 86;
        // 
        // CurrentCourier
        // 
        CurrentCourier.BackColor = Color.FromArgb(47, 48, 51);
        CurrentCourier.Location = new Point(799, 225);
        CurrentCourier.Margin = new Padding(3, 16, 3, 16);
        CurrentCourier.MaximumSize = new Size(129, 126);
        CurrentCourier.MinimumSize = new Size(129, 126);
        CurrentCourier.Name = "CurrentCourier";
        CurrentCourier.ShowToolTip = true;
        CurrentCourier.Size = new Size(129, 126);
        CurrentCourier.Static = true;
        CurrentCourier.TabIndex = 85;
        // 
        // label15
        // 
        label15.AutoSize = true;
        label15.BackColor = Color.Transparent;
        label15.Font = new Font("Segoe UI Emoji", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        label15.ForeColor = Color.Gray;
        label15.Location = new Point(796, 207);
        label15.Name = "label15";
        label15.Size = new Size(52, 15);
        label15.TabIndex = 84;
        label15.Text = "Courier";
        // 
        // CurrentHUDSkin
        // 
        CurrentHUDSkin.BackColor = Color.FromArgb(47, 48, 51);
        CurrentHUDSkin.Location = new Point(646, 225);
        CurrentHUDSkin.Margin = new Padding(3, 12, 3, 12);
        CurrentHUDSkin.MaximumSize = new Size(129, 126);
        CurrentHUDSkin.MinimumSize = new Size(129, 126);
        CurrentHUDSkin.Name = "CurrentHUDSkin";
        CurrentHUDSkin.ShowToolTip = true;
        CurrentHUDSkin.Size = new Size(129, 126);
        CurrentHUDSkin.Static = true;
        CurrentHUDSkin.TabIndex = 83;
        // 
        // label16
        // 
        label16.AutoSize = true;
        label16.BackColor = Color.Transparent;
        label16.Font = new Font("Segoe UI Emoji", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        label16.ForeColor = Color.Gray;
        label16.Location = new Point(643, 207);
        label16.Name = "label16";
        label16.Size = new Size(62, 15);
        label16.TabIndex = 82;
        label16.Text = "Hud Skin";
        // 
        // label10
        // 
        label10.AutoSize = true;
        label10.BackColor = Color.Transparent;
        label10.Font = new Font("Segoe UI Emoji", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        label10.ForeColor = Color.Gray;
        label10.Location = new Point(490, 207);
        label10.Name = "label10";
        label10.Size = new Size(69, 15);
        label10.TabIndex = 81;
        label10.Text = "Emoticons";
        // 
        // CurrentEmoticons
        // 
        CurrentEmoticons.BackColor = Color.FromArgb(47, 48, 51);
        CurrentEmoticons.Location = new Point(493, 225);
        CurrentEmoticons.Margin = new Padding(3, 16, 3, 16);
        CurrentEmoticons.MaximumSize = new Size(129, 126);
        CurrentEmoticons.MinimumSize = new Size(129, 126);
        CurrentEmoticons.Name = "CurrentEmoticons";
        CurrentEmoticons.ShowToolTip = true;
        CurrentEmoticons.Size = new Size(129, 126);
        CurrentEmoticons.Static = true;
        CurrentEmoticons.TabIndex = 80;
        // 
        // CurrentEmblem
        // 
        CurrentEmblem.BackColor = Color.FromArgb(47, 48, 51);
        CurrentEmblem.Location = new Point(340, 225);
        CurrentEmblem.Margin = new Padding(3, 12, 3, 12);
        CurrentEmblem.MaximumSize = new Size(129, 126);
        CurrentEmblem.MinimumSize = new Size(129, 126);
        CurrentEmblem.Name = "CurrentEmblem";
        CurrentEmblem.ShowToolTip = true;
        CurrentEmblem.Size = new Size(129, 126);
        CurrentEmblem.Static = true;
        CurrentEmblem.TabIndex = 79;
        // 
        // label11
        // 
        label11.AutoSize = true;
        label11.BackColor = Color.Transparent;
        label11.Font = new Font("Segoe UI Emoji", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        label11.ForeColor = Color.Gray;
        label11.Location = new Point(337, 207);
        label11.Name = "label11";
        label11.Size = new Size(53, 15);
        label11.TabIndex = 78;
        label11.Text = "Emblem";
        // 
        // CurrentMultikillBanner
        // 
        CurrentMultikillBanner.BackColor = Color.FromArgb(47, 48, 51);
        CurrentMultikillBanner.Location = new Point(187, 225);
        CurrentMultikillBanner.Margin = new Padding(3, 9, 3, 9);
        CurrentMultikillBanner.MaximumSize = new Size(129, 126);
        CurrentMultikillBanner.MinimumSize = new Size(129, 126);
        CurrentMultikillBanner.Name = "CurrentMultikillBanner";
        CurrentMultikillBanner.ShowToolTip = true;
        CurrentMultikillBanner.Size = new Size(129, 126);
        CurrentMultikillBanner.Static = true;
        CurrentMultikillBanner.TabIndex = 77;
        // 
        // label12
        // 
        label12.AutoSize = true;
        label12.BackColor = Color.Transparent;
        label12.Font = new Font("Segoe UI Emoji", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        label12.ForeColor = Color.Gray;
        label12.Location = new Point(184, 207);
        label12.Name = "label12";
        label12.Size = new Size(109, 15);
        label12.TabIndex = 76;
        label12.Text = "Multi kill Banner";
        // 
        // CurrentCursorPack
        // 
        CurrentCursorPack.BackColor = Color.FromArgb(47, 48, 51);
        CurrentCursorPack.Location = new Point(34, 225);
        CurrentCursorPack.Margin = new Padding(3, 7, 3, 7);
        CurrentCursorPack.MaximumSize = new Size(129, 126);
        CurrentCursorPack.MinimumSize = new Size(129, 126);
        CurrentCursorPack.Name = "CurrentCursorPack";
        CurrentCursorPack.ShowToolTip = true;
        CurrentCursorPack.Size = new Size(129, 126);
        CurrentCursorPack.Static = true;
        CurrentCursorPack.TabIndex = 75;
        // 
        // label13
        // 
        label13.AutoSize = true;
        label13.BackColor = Color.Transparent;
        label13.Font = new Font("Segoe UI Emoji", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        label13.ForeColor = Color.Gray;
        label13.Location = new Point(31, 207);
        label13.Name = "label13";
        label13.Size = new Size(78, 15);
        label13.TabIndex = 74;
        label13.Text = "Cursor Pack";
        // 
        // label9
        // 
        label9.AutoSize = true;
        label9.BackColor = Color.Transparent;
        label9.Font = new Font("Segoe UI Emoji", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        label9.ForeColor = Color.Gray;
        label9.Location = new Point(948, 28);
        label9.Name = "label9";
        label9.Size = new Size(91, 15);
        label9.TabIndex = 73;
        label9.Text = "Versus Screen";
        // 
        // CurrentVersusScreen
        // 
        CurrentVersusScreen.BackColor = Color.FromArgb(47, 48, 51);
        CurrentVersusScreen.Location = new Point(951, 46);
        CurrentVersusScreen.Margin = new Padding(3, 21, 3, 21);
        CurrentVersusScreen.MaximumSize = new Size(129, 126);
        CurrentVersusScreen.MinimumSize = new Size(129, 126);
        CurrentVersusScreen.Name = "CurrentVersusScreen";
        CurrentVersusScreen.ShowToolTip = true;
        CurrentVersusScreen.Size = new Size(129, 126);
        CurrentVersusScreen.Static = true;
        CurrentVersusScreen.TabIndex = 72;
        CurrentVersusScreen.Click += Effect_Click;
        // 
        // label8
        // 
        label8.AutoSize = true;
        label8.BackColor = Color.Transparent;
        label8.Font = new Font("Segoe UI Emoji", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        label8.ForeColor = Color.Gray;
        label8.Location = new Point(796, 28);
        label8.Name = "label8";
        label8.Size = new Size(75, 15);
        label8.TabIndex = 71;
        label8.Text = "Dire creeps";
        // 
        // label7
        // 
        label7.AutoSize = true;
        label7.BackColor = Color.Transparent;
        label7.Font = new Font("Segoe UI Emoji", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        label7.ForeColor = Color.Gray;
        label7.Location = new Point(643, 28);
        label7.Name = "label7";
        label7.Size = new Size(97, 15);
        label7.TabIndex = 70;
        label7.Text = "Radiant creeps";
        // 
        // CurrentDireCreeps
        // 
        CurrentDireCreeps.BackColor = Color.FromArgb(47, 48, 51);
        CurrentDireCreeps.Location = new Point(799, 46);
        CurrentDireCreeps.Margin = new Padding(3, 16, 3, 16);
        CurrentDireCreeps.MaximumSize = new Size(129, 126);
        CurrentDireCreeps.MinimumSize = new Size(129, 126);
        CurrentDireCreeps.Name = "CurrentDireCreeps";
        CurrentDireCreeps.ShowToolTip = true;
        CurrentDireCreeps.Size = new Size(129, 126);
        CurrentDireCreeps.Static = true;
        CurrentDireCreeps.TabIndex = 69;
        CurrentDireCreeps.Click += Effect_Click;
        // 
        // CurrentRadiantCreeps
        // 
        CurrentRadiantCreeps.BackColor = Color.FromArgb(47, 48, 51);
        CurrentRadiantCreeps.Location = new Point(646, 46);
        CurrentRadiantCreeps.Margin = new Padding(3, 16, 3, 16);
        CurrentRadiantCreeps.MaximumSize = new Size(129, 126);
        CurrentRadiantCreeps.MinimumSize = new Size(129, 126);
        CurrentRadiantCreeps.Name = "CurrentRadiantCreeps";
        CurrentRadiantCreeps.ShowToolTip = true;
        CurrentRadiantCreeps.Size = new Size(129, 126);
        CurrentRadiantCreeps.Static = true;
        CurrentRadiantCreeps.TabIndex = 68;
        CurrentRadiantCreeps.Click += Effect_Click;
        // 
        // label6
        // 
        label6.AutoSize = true;
        label6.BackColor = Color.Transparent;
        label6.Font = new Font("Segoe UI Emoji", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        label6.ForeColor = Color.Gray;
        label6.Location = new Point(490, 28);
        label6.Name = "label6";
        label6.Size = new Size(78, 15);
        label6.TabIndex = 65;
        label6.Text = "Dire Towers";
        // 
        // CurrentDireTowers
        // 
        CurrentDireTowers.BackColor = Color.FromArgb(47, 48, 51);
        CurrentDireTowers.Location = new Point(493, 46);
        CurrentDireTowers.Margin = new Padding(3, 12, 3, 12);
        CurrentDireTowers.MaximumSize = new Size(129, 126);
        CurrentDireTowers.MinimumSize = new Size(129, 126);
        CurrentDireTowers.Name = "CurrentDireTowers";
        CurrentDireTowers.ShowToolTip = true;
        CurrentDireTowers.Size = new Size(129, 126);
        CurrentDireTowers.Static = true;
        CurrentDireTowers.TabIndex = 64;
        CurrentDireTowers.Click += Effect_Click;
        // 
        // CurrentRadiantTowers
        // 
        CurrentRadiantTowers.BackColor = Color.FromArgb(47, 48, 51);
        CurrentRadiantTowers.Location = new Point(340, 46);
        CurrentRadiantTowers.Margin = new Padding(3, 9, 3, 9);
        CurrentRadiantTowers.MaximumSize = new Size(129, 126);
        CurrentRadiantTowers.MinimumSize = new Size(129, 126);
        CurrentRadiantTowers.Name = "CurrentRadiantTowers";
        CurrentRadiantTowers.ShowToolTip = true;
        CurrentRadiantTowers.Size = new Size(129, 126);
        CurrentRadiantTowers.Static = true;
        CurrentRadiantTowers.TabIndex = 63;
        CurrentRadiantTowers.Click += Effect_Click;
        // 
        // label5
        // 
        label5.AutoSize = true;
        label5.BackColor = Color.Transparent;
        label5.Font = new Font("Segoe UI Emoji", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        label5.ForeColor = Color.Gray;
        label5.Location = new Point(337, 28);
        label5.Name = "label5";
        label5.Size = new Size(100, 15);
        label5.TabIndex = 62;
        label5.Text = "Radiant Towers";
        // 
        // CurrentWeatherEffect
        // 
        CurrentWeatherEffect.BackColor = Color.FromArgb(47, 48, 51);
        CurrentWeatherEffect.Location = new Point(187, 46);
        CurrentWeatherEffect.Margin = new Padding(3, 7, 3, 7);
        CurrentWeatherEffect.MaximumSize = new Size(129, 126);
        CurrentWeatherEffect.MinimumSize = new Size(129, 126);
        CurrentWeatherEffect.Name = "CurrentWeatherEffect";
        CurrentWeatherEffect.ShowToolTip = true;
        CurrentWeatherEffect.Size = new Size(129, 126);
        CurrentWeatherEffect.Static = true;
        CurrentWeatherEffect.TabIndex = 61;
        CurrentWeatherEffect.Click += Effect_Click;
        // 
        // label4
        // 
        label4.AutoSize = true;
        label4.BackColor = Color.Transparent;
        label4.Font = new Font("Segoe UI Emoji", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        label4.ForeColor = Color.Gray;
        label4.Location = new Point(184, 28);
        label4.Name = "label4";
        label4.Size = new Size(103, 15);
        label4.TabIndex = 59;
        label4.Text = "Weather Effects";
        // 
        // CurrentTerrain
        // 
        CurrentTerrain.BackColor = Color.FromArgb(47, 48, 51);
        CurrentTerrain.Location = new Point(34, 46);
        CurrentTerrain.Margin = new Padding(3, 5, 3, 5);
        CurrentTerrain.MaximumSize = new Size(129, 126);
        CurrentTerrain.MinimumSize = new Size(129, 126);
        CurrentTerrain.Name = "CurrentTerrain";
        CurrentTerrain.ShowToolTip = true;
        CurrentTerrain.Size = new Size(129, 126);
        CurrentTerrain.Static = true;
        CurrentTerrain.TabIndex = 58;
        CurrentTerrain.Click += Effect_Click;
        // 
        // label3
        // 
        label3.AutoSize = true;
        label3.BackColor = Color.Transparent;
        label3.Font = new Font("Segoe UI Emoji", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        label3.ForeColor = Color.Gray;
        label3.Location = new Point(31, 28);
        label3.Name = "label3";
        label3.Size = new Size(49, 15);
        label3.TabIndex = 56;
        label3.Text = "Terrain";
        // 
        // tabPage3
        // 
        tabPage3.BackColor = Color.FromArgb(31, 32, 35);
        tabPage3.Controls.Add(CurrentStreakEffect);
        tabPage3.Controls.Add(label25);
        tabPage3.Controls.Add(CurrentPets);
        tabPage3.Controls.Add(label24);
        tabPage3.Controls.Add(CurrentMegaKill);
        tabPage3.Controls.Add(label23);
        tabPage3.Controls.Add(stopSounds);
        tabPage3.Controls.Add(panel4);
        tabPage3.Controls.Add(SetDesktop);
        tabPage3.Controls.Add(ExportAll);
        tabPage3.Controls.Add(ExportThis);
        tabPage3.Controls.Add(label19);
        tabPage3.Controls.Add(panel2);
        tabPage3.Controls.Add(label18);
        tabPage3.Controls.Add(LoadingScreenContainer);
        tabPage3.Controls.Add(LoadingScreenView);
        tabPage3.Controls.Add(CurrentLoadingScreen);
        tabPage3.Controls.Add(label17);
        tabPage3.Controls.Add(CurrentAnnouncer);
        tabPage3.Controls.Add(CurrentMusicPack);
        tabPage3.Controls.Add(label2);
        tabPage3.Controls.Add(label1);
        tabPage3.Location = new Point(4, 4);
        tabPage3.Name = "tabPage3";
        tabPage3.Padding = new Padding(3);
        tabPage3.Size = new Size(1118, 478);
        tabPage3.TabIndex = 2;
        tabPage3.Text = "misc";
        tabPage3.Click += TabPage3_Click;
        // 
        // CurrentStreakEffect
        // 
        CurrentStreakEffect.BackColor = Color.FromArgb(47, 48, 51);
        CurrentStreakEffect.Location = new Point(651, 49);
        CurrentStreakEffect.Margin = new Padding(3, 5, 3, 5);
        CurrentStreakEffect.MaximumSize = new Size(128, 124);
        CurrentStreakEffect.MinimumSize = new Size(128, 124);
        CurrentStreakEffect.Name = "CurrentStreakEffect";
        CurrentStreakEffect.ShowToolTip = true;
        CurrentStreakEffect.Size = new Size(128, 124);
        CurrentStreakEffect.Static = true;
        CurrentStreakEffect.TabIndex = 266;
        // 
        // label25
        // 
        label25.AutoSize = true;
        label25.BackColor = Color.Transparent;
        label25.Font = new Font("Segoe UI Emoji", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        label25.ForeColor = Color.Gray;
        label25.Location = new Point(648, 29);
        label25.Name = "label25";
        label25.Size = new Size(84, 15);
        label25.TabIndex = 265;
        label25.Text = "Streak Effect";
        // 
        // CurrentPets
        // 
        CurrentPets.BackColor = Color.FromArgb(47, 48, 51);
        CurrentPets.Location = new Point(495, 49);
        CurrentPets.Margin = new Padding(3, 4, 3, 4);
        CurrentPets.MaximumSize = new Size(128, 124);
        CurrentPets.MinimumSize = new Size(128, 124);
        CurrentPets.Name = "CurrentPets";
        CurrentPets.ShowToolTip = true;
        CurrentPets.Size = new Size(128, 124);
        CurrentPets.Static = true;
        CurrentPets.TabIndex = 264;
        // 
        // label24
        // 
        label24.AutoSize = true;
        label24.BackColor = Color.Transparent;
        label24.Font = new Font("Segoe UI Emoji", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        label24.ForeColor = Color.Gray;
        label24.Location = new Point(492, 28);
        label24.Name = "label24";
        label24.Size = new Size(65, 15);
        label24.TabIndex = 263;
        label24.Text = "Hero Pets";
        // 
        // CurrentMegaKill
        // 
        CurrentMegaKill.BackColor = Color.FromArgb(47, 48, 51);
        CurrentMegaKill.Location = new Point(341, 49);
        CurrentMegaKill.MaximumSize = new Size(128, 124);
        CurrentMegaKill.MinimumSize = new Size(128, 124);
        CurrentMegaKill.Name = "CurrentMegaKill";
        CurrentMegaKill.ShowToolTip = true;
        CurrentMegaKill.Size = new Size(128, 124);
        CurrentMegaKill.Static = true;
        CurrentMegaKill.TabIndex = 262;
        // 
        // label23
        // 
        label23.AutoSize = true;
        label23.BackColor = Color.Transparent;
        label23.Font = new Font("Segoe UI Emoji", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        label23.ForeColor = Color.Gray;
        label23.Location = new Point(338, 28);
        label23.Name = "label23";
        label23.Size = new Size(95, 15);
        label23.TabIndex = 261;
        label23.Text = "Mega-Kill Pack";
        // 
        // stopSounds
        // 
        stopSounds.BackColor = Color.FromArgb(43, 47, 48);
        stopSounds.Cursor = Cursors.Hand;
        stopSounds.Font = new Font("Segoe UI", 9F);
        stopSounds.ForeColor = Color.FromArgb(147, 157, 160);
        stopSounds.Location = new Point(34, 180);
        stopSounds.Name = "stopSounds";
        stopSounds.Size = new Size(129, 25);
        stopSounds.TabIndex = 260;
        stopSounds.Text = "Stop sounds";
        stopSounds.Visible = false;
        stopSounds.Click += StopSounds_Click;
        // 
        // panel4
        // 
        panel4.BackColor = Color.FromArgb(47, 48, 51);
        panel4.Controls.Add(label22);
        panel4.Controls.Add(label21);
        panel4.Controls.Add(label20);
        panel4.Location = new Point(187, 241);
        panel4.Name = "panel4";
        panel4.Size = new Size(552, 25);
        panel4.TabIndex = 259;
        // 
        // label22
        // 
        label22.AutoSize = true;
        label22.BackColor = Color.Transparent;
        label22.Font = new Font("Segoe UI Emoji", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        label22.ForeColor = Color.Gray;
        label22.Location = new Point(432, 5);
        label22.Name = "label22";
        label22.Size = new Size(42, 15);
        label22.TabIndex = 262;
        label22.Text = "Rarity";
        // 
        // label21
        // 
        label21.AutoSize = true;
        label21.BackColor = Color.Transparent;
        label21.Font = new Font("Segoe UI Emoji", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        label21.ForeColor = Color.Gray;
        label21.Location = new Point(370, 5);
        label21.Name = "label21";
        label21.Size = new Size(20, 15);
        label21.TabIndex = 261;
        label21.Text = "ID";
        // 
        // label20
        // 
        label20.AutoSize = true;
        label20.BackColor = Color.Transparent;
        label20.Font = new Font("Segoe UI Emoji", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        label20.ForeColor = Color.Gray;
        label20.Location = new Point(6, 5);
        label20.Name = "label20";
        label20.Size = new Size(137, 15);
        label20.TabIndex = 260;
        label20.Text = "Loading Screen Name";
        // 
        // SetDesktop
        // 
        SetDesktop.BackColor = Color.FromArgb(43, 47, 48);
        SetDesktop.Cursor = Cursors.Hand;
        SetDesktop.Font = new Font("Segoe UI", 9F);
        SetDesktop.ForeColor = Color.FromArgb(147, 157, 160);
        SetDesktop.Location = new Point(34, 435);
        SetDesktop.Name = "SetDesktop";
        SetDesktop.Size = new Size(129, 25);
        SetDesktop.TabIndex = 258;
        SetDesktop.Text = "Set on Desktop";
        SetDesktop.Click += SetDesktop_Click;
        // 
        // ExportAll
        // 
        ExportAll.BackColor = Color.FromArgb(43, 47, 48);
        ExportAll.Cursor = Cursors.Hand;
        ExportAll.Font = new Font("Segoe UI", 9F);
        ExportAll.ForeColor = Color.FromArgb(147, 157, 160);
        ExportAll.Location = new Point(34, 404);
        ExportAll.Name = "ExportAll";
        ExportAll.Size = new Size(129, 25);
        ExportAll.TabIndex = 257;
        ExportAll.Text = "Export all";
        ExportAll.Click += ExportAll_Click;
        // 
        // ExportThis
        // 
        ExportThis.BackColor = Color.FromArgb(43, 47, 48);
        ExportThis.Cursor = Cursors.Hand;
        ExportThis.Font = new Font("Segoe UI", 9F);
        ExportThis.ForeColor = Color.FromArgb(147, 157, 160);
        ExportThis.Location = new Point(34, 373);
        ExportThis.Name = "ExportThis";
        ExportThis.Size = new Size(129, 25);
        ExportThis.TabIndex = 256;
        ExportThis.Text = "Export current";
        ExportThis.Click += ExportThis_Click;
        // 
        // label19
        // 
        label19.AutoSize = true;
        label19.BackColor = Color.Transparent;
        label19.Font = new Font("Segoe UI Emoji", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        label19.ForeColor = Color.Gray;
        label19.Location = new Point(805, 28);
        label19.Name = "label19";
        label19.Size = new Size(174, 15);
        label19.TabIndex = 64;
        label19.Text = "Import Effects and Particles";
        // 
        // panel2
        // 
        panel2.BackColor = Color.Black;
        panel2.Controls.Add(panel3);
        panel2.Location = new Point(808, 49);
        panel2.Name = "panel2";
        panel2.Padding = new Padding(1);
        panel2.Size = new Size(287, 126);
        panel2.TabIndex = 63;
        // 
        // panel3
        // 
        panel3.BackColor = Color.FromArgb(31, 32, 35);
        panel3.Controls.Add(ExternalModView);
        panel3.Controls.Add(flatButton1);
        panel3.Controls.Add(importBtn);
        panel3.Dock = DockStyle.Fill;
        panel3.Location = new Point(1, 1);
        panel3.Name = "panel3";
        panel3.Padding = new Padding(1);
        panel3.Size = new Size(285, 124);
        panel3.TabIndex = 64;
        // 
        // ExternalModView
        // 
        ExternalModView.BackColor = Color.FromArgb(31, 32, 35);
        ExternalModView.BorderStyle = BorderStyle.None;
        ExternalModView.Columns.AddRange(new ColumnHeader[] { columnHeader5 });
        ExternalModView.Dock = DockStyle.Top;
        ExternalModView.Font = new Font("Segoe UI", 12F);
        ExternalModView.ForeColor = Color.Gray;
        ExternalModView.FullRowSelect = true;
        ExternalModView.HeaderStyle = ColumnHeaderStyle.None;
        ExternalModView.HideSelection = true;
        ExternalModView.Location = new Point(1, 1);
        ExternalModView.MultiSelect = false;
        ExternalModView.Name = "ExternalModView";
        ExternalModView.OwnerDraw = true;
        ExternalModView.Size = new Size(283, 88);
        ExternalModView.TabIndex = 256;
        ExternalModView.UseCompatibleStateImageBehavior = false;
        ExternalModView.UseSelectable = true;
        ExternalModView.View = View.Details;
        // 
        // columnHeader5
        // 
        columnHeader5.Text = "     ";
        columnHeader5.Width = 7;
        // 
        // flatButton1
        // 
        flatButton1.BackColor = Color.FromArgb(43, 47, 48);
        flatButton1.Cursor = Cursors.Hand;
        flatButton1.Font = new Font("Segoe UI", 9F);
        flatButton1.ForeColor = Color.FromArgb(147, 157, 160);
        flatButton1.Location = new Point(4, 95);
        flatButton1.Name = "flatButton1";
        flatButton1.Size = new Size(144, 25);
        flatButton1.TabIndex = 255;
        flatButton1.Text = "Delete external mods";
        flatButton1.Click += FlatButton1_Click;
        // 
        // importBtn
        // 
        importBtn.BackColor = Color.FromArgb(43, 47, 48);
        importBtn.Cursor = Cursors.Hand;
        importBtn.Font = new Font("Segoe UI", 9F);
        importBtn.ForeColor = Color.FromArgb(147, 157, 160);
        importBtn.Location = new Point(155, 95);
        importBtn.Name = "importBtn";
        importBtn.Size = new Size(70, 25);
        importBtn.TabIndex = 254;
        importBtn.Text = "Import";
        importBtn.Click += ImportBtn_Click;
        // 
        // label18
        // 
        label18.AutoSize = true;
        label18.BackColor = Color.Transparent;
        label18.Font = new Font("Segoe UI Emoji", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
        label18.ForeColor = Color.Gray;
        label18.Location = new Point(184, 220);
        label18.Name = "label18";
        label18.Size = new Size(102, 15);
        label18.TabIndex = 62;
        label18.Text = "Double click to set";
        // 
        // LoadingScreenContainer
        // 
        LoadingScreenContainer.Controls.Add(loadScreen);
        LoadingScreenContainer.Location = new Point(758, 239);
        LoadingScreenContainer.Name = "LoadingScreenContainer";
        LoadingScreenContainer.Padding = new Padding(2);
        LoadingScreenContainer.Size = new Size(355, 223);
        LoadingScreenContainer.TabIndex = 61;
        // 
        // loadScreen
        // 
        loadScreen.Dock = DockStyle.Fill;
        loadScreen.Location = new Point(2, 2);
        loadScreen.Name = "loadScreen";
        loadScreen.Size = new Size(351, 219);
        loadScreen.SizeMode = PictureBoxSizeMode.StretchImage;
        loadScreen.TabIndex = 0;
        loadScreen.TabStop = false;
        loadScreen.DoubleClick += LoadScreen_DoubleClick;
        loadScreen.MouseClick += LoadScreen_MouseClick;
        // 
        // LoadingScreenView
        // 
        LoadingScreenView.BackColor = Color.FromArgb(31, 32, 35);
        LoadingScreenView.BorderStyle = BorderStyle.None;
        LoadingScreenView.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3, columnHeader4 });
        LoadingScreenView.Font = new Font("Segoe UI", 12F);
        LoadingScreenView.ForeColor = Color.Gray;
        LoadingScreenView.FullRowSelect = true;
        LoadingScreenView.HeaderStyle = ColumnHeaderStyle.None;
        LoadingScreenView.HideSelection = true;
        LoadingScreenView.Location = new Point(187, 268);
        LoadingScreenView.MultiSelect = false;
        LoadingScreenView.Name = "LoadingScreenView";
        LoadingScreenView.OwnerDraw = true;
        LoadingScreenView.Size = new Size(552, 204);
        LoadingScreenView.TabIndex = 60;
        LoadingScreenView.UseCompatibleStateImageBehavior = false;
        LoadingScreenView.UseSelectable = true;
        LoadingScreenView.View = View.Details;
        LoadingScreenView.SelectedIndexChanged += LoadingScreenView_SelectedIndexChanged;
        LoadingScreenView.DoubleClick += LoadingScreenView_DoubleClick;
        // 
        // columnHeader1
        // 
        columnHeader1.Text = "     ";
        columnHeader1.Width = 7;
        // 
        // columnHeader2
        // 
        columnHeader2.Text = "Name";
        columnHeader2.Width = 365;
        // 
        // columnHeader3
        // 
        columnHeader3.Text = "ID";
        // 
        // columnHeader4
        // 
        columnHeader4.Text = "Rarity";
        columnHeader4.Width = 103;
        // 
        // CurrentLoadingScreen
        // 
        CurrentLoadingScreen.BackColor = Color.FromArgb(47, 48, 51);
        CurrentLoadingScreen.Location = new Point(34, 241);
        CurrentLoadingScreen.MaximumSize = new Size(128, 124);
        CurrentLoadingScreen.MinimumSize = new Size(128, 124);
        CurrentLoadingScreen.Name = "CurrentLoadingScreen";
        CurrentLoadingScreen.ShowToolTip = true;
        CurrentLoadingScreen.Size = new Size(128, 124);
        CurrentLoadingScreen.Static = true;
        CurrentLoadingScreen.TabIndex = 59;
        // 
        // label17
        // 
        label17.AutoSize = true;
        label17.BackColor = Color.Transparent;
        label17.Font = new Font("Segoe UI Emoji", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        label17.ForeColor = Color.Gray;
        label17.Location = new Point(31, 220);
        label17.Name = "label17";
        label17.Size = new Size(100, 15);
        label17.TabIndex = 58;
        label17.Text = "Loading Screen";
        // 
        // CurrentAnnouncer
        // 
        CurrentAnnouncer.BackColor = Color.FromArgb(47, 48, 51);
        CurrentAnnouncer.Location = new Point(186, 49);
        CurrentAnnouncer.Margin = new Padding(3, 2, 3, 2);
        CurrentAnnouncer.MaximumSize = new Size(128, 124);
        CurrentAnnouncer.MinimumSize = new Size(128, 124);
        CurrentAnnouncer.Name = "CurrentAnnouncer";
        CurrentAnnouncer.ShowToolTip = true;
        CurrentAnnouncer.Size = new Size(128, 124);
        CurrentAnnouncer.Static = true;
        CurrentAnnouncer.TabIndex = 57;
        // 
        // CurrentMusicPack
        // 
        CurrentMusicPack.BackColor = Color.FromArgb(47, 48, 51);
        CurrentMusicPack.Location = new Point(34, 49);
        CurrentMusicPack.Margin = new Padding(3, 2, 3, 2);
        CurrentMusicPack.MaximumSize = new Size(128, 124);
        CurrentMusicPack.MinimumSize = new Size(128, 124);
        CurrentMusicPack.Name = "CurrentMusicPack";
        CurrentMusicPack.ShowToolTip = true;
        CurrentMusicPack.Size = new Size(128, 124);
        CurrentMusicPack.Static = true;
        CurrentMusicPack.TabIndex = 55;
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.BackColor = Color.Transparent;
        label2.Font = new Font("Segoe UI Emoji", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        label2.ForeColor = Color.Gray;
        label2.Location = new Point(184, 28);
        label2.Name = "label2";
        label2.Size = new Size(104, 15);
        label2.TabIndex = 53;
        label2.Text = "Announcer Pack";
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.BackColor = Color.Transparent;
        label1.Font = new Font("Segoe UI Emoji", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
        label1.ForeColor = Color.Gray;
        label1.Location = new Point(31, 28);
        label1.Name = "label1";
        label1.Size = new Size(73, 15);
        label1.TabIndex = 52;
        label1.Text = "Music Pack";
        // 
        // tabPage5
        // 
        tabPage5.BackColor = Color.FromArgb(31, 32, 35);
        tabPage5.Location = new Point(4, 4);
        tabPage5.Name = "tabPage5";
        tabPage5.Padding = new Padding(3);
        tabPage5.Size = new Size(1118, 478);
        tabPage5.TabIndex = 4;
        tabPage5.Text = "RUN";
        // 
        // tabPage6
        // 
        tabPage6.BackColor = Color.FromArgb(31, 32, 35);
        tabPage6.Controls.Add(logo);
        tabPage6.Controls.Add(load);
        tabPage6.Location = new Point(4, 4);
        tabPage6.Name = "tabPage6";
        tabPage6.Padding = new Padding(3);
        tabPage6.Size = new Size(1118, 478);
        tabPage6.TabIndex = 5;
        tabPage6.Text = "Start";
        // 
        // logo
        // 
        logo.Image = Resources.dota_logo_and_text;
        logo.Location = new Point(409, 189);
        logo.Name = "logo";
        logo.Size = new Size(338, 66);
        logo.SizeMode = PictureBoxSizeMode.Zoom;
        logo.TabIndex = 54;
        logo.TabStop = false;
        logo.Click += Logo_Click;
        // 
        // load
        // 
        load.AutoSize = true;
        load.Font = new Font("Segoe UI Emoji", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
        load.ForeColor = SystemColors.ActiveBorder;
        load.Location = new Point(506, 274);
        load.Name = "load";
        load.Size = new Size(0, 16);
        load.TabIndex = 53;
        // 
        // ImageMenu
        // 
        ImageMenu.Font = new Font("Segoe UI", 8F);
        ImageMenu.ForeColor = Color.White;
        ImageMenu.Items.AddRange(new ToolStripItem[] { OpenImageMenu, SaveImageMenu });
        ImageMenu.Name = "ImageMenu";
        ImageMenu.ShowImageMargin = false;
        ImageMenu.Size = new Size(113, 48);
        // 
        // OpenImageMenu
        // 
        OpenImageMenu.Name = "OpenImageMenu";
        OpenImageMenu.Size = new Size(112, 22);
        OpenImageMenu.Text = "Open Image";
        OpenImageMenu.Click += OpenImageMenu_Click;
        // 
        // SaveImageMenu
        // 
        SaveImageMenu.Name = "SaveImageMenu";
        SaveImageMenu.Size = new Size(112, 22);
        SaveImageMenu.Text = "Save Image";
        SaveImageMenu.Click += SaveImageMenu_Click;
        // 
        // frmMain
        // 
        AutoScaleMode = AutoScaleMode.Inherit;
        BackColor = Color.FromArgb(31, 32, 35);
        ClientSize = new Size(1152, 572);
        Controls.Add(ClientVersion);
        Controls.Add(boxInfo);
        Controls.Add(createdBy);
        Controls.Add(tabControl1);
        Controls.Add(panel1);
        FormBorderStyle = FormBorderStyle.None;
        Name = "frmMain";
        StartPosition = FormStartPosition.CenterScreen;
        FormClosing += FrmMain_FormClosing;
        MouseMove += FrmMain_MouseMove;
        panel1.ResumeLayout(false);
        panelWord.ResumeLayout(false);
        panelWord.PerformLayout();
        panelCreateMod.ResumeLayout(false);
        panelCreateMod.PerformLayout();
        panelMisc.ResumeLayout(false);
        panelMisc.PerformLayout();
        panelHeroes.ResumeLayout(false);
        panelHeroes.PerformLayout();
        ((ISupportInitialize)OpenSettings).EndInit();
        ((ISupportInitialize)CloseBtn).EndInit();
        ((ISupportInitialize)banner).EndInit();
        tabControl1.ResumeLayout(false);
        tabPage1.ResumeLayout(false);
        tabPage1.PerformLayout();
        tabPage2.ResumeLayout(false);
        tabPage2.PerformLayout();
        ((ISupportInitialize)picRight).EndInit();
        ((ISupportInitialize)picLeft).EndInit();
        ((ISupportInitialize)HeroAvatar).EndInit();
        tabPage4.ResumeLayout(false);
        WordContainer.ResumeLayout(false);
        WordContainer.PerformLayout();
        tabPage3.ResumeLayout(false);
        tabPage3.PerformLayout();
        panel4.ResumeLayout(false);
        panel4.PerformLayout();
        panel2.ResumeLayout(false);
        panel3.ResumeLayout(false);
        LoadingScreenContainer.ResumeLayout(false);
        ((ISupportInitialize)loadScreen).EndInit();
        tabPage6.ResumeLayout(false);
        tabPage6.PerformLayout();
        ((ISupportInitialize)logo).EndInit();
        ImageMenu.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }
    /// <summary>
    /// Asigna el item a un BundleBox usando esta prioridad:
    /// 1. El item guardado en manager (si existe)
    /// 2. Un item "default" de la categoría
    /// 3. Un item fallback creado manualmente
    /// </summary>
    private void AssignBundleItem(BundleBox box, Items managerItem, ItemCategory category, Items fallback)
    {
        if (managerItem != null)
        {
            box.item = managerItem;
            return;
        }

        // Buscar item con "default" en el nombre
        var defaultItem = Catalog.GetByCategoryFast(category)
            .Find(x => x.name != null && x.name.ToLower().Contains("default"));

        box.item = defaultItem ?? fallback;
    }
    /// <summary>
    /// Registra todos los servicios en el contenedor.
    /// Esta es la única configuración de dependencias de la app.
    /// </summary>
    /// <summary>
    /// Registra todos los servicios en el contenedor.
    /// Esta es la única configuración de dependencias de la app.
    /// 
    /// ORDEN: Los servicios sin dependencias se registran primero,
    /// luego los que dependen de otros.
    /// </summary>
    private void RegisterServices()
    {
        // ════════════════════════════════════════════════════════════
        // CAPA 1: INFRAESTRUCTURA (sin dependencias)
        // ════════════════════════════════════════════════════════════
        ServiceContainer.Register<IUIService>(new UIService());
        ServiceContainer.Register<IAudioService>(new AudioService());
        ServiceContainer.Register<ISettingsService>(new SettingsService());

        // El catálogo es propiedad de frmMain (compartida)
        ServiceContainer.Register<IItemCatalog>(this.Catalog);

        // Validador de cache del VPK
        ServiceContainer.Register<IVpkCacheValidator>(
            new VpkCacheValidator(
                settings: ServiceContainer.Get<ISettingsService>(),
                ui: ServiceContainer.Get<IUIService>()));

        // Reporter de progreso
        ServiceContainer.Register<IProgressReporter>(new ProgressReporter());

        // Configuración del mod del usuario (sin dependencias)
        ServiceContainer.Register<IModConfigurationService>(new ModConfigurationService());

        // Estado de UI (sin dependencias)
        ServiceContainer.Register<IUIState>(new UIState());

        // Héroes (sin dependencias)
        ServiceContainer.Register<IHeroesService>(new HeroesService());

        // 🔑 Prefabs (sin dependencias) - va acá porque no depende de nada
        ServiceContainer.Register<IPrefabsService>(new PrefabsService());

        // 🔑 VPK Service (estado del archivo VPK cargado)
        ServiceContainer.Register<IVpkService>(new VpkService());

        // 🔑 VPK Writer Service (generación de pak01_dir.vpk)
        ServiceContainer.Register<IVpkWriterService>(
            new VpkWriterService(ServiceContainer.Get<IUIService>()));

        // 🔑 Hero Attributes Service (Strength/Agility/Intelligence/Universal)
        ServiceContainer.Register<IHeroAttributesService>(
            new HeroAttributesService(
                ui: ServiceContainer.Get<IUIService>(),
                vpkReader: ServiceContainer.Get<IVpkService>()));

        // 🔑 Hero Video Service (rutas de videos .webm para hover animado)
        ServiceContainer.Register<IHeroVideoService>(new HeroVideoService());

        // ════════════════════════════════════════════════════════════
        // CAPA 2: SERVICIOS DE DATOS (dependen de capa 1)
        // ════════════════════════════════════════════════════════════
        ServiceContainer.Register<IItemsService>(
            new ItemsService(this.Catalog));

        // 🔑 RarityColorService va ACÁ porque depende de IItemsService
        ServiceContainer.Register<IRarityColorService>(
            new RarityColorService(ServiceContainer.Get<IItemsService>()));
        // 🔑 Resource Loader Service (carga items_game.txt, lang files, etc.)
        ServiceContainer.Register<IResourceLoaderService>(
            new ResourceLoaderService(
                catalog: this.Catalog,
                prefabs: ServiceContainer.Get<IPrefabsService>(),
                colors: ServiceContainer.Get<IRarityColorService>(),
                ui: ServiceContainer.Get<IUIService>()));

        ServiceContainer.Register<IResourceService>(
            new ResourceService(
                catalog: this.Catalog,
                ui: ServiceContainer.Get<IUIService>()));

        ServiceContainer.Register<ILoadingScreenService>(
            new LoadingScreenService(
                ui: ServiceContainer.Get<IUIService>()));

        // ════════════════════════════════════════════════════════════
        // CAPA 3: SERVICIOS DE NEGOCIO (dependen de capas inferiores)
        // ════════════════════════════════════════════════════════════
        ServiceContainer.Register<IModBuilderService>(
    new ModBuilderService(
        catalog: this.Catalog,
        ui: ServiceContainer.Get<IUIService>(),
        audio: ServiceContainer.Get<IAudioService>(),
        settings: ServiceContainer.Get<ISettingsService>(),
        items: ServiceContainer.Get<IItemsService>(),
        heroes: ServiceContainer.Get<IHeroesService>(),
        vpkWriter: ServiceContainer.Get<IVpkWriterService>(),
        vpkReader: ServiceContainer.Get<IVpkService>())); // 🔑 NUEVO
        // ════════════════════════════════════════════════════════════
        // 🔑 BUNDLE HANDLERS (Strategy pattern)
        // ════════════════════════════════════════════════════════════
        var audio = ServiceContainer.Get<IAudioService>();
        var catalog = ServiceContainer.Get<IItemCatalog>();
        var modConfig = ServiceContainer.Get<IModConfigurationService>();
        var uiState = ServiceContainer.Get<IUIState>();

        var bundleRegistry = new BundleHandlerRegistry();

        bundleRegistry.Register(new AnnouncerHandler(audio, modConfig, uiState));
        bundleRegistry.Register(new MegaKillHandler(audio, modConfig, uiState));
        bundleRegistry.Register(new MultikillBannerHandler(audio, modConfig, uiState));
        bundleRegistry.Register(new WeatherEffectHandler(audio, modConfig, uiState));
        bundleRegistry.Register(new PetsHandler(audio, modConfig, uiState));
        bundleRegistry.Register(new BundleHandler(audio, modConfig, uiState, catalog));
        bundleRegistry.Register(new TerrainHandler(audio, modConfig, uiState));
        bundleRegistry.Register(new MusicHandler(audio, modConfig, uiState));
        bundleRegistry.Register(new CourierHandler(audio, modConfig, uiState));
        bundleRegistry.Register(new RadiantCreepsHandler(audio, modConfig, uiState));
        bundleRegistry.Register(new DireCreepsHandler(audio, modConfig, uiState));
        bundleRegistry.Register(new RadiantTowersHandler(audio, modConfig, uiState));
        bundleRegistry.Register(new DireTowersHandler(audio, modConfig, uiState));
        bundleRegistry.Register(new VersusScreenHandler(audio, modConfig, uiState));
        bundleRegistry.Register(new CursorPackHandler(audio, modConfig, uiState));
        bundleRegistry.Register(new EmblemHandler(audio, modConfig, uiState));
        bundleRegistry.Register(new EmoticonsHandler(audio, modConfig, uiState));
        bundleRegistry.Register(new HUDSkinHandler(audio, modConfig, uiState));
        bundleRegistry.Register(new WardHandler(audio, modConfig, uiState));
        bundleRegistry.Register(new StreakEffectHandler(audio, modConfig, uiState));
        bundleRegistry.Register(new DefaultItemHandler(audio, modConfig, uiState));

        ServiceContainer.Register<BundleHandlerRegistry>(bundleRegistry);
    }
}
