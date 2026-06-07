using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

public struct TOOLINFO
{
    public uint cbSize;

    public uint uFlags;

    public IntPtr hwnd;

    public UIntPtr uId;

    public RECT rect;

    public IntPtr hinst;

    [MarshalAs(UnmanagedType.LPTStr)]
    public string lpszText;

    public IntPtr lParam;
}
