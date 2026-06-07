using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct ButtonStyles
{
    public const long BS_PUSHBUTTON = 0L;

    public const long BS_DEFPUSHBUTTON = 1L;
}
