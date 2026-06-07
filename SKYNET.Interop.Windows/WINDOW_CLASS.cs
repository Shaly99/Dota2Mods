using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct WINDOW_CLASS
{
    public const string TOOLTIPS = "tooltips_class32";
}
