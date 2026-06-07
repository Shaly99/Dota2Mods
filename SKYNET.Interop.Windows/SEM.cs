using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct SEM
{
    public const uint FAILCRITICALERRORS = 1u;

    public const uint NOGPFAULTERRORBOX = 2u;

    public const uint NOALIGNMENTFAULTEXCEPT = 4u;

    public const uint NOOPENFILEERRORBOX = 32768u;
}
