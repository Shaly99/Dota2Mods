using System.Runtime.InteropServices;
using System.Text;

namespace SKYNET.Interop.Windows;

public class User32
{
    public struct ICONINFO
    {
        public bool fIcon;

        public int xHotspot;

        public int yHotspot;

        public IntPtr hbmMask;

        public IntPtr hbmColor;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BLENDFUNCTION
    {
        public byte BlendOp;

        public byte BlendFlags;

        public byte SourceConstantAlpha;

        public byte AlphaFormat;
    }

    public delegate IntPtr HookDelegate(int nCode, UIntPtr wParam, IntPtr lParam);

    public struct GETTEXTEX
    {
        public int cb;

        public int flags;

        public int codepage;

        public IntPtr lpDefaultChar;

        public IntPtr lpUsedDefChar;
    }

    public struct FLASHWINFO(IntPtr hwnd, int dwFlags, int uCount, int dwTimeout)
    {
        public int cbSize = IntPtr.Size + 16;

        public IntPtr hwnd = hwnd;

        public int dwFlags = dwFlags;

        public int uCount = uCount;

        public int dwTimeout = dwTimeout;
    }

    public struct TRACKMOUSEEVENT
    {
        public int cbSize;

        public uint dwFlags;

        public IntPtr hwndTrack;

        public uint dwHoverTime;
    }

    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct FlashStatus
    {
        public static int FLASHW_STOP = 0;

        public static int FLASHW_CAPTION = 1;

        public static int FLASHW_TRAY = 2;

        public static int FLASHW_ALL = FLASHW_CAPTION | FLASHW_TRAY;

        public static int FLASHW_TIMER = 4;

        public static int FLASHW_TIMERNOFG = 12;
    }

    public struct GETTEXTLENGTHEX
    {
        public int flags;

        public int codepage;
    }

    public struct DRAWTEXTPARAMS
    {
        public uint cbSize;

        public int iTabLength;

        public int iLeftMargin;

        public int iRightMargin;

        public uint uiLengthDrawn;
    }

    public enum DT : uint
    {
        TOP = 0u,
        LEFT = 0u,
        CENTER = 1u,
        RIGHT = 2u,
        VCENTER = 4u,
        BOTTOM = 8u,
        WORDBREAK = 16u,
        SINGLELINE = 32u,
        EXPANDTABS = 64u,
        TABSTOP = 128u,
        NOCLIP = 256u,
        EXTERNALLEADING = 512u,
        CALCRECT = 1024u,
        NOPREFIX = 2048u,
        INTERNAL = 4096u,
        EDITCONTROL = 8192u,
        PATH_ELLIPSIS = 16384u,
        END_ELLIPSIS = 32768u,
        MODIFYSTRING = 65536u,
        RTLREADING = 131072u,
        WORD_ELLIPSIS = 262144u,
        NOFULLWIDTHCHARBREAK = 524288u,
        HIDEPREFIX = 1048576u,
        PREFIXONLY = 2097152u
    }

