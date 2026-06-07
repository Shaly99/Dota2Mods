using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct THREAD_PRIORITY
{
    public const int NORMAL = 0;
}
