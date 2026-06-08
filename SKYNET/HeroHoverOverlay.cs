using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SKYNET;

/// <summary>
/// Overlay flotante con video + label translúcido encima + click.
/// Todo se pinta en un solo OnPaint para tener composición alpha real.
/// </summary>
public class HeroHoverOverlay : Control
{
    private Image _heroImage;
    private string _heroDisplayName = "";
    private string _heroNameLine1 = "";
    private string _heroNameLine2 = "";
    private string _lastShownHero = "";
    private string _currentHeroName = "";
    private float _animationProgress;
    private readonly System.Windows.Forms.Timer _animationTimer;
    private readonly System.Windows.Forms.Timer _videoDebounceTimer;
    private bool _isExpanding;
    private Rectangle _targetBounds;
    private Rectangle _originBounds;

    private VlcFrameRenderer _videoRenderer;
    private string _pendingVideoPath;

    private const int AnimationSteps = 4;
    private const float ZoomFactor = 2.8f;
    // 15% de la altura del overlay (se recalcula dinámicamente)
    private int LabelOverlayHeight => (int)(Height * 0.20f);
    private const int VideoDebounceMs = 50;

    public bool IsAnimating => _animationTimer.Enabled;

    public event EventHandler<string> HeroClickedFromOverlay;

    public HeroHoverOverlay()
    {
        SetStyle(
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.ResizeRedraw,
            true);

        BackColor = Color.Black;
        Visible = false;
        Cursor = Cursors.Hand;

        _animationTimer = new System.Windows.Forms.Timer { Interval = 15 };
        _animationTimer.Tick += AnimationTimer_Tick;

        _videoDebounceTimer = new System.Windows.Forms.Timer { Interval = VideoDebounceMs };
        _videoDebounceTimer.Tick += VideoDebounceTimer_Tick;
    }

    // ════════════════════════════════════════════════════════════
    // API
    // ════════════════════════════════════════════════════════════

    public void ShowOver(Rectangle portraitBounds, Image heroImage, string heroName, string displayName, string videoPath)
    {
        if (Visible && _lastShownHero == displayName) return;
        _lastShownHero = displayName;
        _currentHeroName = heroName;

        StopVideo();
        _videoDebounceTimer.Stop();

        _heroImage = heroImage;
        _heroDisplayName = displayName ?? "";
        SplitName(_heroDisplayName);
        _pendingVideoPath = videoPath;

        _originBounds = portraitBounds;

        int newWidth = (int)(portraitBounds.Width * ZoomFactor);
        int newHeight = (int)(portraitBounds.Height * ZoomFactor);
        int newX = portraitBounds.X + (portraitBounds.Width - newWidth) / 2;
        int newY = portraitBounds.Y + (portraitBounds.Height - newHeight) / 2;

        if (Parent != null)
        {
            if (newX < 0) newX = 0;
            if (newY < 0) newY = 0;
            if (newX + newWidth > Parent.Width) newX = Parent.Width - newWidth;
            if (newY + newHeight > Parent.Height) newY = Parent.Height - newHeight;
        }

        _targetBounds = new Rectangle(newX, newY, newWidth, newHeight);

        Bounds = _originBounds;
        BringToFront();
        Visible = true;

        _isExpanding = true;
        _animationProgress = 0f;
        _animationTimer.Start();
        _videoDebounceTimer.Start();
    }

