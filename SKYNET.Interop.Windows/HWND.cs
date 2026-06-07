using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct HWND
{
    public static readonly IntPtr DESKTOP = new IntPtr(0);

    public static readonly IntPtr TOP = new IntPtr(0);

    public static readonly IntPtr BOTTOM = new IntPtr(1);

    public static readonly IntPtr TOPMOST = new IntPtr(-1);

    public static readonly IntPtr NOTOPMOST = new IntPtr(-2);

    public static readonly IntPtr MESSAGE = new IntPtr(-3);
}
