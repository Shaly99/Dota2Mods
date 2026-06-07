using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct TTF
{
    public const uint TRACK = 32u;

    public const uint ABSOLUTE = 128u;
}
