using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct EVENT
{
    public const uint MODIFY_STATE = 2u;
}
