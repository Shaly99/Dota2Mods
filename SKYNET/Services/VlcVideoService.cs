using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibVLCSharp.Shared;

namespace SKYNET;

/// <summary>
/// Reproduce videos con renderizado manual a Bitmap.
/// El consumidor pinta los frames donde quiera, con composición alpha real.
/// </summary>
public sealed class VlcVideoService : IVideoService, IDisposable
{
    private LibVLC? _libVlc;
    private bool _initialized;
    private readonly object _initLock = new object();

    private readonly ConcurrentDictionary<object, VideoEntry> _videos
        = new ConcurrentDictionary<object, VideoEntry>();

    private sealed class VideoEntry
    {
        public VlcFrameRenderer Renderer { get; set; } = null!;
        public MediaPlayer Player { get; set; } = null!;
    }

    public void Initialize()
    {
        if (_initialized) return;

        lock (_initLock)
        {
            if (_initialized) return;

            try
            {
                Core.Initialize();
                _libVlc = new LibVLC(
                    "--no-audio",
                    "--no-video-title-show",
                    "--quiet",
                    "--no-snapshot-preview"
                );
                _initialized = true;
            }
            catch (Exception ex)
            {
                modCommon.Save(ex);
            }
        }
    }

    /// <summary>
    /// Reproduce un video y devuelve el renderer para pintarlo manualmente.
    /// El consumidor debe llamar a renderer.DrawCurrentFrame() en su OnPaint.
    /// </summary>
    public VlcFrameRenderer? PlayForRenderer(string videoPath, object owner)
    {
        if (string.IsNullOrEmpty(videoPath)) return null;
        if (owner == null) return null;
        if (!System.IO.File.Exists(videoPath)) return null;

        if (!_initialized)
        {
            Initialize();
            if (!_initialized) return null;
        }

        try
        {
            // Si ya hay un video para este owner, lo detenemos
            StopForOwner(owner);

            var renderer = new VlcFrameRenderer();
            var player = new MediaPlayer(_libVlc!);
            renderer.AttachPlayer(player);

            var entry = new VideoEntry { Renderer = renderer, Player = player };
            _videos[owner] = entry;

            var media = new LibVLCSharp.Shared.Media(_libVlc!, videoPath, FromType.FromPath);
            media.AddOption(":input-repeat=65535");

            Task.Run(() =>
            {
                try { player.Play(media); }
                catch (Exception ex) { modCommon.Save(ex); }
                finally { try { media.Dispose(); } catch { } }
            });

            return renderer;
        }
        catch (Exception ex)
        {
            modCommon.Save(ex);
            return null;
        }
    }

    public void StopForOwner(object owner)
    {
        if (owner == null) return;
        if (!_videos.TryRemove(owner, out var entry)) return;

        Task.Run(() =>
        {
            try { entry.Player.Stop(); } catch { }
            try { entry.Renderer.DetachPlayer(); } catch { }
            try { entry.Player.Dispose(); } catch { }
            try { entry.Renderer.Dispose(); } catch { }
        });
    }

    public void StopAll()
    {
        foreach (var owner in _videos.Keys)
        {
            StopForOwner(owner);
        }
    }

    // ════════════════════════════════════════════════════════════
    // API VIEJA (compatibilidad con HeroAvatar de frmMain)
    // ════════════════════════════════════════════════════════════

    public bool PlayLoop(string videoPath, Control container)
    {
        // Para HeroAvatar — usa un PictureBox simple con el video adentro
        if (container == null || container.IsDisposed) return false;

        var renderer = PlayForRenderer(videoPath, container);
        if (renderer == null) return false;

        // Crear un control simple que pinte los frames
        var paintControl = new SimpleVideoPaintControl(renderer)
        {
            Dock = DockStyle.Fill
        };

        // Sacar controles previos
        for (int i = container.Controls.Count - 1; i >= 0; i--)
        {
            if (container.Controls[i] is SimpleVideoPaintControl old)
            {
                container.Controls.RemoveAt(i);
                old.Dispose();
            }
        }

        container.Controls.Add(paintControl);
        paintControl.SendToBack();

        return true;
    }

    public void Stop(Control container)
    {
        if (container == null) return;
        StopForOwner(container);

        // Limpiar control de pintado
        for (int i = container.Controls.Count - 1; i >= 0; i--)
        {
            if (container.Controls[i] is SimpleVideoPaintControl old)
            {
                container.Controls.RemoveAt(i);
                old.Dispose();
            }
        }
    }

    public void Release(Control container) => Stop(container);

    public VlcFrameRenderer? GetRenderer(Control container)
    {
        return _videos.TryGetValue(container, out var entry) ? entry.Renderer : null;
    }

    // ════════════════════════════════════════════════════════════
    // CONTROL INTERNO para HeroAvatar
    // ════════════════════════════════════════════════════════════

    private sealed class SimpleVideoPaintControl : Control
    {
        private readonly VlcFrameRenderer _renderer;

        public SimpleVideoPaintControl(VlcFrameRenderer renderer)
        {
            _renderer = renderer;
            SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.ResizeRedraw,
                true);
            BackColor = Color.Black;

            _renderer.FrameReady += OnFrameReady;
        }

        private void OnFrameReady()
        {
            if (IsDisposed || !IsHandleCreated) return;
            try
            {
                BeginInvoke((Action)(() => { if (!IsDisposed) Invalidate(); }));
            }
            catch { }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            _renderer.DrawCurrentFrame(e.Graphics, ClientRectangle);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _renderer.FrameReady -= OnFrameReady;
            }
            base.Dispose(disposing);
        }
    }

    public void Dispose()
    {
        foreach (var owner in _videos.Keys)
        {
            StopForOwner(owner);
        }
        try { _libVlc?.Dispose(); } catch { }
    }
}