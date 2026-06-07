using System.ComponentModel;
using System.Diagnostics;

namespace SKYNET;

public class BundleBox : UserControl
{
    private IContainer components;

    private PictureBox Icon;

    private Panel panel1;

    private Label name;

    private Label rarity;

    private Label label1;

    private Label Styles;

    private ToolTip toolTip1;

    private frmTools Tools;

    private bool _ShowToolTip;

    public string ToDelete = "Items";

    public bool isTool = false;

    private bool _Static { get; set; }

    [DefaultValue(false)]
    public bool Static
    {
        get
        {
            return _Static;
        }
        set
        {
            _Static = value;
        }
    }

    private Items _Item { get; set; }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Items item
    {
        get
        {
            return _Item;
        }
        set
        {
            _Item = value;
            WasReset = false; // 🔑 Al asignar nuevo item, dejar de estar en estado "reset"
            try
            {
                name.Text = _Item?.name;
                if (!_Static)
                {
                    Items items = _Item;
                    if (items != null && items.styles.Count > 0)
                    {
                        Styles.Text = _Item?.styles.Count.ToString() + " STYLES";
                    }
                }
                rarity.Text = modCommon.FirstUpper(_Item?.item_rarity);
                rarity.ForeColor = modCommon.ColorBundleFromRarity(_Item?.ItemID);
                panel1.BackColor = modCommon.ColorBundleFromRarity(_Item?.ItemID);
                Icon.Image = ItemsManager.GetImage(_Item?.image_inventory);
            }
            catch
            {
                Icon.Image = modCommon.DefaultIcon;
            }
        }
    }

    [DefaultValue(false)]
    public bool ShowToolTip
    {
        get
        {
            return _ShowToolTip;
        }
        set
        {
            _ShowToolTip = value;
        }
    }
    /// <summary>
    /// Indica si el BundleBox fue reseteado por el usuario.
    /// En ese estado, el click siempre abre el selector usando el prefab original.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool WasReset { get; private set; }

    public BundleBox()
    {
        InitializeComponent();
        MaximumSize = new Size(128, 124);
        MinimumSize = new Size(128, 124);
        base.Size = new Size(128, 124);
    }

    public BundleBox(frmTools frmtools)
    {
        InitializeComponent();
        Tools = frmtools;
    }

    public void Reset()
    {
        // 🔑 Preservar prefab Y item_slot para que los handlers puedan
        // identificar correctamente la categoría (algunos filtran por item_slot)
        string preservedPrefab = item?.prefab ?? "";
        string preservedItemSlot = item?.item_slot ?? "";
        string preservedImageInventory = item?.image_inventory ?? "";

        _Item = new Items
        {
            prefab = preservedPrefab,
            item_slot = preservedItemSlot,
            image_inventory = preservedImageInventory
        };

        // 🔑 Marcar como reseteado
        WasReset = true;

        name.Text = "Reset by user";
        rarity.Text = "";
        panel1.BackColor = Color.FromArgb(57, 58, 61); // color de fondo default
        Icon.Image = Resources.default_item;
    }

