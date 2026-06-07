using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct MOD
{
    public const uint ALT = 1u;

    public const uint CONTROL = 2u;

    public const uint SHIFT = 4u;

    public const uint WIN = 8u;
}
