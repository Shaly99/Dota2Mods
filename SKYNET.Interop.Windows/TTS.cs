using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct TTS
{
    public const uint ALWAYSTIP = 1u;

    public const uint NOPREFIX = 2u;
}
