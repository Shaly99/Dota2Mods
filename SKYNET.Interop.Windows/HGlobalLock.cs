using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

public class HGlobalLock : IDisposable
{
    private IntPtr hGlobal = IntPtr.Zero;

    private IntPtr pData = IntPtr.Zero;

    public IntPtr Memory => pData;

    public UIntPtr Size
    {
        get
        {
            UIntPtr uIntPtr = Kernel32.GlobalSize(hGlobal);
            if (uIntPtr == UIntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Unexpected error calling GlobalSize");
            }
            return uIntPtr;
        }
    }

    public HGlobalLock(IntPtr hGlobal)
    {
        this.hGlobal = hGlobal;
        Lock();
    }

    public void Dispose()
    {
        Unlock();
    }

    public void Lock()
    {
        pData = Kernel32.GlobalLock(hGlobal);
        if (pData == IntPtr.Zero)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error(), "Error occurred while tyring to lock global memory");
        }
    }

    public void Unlock()
    {
        if (pData != IntPtr.Zero)
        {
            bool flag = Kernel32.GlobalUnlock(hGlobal);
            int lastWin32Error = Marshal.GetLastWin32Error();
            if (!flag && lastWin32Error != 0)
            {
                throw new Win32Exception(lastWin32Error, "Unexpected error occurred calling GlobalUnlock");
            }
            pData = IntPtr.Zero;
        }
    }

    public IntPtr Clone()
    {
        IntPtr intPtr = Kernel32.GlobalAlloc(0u, Size);
        if (intPtr == IntPtr.Zero)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error(), "Unexpected error occurred calling GlobalAlloc");
        }
        Kernel32.CopyMemory(intPtr, Memory, Size);
        return intPtr;
    }
}
