namespace SKYNET.Interop.Windows;

public struct MSG
{
    public IntPtr hwnd;

    public uint message;

    public uint wParam;

    public int lParam;

    public uint time;

    public POINT pt;
}
