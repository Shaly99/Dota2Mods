using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct PM
{
    public const uint NOREMOVE = 0u;

    public const uint REMOVE = 1u;
}
