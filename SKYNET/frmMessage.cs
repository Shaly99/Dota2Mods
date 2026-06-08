using System.ComponentModel;

namespace SKYNET;

public class frmMessage : Form
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

    private FlatButton acepctBtn;

    private FlatButton cancelBtn;

    private Panel panel15;

    private Button Cancel;

    private FlatTextBox2 txtMessage;

    private Panel panel2;

    private Label label1;

    public frmMessage(string message, TypeMessage type = TypeMessage.Normal)
    {
        InitializeComponent();
        typeMessage = type;
        switch (typeMessage)
        {
            case TypeMessage.Normal:
                acepctBtn.Visible = false;
                cancelBtn.Text = "Cerrar";
                break;
        }
        txtMessage.Text = message;
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
        Cancel.PerformClick();
        Close();
    }

    private void acepctBtn_Click(object sender, EventArgs e)
    {
        ok.PerformClick();
    }

    private void frmMessage_Activated(object sender, EventArgs e)
    {
    }

    private void frmMessage_Deactivate(object sender, EventArgs e)
    {
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
        this.acceptBtn = new System.Windows.Forms.Button();
        this.ok = new System.Windows.Forms.Button();
        this.panel1 = new System.Windows.Forms.Panel();
        this.acepctBtn = new FlatButton();
        this.cancelBtn = new FlatButton();
        this.panel15 = new System.Windows.Forms.Panel();
        this.Cancel = new System.Windows.Forms.Button();
        this.txtMessage = new FlatTextBox2();
        this.panel2 = new System.Windows.Forms.Panel();
        this.label1 = new System.Windows.Forms.Label();
        this.panel1.SuspendLayout();
        this.panel15.SuspendLayout();
        this.panel2.SuspendLayout();
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
        this.panel1.Controls.Add(this.acepctBtn);
        this.panel1.Controls.Add(this.cancelBtn);
        this.panel1.Controls.Add(this.panel15);
        this.panel1.Controls.Add(this.panel2);
        this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.panel1.Location = new System.Drawing.Point(1, 1);
        this.panel1.Name = "panel1";
        this.panel1.Size = new System.Drawing.Size(475, 193);
        this.panel1.TabIndex = 25;
        this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(Event_MouseDown);
        this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(Event_MouseMove);
        this.panel1.MouseUp += new System.Windows.Forms.MouseEventHandler(Event_MouseUp);
        this.acepctBtn.BackColor = System.Drawing.Color.FromArgb(43, 47, 48);
        this.acepctBtn.BackColorMouseOver = System.Drawing.Color.FromArgb(57, 62, 63);
        this.acepctBtn.Cursor = System.Windows.Forms.Cursors.Hand;
        this.acepctBtn.Font = FontService.GetRadiance(10f);
        this.acepctBtn.ForeColor = System.Drawing.Color.FromArgb(147, 157, 160);
        this.acepctBtn.ForeColorMouseOver = System.Drawing.Color.Empty;
        this.acepctBtn.ImageAlignment = FlatButton._ImgAlign.Left;
        this.acepctBtn.ImageIcon = null;
        this.acepctBtn.Location = new System.Drawing.Point(273, 152);
        this.acepctBtn.Name = "acepctBtn";
        this.acepctBtn.Rounded = false;
        this.acepctBtn.Size = new System.Drawing.Size(93, 29);
        this.acepctBtn.Style = FlatButton._Style.TextOnly;
        this.acepctBtn.TabIndex = 27;
        this.acepctBtn.Text = "Aceptar";
        this.acepctBtn.Click += new System.EventHandler(acepctBtn_Click);
        this.cancelBtn.BackColor = System.Drawing.Color.FromArgb(43, 47, 48);
        this.cancelBtn.BackColorMouseOver = System.Drawing.Color.FromArgb(57, 62, 63);
        this.cancelBtn.Cursor = System.Windows.Forms.Cursors.Hand;
        this.cancelBtn.Font = FontService.GetRadiance(10f);
        this.cancelBtn.ForeColor = System.Drawing.Color.FromArgb(147, 157, 160);
        this.cancelBtn.ForeColorMouseOver = System.Drawing.Color.Empty;
        this.cancelBtn.ImageAlignment = FlatButton._ImgAlign.Left;
        this.cancelBtn.ImageIcon = null;
        this.cancelBtn.Location = new System.Drawing.Point(372, 152);
        this.cancelBtn.Name = "cancelBtn";
        this.cancelBtn.Rounded = false;
        this.cancelBtn.Size = new System.Drawing.Size(93, 29);
        this.cancelBtn.Style = FlatButton._Style.TextOnly;
        this.cancelBtn.TabIndex = 26;
        this.cancelBtn.Text = "Cancelar";
        this.cancelBtn.Click += new System.EventHandler(cancelBtn_Click);
        this.panel15.BackColor = System.Drawing.Color.FromArgb(28, 29, 32);
        this.panel15.Controls.Add(this.Cancel);
        this.panel15.Controls.Add(this.txtMessage);
        this.panel15.Dock = System.Windows.Forms.DockStyle.Top;
        this.panel15.ForeColor = System.Drawing.Color.FromArgb(147, 157, 160);
        this.panel15.Location = new System.Drawing.Point(0, 26);
        this.panel15.Name = "panel15";
        this.panel15.Size = new System.Drawing.Size(475, 120);
        this.panel15.TabIndex = 25;
        this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        this.Cancel.Location = new System.Drawing.Point(484, 90);
        this.Cancel.Name = "Cancel";
        this.Cancel.Size = new System.Drawing.Size(16, 23);
        this.Cancel.TabIndex = 25;
        this.Cancel.Text = "cancel";
        this.Cancel.UseVisualStyleBackColor = true;
        this.txtMessage.BackColor = System.Drawing.Color.FromArgb(28, 29, 32);
        this.txtMessage.BackColorControl = System.Drawing.Color.FromArgb(28, 29, 32);
        this.txtMessage.Font = FontService.GetRadiance(10f);
        this.txtMessage.ForeColor = System.Drawing.Color.FromArgb(147, 157, 160);
        this.txtMessage.isCustomColor = false;
        this.txtMessage.Location = new System.Drawing.Point(12, 10);
        this.txtMessage.MaxLength = 32767;
        this.txtMessage.Multiline = true;
        this.txtMessage.Name = "txtMessage";
        this.txtMessage.ReadOnly = true;
        this.txtMessage.Size = new System.Drawing.Size(453, 105);
        this.txtMessage.TabIndex = 27;
        this.txtMessage.TextAlignment = System.Windows.Forms.HorizontalAlignment.Left;
        this.txtMessage.UseSystemPasswordChar = false;
        this.panel2.BackColor = System.Drawing.Color.FromArgb(43, 47, 48);
        this.panel2.Controls.Add(this.label1);
        this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
        this.panel2.ForeColor = System.Drawing.Color.White;
        this.panel2.Location = new System.Drawing.Point(0, 0);
        this.panel2.Name = "panel2";
        this.panel2.Size = new System.Drawing.Size(475, 26);
        this.panel2.TabIndex = 24;
        this.panel2.MouseDown += new System.Windows.Forms.MouseEventHandler(Event_MouseDown);
        this.panel2.MouseMove += new System.Windows.Forms.MouseEventHandler(Event_MouseMove);
        this.panel2.MouseUp += new System.Windows.Forms.MouseEventHandler(Event_MouseUp);
        this.label1.AutoSize = true;
        this.label1.ForeColor = System.Drawing.Color.FromArgb(147, 157, 160);
        this.label1.Location = new System.Drawing.Point(9, 4);
        this.label1.Name = "label1";
        this.label1.Size = new System.Drawing.Size(52, 16);
        this.label1.TabIndex = 0;
        this.label1.Text = "Mensaje";
        base.AutoScaleDimensions = new System.Drawing.SizeF(7f, 16f);
        base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
        this.BackColor = System.Drawing.Color.FromArgb(7, 164, 245);
        base.ClientSize = new System.Drawing.Size(477, 195);
        base.Controls.Add(this.panel1);
        base.Controls.Add(this.ok);
        base.Controls.Add(this.acceptBtn);
        this.Font = new System.Drawing.Font("Segoe UI Emoji", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
        base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        base.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
        base.Name = "frmMessage";
        base.Padding = new System.Windows.Forms.Padding(1);
        base.ShowIcon = false;
        base.ShowInTaskbar = false;
        base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "Message";
        base.Activated += new System.EventHandler(frmMessage_Activated);
        base.Deactivate += new System.EventHandler(frmMessage_Deactivate);
        this.panel1.ResumeLayout(false);
        this.panel15.ResumeLayout(false);
        this.panel2.ResumeLayout(false);
        this.panel2.PerformLayout();
        base.ResumeLayout(false);
    }
}
