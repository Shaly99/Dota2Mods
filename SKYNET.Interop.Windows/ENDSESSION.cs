using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct ENDSESSION
{
    public const uint ENDSESSION_CLOSEAPP = 1u;

    public const uint ENDSESSION_CRITICAL = 1073741824u;

    public const uint ENDSESSION_LOGOFF = 2147483648u;
}
