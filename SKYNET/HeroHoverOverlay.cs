using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SKYNET;

/// <summary>
/// Control flotante que aparece sobre un HeroPortrait en hover.
/// Replica el estilo del Dota 2 oficial: video animado + nombre con gradiente abajo.
/// </summary>
public class HeroHoverOverlay : Control
{
    private Image _heroImage;
    private string _heroDisplayName = "";
    private string _heroNameLine1 = "";
    private string _heroNameLine2 = "";
    private float _animationProgress;
    private readonly System.Windows.Forms.Timer _animationTimer;
    private bool _isExpanding;
    private Rectangle _targetBounds;
    private Rectangle _originBounds;

    // Componentes internos
    private PictureBox _videoBox;  // será TransparentPictureBox (subclase)
    private Media _media;
    private bool _videoStarted;
    private string _pendingVideoPath;

    // Configuración visual
    private const int AnimationSteps = 6;       // ~90ms total
    private const float ZoomFactor = 1.55f;
    private const int LabelAreaHeight = 36;     // alto del área del nombre dentro del retrato

    public bool IsAnimating => _animationTimer.Enabled;

    public HeroHoverOverlay()
    {
        SetStyle(
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.ResizeRedraw,
            true);

        BackColor = Color.FromArgb(20, 22, 28);
        Visible = false;

        _animationTimer = new System.Windows.Forms.Timer { Interval = 15 };
        _animationTimer.Tick += AnimationTimer_Tick;

        // PictureBox interno donde se reproduce el video
        _videoBox = new TransparentPictureBox
        {
            SizeMode = PictureBoxSizeMode.StretchImage,
            BackColor = Color.Black,
            Visible = false
        };
        Controls.Add(_videoBox);
    }

