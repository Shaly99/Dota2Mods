using System.ComponentModel;
using System.Diagnostics;

namespace SKYNET;

public class ItemControl : UserControl
{
    private IContainer components;

    private PictureBox Icon;

    private Label Styles;

    private IItemHost host;

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
        }
    }

    public ItemControl(IItemHost frm)
    {
        InitializeComponent();
        host = frm;
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
        this.Icon = new System.Windows.Forms.PictureBox();
        this.Styles = new System.Windows.Forms.Label();
        ((System.ComponentModel.ISupportInitialize)this.Icon).BeginInit();
        base.SuspendLayout();
        this.Icon.Dock = System.Windows.Forms.DockStyle.Fill;
        this.Icon.Location = new System.Drawing.Point(1, 1);
        this.Icon.Name = "Icon";
        this.Icon.Size = new System.Drawing.Size(91, 65);
        this.Icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
        this.Icon.TabIndex = 1;
        this.Icon.TabStop = false;
        this.Icon.Click += new System.EventHandler(Icon_Click);
        this.Icon.MouseMove += new System.Windows.Forms.MouseEventHandler(Icon_MouseMove);
        this.Styles.AutoSize = true;
        this.Styles.BackColor = System.Drawing.Color.FromArgb(41, 42, 45);
        this.Styles.Font = new System.Drawing.Font("Verdana", 6.75f, System.Drawing.FontStyle.Bold);
        this.Styles.ForeColor = System.Drawing.Color.White;
        this.Styles.Location = new System.Drawing.Point(3, 52);
        this.Styles.Name = "Styles";
        this.Styles.Size = new System.Drawing.Size(0, 12);
        this.Styles.TabIndex = 2;
        this.Styles.Click += new System.EventHandler(Icon_Click);
        base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
        this.BackColor = System.Drawing.Color.FromArgb(31, 32, 35);
        base.Controls.Add(this.Styles);
        base.Controls.Add(this.Icon);
        base.Margin = new System.Windows.Forms.Padding(0);
        base.Name = "ItemControl";
        base.Padding = new System.Windows.Forms.Padding(1);
        base.Size = new System.Drawing.Size(93, 67);
        base.Load += new System.EventHandler(BoxTool_Load);
        base.Click += new System.EventHandler(Item_Click);
        ((System.ComponentModel.ISupportInitialize)this.Icon).EndInit();
        base.ResumeLayout(false);
        base.PerformLayout();
    }

    private void BoxTool_Load(object sender, EventArgs e)
    {
        Styles.Parent = Icon;
        try
        {
            if (Item.styles.Count > 0)
            {
                Styles.Text = Item.styles.Count + " STYLES";
            }
            if (Item.ItemID == "0000")
            {
                Icon.Image = ItemsManager.GetDefaultImageItem(Item.item_slot, Item.used_by_heroes);
                return;
            }
            BackColor = modCommon.ColorItemsFromRarity(Item.ItemID);
            Icon.Image = ItemsManager.GetImage(Item.image_inventory);
        }
        catch (Exception)
        {
            Icon.Image = Resources.default_item;
        }
    }

    private void Item_Click(object sender, EventArgs e)
    {
    }

    private void Icon_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(Styles.Text))
        {
            modCommon.ItemIDSelected = Item;
            try
            {
                host.GetOkButton().PerformClick();
                return;
            }
            catch
            {
                return;
            }
        }
        host.LoadStyles(Item);
    }

    private void Icon_MouseMove(object sender, MouseEventArgs e)
    {
        try
        {
        }
        catch
        {
        }
    }
}