using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace HeroItems;

[DefaultEvent("SelectedIndexChanged")]
internal class HeroComboBox : ComboBox
{
    private int X;

    public HeroComboBox()
    {
        base.TextChanged += GhostCombo_TextChanged;
        base.DropDownClosed += GhostComboBox_DropDownClosed;
        SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, value: true);
        base.DrawMode = DrawMode.OwnerDrawFixed;
        base.ItemHeight = 18;
        BackColor = Color.FromArgb(43, 47, 48);
        base.DropDownStyle = ComboBoxStyle.DropDownList;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        X = e.X;
        Invalidate();
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        X = -1;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        if (base.DropDownStyle != ComboBoxStyle.DropDownList)
        {
            base.DropDownStyle = ComboBoxStyle.DropDownList;
        }
        Bitmap bitmap = new Bitmap(base.Width, base.Height);
        Graphics graphics = Graphics.FromImage(bitmap);
        graphics.Clear(Color.FromArgb(43, 47, 48));
        LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, base.Width, base.Height / 5 * 2), Color.FromArgb(63, 67, 68), Color.FromArgb(43, 47, 48), 90f);
        graphics.FillRectangle(brush, new Rectangle(0, 0, base.Width, base.Height / 5 * 2));
        HatchBrush hatchBrush = null;
        int num = (int)graphics.MeasureString("OrainsComboBox", Font).Height;
        if (SelectedIndex != -1)
        {
            graphics.DrawString(base.Items[SelectedIndex].ToString(), Font, new SolidBrush(ForeColor), 4f, base.Height / 2 - num / 2);
        }
        else if ((base.Items != null) & (base.Items.Count > 0))
        {
            graphics.DrawString(base.Items[0].ToString(), Font, new SolidBrush(ForeColor), 4f, base.Height / 2 - num / 2);
        }
        else
        {
            graphics.DrawString("OrainsComboBox", Font, new SolidBrush(ForeColor), 4f, base.Height / 2 - num / 2);
        }
        if ((Control.MouseButtons == MouseButtons.None) & (X > base.Width - 25))
        {
            graphics.FillRectangle(new SolidBrush(Color.FromArgb(7, Color.Red)), 2, 1, base.Width - 5, base.Height - 3);
        }
        graphics.DrawRectangle(Pens.Black, 0, 0, base.Width - 1, base.Height - 1);
        graphics.DrawRectangle(new Pen(Color.FromArgb(40, 40, 40)), 1, 1, base.Width - 3, base.Height - 3);
        graphics.FillPolygon(Brushes.Black, Triangle(new Point(base.Width - 14, base.Height / 2), new Size(5, 3)));
        graphics.FillPolygon(Brushes.White, Triangle(new Point(base.Width - 15, base.Height / 2 - 1), new Size(5, 3)));
        e.Graphics.DrawImage((Bitmap)bitmap.Clone(), 0, 0);
        graphics.Dispose();
        bitmap.Dispose();
    }

    protected override void OnDrawItem(DrawItemEventArgs e)
    {
        if (e.Index >= 0)
        {
            Rectangle rectangle = new Rectangle
            {
                X = e.Bounds.X,
                Y = e.Bounds.Y,
                Width = e.Bounds.Width - 1,
                Height = e.Bounds.Height - 1
            };
            e.DrawBackground();
            if ((e.State == (DrawItemState.Focus | DrawItemState.NoAccelerator | DrawItemState.NoFocusRect | DrawItemState.Selected)) | (e.State == (DrawItemState.Focus | DrawItemState.Selected)))
            {
                e.Graphics.FillRectangle(new SolidBrush(BackColor), e.Bounds);
                Rectangle rect = new Rectangle(e.Bounds.Location, new Size(e.Bounds.Width + 2, e.Bounds.Height));
                Rectangle rect2 = new Rectangle(rect.Location, new Size(rect.Width, rect.Height / 2 - 1));
                LinearGradientBrush linearGradientBrush = new LinearGradientBrush(new Point(rect.X, rect.Y), new Point(rect.X, rect.Y + rect.Height), Color.FromArgb(63, 67, 68), Color.FromArgb(23, 27, 28));
                HatchBrush brush = new HatchBrush(HatchStyle.DarkUpwardDiagonal, Color.FromArgb(63, 67, 68), Color.Transparent);
                e.Graphics.FillRectangle(linearGradientBrush, rect);
                linearGradientBrush.Dispose();
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(43, 47, 48)), rect2);
                e.Graphics.FillRectangle(brush, rect);
                linearGradientBrush.Dispose();
                e.Graphics.DrawString(" " + base.Items[e.Index].ToString(), Font, Brushes.White, e.Bounds.X, e.Bounds.Y + 1);
            }
            else
            {
                e.Graphics.FillRectangle(new SolidBrush(BackColor), e.Bounds);
                e.Graphics.DrawString(" " + base.Items[e.Index].ToString(), Font, Brushes.Gray, e.Bounds.X, e.Bounds.Y + 1);
            }
            base.OnDrawItem(e);
        }
    }

    public Point[] Triangle(Point Location, Size Size)
    {
        return new Point[4]
        {
            Location,
            new Point(Location.X + Size.Width, Location.Y),
            new Point(Location.X + Size.Width / 2, Location.Y + Size.Height),
            Location
        };
    }

    private void GhostComboBox_DropDownClosed(object sender, EventArgs e)
    {
        base.DropDownStyle = ComboBoxStyle.Simple;
        Application.DoEvents();
        base.DropDownStyle = ComboBoxStyle.DropDownList;
    }

    private void GhostCombo_TextChanged(object sender, EventArgs e)
    {
        Invalidate();
    }

    private void InitializeComponent()
    {
        base.SuspendLayout();
        base.SelectedIndexChanged += new System.EventHandler(HeroComboBox_SelectedIndexChanged);
        base.ResumeLayout(false);
    }

    protected override void OnClick(EventArgs e)
    {
        base.OnClick(e);
        Invalidate();
    }

    private void HeroComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        base.SelectedIndexChanged += HeroComboBox_SelectedIndexChanged;
        Invalidate();
    }
}
