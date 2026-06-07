using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct SWP
{
    public const uint NOSIZE = 1u;

    public const uint NOMOVE = 2u;

    public const uint NOZORDER = 4u;

    public const uint SHOWWINDOW = 64u;

    public const uint NOACTIVATE = 16u;

    public const uint HIDEWINDOW = 128u;

    public const uint NOOWNERZORDER = 512u;
}
