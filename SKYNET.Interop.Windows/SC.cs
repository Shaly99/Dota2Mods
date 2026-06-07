using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct SC
{
    public static readonly UIntPtr SIZE = new UIntPtr(61440u);

    public static readonly UIntPtr MOVE = new UIntPtr(61456u);

    public static readonly UIntPtr MINIMIZE = new UIntPtr(61472u);

    public static readonly UIntPtr MAXIMIZE = new UIntPtr(61488u);

    public static readonly UIntPtr NEXTWINDOW = new UIntPtr(61504u);

    public static readonly UIntPtr PREVWINDOW = new UIntPtr(61520u);

    public static readonly UIntPtr CLOSE = new UIntPtr(61536u);

    public static readonly UIntPtr VSCROLL = new UIntPtr(61552u);

    public static readonly UIntPtr HSCROLL = new UIntPtr(61568u);

    public static readonly UIntPtr MOUSEMENU = new UIntPtr(61584u);

    public static readonly UIntPtr KEYMENU = new UIntPtr(61696u);

    public static readonly UIntPtr ARRANGE = new UIntPtr(61712u);

    public static readonly UIntPtr RESTORE = new UIntPtr(61728u);

    public static readonly UIntPtr TASKLIST = new UIntPtr(61744u);

    public static readonly UIntPtr SCREENSAVE = new UIntPtr(61760u);

    public static readonly UIntPtr HOTKEY = new UIntPtr(61776u);

    public static readonly UIntPtr DEFAULT = new UIntPtr(61792u);

    public static readonly UIntPtr MONITORPOWER = new UIntPtr(61808u);

    public static readonly UIntPtr CONTEXTHELP = new UIntPtr(61824u);

    public static readonly UIntPtr SEPARATOR = new UIntPtr(61455u);
}
