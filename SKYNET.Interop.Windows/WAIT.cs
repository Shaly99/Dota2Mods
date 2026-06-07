using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct WAIT
{
    public const uint FAILED = uint.MaxValue;

    public const uint OBJECT_0 = 0u;

    public const uint ABANDONED = 128u;

    public const uint TIMEOUT = 258u;
}
