using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct FILE_ATTRIBUTE
{
    public const uint READONLY = 1u;

    public const uint HIDDEN = 2u;

    public const uint SYSTEM = 4u;

    public const uint DIRECTORY = 16u;

    public const uint ARCHIVE = 32u;

    public const uint DEVICE = 64u;

    public const uint NORMAL = 128u;

    public const uint TEMPORARY = 256u;
}
