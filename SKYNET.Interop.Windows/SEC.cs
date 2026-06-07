using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct SEC
{
    public const uint IMAGE = 16777216u;

    public const uint RESERVE = 67108864u;

    public const uint COMMIT = 134217728u;

    public const uint NOCACHE = 268435456u;
}
