using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;

namespace SKYNET;

/// <summary>
/// UserControl que representa el retrato de un héroe en el grid.
/// Maneja hover, click y comunicación con el overlay de animación.
/// </summary>
public class HeroPortrait : Control
{
    private Image _heroImage;
    private bool _isHovered;
    private bool _isDimmed;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string HeroName { get; set; }
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string HeroDisplayName { get; set; }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Image HeroImage
    {
        get => _heroImage;
        set { _heroImage = value; Invalidate(); }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool IsDimmed
    {
        get => _isDimmed;
        set { _isDimmed = value; Invalidate(); }
    }

    public event EventHandler<HeroPortrait> HeroClicked;

    public HeroPortrait()
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
    }

    // ════════════════════════════════════════════════════════════
    // DIBUJO
    // ════════════════════════════════════════════════════════════

    protected override void OnPaint(PaintEventArgs e)
    {
        if (_heroImage == null) return;

        var g = e.Graphics;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;

        var rect = new Rectangle(0, 0, Width, Height);

        // Aplicar dim si está marcado (por filtro de teclado)
        if (_isDimmed)
        {
            // Dibujar la imagen con opacidad reducida
            var attrs = new System.Drawing.Imaging.ImageAttributes();
            var matrix = new System.Drawing.Imaging.ColorMatrix
            {
                Matrix33 = 0.25f  // 25% de opacidad
            };
            attrs.SetColorMatrix(matrix);

            g.DrawImage(_heroImage, rect, 0, 0,
                _heroImage.Width, _heroImage.Height,
                GraphicsUnit.Pixel, attrs);
        }
        else
        {
            g.DrawImage(_heroImage, rect);
        }
    }

    // ════════════════════════════════════════════════════════════
    // EVENTOS DE MOUSE
    // ════════════════════════════════════════════════════════════

    protected override void OnClick(EventArgs e)
    {
        base.OnClick(e);
        HeroClicked?.Invoke(this, this);
    }
}