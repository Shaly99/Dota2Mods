using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct UISF
{
    public const int HIDEFOCUS = 1;

    public const int HIDEACCEL = 2;

    public const int ACTIVE = 4;
}
