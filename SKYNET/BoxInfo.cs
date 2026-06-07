using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;

namespace SKYNET;

public class BoxInfo : UserControl
{
    private IContainer components;

    private Panel Content;

    private Panel panel1;

    private Panel RarityPanel;

    public Label item_name;

    private Panel panel3;

    private PictureBox HeroAvatar;

    private Panel panel4;

    private Label labelRarity;

    public Label Type;

    public Label item_slot;

    public Label item_rarity;

    private Panel panel2;

    public TextBox item_description;

    private frmTools Tools;

    public string ToDelete = "Items";

    public bool isTool = false;

    private bool _Static { get; set; }

    [DefaultValue(false)]
    public bool Static
    {
        get { return _Static; }
        set { _Static = value; }
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

            // 🔑 Protección: si los controles internos no están inicializados aún, salimos
            if (Type == null || item_name == null || item_rarity == null
                || item_slot == null || item_description == null
                || HeroAvatar == null || RarityPanel == null || labelRarity == null)
            {
                return;
            }

            try
            {
                string text = "";
                if (_Item?.item_slot != null)
                {
                    text = _Item.item_slot;
                    Type.Text = "Espacio:";
                }
                else if (_Item?.prefab != null)
                {
                    text = modCommon.PreparePrefab(_Item.prefab);
                    Type.Text = "Tipo:";
                }

                item_description.Text = "";
                labelRarity.Text = "Rareza:";

                // Nombre del item con protección contra null
                string description = ItemsManager.GetDescription(_Item?.item_name) ?? "";

                if (string.IsNullOrEmpty(description))
                {
                    description = _Item?.name ?? "Sin descripción";
                }
                else if (description.StartsWith("#"))
                {
                    description = _Item?.name ?? description;
                }
                else
                {
                    description = ProcessName(description);
                }

                item_name.Text = description;

                // Rareza con protección
                item_rarity.Text = modCommon.FirstUpper(_Item?.item_rarity ?? "");
                item_rarity.ForeColor = modCommon.ColorBundleFromRarity(_Item?.ItemID);
                RarityPanel.BackColor = modCommon.ColorBundleFromRarity(_Item?.ItemID);

                // Imagen del héroe o del item
                if (string.IsNullOrEmpty(_Item?.used_by_heroes))
                {
                    HeroAvatar.Image = ItemsManager.GetImage(_Item?.image_inventory ?? "");
                }
                else
                {
                    HeroAvatar.Image = ItemsManager.GetImage("heroes/" + _Item.used_by_heroes);
                }

                item_slot.Text = text;

                // 🔑 PROTECCIÓN CONTRA EL CRASH ACTUAL
                // _Item?.item_description.ToLower() crashea si item_description es null
                string itemDesc = _Item?.item_description ?? "";
                string descriptionText = ItemsManager.GetDescription(itemDesc.ToLower());
                item_description.Text = modCommon.ConvertHtmlTotext(descriptionText ?? "");

                SetVisible = true;
            }
            catch (Exception ex)
            {
                // Loggear el error real para diagnóstico
                try { modCommon.Save(ex); } catch { }

                // Protección extra: si algo más falla, no crashear
                try
                {
                    if (labelRarity != null) labelRarity.Text = "";
                    if (Type != null) Type.Text = "";
                    if (item_name != null) item_name.Text = "Sin descripción";
                    if (item_rarity != null) item_rarity.Text = "";
                    if (RarityPanel != null) RarityPanel.BackColor = Color.Red;
                    if (HeroAvatar != null) HeroAvatar.Image = Resources.default_item;
                    if (item_slot != null) item_slot.Text = "";
                    if (item_description != null) item_description.Text = "";
                    SetVisible = false;
                }
                catch { }
            }
        }
    }

    public bool SetVisible { get; private set; }

    public BoxInfo()
    {
        InitializeComponent();
    }

    public BoxInfo(frmTools frmtools)
    {
        InitializeComponent();
        Tools = frmtools;
    }

    private static string HtmlToPlainText(string html)
    {
        Regex regex = new Regex("<(br|BR)\\s{0,1}\\/{0,1}>", RegexOptions.Multiline);
        Regex regex2 = new Regex("<[^>]*(>|$)", RegexOptions.Multiline);
        Regex regex3 = new Regex("(>|$)(\\W|\\n|\\r)+<", RegexOptions.Multiline);
        string value = html;
        value = WebUtility.HtmlDecode(value);
        value = regex3.Replace(value, "><");
        value = regex.Replace(value, Environment.NewLine);
        return regex2.Replace(value, string.Empty);
    }

    private string ProcessName(string itemName)
    {
        if (string.IsNullOrEmpty(itemName)) return "";

        string text = "";
        int num = 0;
        string[] array = itemName.Split(' ');
        bool flag = true;
        string[] array2 = array;
        foreach (string text2 in array2)
        {
            if (!flag)
            {
                text = text + text2 + " ";
            }
            if (num + text2.Length < 25 && flag)
            {
                text = text + text2 + " ";
                num += text2.Length;
                continue;
            }
            if (flag)
            {
                text = text + Environment.NewLine + text2 + " ";
            }
            flag = false;
        }
        return text;
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
        this.Content = new System.Windows.Forms.Panel();
        this.panel4 = new System.Windows.Forms.Panel();
        this.panel3 = new System.Windows.Forms.Panel();
        this.item_slot = new System.Windows.Forms.Label();
        this.item_rarity = new System.Windows.Forms.Label();
        this.Type = new System.Windows.Forms.Label();
        this.labelRarity = new System.Windows.Forms.Label();
        this.HeroAvatar = new System.Windows.Forms.PictureBox();
        this.RarityPanel = new System.Windows.Forms.Panel();
        this.panel2 = new System.Windows.Forms.Panel();
        this.item_description = new System.Windows.Forms.TextBox();
        this.panel1 = new System.Windows.Forms.Panel();
        this.item_name = new System.Windows.Forms.Label();
        this.Content.SuspendLayout();
        this.panel3.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)this.HeroAvatar).BeginInit();
        this.panel2.SuspendLayout();
        this.panel1.SuspendLayout();
        base.SuspendLayout();
        this.Content.BackColor = System.Drawing.Color.FromArgb(47, 48, 51);
        this.Content.Controls.Add(this.panel4);
        this.Content.Controls.Add(this.panel3);
        this.Content.Controls.Add(this.RarityPanel);
        this.Content.Controls.Add(this.panel2);
        this.Content.Controls.Add(this.panel1);
        this.Content.Dock = System.Windows.Forms.DockStyle.Fill;
        this.Content.Location = new System.Drawing.Point(1, 1);
        this.Content.Name = "Content";
        this.Content.Size = new System.Drawing.Size(147, 160);
        this.Content.TabIndex = 0;
        this.Content.MouseMove += new System.Windows.Forms.MouseEventHandler(Content_MouseMove);
        this.panel4.BackColor = System.Drawing.Color.Black;
        this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
        this.panel4.Location = new System.Drawing.Point(0, 63);
        this.panel4.Name = "panel4";
        this.panel4.Size = new System.Drawing.Size(147, 2);
        this.panel4.TabIndex = 5;
        this.panel3.Controls.Add(this.item_slot);
        this.panel3.Controls.Add(this.item_rarity);
        this.panel3.Controls.Add(this.Type);
        this.panel3.Controls.Add(this.labelRarity);
        this.panel3.Controls.Add(this.HeroAvatar);
        this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
        this.panel3.Location = new System.Drawing.Point(0, 35);
        this.panel3.Name = "panel3";
        this.panel3.Size = new System.Drawing.Size(147, 28);
        this.panel3.TabIndex = 4;
        this.item_slot.AutoSize = true;
        this.item_slot.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
        this.item_slot.ForeColor = System.Drawing.Color.FromArgb(223, 223, 222);
        this.item_slot.Location = new System.Drawing.Point(90, 13);
        this.item_slot.Name = "item_slot";
        this.item_slot.Size = new System.Drawing.Size(27, 12);
        this.item_slot.TabIndex = 8;
        this.item_slot.Text = "Head";
        this.item_rarity.AutoSize = true;
        this.item_rarity.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
        this.item_rarity.ForeColor = System.Drawing.Color.FromArgb(223, 223, 222);
        this.item_rarity.Location = new System.Drawing.Point(90, 1);
        this.item_rarity.Name = "item_rarity";
        this.item_rarity.Size = new System.Drawing.Size(39, 12);
        this.item_rarity.TabIndex = 7;
        this.item_rarity.Text = "Inmortal";
        this.Type.AutoSize = true;
        this.Type.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
        this.Type.ForeColor = System.Drawing.Color.FromArgb(223, 223, 222);
        this.Type.Location = new System.Drawing.Point(49, 13);
        this.Type.Name = "Type";
        this.Type.Size = new System.Drawing.Size(41, 12);
        this.Type.TabIndex = 6;
        this.Type.Text = "Espacio:";
        this.labelRarity.AutoSize = true;
        this.labelRarity.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
        this.labelRarity.ForeColor = System.Drawing.Color.FromArgb(223, 223, 222);
        this.labelRarity.Location = new System.Drawing.Point(49, 1);
        this.labelRarity.Name = "labelRarity";
        this.labelRarity.Size = new System.Drawing.Size(38, 12);
        this.labelRarity.TabIndex = 5;
        this.labelRarity.Text = "Rareza:";
        this.HeroAvatar.Dock = System.Windows.Forms.DockStyle.Left;
        this.HeroAvatar.Location = new System.Drawing.Point(0, 0);
        this.HeroAvatar.Name = "HeroAvatar";
        this.HeroAvatar.Size = new System.Drawing.Size(46, 28);
        this.HeroAvatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
        this.HeroAvatar.TabIndex = 0;
        this.HeroAvatar.TabStop = false;
        this.RarityPanel.BackColor = System.Drawing.Color.Lime;
        this.RarityPanel.Dock = System.Windows.Forms.DockStyle.Top;
        this.RarityPanel.Location = new System.Drawing.Point(0, 33);
        this.RarityPanel.Name = "RarityPanel";
        this.RarityPanel.Size = new System.Drawing.Size(147, 2);
        this.RarityPanel.TabIndex = 3;
        this.panel2.Controls.Add(this.item_description);
        this.panel2.Location = new System.Drawing.Point(0, 65);
        this.panel2.Name = "panel2";
        this.panel2.Padding = new System.Windows.Forms.Padding(3);
        this.panel2.Size = new System.Drawing.Size(147, 92);
        this.panel2.TabIndex = 2;
        this.item_description.BackColor = System.Drawing.Color.FromArgb(47, 48, 51);
        this.item_description.BorderStyle = System.Windows.Forms.BorderStyle.None;
        this.item_description.Dock = System.Windows.Forms.DockStyle.Fill;
        this.item_description.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75f);
        this.item_description.ForeColor = System.Drawing.Color.Gray;
        this.item_description.Location = new System.Drawing.Point(3, 3);
        this.item_description.Multiline = true;
        this.item_description.Name = "item_description";
        this.item_description.Size = new System.Drawing.Size(141, 86);
        this.item_description.TabIndex = 0;
        this.panel1.Controls.Add(this.item_name);
        this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
        this.panel1.Location = new System.Drawing.Point(0, 0);
        this.panel1.Name = "panel1";
        this.panel1.Size = new System.Drawing.Size(147, 33);
        this.panel1.TabIndex = 1;
        this.item_name.AutoSize = true;
        this.item_name.ForeColor = System.Drawing.Color.FromArgb(223, 223, 222);
        this.item_name.Location = new System.Drawing.Point(3, 4);
        this.item_name.Name = "item_name";
        this.item_name.Size = new System.Drawing.Size(125, 13);
        this.item_name.TabIndex = 4;
        this.item_name.Text = "Manifold Paradox Bundle";
        base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
        this.BackColor = System.Drawing.Color.Black;
        base.Controls.Add(this.Content);
        base.Name = "BoxInfo";
        base.Padding = new System.Windows.Forms.Padding(1);
        base.Size = new System.Drawing.Size(149, 162);
        base.Load += new System.EventHandler(BoxTool_Load);
        this.Content.ResumeLayout(false);
        this.panel3.ResumeLayout(false);
        this.panel3.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)this.HeroAvatar).EndInit();
        this.panel2.ResumeLayout(false);
        this.panel2.PerformLayout();
        this.panel1.ResumeLayout(false);
        this.panel1.PerformLayout();
        base.ResumeLayout(false);
    }

    private void BoxTool_Load(object sender, EventArgs e)
    {
    }

    private void Content_MouseMove(object sender, MouseEventArgs e)
    {
        base.Visible = false;
    }
}