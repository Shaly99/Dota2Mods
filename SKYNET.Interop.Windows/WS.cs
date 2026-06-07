using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct WS
{
    public const uint CHILD = 1073741824u;

    public const uint VISIBLE = 268435456u;

    public const uint CLIPCHILDREN = 33554432u;

    public const uint CLIPSIBLINGS = 67108864u;

    public const uint HSCROLL = 1048576u;

    public const uint POPUP = 2147483648u;

    public const uint TABSTOP = 65536u;

    public const uint VSCROLL = 2097152u;

    public const uint EX_TOOLWINDOW = 128u;

    public const uint EX_APPWINDOW = 262144u;

    public const uint EX_TOPMOST = 8u;

    public const uint BORDER = 8388608u;

    public const uint THICKFRAME = 262144u;

    public const uint SYSMENU = 524288u;

    public const uint MINIMIZEBOX = 131072u;

    public const uint MAXIMIZEBOX = 65536u;
}
