using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct SM
{
    public const int CXSIZEFRAME = 32;

    public const int CYSIZEFRAME = 33;
}
