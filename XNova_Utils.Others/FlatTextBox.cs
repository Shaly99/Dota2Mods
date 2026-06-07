using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace XNova_Utils.Others;

[DefaultEvent("TextChanged")]
internal class FlatTextBox : Control
{
    public TextBox iTalkTB = new TextBox();

    private GraphicsPath Shape;

    private int _maxchars = 32767;

    private bool _ReadOnly;

    private bool _Multiline;

    private HorizontalAlignment ALNType;

    private bool isPasswordMasked = false;

    private Pen P1;

    private SolidBrush B1;

    private Color _BackColorControl { get; set; }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color BackColorControl
    {
        get
        {
            return _BackColorControl;
        }
        set
        {
            _BackColorControl = value;
        }
    }

    private bool _OnlyNumber { get; set; }

    [DefaultValue(false)]
    public bool OnlyNumber
    {
        get
        {
            return _OnlyNumber;
        }
        set
        {
            _OnlyNumber = value;
        }
    }

    private bool _isCustomColor { get; set; }

    [DefaultValue(false)]
    public bool isCustomColor
    {
        get
        {
            return _isCustomColor;
        }
        set
        {
            _isCustomColor = value;
        }
    }

    [DefaultValue(HorizontalAlignment.Left)]
    public HorizontalAlignment TextAlignment
    {
        get
        {
            return ALNType;
        }
        set
        {
            ALNType = value;
            Invalidate();
        }
    }

    [DefaultValue(32767)]
    public int MaxLength
    {
        get
        {
            return _maxchars;
        }
        set
        {
            _maxchars = value;
            iTalkTB.MaxLength = MaxLength;
            Invalidate();
        }
    }

    [DefaultValue(false)]
    public bool UseSystemPasswordChar
    {
        get
        {
            return isPasswordMasked;
        }
        set
        {
            iTalkTB.UseSystemPasswordChar = UseSystemPasswordChar;
            isPasswordMasked = value;
            Invalidate();
        }
    }

    [DefaultValue(false)]
    public bool ReadOnly
    {
        get
        {
            return _ReadOnly;
        }
        set
        {
            _ReadOnly = value;
            if (iTalkTB != null)
            {
                iTalkTB.ReadOnly = value;
            }
        }
    }

    [DefaultValue(false)]
    public bool Multiline
    {
        get
        {
            return _Multiline;
        }
        set
        {
            _Multiline = value;
            if (iTalkTB != null)
            {
                iTalkTB.Multiline = value;
                if (value)
                {
                    iTalkTB.Height = base.Height - 10;
                }
                else
                {
                    base.Height = iTalkTB.Height + 10;
                }
            }
        }
    }

    protected override void OnTextChanged(EventArgs e)
    {
        base.OnTextChanged(e);
        iTalkTB.Text = Text;
        Invalidate();
    }

    protected void OnDoubleClick(object s, EventArgs e)
    {
        base.OnDoubleClick(e);
    }

    private void OnBaseTextChanged(object s, EventArgs e)
    {
        Text = iTalkTB.Text;
    }

