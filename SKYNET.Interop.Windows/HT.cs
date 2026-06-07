using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct HT
{
    public const int ERROR = -2;

    public const int TRANSPARENT = -1;

    public const int NOWHERE = 0;

    public const int CLIENT = 1;

    public const int CAPTION = 2;

    public const int SYSMENU = 3;

    public const int GROWBOX = 4;

    public const int SIZE = 4;

    public const int MENU = 5;

    public const int HSCROLL = 6;

    public const int VSCROLL = 7;

    public const int MINBUTTON = 8;

    public const int MAXBUTTON = 9;

    public const int LEFT = 10;

    public const int RIGHT = 11;

    public const int TOP = 12;

    public const int TOPLEFT = 13;

    public const int TOPRIGHT = 14;

    public const int BOTTOM = 15;

    public const int BOTTOMLEFT = 16;

    public const int BOTTOMRIGHT = 17;

    public const int BORDER = 18;

    public const int REDUCE = 8;

    public const int ZOOM = 9;

    public const int SIZEFIRST = 10;

    public const int SIZELAST = 17;

    public const int OBJECT = 19;

    public const int CLOSE = 20;

    public const int HELP = 21;
}
