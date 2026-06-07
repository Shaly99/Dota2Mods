using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct SW
{
    public const int HIDE = 0;

    public const int SHOWNORMAL = 1;

    public const int NORMAL = 1;

    public const int SHOWMINIMIZED = 2;

    public const int SHOWMAXIMIZED = 3;

    public const int MAXIMIZE = 3;

    public const int SHOWNOACTIVATE = 4;

    public const int SHOW = 5;

    public const int MINIMIZE = 6;

    public const int SHOWMINNOACTIVE = 7;

    public const int SHOWNA = 8;

    public const int RESTORE = 9;

    public const int SHOWDEFAULT = 10;

    public const int FORCEMINIMIZE = 11;

    public const int MAX = 11;
}
