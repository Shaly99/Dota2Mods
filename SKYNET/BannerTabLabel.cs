using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace SKYNET;

/// <summary>
/// Tab label custom para el banner superior estilo Dota 2 oficial.
/// - Font Radiance Bold con tracking aumentado entre letras
/// - Outline sutil oscuro detrás del texto
/// - Color blanco si activo, gris si inactivo
/// - Auto-calcula su ancho según el texto
/// </summary>
public class BannerTabLabel : Control
{
    private bool _isActive;
    private bool _isHovered;
    private float _letterSpacing = 0.8f;

    // Colores Dota oficial
    private static readonly Color ColorActive = Color.FromArgb(255, 255, 255, 255);
    private static readonly Color ColorInactive = Color.FromArgb(255, 168, 175, 184);
    private static readonly Color ColorHover = Color.FromArgb(255, 220, 225, 232);
    private static readonly Color ColorOutline = Color.FromArgb(180, 0, 0, 0);

    public event EventHandler TabClicked;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;
            Invalidate();
        }
    }

    /// <summary>
    /// Espacio extra en píxeles entre cada letra.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public float LetterSpacing
    {
        get => _letterSpacing;
        set
        {
            _letterSpacing = value;
            RecalculateSize();
            Invalidate();
        }
    }

    public BannerTabLabel()
    {
        SetStyle(
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.ResizeRedraw |
            ControlStyles.SupportsTransparentBackColor,
            true);

        BackColor = Color.Transparent;
        Cursor = Cursors.Hand;
        Font = FontService.GetRadiance(13f);
        Height = 40;
    }

    protected override void OnTextChanged(EventArgs e)
    {
        base.OnTextChanged(e);
        RecalculateSize();
        Invalidate();
    }

    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);
        RecalculateSize();
        Invalidate();
    }

    /// <summary>
    /// Recalcula el ancho del control según el texto + tracking.
    /// </summary>
    private void RecalculateSize()
    {
        if (string.IsNullOrEmpty(Text) || Font == null) return;

        using var bmp = new Bitmap(1, 1);
        using var g = Graphics.FromImage(bmp);
        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

        float totalWidth = 0;
        string upper = Text.ToUpper();

        foreach (char c in upper)
        {
            var charSize = g.MeasureString(c.ToString(), Font, PointF.Empty, StringFormat.GenericTypographic);
            totalWidth += charSize.Width + _letterSpacing;
        }

        // Restar el último spacing (no va después de la última letra)
        if (upper.Length > 0) totalWidth -= _letterSpacing;

        // Padding horizontal
        Width = (int)Math.Ceiling(totalWidth) + 8;
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);
        _isHovered = true;
        Invalidate();
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        _isHovered = false;
        Invalidate();
    }

    protected override void OnClick(EventArgs e)
    {
        base.OnClick(e);
        TabClicked?.Invoke(this, EventArgs.Empty);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        if (string.IsNullOrEmpty(Text) || Font == null) return;

        var g = e.Graphics;
        FontService.ApplyTextSmoothing(g);

        string upper = Text.ToUpper();

        // Color según estado
        Color textColor = _isActive ? ColorActive : (_isHovered ? ColorHover : ColorInactive);

        // Medir cada letra para posicionar con tracking
        float[] letterWidths = new float[upper.Length];
        float totalWidth = 0;

        for (int i = 0; i < upper.Length; i++)
        {
            var size = g.MeasureString(upper[i].ToString(), Font, PointF.Empty, StringFormat.GenericTypographic);
            letterWidths[i] = size.Width;
            totalWidth += size.Width;
            if (i < upper.Length - 1) totalWidth += _letterSpacing;
        }

        // Centrar horizontalmente
        float startX = (Width - totalWidth) / 2f;

        // Centrar verticalmente
        var fontHeight = Font.GetHeight(g);
        float y = (Height - fontHeight) / 2f;

        // Dibujar cada letra con outline + texto
        float currentX = startX;
        using var textBrush = new SolidBrush(textColor);
        using var outlineBrush = new SolidBrush(ColorOutline);

        for (int i = 0; i < upper.Length; i++)
        {
            string ch = upper[i].ToString();

            // Outline sutil (4 direcciones, 2px)
            g.DrawString(ch, Font, outlineBrush, currentX + 2, y);
            g.DrawString(ch, Font, outlineBrush, currentX, y + 2);
            g.DrawString(ch, Font, outlineBrush, currentX + 2, y + 2);


            // Texto principal
            g.DrawString(ch, Font, textBrush, currentX, y);

            currentX += letterWidths[i] + _letterSpacing;
        }
    }
}