    public void Hide()
    {
        if (!Visible) return;
        _videoDebounceTimer.Stop();
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
                _lastShownHero = "";
                _currentHeroName = "";
                Visible = false;
                return;
            }
        }

        float eased = EaseOutQuad(_animationProgress);
        Bounds = InterpolateBounds(_originBounds, _targetBounds, eased);

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

    private void VideoDebounceTimer_Tick(object sender, EventArgs e)
    {
        _videoDebounceTimer.Stop();
        StartVideoIfReady();
    }

    private void StartVideoIfReady()
    {
        if (string.IsNullOrEmpty(_pendingVideoPath)) return;

        try
        {
            var videoService = ServiceContainer.TryGet<IVideoService>();
            if (videoService == null) return;

            _videoRenderer = videoService.PlayForRenderer(_pendingVideoPath, this);
            if (_videoRenderer != null)
            {
                _videoRenderer.FrameReady += OnVideoFrameReady;
            }
        }
        catch { }
    }

    private void OnVideoFrameReady()
    {
        if (IsDisposed || !IsHandleCreated) return;
        try
        {
            BeginInvoke((Action)(() => { if (!IsDisposed) Invalidate(); }));
        }
        catch { }
    }

    private void StopVideo()
    {
        try
        {
            if (_videoRenderer != null)
            {
                _videoRenderer.FrameReady -= OnVideoFrameReady;
                _videoRenderer = null;
            }

            var videoService = ServiceContainer.TryGet<IVideoService>();
            videoService?.StopForOwner(this);
        }
        catch { }
    }

    // ════════════════════════════════════════════════════════════
    // DIBUJO — TODO en una sola superficie (composición alpha real)
    // ════════════════════════════════════════════════════════════

    protected override void OnPaintBackground(PaintEventArgs e) { }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;

        // Solo borde negro fino exterior (sin borde interior)
        const int outerBorder = 1;

        var fullRect = new Rectangle(0, 0, Width, Height);
        var videoRect = new Rectangle(
            outerBorder, outerBorder,
            Width - outerBorder * 2,
            Height - outerBorder * 2);

        // 1. Borde negro exterior
        if (_animationProgress > 0.1f)
        {
            int alpha = (int)(255 * _animationProgress);
            using var blackBrush = new SolidBrush(Color.FromArgb(alpha, 0, 0, 0));
            g.FillRectangle(blackBrush, fullRect);
        }

        // 2. Video o imagen del héroe
        if (_videoRenderer != null && _videoRenderer.HasFrame)
        {
            _videoRenderer.DrawCurrentFrame(g, videoRect);
        }
        else if (_heroImage != null)
        {
            g.DrawImage(_heroImage, videoRect);
        }

        // 3. Panel del nombre (negro casi opaco)
        int labelHeight = LabelOverlayHeight;
        if (_animationProgress > 0.2f)
        {
            int alpha = (int)(130 * _animationProgress);
            var nameRect = new Rectangle(
                videoRect.X,
                videoRect.Bottom - labelHeight,
                videoRect.Width,
                labelHeight);

            using var nameBrush = new SolidBrush(Color.FromArgb(alpha, 5, 7, 10));
            g.FillRectangle(nameBrush, nameRect);
        }

        // 4. Nombre del héroe
        if (_animationProgress > 0.4f)
        {
            var labelRect = new Rectangle(
                videoRect.X,
                videoRect.Bottom - labelHeight,
                videoRect.Width,
                labelHeight);
            DrawHeroName(g, labelRect);
        }
    }

    private void DrawHeroName(Graphics g, Rectangle bounds)
    {
        if (string.IsNullOrEmpty(_heroDisplayName)) return;

        int alpha = (int)(255 * _animationProgress);
        bool isMultiLine = !string.IsNullOrEmpty(_heroNameLine2);

        using var font = FontService.GetRadiance(10f);
        using var whiteBrush = new SolidBrush(Color.FromArgb(alpha, 255, 255, 255));

        var sf = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
            FormatFlags = StringFormatFlags.NoWrap
        };

        if (isMultiLine)
        {
            string upper1 = _heroNameLine1.ToUpper();
            string upper2 = _heroNameLine2.ToUpper();

            // Altura real de UNA línea de texto (sin padding extra de la font)
            float lineHeight = font.GetHeight(g);

            // Solapado leve entre líneas (negativo = se pegan)
            float lineOverlap = -3f;

            float totalHeight = (lineHeight * 2) + lineOverlap;

            // Centrar verticalmente el bloque entero
            float startY = bounds.Y + (bounds.Height - totalHeight) / 1.3f;

            var line1Rect = new RectangleF(bounds.X, startY, bounds.Width, lineHeight);
            var line2Rect = new RectangleF(bounds.X, startY + lineHeight + lineOverlap, bounds.Width, lineHeight);

            g.DrawString(upper1, font, whiteBrush, line1Rect, sf);
            g.DrawString(upper2, font, whiteBrush, line2Rect, sf);
        }
        else
        {
            g.DrawString(_heroDisplayName.ToUpper(), font, whiteBrush, bounds, sf);
        }
    }

    private void DrawShadowed(Graphics g, string text, Font font, Rectangle rect, StringFormat sf, int alpha)
    {
        var shadowRect = rect;
        shadowRect.Offset(1, 1);
        using (var shadow = new SolidBrush(Color.FromArgb(alpha, 0, 0, 0)))
            g.DrawString(text.ToUpper(), font, shadow, shadowRect, sf);

        using var brush = new SolidBrush(Color.FromArgb(alpha, 245, 245, 250));
        g.DrawString(text.ToUpper(), font, brush, rect, sf);
    }

    // ════════════════════════════════════════════════════════════
    // CLICK — sin pass-through, el overlay captura el click y dispara el evento
    // ════════════════════════════════════════════════════════════

    protected override void OnClick(EventArgs e)
    {
        base.OnClick(e);
        if (!string.IsNullOrEmpty(_currentHeroName))
        {
            HeroClickedFromOverlay?.Invoke(this, _currentHeroName);
        }
    }

    // ════════════════════════════════════════════════════════════
    // HELPERS
    // ════════════════════════════════════════════════════════════

    private void SplitName(string fullName)
    {
        if (string.IsNullOrEmpty(fullName))
        {
            _heroNameLine1 = "";
            _heroNameLine2 = "";
            return;
        }

        var parts = fullName.Split(new[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 2 && fullName.Length > 10)
        {
            if (parts.Length == 2)
            {
                _heroNameLine1 = parts[0];
                _heroNameLine2 = parts[1];
            }
            else
            {
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

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            StopVideo();
            _videoDebounceTimer?.Dispose();
            _animationTimer?.Dispose();
        }
        base.Dispose(disposing);
    }
}