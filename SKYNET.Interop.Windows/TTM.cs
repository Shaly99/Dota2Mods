using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct TTM
{
    public const uint ADDTOOL = 1074u;

    public const uint TRACKACTIVATE = 1041u;

    public const uint TRACKPOSITION = 1042u;
}
