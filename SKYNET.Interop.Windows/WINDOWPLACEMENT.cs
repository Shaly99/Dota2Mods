namespace SKYNET.Interop.Windows;

public struct WINDOWPLACEMENT
{
    public uint length;

    public uint flags;

    public uint showCmd;

    public POINT ptMinPosition;

    public POINT ptMaxPosition;

    public RECT rcNormalPosition;
}
