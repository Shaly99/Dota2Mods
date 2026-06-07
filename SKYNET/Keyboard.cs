using System.Runtime.InteropServices;

namespace SKYNET;

internal class Keyboard : KeyboardHook
{
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct HC
    {
        public const int ACTION = 0;

        public const int GETNEXT = 1;

        public const int SKIP = 2;

        public const int NOREMOVE = 3;

        public const int SYSMODALON = 4;

        public const int SYSMODALOFF = 5;
    }

    protected override IntPtr OnKeyHooked(int nCode, UIntPtr wParam, IntPtr lParam)
    {
        if (nCode == 0 && ((int)lParam & -1073741824) == 0)
        {
            Keys keyData = (Keys)((int)((uint)wParam & 0xFFFF) | (int)KeyboardHelper.GetModifierKeys());
            frmMain.frm.ProcessKey(keyData);
        }
        return CallNextHook(nCode, wParam, lParam);
    }
}
