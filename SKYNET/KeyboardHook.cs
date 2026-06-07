using SKYNET.Interop.Windows;

namespace SKYNET;

public abstract class KeyboardHook
{
    private IntPtr m_hHook = IntPtr.Zero;

    private User32.HookDelegate m_hookDelegate;

    public bool IsInstalled => m_hHook != IntPtr.Zero;

    public void Install(IntPtr hWnd)
    {
        if (!(m_hHook != IntPtr.Zero))
        {
            uint windowThreadProcessId = User32.GetWindowThreadProcessId(hWnd, IntPtr.Zero);
            Install(windowThreadProcessId);
        }
    }

    public void Install(uint dwThreadId)
    {
        m_hookDelegate = KeyboardProc;
        m_hHook = User32.SetWindowsHookEx(WH.KEYBOARD, m_hookDelegate, IntPtr.Zero, dwThreadId);
    }

    public void Remove()
    {
        if (m_hHook != IntPtr.Zero)
        {
            bool flag = User32.UnhookWindowsHookEx(m_hHook);
            m_hHook = IntPtr.Zero;
        }
        m_hookDelegate = null;
    }

    protected IntPtr CallNextHook(int nCode, UIntPtr wParam, IntPtr lParam)
    {
        return User32.CallNextHookEx(m_hHook, nCode, wParam, lParam);
    }

    protected abstract IntPtr OnKeyHooked(int nCode, UIntPtr wParam, IntPtr lParam);

    private IntPtr KeyboardProc(int nCode, UIntPtr wParam, IntPtr lParam)
    {
        try
        {
            return OnKeyHooked(nCode, wParam, lParam);
        }
        catch (Exception)
        {
            return new IntPtr(1);
        }
    }
}
