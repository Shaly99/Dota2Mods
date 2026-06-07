using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

public struct LASTINPUTINFO
{
    public static readonly int SizeOf = Marshal.SizeOf(typeof(LASTINPUTINFO));

    [MarshalAs(UnmanagedType.U4)]
    public int cbSize;

    [MarshalAs(UnmanagedType.U4)]
    public int dwTime;
}