    public delegate void WinEventProc(IntPtr hWinEventHook, EVENT_SYSTEM evt, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

    public static readonly int GWL_EXSTYLE = -20;

    public const uint WS_EX_LAYOUTRTL = 4194304u;

    public const uint TME_NONCLIENT = 16u;

    public const uint TME_HOVER = 1u;

    public const uint TME_QUERY = 1073741824u;

    public const uint TME_CANCEL = 2147483648u;

    public const uint TME_LEAVE = 16u;

    public const int EM_GETTEXTEX = 1118;

    public const int EM_GETTEXTLENGTHEX = 1119;

    public const int GTL_DEFAULT = 0;

    public const int GTL_USECRLF = 1;

    public const int GTL_PRECISE = 2;

    public const int GTL_CLOSE = 4;

    public const int GTL_NUMCHARS = 8;

    public const int GTL_NUMBYTES = 16;

    [DllImport("user32.dll", ExactSpelling = true)]
    public static extern int GetKeyboardLayout(int dwLayout);

    [DllImport("user32.dll")]
    public static extern bool SetMenu(IntPtr hWnd, IntPtr hMenu);

    [DllImport("user32.dll")]
    public static extern IntPtr GetDC(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern int GetClassName(IntPtr hWnd, [Out] StringBuilder lpClassName, int nMaxCount);

    [DllImport("user32.dll")]
    public static extern int GetIconInfo(IntPtr hIcon, out ICONINFO piconinfo);

    [DllImport("User32.dll")]
    public static extern bool ReleaseCapture();

    [DllImport("User32.dll")]
    public static extern IntPtr GetCapture();

    [DllImport("User32.dll")]
    public static extern IntPtr SetCapture(IntPtr hWnd);

    [DllImport("User32.dll")]
    public static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

    [DllImport("User32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr LoadCursor(IntPtr hInstance, IntPtr lpCursorName);

    [DllImport("User32.dll")]
    public static extern bool OpenClipboard(IntPtr hWnd);

    [DllImport("User32.dll")]
    public static extern int CountClipboardFormats();

    [DllImport("User32.dll")]
    public static extern bool EmptyClipboard();

    [DllImport("User32.dll")]
    public static extern bool CloseClipboard();

    [DllImport("User32.dll")]
    public static extern bool SetSystemCursor(IntPtr hcur, uint id);

    [DllImport("User32.dll")]
    public static extern IntPtr SetCursor(IntPtr hcur);

    [DllImport("User32.dll")]
    public static extern bool EnableWindow(IntPtr hcur, bool bEnable);

    [DllImport("User32.dll")]
    public static extern uint MsgWaitForMultipleObjects(uint nCount, IntPtr[] pHandles, bool bWaitAll, uint dwMilliseconds, QS dwWakeMask);

    [DllImport("User32.dll")]
    public static extern bool InvalidateRect(IntPtr hWnd, ref RECT lpRect, bool bErase);

    [DllImport("User32.dll", EntryPoint = "InvalidateRect")]
    public static extern bool InvalidateWindow(IntPtr hWnd, IntPtr lpRect, bool bErase);

    [DllImport("User32.dll")]
    public static extern bool UpdateWindow(IntPtr hWnd);

    [DllImport("User32.dll")]
    public static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref POINT pptDst, ref SIZE psize, IntPtr hdcSrc, ref POINT pptSrc, uint crKey, ref BLENDFUNCTION pblend, uint dwFlags);

    [DllImport("User32.dll")]
    public static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("User32.dll", CharSet = CharSet.Auto)]
    public static extern bool SetWindowText(IntPtr hWnd, [MarshalAs(UnmanagedType.LPTStr)] string lpString);

    [DllImport("User32.dll", CharSet = CharSet.Auto)]
    public static extern uint RegisterWindowMessage([MarshalAs(UnmanagedType.LPTStr)] string lpString);

    [DllImport("User32.dll")]
    public static extern bool DestroyIcon(IntPtr hIcon);

    [DllImport("User32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, [In][MarshalAs(UnmanagedType.LPTStr)] string lpszClass, [In][MarshalAs(UnmanagedType.LPTStr)] string lpszWindow);

    [DllImport("User32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr FindWindow([In][MarshalAs(UnmanagedType.LPTStr)] string lpszClass, [In][MarshalAs(UnmanagedType.LPTStr)] string lpszWindow);

    [DllImport("User32.dll", SetLastError = true)]
    public static extern IntPtr SetFocus(IntPtr hWnd);

    [DllImport("User32.dll")]
    public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

    [DllImport("User32.dll")]
    public static extern IntPtr SetActiveWindow(IntPtr hWnd);

    [DllImport("User32.dll")]
    public static extern IntPtr GetActiveWindow();

    [DllImport("User32.dll")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("User32.dll")]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("User32.dll")]
    public static extern bool BringWindowToTop(IntPtr hWnd);

    [DllImport("User32.dll")]
    public static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

    [DllImport("User32.dll")]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("User32.dll")]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("User32.dll")]
    public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

    [DllImport("User32.dll")]
    public static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern int MapWindowPoints(IntPtr hwndFrom, IntPtr hwndTo, ref POINT lpPoints, [MarshalAs(UnmanagedType.U4)] int cPoints);

    [DllImport("User32.dll")]
    public static extern IntPtr GetParent(IntPtr hWnd);

    [DllImport("User32.dll")]
    public static extern IntPtr GetAncestor(IntPtr hWnd, GA gaFlags);

    [DllImport("User32.dll")]
    public static extern bool AnimateWindow(IntPtr hWnd, uint dwTime, AW dwFlags);

    [DllImport("User32.dll")]
    public static extern bool IsWindow(IntPtr hWnd);

    [DllImport("User32.dll")]
    public static extern uint WaitForInputIdle(IntPtr hProcess, uint dwMilliseconds);

    [DllImport("User32.dll")]
    public static extern IntPtr GetWindowDC(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern IntPtr GetDCEx(IntPtr hWnd, IntPtr region, uint dw);

    [DllImport("user32.dll")]
    public static extern bool ReleaseDC(IntPtr hWnd, IntPtr dc);

    [DllImport("user32.dll")]
    public static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern int GetWindowText(IntPtr hWnd, [Out] StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, UIntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    [DllImport("User32.dll")]
    public static extern bool PeekMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);

    [DllImport("user32.dll")]
    public static extern bool TranslateMessage([In] ref MSG msg);

    [DllImport("user32.dll")]
    public static extern IntPtr DispatchMessage([In] ref MSG msg);

    [DllImport("user32.dll")]
    public static extern IntPtr SetWindowsHookEx(WH hookType, HookDelegate lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll")]
    public static extern int GetSystemMetrics(int nIndex);

    [DllImport("user32.dll")]
    public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, UIntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    public static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr lpdwProcessId);

    [DllImport("user32.dll")]
    public static extern short GetKeyState(int nVirtKey);

    [DllImport("user32.dll")]
    public static extern IntPtr GetDesktopWindow();

    [DllImport("user32.dll")]
    public static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

    [DllImport("user32.dll")]
    public static extern bool PostMessage(IntPtr hWnd, uint Msg, UIntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    public static extern uint GetMenuItemID(IntPtr hMenu, int nPos);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool TrackMouseEvent(ref TRACKMOUSEEVENT lpEventTrack);

    [DllImport("user32.dll")]
    public static extern int TrackPopupMenu(IntPtr hMenu, uint uFlags, int x, int y, int nReserved, IntPtr hWnd, IntPtr prcRect);

    [DllImport("User32.dll")]
    public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

    [DllImport("User32.dll")]
    public static extern bool DeleteMenu(IntPtr hMenu, uint uPosition, uint uFlags);

    [DllImport("user32.dll")]
    public static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

    [DllImport("User32.dll")]
    public static extern uint RegisterClipboardFormat(string lpszFormat);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "SetWindowLong")]
    public static extern int SetWindowProc(IntPtr hWnd, int nIndex, WndProcDelegate lpWndProc);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int nValue);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr CallWindowProc(IntPtr lpWndProc, IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    public static extern bool GetTitleBarInfo(IntPtr hWnd, ref TITLEBARINFO pti);

    [DllImport("user32.dll")]
    public static extern bool GetWindowInfo(IntPtr hWnd, ref WINDOWINFO pwi);

    [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "SendMessage")]
    public static extern int GetText(IntPtr hWnd, int Msg, ref GETTEXTEX wParam, StringBuilder lParam);

    [DllImport("user32.dll")]
    public static extern bool FlashWindow(IntPtr hwnd, bool bInvert);

    [DllImport("user32.dll")]
    public static extern bool FlashWindowEx(ref FLASHWINFO pfwi);

    [DllImport("user32.dll")]
    public static extern bool EnumWindows(EnumWindowsDelegate lpEnumFunc, GCHandle lParam);

    [DllImport("user32.dll")]
    public static extern bool EnumChildWindows(IntPtr hWndParent, EnumWindowsDelegate lpEnumFunc, GCHandle lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "SendMessage")]
    public static extern int GetTextLength(IntPtr hWnd, int Msg, ref GETTEXTLENGTHEX wParam, StringBuilder lParam);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int DrawTextEx(IntPtr hdc, StringBuilder lpchText, int cchText, ref RECT lprc, DT dwDTFormat, ref DRAWTEXTPARAMS lpDTParams);

    [DllImport("user32.dll")]
    public static extern IntPtr SetWinEventHook(EVENT_SYSTEM eventMin, EVENT_SYSTEM eventMax, IntPtr hmodWinEventProc, WinEventProc lpfnWinEventProc, uint idProcess, uint idThread, WINEVENT dwFlags);

    [DllImport("user32.dll")]
    public static extern bool UnhookWinEvent(IntPtr hWinEventHook);

    [DllImport("user32.dll")]
    public static extern bool GetLastInputInfo(out LASTINPUTINFO plii);

    [DllImport("ComDlg32.dll", CharSet = CharSet.Unicode)]
    public static extern bool GetOpenFileName(ref OpenFileName ofn);

    [DllImport("ComDlg32.dll", CharSet = CharSet.Unicode)]
    public static extern int CommDlgExtendedError();

    [DllImport("User32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr GetDlgItem(IntPtr hWndDlg, int Id);

    [DllImport("User32.dll", SetLastError = true)]
    public static extern IntPtr GetDlgCtrlID(IntPtr hwndCtl);

    [DllImport("User32.dll", SetLastError = true)]
    public static extern bool AllowSetForegroundWindow(int procId);

    [DllImport("User32.dll", CharSet = CharSet.Auto)]
    public static extern int SystemParametersInfo(int uAction, int uParam, ref NONCLIENTMETRICS lpvParam, int fuWinIni);

    [DllImport("user32.dll")]
    public static extern int DrawMenuBar(IntPtr hwnd);

    [DllImport("user32.dll")]
    public static extern int SetMenuInfo(IntPtr hmenu, ref MENUINFO mi);
}
