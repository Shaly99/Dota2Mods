using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct WPF
{
    public const int RESTORETOMAXIMIZED = 2;
}
