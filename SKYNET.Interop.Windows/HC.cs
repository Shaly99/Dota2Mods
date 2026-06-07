using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct HC
{
    public const int ACTION = 0;

    public const int GETNEXT = 1;

    public const int SKIP = 2;

    public const int NOREMOVE = 3;

    public const int SYSMODALON = 4;

    public const int SYSMODALOFF = 5;
}
