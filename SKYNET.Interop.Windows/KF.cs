using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct KF
{
    public static readonly IntPtr EXTENDED = new IntPtr(256);

    public static readonly IntPtr DLGMODE = new IntPtr(2048);

    public static readonly IntPtr MENUMODE = new IntPtr(4096);

    public static readonly IntPtr ALTDOWN = new IntPtr(8192);

    public static readonly IntPtr REPEAT = new IntPtr(16384);

    public static readonly IntPtr UP = new IntPtr(32768);
}
