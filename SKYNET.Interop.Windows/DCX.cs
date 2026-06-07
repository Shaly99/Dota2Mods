using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct DCX
{
    public const uint WINDOW = 1u;

    public const uint CACHE = 2u;

    public const uint NORESETATTRS = 4u;

    public const uint CLIPCHILDREN = 8u;

    public const uint CLIPSIBLINGS = 16u;

    public const uint PARENTCLIP = 32u;

    public const uint EXCLUDERGN = 64u;

    public const uint INTERSECTRGN = 128u;

    public const uint EXCLUDEUPDATE = 256u;

    public const uint INTERSECTUPDATE = 512u;

    public const uint LOCKWINDOWUPDATE = 1024u;

    public const uint VALIDATE = 2097152u;
}
