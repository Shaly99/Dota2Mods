using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct FILE_MAP
{
    public const uint COPY = 1u;

    public const uint WRITE = 2u;

    public const uint READ = 4u;
}