    /// <summary>
    /// PictureBox que deja pasar todos los eventos de mouse al control de abajo.
    /// Necesario para que el video no rompa el hit-testing del overlay.
    /// </summary>
    private class TransparentPictureBox : PictureBox
    {
        private const int WM_NCHITTEST = 0x84;
        private const int HTTRANSPARENT = -1;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_NCHITTEST)
            {
                m.Result = (IntPtr)HTTRANSPARENT;
                return;
            }
            base.WndProc(ref m);
        }
    }

    // ════════════════════════════════════════════════════════════
    // API PÚBLICA
    // ════════════════════════════════════════════════════════════

    public void ShowOver(Rectangle portraitBounds, Image heroImage, string displayName, string videoPath)
    {
        // Detener cualquier video anterior
        StopVideo();

        _heroImage = heroImage;
        _heroDisplayName = displayName ?? "";
        SplitName(_heroDisplayName);
        _pendingVideoPath = videoPath;
        _videoStarted = false;

        _originBounds = portraitBounds;

        // Calcular bounds objetivo
        int newWidth = (int)(portraitBounds.Width * ZoomFactor);
        int newHeight = (int)(portraitBounds.Height * ZoomFactor);
        int newX = portraitBounds.X + (portraitBounds.Width - newWidth) / 2;
        int newY = portraitBounds.Y + (portraitBounds.Height - newHeight) / 2;

        _targetBounds = new Rectangle(newX, newY, newWidth, newHeight);

        // Posicionar y mostrar
        Bounds = _originBounds;
        BringToFront();
        Visible = true;

        _isExpanding = true;
        _animationProgress = 0f;
        _animationTimer.Start();
    }

    public void Hide()
    {
        if (!Visible) return;

        _isExpanding = false;
        _animationProgress = 1f;
        _animationTimer.Start();
    }

    // ════════════════════════════════════════════════════════════
    // ANIMACIÓN
    // ════════════════════════════════════════════════════════════

    private void AnimationTimer_Tick(object sender, EventArgs e)
    {
        float step = 1f / AnimationSteps;

        if (_isExpanding)
        {
            _animationProgress += step;
            if (_animationProgress >= 1f)
            {
                _animationProgress = 1f;
                _animationTimer.Stop();
                StartVideoIfReady();
            }
        }
        else
        {
            _animationProgress -= step;
            if (_animationProgress <= 0f)
            {
                _animationProgress = 0f;
                _animationTimer.Stop();
                StopVideo();
                Visible = false;
                return;
            }
        }

        float eased = EaseOutQuad(_animationProgress);
        Bounds = InterpolateBounds(_originBounds, _targetBounds, eased);

        // Reposicionar el videoBox para que ocupe todo el control
        _videoBox.Bounds = new Rectangle(0, 0, Width, Height);

        Invalidate();
    }

    private static float EaseOutQuad(float t) => 1f - (1f - t) * (1f - t);

    private static Rectangle InterpolateBounds(Rectangle a, Rectangle b, float t)
    {
        return new Rectangle(
            (int)(a.X + (b.X - a.X) * t),
            (int)(a.Y + (b.Y - a.Y) * t),
            (int)(a.Width + (b.Width - a.Width) * t),
            (int)(a.Height + (b.Height - a.Height) * t));
    }

    // ════════════════════════════════════════════════════════════
    // VIDEO
    // ════════════════════════════════════════════════════════════

    private void StartVideoIfReady()
    {
        if (_videoStarted) return;
        if (string.IsNullOrEmpty(_pendingVideoPath)) return;
        if (!System.IO.File.Exists(_pendingVideoPath)) return;

        try
        {
            _videoBox.Bounds = new Rectangle(0, 0, Width, Height);
            _videoBox.Visible = true;
            _videoBox.BringToFront();

            _media = new Media();
            _media.Open(_pendingVideoPath, _videoBox);
            _media.Repeat = true;
            _media.Play();

            _videoStarted = true;
            Invalidate();
        }
        catch
        {
            // Si falla, no pasa nada — se queda la imagen estática
            _videoBox.Visible = false;
        }
    }

    private void StopVideo()
    {
        try
        {
            _media?.Stop();
            _media = null;
            _videoStarted = false;
            _videoBox.Visible = false;
            _videoBox.Image = null;
        }
        catch { }
    }

    // ════════════════════════════════════════════════════════════
    // DIBUJO
    // ════════════════════════════════════════════════════════════

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        // No pintamos fondo — el video o la imagen ocupan todo
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;

        var rect = new Rectangle(0, 0, Width, Height);

        // 1. Imagen estática del héroe (mientras el video no está listo)
        if (!_videoStarted && _heroImage != null)
        {
            g.DrawImage(_heroImage, rect);
        }

        // 2. Gradiente oscuro en la parte inferior (donde va el nombre)
        if (_animationProgress > 0.3f)
        {
            DrawNameGradient(g, rect);
        }

        // 3. Nombre del héroe
        if (_animationProgress > 0.5f)
        {
            DrawHeroName(g, rect);
        }

        // 4. Borde sutil (último para que quede encima de todo)
        if (_animationProgress > 0.2f)
        {
            int alpha = (int)(200 * _animationProgress);
            using var borderPen = new Pen(Color.FromArgb(alpha, 200, 200, 210), 1f);
            g.DrawRectangle(borderPen,
                new Rectangle(0, 0, Width - 1, Height - 1));
        }
    }

    private void DrawNameGradient(Graphics g, Rectangle bounds)
    {
        int gradientHeight = (int)(LabelAreaHeight * 1.5);
        int gradientY = bounds.Bottom - gradientHeight;

        var gradientRect = new Rectangle(
            bounds.X, gradientY,
            bounds.Width, gradientHeight);

        int alpha = (int)(220 * _animationProgress);

        using var brush = new LinearGradientBrush(
            new Point(0, gradientY),
            new Point(0, bounds.Bottom),
            Color.FromArgb(0, 0, 0, 0),
            Color.FromArgb(alpha, 0, 0, 0));

        g.FillRectangle(brush, gradientRect);
    }

    private void DrawHeroName(Graphics g, Rectangle bounds)
    {
        if (string.IsNullOrEmpty(_heroDisplayName)) return;

        int alpha = (int)(255 * _animationProgress);
        bool isMultiLine = !string.IsNullOrEmpty(_heroNameLine2);

        float fontSize = bounds.Width > 110 ? 11f : 9f;
        using var font = new Font("Segoe UI", fontSize, FontStyle.Bold);

        var sf = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
            FormatFlags = StringFormatFlags.NoWrap
        };

        if (isMultiLine)
        {
            // Línea 1
            var line1Rect = new Rectangle(
                bounds.X,
                bounds.Bottom - LabelAreaHeight - 2,
                bounds.Width,
                LabelAreaHeight / 2);

            // Línea 2
            var line2Rect = new Rectangle(
                bounds.X,
                bounds.Bottom - (LabelAreaHeight / 2) - 2,
                bounds.Width,
                LabelAreaHeight / 2);

            DrawTextWithShadow(g, _heroNameLine1, font, line1Rect, sf, alpha);
            DrawTextWithShadow(g, _heroNameLine2, font, line2Rect, sf, alpha);
        }
        else
        {
            var nameRect = new Rectangle(
                bounds.X,
                bounds.Bottom - LabelAreaHeight,
                bounds.Width,
                LabelAreaHeight);

            DrawTextWithShadow(g, _heroDisplayName, font, nameRect, sf, alpha);
        }
    }

    private void DrawTextWithShadow(Graphics g, string text, Font font, Rectangle rect, StringFormat sf, int alpha)
    {
        // Sombra
        var shadowRect = rect;
        shadowRect.Offset(1, 1);
        using (var shadowBrush = new SolidBrush(Color.FromArgb(alpha, 0, 0, 0)))
        {
            g.DrawString(text.ToUpper(), font, shadowBrush, shadowRect, sf);
        }

        // Texto principal
        using var textBrush = new SolidBrush(Color.FromArgb(alpha, 245, 245, 250));
        g.DrawString(text.ToUpper(), font, textBrush, rect, sf);
    }

    // ════════════════════════════════════════════════════════════
    // HELPERS
    // ════════════════════════════════════════════════════════════

    /// <summary>
    /// Divide nombres de más de 1 palabra en 2 líneas (ANCIENT \n APPARITION).
    /// Nombres de una sola palabra se dejan en _heroDisplayName y _line2 queda vacío.
    /// </summary>
    private void SplitName(string fullName)
    {
        if (string.IsNullOrEmpty(fullName))
        {
            _heroNameLine1 = "";
            _heroNameLine2 = "";
            return;
        }

        // Solo dividimos si tiene 2+ palabras Y el nombre es largo (>10 caracteres)
        var parts = fullName.Split(new[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 2 && fullName.Length > 10)
        {
            // Si tiene 2 palabras: una arriba, otra abajo
            if (parts.Length == 2)
            {
                _heroNameLine1 = parts[0];
                _heroNameLine2 = parts[1];
            }
            else
            {
                // Si tiene 3+: dividir por la mitad
                int mid = parts.Length / 2;
                _heroNameLine1 = string.Join(" ", parts, 0, mid);
                _heroNameLine2 = string.Join(" ", parts, mid, parts.Length - mid);
            }
        }
        else
        {
            _heroNameLine1 = fullName;
            _heroNameLine2 = "";
        }
    }

    // ════════════════════════════════════════════════════════════
    // DISPOSE
    // ════════════════════════════════════════════════════════════

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            StopVideo();
            _animationTimer?.Dispose();
        }
        base.Dispose(disposing);
    }

    // ════════════════════════════════════════════════════════════
    // PASS-THROUGH TOTAL
    // El overlay deja pasar TODOS los eventos de mouse al control de abajo.
    // Sin esto, el cursor "ve" el overlay y dispara loops de HoverEnter/Leave.
    // ════════════════════════════════════════════════════════════

    private const int WM_NCHITTEST = 0x84;
    private const int HTTRANSPARENT = -1;

    protected override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;
            // WS_EX_TRANSPARENT (0x20) + WS_EX_LAYERED (0x80000)
            // Hace que el control sea visualmente visible pero invisible al mouse
            cp.ExStyle |= 0x20;
            return cp;
        }
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WM_NCHITTEST)
        {
            m.Result = (IntPtr)HTTRANSPARENT;
            return;
        }
        base.WndProc(ref m);
    }
}