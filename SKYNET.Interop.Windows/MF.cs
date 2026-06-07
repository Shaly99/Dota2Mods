using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct MF
{
    public const uint BYCOMMAND = 0u;

    public const uint BYPOSITION = 1024u;

    public const uint ENABLED = 0u;

    public const uint DISABLED = 2u;
}
