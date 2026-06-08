using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using LibVLCSharp.Shared;

namespace SKYNET;

/// <summary>
/// Recibe frames de LibVLC y los expone como Bitmap thread-safe.
/// No es un Control — solo procesa frames y notifica cuando hay uno nuevo.
/// </summary>
public sealed class VlcFrameRenderer : IDisposable
{
    private IntPtr _frameBuffer = IntPtr.Zero;
    private int _videoWidth;
    private int _videoHeight;
    private int _stride;
    private readonly object _bufferLock = new object();
    private Bitmap _currentFrame;

    private MediaPlayer.LibVLCVideoLockCb _lockCallback;
    private MediaPlayer.LibVLCVideoUnlockCb _unlockCallback;
    private MediaPlayer.LibVLCVideoDisplayCb _displayCallback;
    private MediaPlayer.LibVLCVideoFormatCb _formatCallback;
    private MediaPlayer.LibVLCVideoCleanupCb _cleanupCallback;

    private MediaPlayer _player;

    /// <summary>
    /// Se dispara cuando hay un frame nuevo listo para pintar.
    /// </summary>
    public event Action FrameReady;

    public VlcFrameRenderer()
    {
        _lockCallback = OnLock;
        _unlockCallback = OnUnlock;
        _displayCallback = OnDisplay;
        _formatCallback = OnFormat;
        _cleanupCallback = OnCleanup;
    }

    public void AttachPlayer(MediaPlayer player)
    {
        DetachPlayer();
        _player = player;
        _player.SetVideoFormatCallbacks(_formatCallback, _cleanupCallback);
        _player.SetVideoCallbacks(_lockCallback, _unlockCallback, _displayCallback);
    }

    public void DetachPlayer()
    {
        if (_player == null) return;

        try
        {
            _player.SetVideoCallbacks(null, null, null);
            _player.SetVideoFormatCallbacks(null, null);
        }
        catch { }

        _player = null;

        lock (_bufferLock)
        {
            if (_frameBuffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_frameBuffer);
                _frameBuffer = IntPtr.Zero;
            }
            _currentFrame?.Dispose();
            _currentFrame = null;
        }
    }

    /// <summary>
    /// Pinta el frame actual sobre el Graphics dado.
    /// Thread-safe.
    /// </summary>
    public void DrawCurrentFrame(Graphics g, Rectangle destRect)
    {
        lock (_bufferLock)
        {
            if (_currentFrame == null) return;

            try
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(_currentFrame, destRect);
            }
            catch { }
        }
    }

    public bool HasFrame
    {
        get
        {
            lock (_bufferLock)
            {
                return _currentFrame != null;
            }
        }
    }

    // ════════════════════════════════════════════════════════════
    // CALLBACKS DE LIBVLC
    // ════════════════════════════════════════════════════════════

    private uint OnFormat(ref IntPtr opaque, IntPtr chroma, ref uint width, ref uint height,
                          ref uint pitches, ref uint lines)
    {
        byte[] rv32 = System.Text.Encoding.ASCII.GetBytes("RV32");
        Marshal.Copy(rv32, 0, chroma, 4);

        _videoWidth = (int)width;
        _videoHeight = (int)height;
        _stride = _videoWidth * 4;

        pitches = (uint)_stride;
        lines = (uint)_videoHeight;

        lock (_bufferLock)
        {
            if (_frameBuffer != IntPtr.Zero)
                Marshal.FreeHGlobal(_frameBuffer);

            _frameBuffer = Marshal.AllocHGlobal(_stride * _videoHeight);
        }

        return 1;
    }

    private void OnCleanup(ref IntPtr opaque)
    {
        lock (_bufferLock)
        {
            if (_frameBuffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_frameBuffer);
                _frameBuffer = IntPtr.Zero;
            }
        }
    }

    private IntPtr OnLock(IntPtr opaque, IntPtr planes)
    {
        lock (_bufferLock)
        {
            Marshal.WriteIntPtr(planes, _frameBuffer);
            return _frameBuffer;
        }
    }

    private void OnUnlock(IntPtr opaque, IntPtr picture, IntPtr planes)
    {
        lock (_bufferLock)
        {
            if (_frameBuffer == IntPtr.Zero) return;
            if (_videoWidth <= 0 || _videoHeight <= 0) return;

            try
            {
                var newFrame = new Bitmap(_videoWidth, _videoHeight, PixelFormat.Format32bppRgb);
                var bmpData = newFrame.LockBits(
                    new Rectangle(0, 0, _videoWidth, _videoHeight),
                    ImageLockMode.WriteOnly,
                    PixelFormat.Format32bppRgb);

                try
                {
                    int totalBytes = _stride * _videoHeight;
                    unsafe
                    {
                        Buffer.MemoryCopy(
                            (void*)_frameBuffer,
                            (void*)bmpData.Scan0,
                            totalBytes,
                            totalBytes);
                    }
                }
                finally
                {
                    newFrame.UnlockBits(bmpData);
                }

                var oldFrame = _currentFrame;
                _currentFrame = newFrame;
                oldFrame?.Dispose();
            }
            catch { }
        }
    }

    private void OnDisplay(IntPtr opaque, IntPtr picture)
    {
        try { FrameReady?.Invoke(); } catch { }
    }

    public void Dispose()
    {
        DetachPlayer();
        lock (_bufferLock)
        {
            _currentFrame?.Dispose();
        }
    }
}