    [DebuggerNonUserCode]
    protected override void Dispose(bool disposing)
    {
        try
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
        }
        finally
        {
            base.Dispose(disposing);
        }
    }

    [System.Diagnostics.DebuggerStepThrough]
    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.panel1 = new System.Windows.Forms.Panel();
        this.Styles = new System.Windows.Forms.Label();
        this.Icon = new System.Windows.Forms.PictureBox();
        this.name = new System.Windows.Forms.Label();
        this.rarity = new System.Windows.Forms.Label();
        this.label1 = new System.Windows.Forms.Label();
        this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
        this.panel1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)this.Icon).BeginInit();
        base.SuspendLayout();
        this.panel1.Controls.Add(this.Styles);
        this.panel1.Controls.Add(this.Icon);
        this.panel1.Location = new System.Drawing.Point(3, 4);
        this.panel1.Name = "panel1";
        this.panel1.Padding = new System.Windows.Forms.Padding(1);
        this.panel1.Size = new System.Drawing.Size(122, 85);
        this.panel1.TabIndex = 2;
        this.Styles.AutoSize = true;
        this.Styles.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
        this.Styles.Font = new System.Drawing.Font("Verdana", 6.75f, System.Drawing.FontStyle.Bold);
        this.Styles.ForeColor = System.Drawing.Color.White;
        this.Styles.Location = new System.Drawing.Point(3, 50);
        this.Styles.Name = "Styles";
        this.Styles.Size = new System.Drawing.Size(0, 12);
        this.Styles.TabIndex = 5;
        this.Styles.Click += new System.EventHandler(Item_Click);
        this.Icon.Dock = System.Windows.Forms.DockStyle.Fill;
        this.Icon.Location = new System.Drawing.Point(1, 1);
        this.Icon.Name = "Icon";
        this.Icon.Size = new System.Drawing.Size(120, 83);
        this.Icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
        this.Icon.TabIndex = 0;
        this.Icon.TabStop = false;
        this.Icon.Click += new System.EventHandler(Item_Click);
        this.Icon.MouseLeave += new System.EventHandler(Icon_MouseLeave);
        this.Icon.MouseHover += new System.EventHandler(Icon_MouseHover);
        this.name.AutoSize = true;
        this.name.Font = new System.Drawing.Font("Segoe UI Emoji", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
        this.name.ForeColor = System.Drawing.SystemColors.ControlDark;
        this.name.Location = new System.Drawing.Point(0, 91);
        this.name.MaximumSize = new System.Drawing.Size(110, 16);
        this.name.Name = "name";
        this.name.Size = new System.Drawing.Size(70, 15);
        this.name.TabIndex = 2;
        this.name.Text = "Default item";
        this.name.Click += new System.EventHandler(Item_Click);
        this.rarity.AutoSize = true;
        this.rarity.Font = new System.Drawing.Font("Segoe UI Emoji", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
        this.rarity.ForeColor = System.Drawing.SystemColors.ControlDark;
        this.rarity.Location = new System.Drawing.Point(35, 105);
        this.rarity.Margin = new System.Windows.Forms.Padding(0);
        this.rarity.Name = "rarity";
        this.rarity.Size = new System.Drawing.Size(0, 15);
        this.rarity.TabIndex = 3;
        this.label1.AutoSize = true;
        this.label1.Font = new System.Drawing.Font("Segoe UI Emoji", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
        this.label1.ForeColor = System.Drawing.SystemColors.ControlDark;
        this.label1.Location = new System.Drawing.Point(1, 105);
        this.label1.Margin = new System.Windows.Forms.Padding(0);
        this.label1.Name = "label1";
        this.label1.Size = new System.Drawing.Size(44, 15);
        this.label1.TabIndex = 4;
        this.label1.Text = "Rareza:";
        base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
        this.BackColor = System.Drawing.Color.FromArgb(57, 58, 61);
        base.Controls.Add(this.label1);
        base.Controls.Add(this.rarity);
        base.Controls.Add(this.name);
        base.Controls.Add(this.panel1);
        this.MaximumSize = new System.Drawing.Size(128, 124);
        this.MinimumSize = new System.Drawing.Size(128, 124);
        base.Name = "BundleBox";
        base.Size = new System.Drawing.Size(128, 124);
        base.Load += new System.EventHandler(BoxTool_Load);
        base.Click += new System.EventHandler(Item_Click);
        this.panel1.ResumeLayout(false);
        this.panel1.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)this.Icon).EndInit();
        base.ResumeLayout(false);
        base.PerformLayout();
    }

    private void BoxTool_Load(object sender, EventArgs e)
    {
    }

    private void Item_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(Styles.Text))
        {
            frmMain.frm.Bundle_Click(this);
            frmTools.frm?.Close();
        }
        else
        {
            Tools.LoadStyles(item);
        }
    }

    private void Icon_MouseHover(object sender, EventArgs e)
    {
        if (ShowToolTip)
        {
            frmMain.SetBoxInfo(this);
        }
    }

    private void Icon_MouseLeave(object sender, EventArgs e)
    {
        frmMain.frm.boxInfo.Visible = false;
    }
}