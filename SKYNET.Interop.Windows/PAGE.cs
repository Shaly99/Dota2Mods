using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct PAGE
{
    public const uint READONLY = 2u;

    public const uint READWRITE = 4u;

    public const uint WRITECOPY = 8u;
}
