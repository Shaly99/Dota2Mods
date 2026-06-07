using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

[StructLayout(LayoutKind.Explicit)]
public struct NMHDR
{
    [FieldOffset(0)]
    public IntPtr hWndFrom;

    [FieldOffset(4)]
    public ushort idFrom;

    [FieldOffset(8)]
    public ushort code;
}
