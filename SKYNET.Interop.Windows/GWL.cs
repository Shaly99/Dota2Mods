using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct GWL
{
    public const int WNDPROC = -4;

    public const int HINSTANCE = -6;

    public const int HWNDPARENT = -8;

    public const int STYLE = -16;

    public const int EXSTYLE = -20;

    public const int USERDATA = -21;

    public const int ID = -12;
}
