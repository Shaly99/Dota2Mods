using System.ComponentModel;

namespace SKYNET;

public class frmLoading : Form
{
    private Items items;

    private IContainer components = null;

    private Panel panel1;

    private Panel panel2;

    public frmLoading(Items item)
    {
        InitializeComponent();
        items = item;
    }

    private void CloseBox_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void FrmSettings_Load(object sender, EventArgs e)
    {
    }

    private void CursorWorker_DoWork(object sender, DoWorkEventArgs e)
    {
        while (true)
        {
            for (int i = 0; i < base.Controls.Count; i++)
            {
                try
                {
                    Icon cursor = Resources.cursor;
                    base.Controls[i].Cursor = new Cursor(cursor.Handle);
                    Cursor = new Cursor(cursor.Handle);
                }
                catch
                {
                }
            }
        }
    }

    private void FrmLoading_Shown(object sender, EventArgs e)
    {
        // Limpiar selección previa
        modCommon.ItemIDSelected = null;

        // Abrir frmTools (modal)
        frmTools frmTools2 = new frmTools(items);
        DialogResult result = frmTools2.ShowDialog();

        // 🔑 Si el usuario seleccionó un item, aplicarlo
        if (result == DialogResult.OK && modCommon.ItemIDSelected != null)
        {
            ApplySelection(modCommon.ItemIDSelected);
        }

        // Cerrar este formulario
        Close();
    }

    /// <summary>
    /// Aplica el item seleccionado al BundleBox correspondiente en frmMain.
    /// Simula el click con isTool=true para activar la lógica de aplicación.
    /// </summary>
    private void ApplySelection(Items selectedItem)
    {
        if (selectedItem == null || frmMain.frm == null) return;

        try
        {
            // Crear un BundleBox temporal con el item seleccionado y isTool=true
            // para que Bundle_Click() aplique los cambios
            BundleBox tempBox = new BundleBox
            {
                item = selectedItem,
                isTool = true  // 🔑 Esto activa el camino de "aplicar" en Bundle_Click
            };

            // Llamar a Bundle_Click para aplicar el cambio
            frmMain.frm.Bundle_Click(tempBox);
        }
        catch (Exception ex)
        {
            modCommon.Save(ex);
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
        this.panel2 = new System.Windows.Forms.Panel();
        this.panel1.SuspendLayout();
        base.SuspendLayout();
        this.panel1.BackColor = System.Drawing.Color.FromArgb(29, 30, 33);
        this.panel1.Controls.Add(this.panel2);
        this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.panel1.Font = new System.Drawing.Font("MS Reference Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
        this.panel1.Location = new System.Drawing.Point(1, 1);
        this.panel1.Name = "panel1";
        this.panel1.Size = new System.Drawing.Size(402, 126);
        this.panel1.TabIndex = 0;
        this.panel2.BackColor = System.Drawing.Color.FromArgb(24, 24, 26);
        this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
        this.panel2.Location = new System.Drawing.Point(0, 0);
        this.panel2.Name = "panel2";
        this.panel2.Size = new System.Drawing.Size(402, 26);
        this.panel2.TabIndex = 0;
        base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
        this.BackColor = System.Drawing.Color.FromArgb(40, 44, 44);
        base.ClientSize = new System.Drawing.Size(404, 128);
        base.Controls.Add(this.panel1);
        base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        base.Name = "frmLoading";
        base.Padding = new System.Windows.Forms.Padding(1);
        base.ShowIcon = false;
        base.ShowInTaskbar = false;
        base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "frmSettings";
        base.Load += new System.EventHandler(FrmSettings_Load);
        base.Shown += new System.EventHandler(FrmLoading_Shown);
        this.panel1.ResumeLayout(false);
        base.ResumeLayout(false);
    }
}