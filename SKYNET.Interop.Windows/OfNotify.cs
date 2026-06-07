using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

[StructLayout(LayoutKind.Explicit)]
public struct OfNotify
{
    [FieldOffset(0)]
    public NMHDR hdr;

    [FieldOffset(12)]
    public IntPtr ipOfn;

    [FieldOffset(16)]
    public IntPtr ipFile;
}
