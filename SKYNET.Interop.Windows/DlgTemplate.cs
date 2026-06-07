using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

[StructLayout(LayoutKind.Sequential)]
public class DlgTemplate
{
    public int style = 1140851972;

    public int extendedStyle = 65536;

    public short numItems = 1;

    public short x = 0;

    public short y = 0;

    public short cx = 0;

    public short cy = 0;

    public short reservedMenu = 0;

    public short reservedClass = 0;

    public short reservedTitle = 0;

    public int itemStyle = 1073741824;

    public int itemExtendedStyle = 4;

    public short itemX = 0;

    public short itemY = 0;

    public short itemCx = 0;

    public short itemCy = 0;

    public short itemId = 0;

    public ushort itemClassHdr = ushort.MaxValue;

    public short itemClass = 130;

    public short itemText = 0;

    public short itemData = 0;
}
