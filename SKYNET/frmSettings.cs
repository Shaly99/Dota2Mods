using HeroItems;
using System.ComponentModel;
using XNova_Utils.Others;

namespace SKYNET;

public class frmSettings : Form
{
    private bool mouseDown;

    private Point lastLocation;

    private IContainer components = null;

    private Panel panel1;

    private Panel panel2;

    private Label SETTINGS;

    private PictureBox CloseBox;

    private FlatCheckBox GenerateOnStart;

    private FlatCheckBox OpenDota;

    private Label label1;

    private FlatTextBox Dota2Path;

    private FlatButton CheckCache;

    private Label label2;

    private FlatButton saveBtn;

    private Label label3;

    private FlatButton deleteMod;

    private Label label4;

    private FlatCheckBox ActiveSounds;

    private Label label5;

    private FlatButton Reset;

    private HeroComboBox ItemsToReset;

    private FlatButton changeBtn;

    public frmSettings()
    {
        InitializeComponent();
        ActiveSounds.CheckedChanged += ActiveSounds_CheckedChanged;
    }

    private void ActiveSounds_CheckedChanged(object sender)
    {
        if (!ActiveSounds.Checked)
        {
            AudioPlayer.StopSounds();
        }
    }

    private void CloseBox_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void CloseBox_MouseMove(object sender, MouseEventArgs e)
    {
        Settings.GenerateOnStart = GenerateOnStart.Checked;
        Settings.Dota2Path = Dota2Path.Text;
        Settings.OpenDota = OpenDota.Checked;
        Settings.ActiveSounds = ActiveSounds.Checked;
        CloseBox.Image = Resources.SClose2;
    }

    private void CloseBox_MouseLeave(object sender, EventArgs e)
    {
        CloseBox.Image = Resources.SClose1;
    }

    private void FrmSettings_Load(object sender, EventArgs e)
    {
        AudioPlayer.PlaySound("sounds/ui/panorama/panorama_topmenu_select_03.vsnd", toStop: false);
        GenerateOnStart.Checked = Settings.GenerateOnStart;
        Dota2Path.Text = Settings.Dota2Path;
        OpenDota.Checked = Settings.OpenDota;
        ActiveSounds.Checked = Settings.ActiveSounds;
        ItemsToReset.Items.Add("");
        ItemsToReset.Items.Add("Announcer");
        ItemsToReset.Items.Add("Mega-Kill Pack");
        ItemsToReset.Items.Add("Multi-kill Banner");
        ItemsToReset.Items.Add("Courier");
        ItemsToReset.Items.Add("Cursor Pack");
        ItemsToReset.Items.Add("Dire Creeps");
        ItemsToReset.Items.Add("Dire Towers");
        ItemsToReset.Items.Add("Radiant Creeps");
        ItemsToReset.Items.Add("Radiant Towers");
        ItemsToReset.Items.Add("Music Pack");
        ItemsToReset.Items.Add("Pets");
        ItemsToReset.Items.Add("Versus Screen");
        ItemsToReset.Items.Add("Ward");
        ItemsToReset.Items.Add("Weather Effect");
        ItemsToReset.Items.Add("Loading Screen");
        ItemsToReset.Items.Add("Emblem");
        ItemsToReset.Items.Add("Emoticons");
        ItemsToReset.Items.Add("Hud");
        ItemsToReset.Items.Add("Terrain");
        ItemsToReset.Items.Add("Streak Effect");
    }

    private void SaveBtn_Click(object sender, EventArgs e)
    {
        frmMain.manager.Save();
    }

    private void CheckCache_Click(object sender, EventArgs e)
    {
        frmMain.frm.GenerateCache();
    }

