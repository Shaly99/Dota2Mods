using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct SPI
{
    public const int GETNONCLIENTMETRICS = 41;
}
