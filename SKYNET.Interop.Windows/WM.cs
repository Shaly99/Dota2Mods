using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct WM
{
    public const uint COMMAND = 273u;

    public const uint DESTROY = 2u;

    public const uint SIZE = 5u;

    public const uint CLOSE = 16u;

    public const uint QUERYENDSESSION = 17u;

    public const uint ENDSESSION = 22u;

    public const uint VSCROLL = 277u;

    public const uint SETCURSOR = 32u;

    public const uint MOUSEACTIVATE = 33u;

    public const uint NOTIFY = 78u;

    public const uint INITDIALOG = 272u;

    public const uint GETTEXT = 13u;

    public const uint GETTEXTLENGTH = 14u;

    public const uint USER = 1024u;

    public const uint NULL = 0u;

    public const uint GETMINMAXINFO = 36u;

    public const uint HOTKEY = 786u;

    public const uint SETFOCUS = 7u;

    public const uint KEYDOWN = 256u;

    public const uint KEYUP = 257u;

    public const uint CHAR = 258u;

    public const uint SYSKEYDOWN = 260u;

    public const uint SYSKEYUP = 261u;

    public const uint SYSCHAR = 262u;

    public const uint NCDESTROY = 130u;

    public const uint NCPAINT = 133u;

    public const uint NCACTIVATE = 134u;

    public const uint ERASEBKGND = 20u;

    public const uint NCHITTEST = 132u;

    public const uint SYSCOMMAND = 274u;

    public const uint MOUSEMOVE = 512u;

    public const uint LBUTTONDOWN = 513u;

    public const uint LBUTTONUP = 514u;

    public const uint LBUTTONDBLCLK = 515u;

    public const uint RBUTTONDOWN = 516u;

    public const uint RBUTTONUP = 517u;

    public const uint CHANGEUISTATE = 295u;

    public const uint UPDATEUISTATE = 296u;

    public const uint ENTERMENULOOP = 529u;

    public const uint EXITMENULOOP = 530u;

    public const uint CUT = 768u;

    public const uint COPY = 769u;

    public const uint PASTE = 770u;

    public const uint CLEAR = 771u;

    public const uint ACTIVATE = 6u;

    public const uint COPYDATA = 74u;

    public const uint CONTEXTMENU = 123u;

    public const uint WININICHANGE = 26u;

    public const uint ENTERSIZEMOVE = 561u;

    public const uint EXITSIZEMOVE = 562u;

    public const uint NCMOUSEMOVE = 160u;

    public const uint NCLBUTTONDOWN = 161u;

    public const uint NCLBUTTONUP = 162u;

    public const uint NCLBUTTONDBLCLK = 163u;

    public const uint NCRBUTTONDOWN = 164u;

    public const uint NCRBUTTONUP = 165u;

    public const uint NCRBUTTONDBLCLK = 166u;

    public const uint NCMBUTTONDOWN = 167u;

    public const uint NCMBUTTONUP = 168u;

    public const uint NCMBUTTONDBLCLK = 169u;

    public const uint NCMOUSEHOVER = 672u;

    public const uint NCMOUSELEAVE = 674u;

    public const uint DM_SETDEFID = 1025u;

    public const uint EM_REPLACESEL = 194u;

    public const uint EM_POSFROMCHAR = 214u;

    public const uint EM_SETMARGINS = 211u;

    public const uint EM_GETMARGINS = 212u;

    public const uint REFLECT = 8192u;
}
