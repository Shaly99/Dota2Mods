using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

public struct SECURITY_ATTRIBUTES
{
    public uint nLength;

    public IntPtr lpSecurityDescriptor;

    [MarshalAs(UnmanagedType.Bool)]
    public bool bInheritHandle;
}
