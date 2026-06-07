using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SKYNET;

public class FrmItems : Form, IItemHost
{
    public static FrmItems frm;

    private Items Item;

    private IContainer components = null;

    private Button acceptBtn;

    public Button ok;

    private MetroPanel ItemContainer;

    // Hook global para detectar clicks fuera de la ventana
    private const int WH_MOUSE_LL = 14;
    private const int WM_LBUTTONDOWN = 0x0201;
    private const int WM_RBUTTONDOWN = 0x0204;

    // IMPORTANTE: mantener referencia estática para que el GC no lo libere
    private static LowLevelMouseProc _mouseProcStatic;
    private LowLevelMouseProc _mouseProc;
    private IntPtr _mouseHookID = IntPtr.Zero;

    private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MSLLHOOKSTRUCT
    {
        public POINT pt;
        public uint mouseData;
        public uint flags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    public FrmItems(Items item)
    {
        InitializeComponent();
        FontService.ApplyRadianceToForm(this);
        Item = item;
        frm = this;

        // 🔑 IMPORTANTE: para que el form reciba teclas aunque haya hijos con foco
        this.KeyPreview = true;

        LoadItems(item.used_by_heroes, item.item_slot, item.prefab);
        Icon cursor = Resources.cursor;
        Cursor = new Cursor(cursor.Handle);
    }

    // 🔑 Implementación de IItemHost
    public Button GetOkButton() => ok;

    public void LoadStyles(Items item)
    {
        ItemContainer.Controls.Clear();
        List<Items> list = new List<Items>();
        for (int i = 0; i < item.styles.Count; i++)
        {
            string text = "";
            text = ((!string.IsNullOrEmpty(item.styles[i].icon_path)) ? item.styles[i].icon_path : item.image_inventory);
            Items item2 = new Items
            {
                ItemID = item.ItemID,
                name = item.name,
                prefab = item.prefab,
                style = item.styles[i],
                image_inventory = text,
                item_description = item.item_description,
                item_name = item.item_name,
                item_rarity = item.item_rarity,
                item_slot = item.item_slot,
                item_type_name = item.item_type_name,
                used_by_heroes = item.used_by_heroes
            };
            list.Add(item2);
        }

        // Posicionar items en grid de 4 columnas
        const int COLUMNS = 4;
        const int ITEM_WIDTH = 106;
        const int ITEM_HEIGHT = 80;
        const int SPACING_X = 9;
        const int SPACING_Y = 12;
        const int START_X = 5;
        const int START_Y = 5;

        for (int i = 0; i < list.Count; i++)
        {
            int row = i / COLUMNS;
            int col = i % COLUMNS;

            int x = START_X + col * (ITEM_WIDTH + SPACING_X);
            int y = START_Y + row * (ITEM_HEIGHT + SPACING_Y);

            ItemContainer.Controls.Add(new ItemControl(this)
            {
                Item = list[i],
                Size = new Size(ITEM_WIDTH, ITEM_HEIGHT),
                Location = new Point(x, y)
            });
        }

        SetSize();
    }

    private void LoadItems(string Hero, string Slot, string prefab)
    {
        try
        {
            List<Items> list = (prefab == "taunt")
                ? frmMain.frm.Catalog.GetByCategoryFast(ItemCategory.Taunts)
                    .FindAll(i => i.used_by_heroes == Hero)
                : frmMain.frm.Catalog.GetByCategoryFast(ItemCategory.Items)
                    .FindAll(i => i.used_by_heroes == Hero && i.item_slot == Slot);

            list.Sort((Items s1, Items s2) => s2.ItemID.CompareTo(s1.ItemID));
            list.Add(new Items
            {
                name = "Default item",
                ItemID = "0000",
                item_slot = Slot,
                prefab = prefab,
                used_by_heroes = Hero
            });

            // Posicionar items en grid de 4 columnas
            const int COLUMNS = 4;
            const int ITEM_WIDTH = 106;
            const int ITEM_HEIGHT = 80;
            const int SPACING_X = 9;
            const int SPACING_Y = 12;
            const int START_X = 5;
            const int START_Y = 5;

            for (int i = 0; i < list.Count; i++)
            {
                int row = i / COLUMNS;
                int col = i % COLUMNS;

                int x = START_X + col * (ITEM_WIDTH + SPACING_X);
                int y = START_Y + row * (ITEM_HEIGHT + SPACING_Y);

                ItemContainer.Controls.Add(new ItemControl(this)
                {
                    Name = Hero,
                    Item = list[i],
                    Size = new Size(ITEM_WIDTH, ITEM_HEIGHT),
                    Location = new Point(x, y)
                });
            }

            for (int i = 0; i < ItemContainer.Controls.Count; i++)
            {
                ItemContainer.Controls[i].Click += Item_Click;
            }
        }
        catch
        {
        }
        SetSize();
    }

    private void Item_Click(object sender, EventArgs e)
    {
    }

    private void frmMessage_Activated(object sender, EventArgs e)
    {
        // 🔑 Instalamos el hook cuando la ventana se activa
        InstallMouseHook();
    }

    private void frmMessage_Deactivate(object sender, EventArgs e)
    {
        // Desinstalar el hook ANTES de cerrar, para evitar reentrancia
        UninstallMouseHook();

        // Si la ventana pierde foco (cambio a otra app), también cerramos
        if (!this.IsDisposed)
        {
            Close();
        }
    }

    private void FrmItems_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }
    }

    private void SetSize()
    {
        // Configuración del grid: 4 columnas × hasta 4 filas visibles
        const int COLUMNS = 4;
        const int MAX_VISIBLE_ROWS = 4;
        const int ITEM_WIDTH = 106;
        const int ITEM_HEIGHT = 80;
        const int ITEM_SPACING_X = 9;
        const int ITEM_SPACING_Y = 12;
        const int FORM_PADDING = 12;

        int itemCount = ItemContainer.Controls.Count;

        // Calcular cuántas filas necesitamos en total
        int totalRows = (int)Math.Ceiling((double)itemCount / COLUMNS);

        // Cuántas filas mostrar (máximo MAX_VISIBLE_ROWS, después aparece scroll)
        int visibleRows = Math.Min(totalRows, MAX_VISIBLE_ROWS);
        if (visibleRows < 1) visibleRows = 1;

        // Calcular dimensiones del form
        int formWidth = (COLUMNS * ITEM_WIDTH) + ((COLUMNS + 1) * ITEM_SPACING_X) + FORM_PADDING;
        int formHeight = (visibleRows * ITEM_HEIGHT) + ((visibleRows + 1) * ITEM_SPACING_Y) + FORM_PADDING;

        // Si hay más filas de las visibles, dejamos espacio para el scrollbar
        if (totalRows > MAX_VISIBLE_ROWS)
        {
            formWidth += 12; // ancho del scrollbar vertical
        }

        // Aplicar el tamaño
        MaximumSize = new Size(formWidth, formHeight);
        MinimumSize = new Size(formWidth, formHeight);
        base.Size = new Size(formWidth, formHeight);
    }

    private void FrmItems_Load(object sender, EventArgs e)
    {
    }

    private void TextBox1_KeyDown(object sender, KeyEventArgs e)
    {
        modCommon.Show("");
    }

    // ═══════════════════════════════════════════════════════
    // 🔑 HOOK DE MOUSE PARA DETECTAR CLICK FUERA DE LA VENTANA
    // ═══════════════════════════════════════════════════════

    private void InstallMouseHook()
    {
        if (_mouseHookID != IntPtr.Zero)
            return; // Ya está instalado

        try
        {
            _mouseProc = HookCallback;
            _mouseProcStatic = _mouseProc; // 🔑 Referencia estática para evitar GC

            using (var curProcess = System.Diagnostics.Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                IntPtr moduleHandle = GetModuleHandle(curModule.ModuleName);
                _mouseHookID = SetWindowsHookEx(WH_MOUSE_LL, _mouseProc, moduleHandle, 0);

                if (_mouseHookID == IntPtr.Zero)
                {
                    // Falló silenciosamente, no es crítico
                    _mouseProcStatic = null;
                }
            }
        }
        catch
        {
            // Si falla la instalación, no romper la app
            _mouseHookID = IntPtr.Zero;
            _mouseProcStatic = null;
        }
    }

    private void UninstallMouseHook()
    {
        try
        {
            if (_mouseHookID != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_mouseHookID);
                _mouseHookID = IntPtr.Zero;
            }
            _mouseProcStatic = null;
            _mouseProc = null;
        }
        catch
        {
            // Nunca crashear al desinstalar
        }
    }

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        try
        {
            // Verificar si la ventana sigue viva
            if (this.IsDisposed || !this.IsHandleCreated)
            {
                return CallNextHookEx(_mouseHookID, nCode, wParam, lParam);
            }

            if (nCode >= 0 && (wParam == (IntPtr)WM_LBUTTONDOWN || wParam == (IntPtr)WM_RBUTTONDOWN))
            {
                MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                Point clickPoint = new Point(hookStruct.pt.x, hookStruct.pt.y);

                // Verificamos si el click fue FUERA de esta ventana
                if (!this.Bounds.Contains(clickPoint))
                {
                    // Cerramos la ventana en el hilo de UI de forma segura
                    if (this.IsHandleCreated && !this.IsDisposed)
                    {
                        this.BeginInvoke((Action)(() =>
                        {
                            try
                            {
                                if (!this.IsDisposed)
                                {
                                    this.DialogResult = DialogResult.Cancel;
                                    Close();
                                }
                            }
                            catch
                            {
                                // Ignorar si ya se está cerrando
                            }
                        }));
                    }

                    // 🔑 CLAVE: consumir el click para que no se propague
                    // Esto evita el sonido "ding" de Windows
                    return (IntPtr)1;
                }
            }
        }
        catch
        {
            // Nunca dejar que una excepción del hook crashee la app
        }

        return CallNextHookEx(_mouseHookID, nCode, wParam, lParam);
    }

    protected override void Dispose(bool disposing)
    {
        // 🔑 IMPORTANTE: desinstalar el hook al cerrar
        UninstallMouseHook();

        if (disposing && components != null)
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.acceptBtn = new System.Windows.Forms.Button();
        this.ok = new System.Windows.Forms.Button();
        this.ItemContainer = new MetroPanel();
        base.SuspendLayout();
        this.acceptBtn.BackColor = System.Drawing.Color.FromArgb(47, 48, 51);
        this.acceptBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
        this.acceptBtn.Location = new System.Drawing.Point(485, 375);
        this.acceptBtn.Name = "acceptBtn";
        this.acceptBtn.Size = new System.Drawing.Size(75, 23);
        this.acceptBtn.TabIndex = 16;
        this.acceptBtn.Text = "button1";
        this.acceptBtn.UseVisualStyleBackColor = false;
        this.ok.DialogResult = System.Windows.Forms.DialogResult.OK;
        this.ok.Location = new System.Drawing.Point(483, 145);
        this.ok.Name = "ok";
        this.ok.Size = new System.Drawing.Size(18, 23);
        this.ok.TabIndex = 24;
        this.ok.Text = "ok";
        this.ok.UseVisualStyleBackColor = true;
        this.ItemContainer.AutoScroll = true;
        this.ItemContainer.Dock = System.Windows.Forms.DockStyle.Fill;
        this.ItemContainer.HorizontalScrollbar = true;
        this.ItemContainer.HorizontalScrollbarBarColor = true;
        this.ItemContainer.HorizontalScrollbarHighlightOnWheel = false;
        this.ItemContainer.HorizontalScrollbarSize = 10;
        this.ItemContainer.Location = new System.Drawing.Point(3, 3);
        this.ItemContainer.Margin = new System.Windows.Forms.Padding(3, 3, 3, 6);
        this.ItemContainer.Name = "ItemContainer";
        this.ItemContainer.Size = new System.Drawing.Size(339, 319);
        this.ItemContainer.TabIndex = 25;
        this.ItemContainer.UseSelectable = false;
        this.ItemContainer.VerticalScrollbar = true;
        this.ItemContainer.VerticalScrollbarBarColor = true;
        this.ItemContainer.VerticalScrollbarHighlightOnWheel = false;
        this.ItemContainer.VerticalScrollbarSize = 10;
        base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
        this.BackColor = System.Drawing.Color.FromArgb(47, 48, 51);
        base.ClientSize = new System.Drawing.Size(345, 325);
        base.Controls.Add(this.ItemContainer);
        base.Controls.Add(this.ok);
        base.Controls.Add(this.acceptBtn);
        this.Font = new System.Drawing.Font("Segoe UI Emoji", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
        base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        base.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
        base.Name = "frmItems";
        base.Padding = new System.Windows.Forms.Padding(3);
        base.ShowIcon = false;
        base.ShowInTaskbar = false;
        base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "Message";
        base.Activated += new System.EventHandler(frmMessage_Activated);
        base.Deactivate += new System.EventHandler(frmMessage_Deactivate);
        base.Load += new System.EventHandler(FrmItems_Load);
        base.KeyDown += new System.Windows.Forms.KeyEventHandler(FrmItems_KeyDown);
        base.ResumeLayout(false);
    }
}