using System.ComponentModel;
using System.Diagnostics;

namespace SKYNET;

public class BoxItems : UserControl
{
    private IContainer components;

    private Label name;

    private Panel panel2;

    private Panel panel3;

    private Panel panel1;

    private PictureBox Icon;

    private Panel panelRarity;

    public string HostName;

    public string ToDelete = "Items";

    [DefaultValue(null)]
    public string Hero { get; set; }

    private string _Type { get; set; }

    [DefaultValue(null)]
    public string Type
    {
        get
        {
            return _Type;
        }
        set
        {
            _Type = value;
            name.Text = value;
            if (string.IsNullOrEmpty(_Item?.ItemID))
            {
                if (value == "taunt")
                {
                    Icon.Image = Resources.default_taunt;
                }
                else
                {
                    Icon.Image = ItemsManager.GetDefaultImageItem(value, Hero);
                }
            }
        }
    }

    private Items _Item { get; set; }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Items Item
    {
        get
        {
            return _Item;
        }
        set
        {
            _Item = value;
            try
            {
                // 🔑 Usar el servicio para guardar el bundle
                if (_Item != null && !string.IsNullOrEmpty(_Item.used_by_heroes))
                {
                    var heroesService = ServiceContainer.Get<IHeroesService>();
                    heroesService.SetBundle(_Item.used_by_heroes, _Item);
                }

                // Actualizar UI
                if (_Item?.ItemID == "0000")
                {
                    Icon.Image = ItemsManager.GetDefaultImageItem(_Item.item_slot, _Item.used_by_heroes);
                    return;
                }
                Icon.Image = ItemsManager.GetImage(_Item.image_inventory);
                panelRarity.BackColor = modCommon.ColorItemsFromRarity(_Item.ItemID);
            }
            catch (Exception)
            {
            }
        }
    }

    public BoxItems()
    {
        InitializeComponent();
        MaximumSize = new Size(93, 76);
        MinimumSize = new Size(93, 76);
        base.Size = new Size(93, 76);
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
        this.name = new System.Windows.Forms.Label();
        this.panel2 = new System.Windows.Forms.Panel();
        this.panel3 = new System.Windows.Forms.Panel();
        this.panel1 = new System.Windows.Forms.Panel();
        this.Icon = new System.Windows.Forms.PictureBox();
        this.panelRarity = new System.Windows.Forms.Panel();
        this.panel2.SuspendLayout();
        this.panel3.SuspendLayout();
        this.panel1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)this.Icon).BeginInit();
        base.SuspendLayout();
        this.name.AutoSize = true;
        this.name.Font = FontService.GetRadiance(10f);
        this.name.ForeColor = System.Drawing.Color.Silver;
        this.name.Location = new System.Drawing.Point(0, 3);
        this.name.Name = "name";
        this.name.Size = new System.Drawing.Size(35, 12);
        this.name.TabIndex = 1;
        this.name.Text = "Name";
        this.name.Click += new System.EventHandler(Item_Click);
        this.panel2.Controls.Add(this.name);
        this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
        this.panel2.Location = new System.Drawing.Point(0, 0);
        this.panel2.Name = "panel2";
        this.panel2.Padding = new System.Windows.Forms.Padding(1);
        this.panel2.Size = new System.Drawing.Size(114, 19);
        this.panel2.TabIndex = 3;
        this.panel3.Controls.Add(this.panel1);
        this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
        this.panel3.Location = new System.Drawing.Point(0, 19);
        this.panel3.Name = "panel3";
        this.panel3.Padding = new System.Windows.Forms.Padding(2);
        this.panel3.Size = new System.Drawing.Size(114, 77);
        this.panel3.TabIndex = 6;
        this.panel1.BackColor = System.Drawing.Color.FromArgb(31, 32, 35);
        this.panel1.Controls.Add(this.Icon);
        this.panel1.Controls.Add(this.panelRarity);
        this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.panel1.Location = new System.Drawing.Point(2, 2);
        this.panel1.Name = "panel1";
        this.panel1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
        this.panel1.Size = new System.Drawing.Size(110, 73);
        this.panel1.TabIndex = 2;
        this.Icon.BackColor = System.Drawing.Color.FromArgb(31, 32, 35);
        this.Icon.Dock = System.Windows.Forms.DockStyle.Fill;
        this.Icon.Location = new System.Drawing.Point(0, 0);
        this.Icon.Name = "Icon";
        this.Icon.Size = new System.Drawing.Size(110, 70);
        this.Icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
        this.Icon.TabIndex = 2;
        this.Icon.TabStop = false;
        this.Icon.Click += new System.EventHandler(Item_Click);
        this.panelRarity.Dock = System.Windows.Forms.DockStyle.Bottom;
        this.panelRarity.Location = new System.Drawing.Point(0, 70);
        this.panelRarity.Name = "panelRarity";
        this.panelRarity.Size = new System.Drawing.Size(110, 1);
        this.panelRarity.TabIndex = 1;
        base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
        this.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
        base.Controls.Add(this.panel3);
        base.Controls.Add(this.panel2);
        base.Name = "BoxItems";
        base.Size = new System.Drawing.Size(114, 96);
        base.Load += new System.EventHandler(BoxTool_Load);
        base.Click += new System.EventHandler(Item_Click);
        this.panel2.ResumeLayout(false);
        this.panel2.PerformLayout();
        this.panel3.ResumeLayout(false);
        this.panel1.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)this.Icon).EndInit();
        base.ResumeLayout(false);
    }

    private void BoxTool_Load(object sender, EventArgs e)
    {
    }

    private void Item_Click(object sender, EventArgs e)
    {
        FrmItems frmItems2 = new FrmItems(Item);
        DialogResult dialogResult = frmItems2.ShowDialog();
        if (dialogResult == DialogResult.OK)
        {
            Item = modCommon.ItemIDSelected;
        }
    }
}