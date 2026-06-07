using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct GMEM
{
    public const uint FIXED = 0u;

    public const uint MOVEABLE = 2u;

    public const uint ZEROINIT = 64u;

    public const uint SHARE = 8192u;
}
