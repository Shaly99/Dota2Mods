using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct VK
{
    public const int RETURN = 13;

    public const int BACK = 8;

    public const int TAB = 9;

    public const int SHIFT = 16;

    public const int CONTROL = 17;

    public const int MENU = 18;

    public const int LMENU = 164;

    public const int RMENU = 165;

    public const int END = 35;

    public const int HOME = 36;

    public const int LEFT = 37;

    public const int UP = 38;

    public const int RIGHT = 39;

    public const int DOWN = 40;

    public const int INSERT = 45;

    public const int DELETE = 46;
}
