using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct MSGBOX_ID
{
    public const int OK = 1;

    public const int CANCEL = 2;

    public const int ABORT = 3;

    public const int RETRY = 4;

    public const int IGNORE = 5;

    public const int YES = 6;

    public const int NO = 7;

    public const int CLOSE = 8;

    public const int HELP = 9;
}
