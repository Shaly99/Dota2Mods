using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct UIS
{
    public const int SET = 1;

    public const int CLEAR = 2;

    public const int INITIALIZE = 3;
}
