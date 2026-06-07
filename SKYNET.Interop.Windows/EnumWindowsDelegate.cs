using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

public delegate bool EnumWindowsDelegate(IntPtr hwnd, GCHandle lParam);
