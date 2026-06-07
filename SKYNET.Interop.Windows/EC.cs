using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct EC
{
    public const uint RIGHTMARGIN = 2u;

    public const uint LEFTMARGIN = 1u;
}
