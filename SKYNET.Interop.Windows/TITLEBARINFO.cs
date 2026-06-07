using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

public struct TITLEBARINFO
{
    public uint cbSize;

    public RECT rcTitleBar;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
    public uint[] rgstate;
}