    protected override void OnForeColorChanged(EventArgs e)
    {
        base.OnForeColorChanged(e);
        iTalkTB.ForeColor = ForeColor;
        Invalidate();
    }

    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);
        iTalkTB.Font = Font;
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        base.OnPaintBackground(e);
    }

    private new void KeyDown(object Obj, KeyEventArgs e)
    {
        if (_OnlyNumber)
        {
            switch (e.KeyData.ToString())
            {
                case "D0":
                    base.OnKeyDown(e);
                    break;
                case "D1":
                    base.OnKeyDown(e);
                    break;
                case "D2":
                    base.OnKeyDown(e);
                    break;
                case "D3":
                    base.OnKeyDown(e);
                    break;
                case "D4":
                    base.OnKeyDown(e);
                    break;
                case "D5":
                    base.OnKeyDown(e);
                    break;
                case "D6":
                    base.OnKeyDown(e);
                    break;
                case "D7":
                    base.OnKeyDown(e);
                    break;
                case "D8":
                    base.OnKeyDown(e);
                    break;
                case "D9":
                    base.OnKeyDown(e);
                    break;
                case "NumPad0":
                    base.OnKeyDown(e);
                    break;
                case "NumPad1":
                    base.OnKeyDown(e);
                    break;
                case "NumPad2":
                    base.OnKeyDown(e);
                    break;
                case "NumPad3":
                    base.OnKeyDown(e);
                    break;
                case "NumPad4":
                    base.OnKeyDown(e);
                    break;
                case "NumPad5":
                    base.OnKeyDown(e);
                    break;
                case "NumPad6":
                    base.OnKeyDown(e);
                    break;
                case "NumPad7":
                    base.OnKeyDown(e);
                    break;
                case "NumPad8":
                    base.OnKeyDown(e);
                    break;
                case "NumPad9":
                    base.OnKeyDown(e);
                    break;
                case "Delete":
                    base.OnKeyDown(e);
                    break;
                case "Back":
                    base.OnKeyDown(e);
                    break;
                case "Left":
                    base.OnKeyDown(e);
                    break;
                case "Up":
                    base.OnKeyDown(e);
                    break;
                case "Right":
                    base.OnKeyDown(e);
                    break;
                case "Down":
                    base.OnKeyDown(e);
                    break;
                default:
                    e.SuppressKeyPress = true;
                    break;
            }
        }
        else
        {
            base.OnKeyDown(e);
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        if (_Multiline)
        {
            iTalkTB.Height = base.Height - 10;
        }
        else
        {
            base.Height = iTalkTB.Height + 10;
        }
        Shape = new GraphicsPath();
        GraphicsPath shape = Shape;
        shape.AddArc(0, 0, 10, 10, 180f, 90f);
        shape.AddArc(base.Width - 11, 0, 10, 10, -90f, 90f);
        shape.AddArc(base.Width - 11, base.Height - 11, 10, 10, 0f, 90f);
        shape.AddArc(0, base.Height - 11, 10, 10, 90f, 90f);
        shape.CloseAllFigures();
    }

    protected override void OnGotFocus(EventArgs e)
    {
        base.OnGotFocus(e);
        iTalkTB.Focus();
    }

    public void AddTextBox()
    {
        TextBox textBox = iTalkTB;
        textBox.Size = new Size(base.Width - 10, 33);
        textBox.Location = new Point(7, 5);
        textBox.Text = string.Empty;
        textBox.BorderStyle = BorderStyle.None;
        textBox.TextAlign = HorizontalAlignment.Left;
        textBox.Font = Font;
        textBox.UseSystemPasswordChar = UseSystemPasswordChar;
        textBox.Multiline = false;
        if (isCustomColor)
        {
            iTalkTB.BackColor = Color.FromArgb(44, 48, 49);
        }
        else
        {
            iTalkTB.BackColor = Color.FromArgb(44, 48, 49);
        }
    }

    public FlatTextBox()
    {
        SetStyle(ControlStyles.SupportsTransparentBackColor, value: true);
        SetStyle(ControlStyles.UserPaint, value: true);
        P1 = new Pen(_BackColorControl);
        B1 = new SolidBrush(_BackColorControl);
        AddTextBox();
        base.Controls.Add(iTalkTB);
        iTalkTB.KeyDown += KeyDown;
        iTalkTB.TextChanged += OnBaseTextChanged;
        iTalkTB.DoubleClick += OnDoubleClick;
        Text = null;
        Font = new Font("Tahoma", 11f);
        base.Size = new Size(135, 30);
        DoubleBuffered = true;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Bitmap bitmap = new Bitmap(base.Width, base.Height);
        Graphics graphics = Graphics.FromImage(bitmap);
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        TextBox textBox = iTalkTB;
        textBox.Width = base.Width - 12;
        textBox.TextAlign = TextAlignment;
        textBox.UseSystemPasswordChar = UseSystemPasswordChar;
        graphics.FillPath(B1, Shape);
        graphics.DrawPath(P1, Shape);
        e.Graphics.DrawImage((Image)bitmap.Clone(), 0, 0);
        graphics.Dispose();
        bitmap.Dispose();
    }
}