    private void ChangeBtn_Click(object sender, EventArgs e)
    {
        try
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select pak01_dir.vpk in \"dota 2 beta\\game\\dota\"",
                Filter = "Valve File | pak01_dir.vpk",
                Multiselect = false
            };
            DialogResult dialogResult = openFileDialog.ShowDialog();
        }
        catch (Exception)
        {
            modCommon.Show("Error searching location, please run program with admin privilegies");
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

    private void FrmSettings_Deactivate(object sender, EventArgs e)
    {
        Close();
    }

    private void DeleteMod_Click(object sender, EventArgs e)
    {
        string path = Paths.GetParent(Settings.Dota2Path).ToString() + "/SkynetMod";
        if (Directory.Exists(path))
        {
            try
            {
                Directory.Delete(path, recursive: true);
            }
            catch
            {
            }
        }
        modHelpers.ClearGameInfo();
    }

    private void FrmSettings_FormClosing(object sender, FormClosingEventArgs e)
    {
        AudioPlayer.PlaySound("sounds/ui/panorama/panorama_topmenu_select_01.vsnd", toStop: false);
    }

    private void Reset_Click(object sender, EventArgs e)
    {
        var category = MapTextToCategory(ItemsToReset.Text);
        if (category == null) return;

        var modConfig = ServiceContainer.Get<IModConfigurationService>();
        var uiState = ServiceContainer.Get<IUIState>();

        // 1. Resetear la configuración del manager (poner en null)
        modConfig.ResetCategory(category.Value);

        // 2. Cargar el item "default" en el BundleBox (vuelve al estado inicial visible)
        uiState.LoadDefaultForCategory(category.Value);
    }

    /// <summary>
    /// Mapea el texto del ComboBox a la ItemCategory correspondiente.
    /// </summary>
    private static ItemCategory? MapTextToCategory(string text)
    {
        switch (text)
        {
            case "Announcer": return ItemCategory.Announcer;
            case "Mega-Kill Pack": return ItemCategory.MegaKillAnnouncer;
            case "Multi-kill Banner": return ItemCategory.MultikillBanner;
            case "Courier": return ItemCategory.Courier;
            case "Cursor Pack": return ItemCategory.CursorPack;
            case "Dire Creeps": return ItemCategory.DireCreeps;
            case "Dire Towers": return ItemCategory.DireTowers;
            case "Radiant Creeps": return ItemCategory.RadiantCreeps;
            case "Radiant Towers": return ItemCategory.RadiantTowers;
            case "Music Pack": return ItemCategory.MusicPack;
            case "Pets": return ItemCategory.Pets;
            case "Versus Screen": return ItemCategory.VersusScreen;
            case "Ward": return ItemCategory.Ward;
            case "Weather Effect": return ItemCategory.WeatherEffect;
            case "Loading Screen": return ItemCategory.LoadingScreen;
            case "Emblem": return ItemCategory.Emblem;
            case "Emoticons": return ItemCategory.Emoticons;
            case "Hud": return ItemCategory.HUDSkin;
            case "Terrain": return ItemCategory.Terrain;
            case "Streak Effect": return ItemCategory.StreakEffect;
            default: return null;
        }
    }

    private void SETTINGS_Click(object sender, EventArgs e)
    {
    }

    private void Dota2Path_TextChanged(object sender, EventArgs e)
    {
        try
        {
            if (Directory.Exists(Dota2Path.Text) && File.Exists(Dota2Path.Text + "/pak01_dir.vpk"))
            {
                Settings.Dota2Path = Dota2Path.Text;
                modCommon.VPKLocation = Dota2Path.Text + "/pak01_dir.vpk";
            }
        }
        catch (Exception)
        {
        }
    }

    private void label5_Click(object sender, EventArgs e)
    {
        if (!(Environment.UserName == "Hackerprod"))
        {
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
        this.panel1 = new System.Windows.Forms.Panel();
        this.label5 = new System.Windows.Forms.Label();
        this.label4 = new System.Windows.Forms.Label();
        this.label3 = new System.Windows.Forms.Label();
        this.label2 = new System.Windows.Forms.Label();
        this.label1 = new System.Windows.Forms.Label();
        this.CloseBox = new System.Windows.Forms.PictureBox();
        this.panel2 = new System.Windows.Forms.Panel();
        this.SETTINGS = new System.Windows.Forms.Label();
        this.changeBtn = new FlatButton();
        this.Reset = new FlatButton();
        this.ItemsToReset = new HeroItems.HeroComboBox();
        this.ActiveSounds = new FlatCheckBox();
        this.deleteMod = new FlatButton();
        this.saveBtn = new FlatButton();
        this.CheckCache = new FlatButton();
        this.Dota2Path = new XNova_Utils.Others.FlatTextBox();
        this.OpenDota = new FlatCheckBox();
        this.GenerateOnStart = new FlatCheckBox();
        this.panel1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)this.CloseBox).BeginInit();
        this.panel2.SuspendLayout();
        base.SuspendLayout();
        this.panel1.BackColor = System.Drawing.Color.FromArgb(29, 30, 33);
        this.panel1.Controls.Add(this.changeBtn);
        this.panel1.Controls.Add(this.Reset);
        this.panel1.Controls.Add(this.ItemsToReset);
        this.panel1.Controls.Add(this.label5);
        this.panel1.Controls.Add(this.ActiveSounds);
        this.panel1.Controls.Add(this.deleteMod);
        this.panel1.Controls.Add(this.label4);
        this.panel1.Controls.Add(this.saveBtn);
        this.panel1.Controls.Add(this.label3);
        this.panel1.Controls.Add(this.CheckCache);
        this.panel1.Controls.Add(this.label2);
        this.panel1.Controls.Add(this.Dota2Path);
        this.panel1.Controls.Add(this.label1);
        this.panel1.Controls.Add(this.OpenDota);
        this.panel1.Controls.Add(this.GenerateOnStart);
        this.panel1.Controls.Add(this.CloseBox);
        this.panel1.Controls.Add(this.panel2);
        this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.panel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
        this.panel1.ForeColor = System.Drawing.Color.White;
        this.panel1.Location = new System.Drawing.Point(1, 1);
        this.panel1.Name = "panel1";
        this.panel1.Size = new System.Drawing.Size(767, 406);
        this.panel1.TabIndex = 0;
        this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(Event_MouseDown);
        this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(Event_MouseMove);
        this.panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(Event_MouseUp);
        this.label5.AutoSize = true;
        this.label5.Font = new System.Drawing.Font("Segoe UI", 10f);
        this.label5.ForeColor = System.Drawing.Color.White;
        this.label5.Location = new System.Drawing.Point(313, 337);
        this.label5.Name = "label5";
        this.label5.Size = new System.Drawing.Size(233, 19);
        this.label5.TabIndex = 262;
        this.label5.Text = "Reset individual items from database";
        this.label5.Click += new System.EventHandler(label5_Click);
        this.label4.AutoSize = true;
        this.label4.Font = new System.Drawing.Font("Segoe UI", 10f);
        this.label4.ForeColor = System.Drawing.Color.White;
        this.label4.Location = new System.Drawing.Point(10, 337);
        this.label4.Name = "label4";
        this.label4.Size = new System.Drawing.Size(166, 19);
        this.label4.TabIndex = 259;
        this.label4.Text = "Remove Mod from Dota2";
        this.label3.AutoSize = true;
        this.label3.Font = new System.Drawing.Font("Segoe UI", 10f);
        this.label3.ForeColor = System.Drawing.Color.White;
        this.label3.Location = new System.Drawing.Point(11, 279);
        this.label3.Name = "label3";
        this.label3.Size = new System.Drawing.Size(165, 19);
        this.label3.TabIndex = 257;
        this.label3.Text = "Save changes in database";
        this.label2.AutoSize = true;
        this.label2.Font = new System.Drawing.Font("Segoe UI", 10f);
        this.label2.ForeColor = System.Drawing.Color.White;
        this.label2.Location = new System.Drawing.Point(10, 221);
        this.label2.Name = "label2";
        this.label2.Size = new System.Drawing.Size(143, 19);
        this.label2.TabIndex = 255;
        this.label2.Text = "Check cache manually";
        this.label1.AutoSize = true;
        this.label1.Font = new System.Drawing.Font("Segoe UI", 10f);
        this.label1.ForeColor = System.Drawing.Color.White;
        this.label1.Location = new System.Drawing.Point(10, 159);
        this.label1.Name = "label1";
        this.label1.Size = new System.Drawing.Size(439, 19);
        this.label1.TabIndex = 252;
        this.label1.Text = "\"dota\" folder location (Commonly at common/dota 2 beta/game/dota)";
        this.CloseBox.Image = SKYNET.Properties.Resources.SClose1;
        this.CloseBox.Location = new System.Drawing.Point(5, 3);
        this.CloseBox.Name = "CloseBox";
        this.CloseBox.Size = new System.Drawing.Size(22, 23);
        this.CloseBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
        this.CloseBox.TabIndex = 1;
        this.CloseBox.TabStop = false;
        this.CloseBox.Click += new System.EventHandler(CloseBox_Click);
        this.CloseBox.MouseLeave += new System.EventHandler(CloseBox_MouseLeave);
        this.CloseBox.MouseMove += new System.Windows.Forms.MouseEventHandler(CloseBox_MouseMove);
        this.panel2.BackColor = System.Drawing.Color.FromArgb(24, 24, 26);
        this.panel2.Controls.Add(this.SETTINGS);
        this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
        this.panel2.Location = new System.Drawing.Point(0, 0);
        this.panel2.Name = "panel2";
        this.panel2.Size = new System.Drawing.Size(767, 30);
        this.panel2.TabIndex = 0;
        this.panel2.MouseDown += new System.Windows.Forms.MouseEventHandler(Event_MouseDown);
        this.panel2.MouseMove += new System.Windows.Forms.MouseEventHandler(Event_MouseMove);
        this.panel2.MouseUp += new System.Windows.Forms.MouseEventHandler(Event_MouseUp);
        this.SETTINGS.AutoSize = true;
        this.SETTINGS.Font = new System.Drawing.Font("Verdana", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
        this.SETTINGS.ForeColor = System.Drawing.Color.Gainsboro;
        this.SETTINGS.Location = new System.Drawing.Point(37, 8);
        this.SETTINGS.Name = "SETTINGS";
        this.SETTINGS.Size = new System.Drawing.Size(71, 13);
        this.SETTINGS.TabIndex = 0;
        this.SETTINGS.Text = "SETTINGS";
        this.SETTINGS.Click += new System.EventHandler(SETTINGS_Click);
        this.SETTINGS.MouseDown += new System.Windows.Forms.MouseEventHandler(Event_MouseDown);
        this.SETTINGS.MouseMove += new System.Windows.Forms.MouseEventHandler(Event_MouseMove);
        this.SETTINGS.MouseUp += new System.Windows.Forms.MouseEventHandler(Event_MouseUp);
        this.changeBtn.BackColor = System.Drawing.Color.FromArgb(44, 48, 49);
        this.changeBtn.BackColorMouseOver = System.Drawing.Color.Empty;
        this.changeBtn.Cursor = System.Windows.Forms.Cursors.Hand;
        this.changeBtn.Font = new System.Drawing.Font("Segoe UI", 9f);
        this.changeBtn.ForeColorMouseOver = System.Drawing.Color.Empty;
        this.changeBtn.ImageAlignment = FlatButton._ImgAlign.Left;
        this.changeBtn.ImageIcon = null;
        this.changeBtn.Location = new System.Drawing.Point(455, 181);
        this.changeBtn.Name = "changeBtn";
        this.changeBtn.Rounded = false;
        this.changeBtn.Size = new System.Drawing.Size(93, 28);
        this.changeBtn.Style = FlatButton._Style.TextOnly;
        this.changeBtn.TabIndex = 265;
        this.changeBtn.Text = "Change";
        this.changeBtn.Visible = false;
        this.changeBtn.Click += new System.EventHandler(ChangeBtn_Click);
        this.Reset.BackColor = System.Drawing.Color.FromArgb(44, 48, 49);
        this.Reset.BackColorMouseOver = System.Drawing.Color.FromArgb(57, 62, 63);
        this.Reset.Cursor = System.Windows.Forms.Cursors.Hand;
        this.Reset.Font = new System.Drawing.Font("Segoe UI", 9f);
        this.Reset.ForeColor = System.Drawing.Color.White;
        this.Reset.ForeColorMouseOver = System.Drawing.Color.Empty;
        this.Reset.ImageAlignment = FlatButton._ImgAlign.Left;
        this.Reset.ImageIcon = null;
        this.Reset.Location = new System.Drawing.Point(561, 360);
        this.Reset.Name = "Reset";
        this.Reset.Rounded = false;
        this.Reset.Size = new System.Drawing.Size(93, 29);
        this.Reset.Style = FlatButton._Style.TextOnly;
        this.Reset.TabIndex = 264;
        this.Reset.Text = "Reset item";
        this.Reset.Click += new System.EventHandler(Reset_Click);
        this.ItemsToReset.BackColor = System.Drawing.Color.FromArgb(69, 73, 85);
        this.ItemsToReset.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
        this.ItemsToReset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.ItemsToReset.ForeColor = System.Drawing.Color.White;
        this.ItemsToReset.FormattingEnabled = true;
        this.ItemsToReset.ItemHeight = 18;
        this.ItemsToReset.Location = new System.Drawing.Point(317, 363);
        this.ItemsToReset.Name = "ItemsToReset";
        this.ItemsToReset.Size = new System.Drawing.Size(229, 24);
        this.ItemsToReset.TabIndex = 263;
        this.ActiveSounds.BackColor = System.Drawing.Color.FromArgb(29, 30, 33);
        this.ActiveSounds.BoxBackColor = System.Drawing.Color.FromArgb(44, 48, 49);
        this.ActiveSounds.BoxColor = System.Drawing.Color.FromArgb(7, 164, 245);
        this.ActiveSounds.Checked = false;
        this.ActiveSounds.Cursor = System.Windows.Forms.Cursors.Hand;
        this.ActiveSounds.Font = new System.Drawing.Font("Segoe UI", 10f);
        this.ActiveSounds.ForeColor = System.Drawing.Color.White;
        this.ActiveSounds.Location = new System.Drawing.Point(14, 115);
        this.ActiveSounds.Name = "ActiveSounds";
        this.ActiveSounds.Options = FlatCheckBox._Options.Style1;
        this.ActiveSounds.Size = new System.Drawing.Size(177, 22);
        this.ActiveSounds.TabIndex = 261;
        this.ActiveSounds.Text = "Sounds of the program";
        this.deleteMod.BackColor = System.Drawing.Color.FromArgb(44, 48, 49);
        this.deleteMod.BackColorMouseOver = System.Drawing.Color.FromArgb(57, 62, 63);
        this.deleteMod.Cursor = System.Windows.Forms.Cursors.Hand;
        this.deleteMod.Font = new System.Drawing.Font("Segoe UI", 9f);
        this.deleteMod.ForeColor = System.Drawing.Color.White;
        this.deleteMod.ForeColorMouseOver = System.Drawing.Color.Empty;
        this.deleteMod.ImageAlignment = FlatButton._ImgAlign.Left;
        this.deleteMod.ImageIcon = null;
        this.deleteMod.Location = new System.Drawing.Point(14, 359);
        this.deleteMod.Name = "deleteMod";
        this.deleteMod.Rounded = false;
        this.deleteMod.Size = new System.Drawing.Size(93, 29);
        this.deleteMod.Style = FlatButton._Style.TextOnly;
        this.deleteMod.TabIndex = 260;
        this.deleteMod.Text = "Remove mod";
        this.deleteMod.Click += new System.EventHandler(DeleteMod_Click);
        this.saveBtn.BackColor = System.Drawing.Color.FromArgb(44, 48, 49);
        this.saveBtn.BackColorMouseOver = System.Drawing.Color.FromArgb(57, 62, 63);
        this.saveBtn.Cursor = System.Windows.Forms.Cursors.Hand;
        this.saveBtn.Font = new System.Drawing.Font("Segoe UI", 9f);
        this.saveBtn.ForeColor = System.Drawing.Color.White;
        this.saveBtn.ForeColorMouseOver = System.Drawing.Color.Empty;
        this.saveBtn.ImageAlignment = FlatButton._ImgAlign.Left;
        this.saveBtn.ImageIcon = null;
        this.saveBtn.Location = new System.Drawing.Point(15, 301);
        this.saveBtn.Name = "saveBtn";
        this.saveBtn.Rounded = false;
        this.saveBtn.Size = new System.Drawing.Size(93, 29);
        this.saveBtn.Style = FlatButton._Style.TextOnly;
        this.saveBtn.TabIndex = 258;
        this.saveBtn.Text = "Save changes";
        this.saveBtn.Click += new System.EventHandler(SaveBtn_Click);
        this.CheckCache.BackColor = System.Drawing.Color.FromArgb(44, 48, 49);
        this.CheckCache.BackColorMouseOver = System.Drawing.Color.FromArgb(57, 62, 63);
        this.CheckCache.Cursor = System.Windows.Forms.Cursors.Hand;
        this.CheckCache.Font = new System.Drawing.Font("Segoe UI", 9f);
        this.CheckCache.ForeColor = System.Drawing.Color.White;
        this.CheckCache.ForeColorMouseOver = System.Drawing.Color.Empty;
        this.CheckCache.ImageAlignment = FlatButton._ImgAlign.Left;
        this.CheckCache.ImageIcon = null;
        this.CheckCache.Location = new System.Drawing.Point(14, 243);
        this.CheckCache.Name = "CheckCache";
        this.CheckCache.Rounded = false;
        this.CheckCache.Size = new System.Drawing.Size(93, 29);
        this.CheckCache.Style = FlatButton._Style.TextOnly;
        this.CheckCache.TabIndex = 256;
        this.CheckCache.Text = "Check cache";
        this.CheckCache.Click += new System.EventHandler(CheckCache_Click);
        this.Dota2Path.BackColor = System.Drawing.Color.FromArgb(44, 48, 49);
        this.Dota2Path.BackColorControl = System.Drawing.Color.Empty;
        this.Dota2Path.Font = new System.Drawing.Font("Segoe UI", 10f);
        this.Dota2Path.ForeColor = System.Drawing.Color.White;
        this.Dota2Path.isCustomColor = false;
        this.Dota2Path.Location = new System.Drawing.Point(14, 181);
        this.Dota2Path.MaxLength = 32767;
        this.Dota2Path.Multiline = false;
        this.Dota2Path.Name = "Dota2Path";
        this.Dota2Path.OnlyNumber = false;
        this.Dota2Path.ReadOnly = false;
        this.Dota2Path.Size = new System.Drawing.Size(435, 28);
        this.Dota2Path.TabIndex = 254;
        this.Dota2Path.Text = "flatTextBox1";
        this.Dota2Path.TextAlignment = System.Windows.Forms.HorizontalAlignment.Left;
        this.Dota2Path.UseSystemPasswordChar = false;
        this.Dota2Path.TextChanged += new System.EventHandler(Dota2Path_TextChanged);
        this.OpenDota.BackColor = System.Drawing.Color.FromArgb(29, 30, 33);
        this.OpenDota.BoxBackColor = System.Drawing.Color.FromArgb(44, 48, 49);
        this.OpenDota.BoxColor = System.Drawing.Color.FromArgb(7, 164, 245);
        this.OpenDota.Checked = false;
        this.OpenDota.Cursor = System.Windows.Forms.Cursors.Hand;
        this.OpenDota.Font = new System.Drawing.Font("Segoe UI", 10f);
        this.OpenDota.ForeColor = System.Drawing.Color.White;
        this.OpenDota.Location = new System.Drawing.Point(14, 87);
        this.OpenDota.Name = "OpenDota";
        this.OpenDota.Options = FlatCheckBox._Options.Style1;
        this.OpenDota.Size = new System.Drawing.Size(346, 22);
        this.OpenDota.TabIndex = 250;
        this.OpenDota.Text = "Open Dota2 when finish operation";
        this.GenerateOnStart.BackColor = System.Drawing.Color.FromArgb(29, 30, 33);
        this.GenerateOnStart.BoxBackColor = System.Drawing.Color.FromArgb(44, 48, 49);
        this.GenerateOnStart.BoxColor = System.Drawing.Color.FromArgb(7, 164, 245);
        this.GenerateOnStart.Checked = false;
        this.GenerateOnStart.Cursor = System.Windows.Forms.Cursors.Hand;
        this.GenerateOnStart.Font = new System.Drawing.Font("Segoe UI", 10f);
        this.GenerateOnStart.ForeColor = System.Drawing.Color.White;
        this.GenerateOnStart.Location = new System.Drawing.Point(14, 59);
        this.GenerateOnStart.Name = "GenerateOnStart";
        this.GenerateOnStart.Options = FlatCheckBox._Options.Style1;
        this.GenerateOnStart.Size = new System.Drawing.Size(313, 22);
        this.GenerateOnStart.TabIndex = 249;
        this.GenerateOnStart.Text = "Check cache when the program is starting ";
        base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
        this.BackColor = System.Drawing.Color.FromArgb(40, 44, 44);
        base.ClientSize = new System.Drawing.Size(769, 408);
        base.Controls.Add(this.panel1);
        base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        base.Name = "frmSettings";
        base.Padding = new System.Windows.Forms.Padding(1);
        base.ShowIcon = false;
        base.ShowInTaskbar = false;
        base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "frmSettings";
        base.Deactivate += new System.EventHandler(FrmSettings_Deactivate);
        base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(FrmSettings_FormClosing);
        base.Load += new System.EventHandler(FrmSettings_Load);
        this.panel1.ResumeLayout(false);
        this.panel1.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)this.CloseBox).EndInit();
        this.panel2.ResumeLayout(false);
        this.panel2.PerformLayout();
        base.ResumeLayout(false);
    }
}
