using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct TPM
{
    public const uint LEFTBUTTON = 0u;

    public const uint RIGHTBUTTON = 2u;

    public const uint LEFTALIGN = 0u;

    public const uint CENTERALIGN = 4u;

    public const uint RIGHTALIGN = 8u;

    public const uint NONOTIFY = 128u;

    public const uint RETURNCMD = 256u;

    public const uint VERNEGANIMATION = 8192u;

    public const uint LAYOUTRTL = 32768u;
}
