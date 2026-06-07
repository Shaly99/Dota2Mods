using System.ComponentModel;

namespace SKYNET;

public class frmAbout : Form
{
    public enum TypeMessage
    {
        Alert,
        Normal,
        YesNo
    }

    private bool mouseDown;

    private Point lastLocation;

    public TypeMessage typeMessage;

    private IContainer components = null;

    private Button acceptBtn;

    private Button ok;

    private Panel panel1;

    private Panel panel2;

    private PictureBox pictureBox1;

    private Label label1;

    private Label label3;

    private Label label2;

    private Label label6;

    private Label VersionInfo;

    private Label YearInfo;

    private TextBox textBox1;

    public frmAbout()
    {
        InitializeComponent();
        FontService.ApplyRadianceToForm(this);
        textBox1.Focus();
        VersionInfo.Text = "v1.10";
        AudioPlayer.PlaySound("sounds/ui/projection/doom_pick.vsnd", toStop: false);
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

    private void frmMessage_Activated(object sender, EventArgs e)
    {
        textBox1.Focus();
    }

    private void frmMessage_Deactivate(object sender, EventArgs e)
    {
        Close();
    }

    private void FrmAbout_Load(object sender, EventArgs e)
    {
        pictureBox1.Focus();
    }

    private void TextBox1_KeyDown(object sender, KeyEventArgs e)
    {
        Close();
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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SKYNET.frmAbout));
        this.acceptBtn = new System.Windows.Forms.Button();
        this.ok = new System.Windows.Forms.Button();
        this.panel1 = new System.Windows.Forms.Panel();
        this.textBox1 = new System.Windows.Forms.TextBox();
        this.label6 = new System.Windows.Forms.Label();
        this.VersionInfo = new System.Windows.Forms.Label();
        this.YearInfo = new System.Windows.Forms.Label();
        this.label3 = new System.Windows.Forms.Label();
        this.label2 = new System.Windows.Forms.Label();
        this.label1 = new System.Windows.Forms.Label();
        this.pictureBox1 = new System.Windows.Forms.PictureBox();
        this.panel2 = new System.Windows.Forms.Panel();
        this.panel1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)this.pictureBox1).BeginInit();
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
        this.panel1.Controls.Add(this.textBox1);
        this.panel1.Controls.Add(this.label6);
        this.panel1.Controls.Add(this.VersionInfo);
        this.panel1.Controls.Add(this.YearInfo);
        this.panel1.Controls.Add(this.label3);
        this.panel1.Controls.Add(this.label2);
        this.panel1.Controls.Add(this.label1);
        this.panel1.Controls.Add(this.pictureBox1);
        this.panel1.Controls.Add(this.panel2);
        this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.panel1.Location = new System.Drawing.Point(1, 1);
        this.panel1.Name = "panel1";
        this.panel1.Size = new System.Drawing.Size(406, 158);
        this.panel1.TabIndex = 25;
        this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(Event_MouseDown);
        this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(Event_MouseMove);
        this.panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(Event_MouseUp);
        this.textBox1.Location = new System.Drawing.Point(407, 127);
        this.textBox1.Name = "textBox1";
        this.textBox1.Size = new System.Drawing.Size(10, 23);
        this.textBox1.TabIndex = 32;
        this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(TextBox1_KeyDown);
        this.label6.AutoSize = true;
        this.label6.BackColor = System.Drawing.Color.Transparent;
        this.label6.Font = new System.Drawing.Font("Segoe UI Emoji", 8.75f);
        this.label6.ForeColor = System.Drawing.Color.FromArgb(167, 177, 180);
        this.label6.Location = new System.Drawing.Point(72, 97);
        this.label6.Name = "label6";
        this.label6.Size = new System.Drawing.Size(70, 16);
        this.label6.TabIndex = 31;
        this.label6.Text = "Hackerprod";
        this.label6.MouseDown += new System.Windows.Forms.MouseEventHandler(Event_MouseDown);
        this.label6.MouseMove += new System.Windows.Forms.MouseEventHandler(Event_MouseMove);
        this.label6.MouseUp += new System.Windows.Forms.MouseEventHandler(Event_MouseUp);
        this.VersionInfo.AutoSize = true;
        this.VersionInfo.BackColor = System.Drawing.Color.Transparent;
        this.VersionInfo.Font = new System.Drawing.Font("Segoe UI Emoji", 8.75f);
        this.VersionInfo.ForeColor = System.Drawing.Color.FromArgb(167, 177, 180);
        this.VersionInfo.Location = new System.Drawing.Point(72, 75);
        this.VersionInfo.Name = "VersionInfo";
        this.VersionInfo.Size = new System.Drawing.Size(28, 16);
        this.VersionInfo.TabIndex = 30;
        this.VersionInfo.Text = "v1.x";
        this.VersionInfo.MouseDown += new System.Windows.Forms.MouseEventHandler(Event_MouseDown);
        this.VersionInfo.MouseMove += new System.Windows.Forms.MouseEventHandler(Event_MouseMove);
        this.VersionInfo.MouseUp += new System.Windows.Forms.MouseEventHandler(Event_MouseUp);
        this.YearInfo.AutoSize = true;
        this.YearInfo.BackColor = System.Drawing.Color.Transparent;
        this.YearInfo.Font = new System.Drawing.Font("Segoe UI Emoji", 8.75f);
        this.YearInfo.ForeColor = System.Drawing.Color.FromArgb(167, 177, 180);
        this.YearInfo.Location = new System.Drawing.Point(14, 120);
        this.YearInfo.Name = "YearInfo";
        this.YearInfo.Size = new System.Drawing.Size(107, 16);
        this.YearInfo.TabIndex = 29;
        this.YearInfo.Text = "Copyright © 2021";
        this.YearInfo.MouseDown += new System.Windows.Forms.MouseEventHandler(Event_MouseDown);
        this.YearInfo.MouseMove += new System.Windows.Forms.MouseEventHandler(Event_MouseMove);
        this.YearInfo.MouseUp += new System.Windows.Forms.MouseEventHandler(Event_MouseUp);
        this.label3.AutoSize = true;
        this.label3.BackColor = System.Drawing.Color.Transparent;
        this.label3.Font = new System.Drawing.Font("Segoe UI Emoji", 8.75f);
        this.label3.ForeColor = System.Drawing.Color.FromArgb(167, 177, 180);
        this.label3.Location = new System.Drawing.Point(14, 97);
        this.label3.Name = "label3";
        this.label3.Size = new System.Drawing.Size(48, 16);
        this.label3.TabIndex = 28;
        this.label3.Text = "Author:";
        this.label3.MouseDown += new System.Windows.Forms.MouseEventHandler(Event_MouseDown);
        this.label3.MouseMove += new System.Windows.Forms.MouseEventHandler(Event_MouseMove);
        this.label3.MouseUp += new System.Windows.Forms.MouseEventHandler(Event_MouseUp);
        this.label2.AutoSize = true;
        this.label2.BackColor = System.Drawing.Color.Transparent;
        this.label2.Font = new System.Drawing.Font("Segoe UI Emoji", 8.75f);
        this.label2.ForeColor = System.Drawing.Color.FromArgb(167, 177, 180);
        this.label2.Location = new System.Drawing.Point(14, 75);
        this.label2.Name = "label2";
        this.label2.Size = new System.Drawing.Size(50, 16);
        this.label2.TabIndex = 27;
        this.label2.Text = "Version:";
        this.label2.MouseDown += new System.Windows.Forms.MouseEventHandler(Event_MouseDown);
        this.label2.MouseMove += new System.Windows.Forms.MouseEventHandler(Event_MouseMove);
        this.label2.MouseUp += new System.Windows.Forms.MouseEventHandler(Event_MouseUp);
        this.label1.AutoSize = true;
        this.label1.BackColor = System.Drawing.Color.Transparent;
        this.label1.Font = new System.Drawing.Font("Segoe MDL2 Assets", 21.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
        this.label1.ForeColor = System.Drawing.Color.FromArgb(187, 197, 200);
        this.label1.Location = new System.Drawing.Point(10, 30);
        this.label1.Name = "label1";
        this.label1.Size = new System.Drawing.Size(264, 29);
        this.label1.TabIndex = 26;
        this.label1.Text = "SKYNET DOTA2 MODS";
        this.label1.MouseDown += new System.Windows.Forms.MouseEventHandler(Event_MouseDown);
        this.label1.MouseMove += new System.Windows.Forms.MouseEventHandler(Event_MouseMove);
        this.label1.MouseUp += new System.Windows.Forms.MouseEventHandler(Event_MouseUp);
        this.pictureBox1.Image = SKYNET.Properties.Resources.Frondillo;
        this.pictureBox1.Location = new System.Drawing.Point(276, 31);
        this.pictureBox1.Name = "pictureBox1";
        this.pictureBox1.Size = new System.Drawing.Size(120, 120);
        this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
        this.pictureBox1.TabIndex = 25;
        this.pictureBox1.TabStop = false;
        this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(Event_MouseDown);
        this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(Event_MouseMove);
        this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(Event_MouseUp);
        this.panel2.BackColor = System.Drawing.Color.FromArgb(43, 47, 48);
        this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
        this.panel2.ForeColor = System.Drawing.Color.White;
        this.panel2.Location = new System.Drawing.Point(0, 0);
        this.panel2.Name = "panel2";
        this.panel2.Size = new System.Drawing.Size(406, 26);
        this.panel2.TabIndex = 24;
        this.panel2.MouseDown += new System.Windows.Forms.MouseEventHandler(Event_MouseDown);
        this.panel2.MouseMove += new System.Windows.Forms.MouseEventHandler(Event_MouseMove);
        this.panel2.MouseUp += new System.Windows.Forms.MouseEventHandler(Event_MouseUp);
        base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
        this.BackColor = System.Drawing.Color.FromArgb(7, 164, 245);
        base.ClientSize = new System.Drawing.Size(408, 160);
        base.Controls.Add(this.panel1);
        base.Controls.Add(this.ok);
        base.Controls.Add(this.acceptBtn);
        this.Font = new System.Drawing.Font("Segoe UI Emoji", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
        base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        base.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
        base.Name = "frmAbout";
        base.Padding = new System.Windows.Forms.Padding(1);
        base.ShowIcon = false;
        base.ShowInTaskbar = false;
        base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "Message";
        base.Activated += new System.EventHandler(frmMessage_Activated);
        base.Deactivate += new System.EventHandler(frmMessage_Deactivate);
        base.Load += new System.EventHandler(FrmAbout_Load);
        this.panel1.ResumeLayout(false);
        this.panel1.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)this.pictureBox1).EndInit();
        base.ResumeLayout(false);
    }
}
