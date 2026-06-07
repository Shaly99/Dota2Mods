using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct REPLACEFILE
{
    public const uint WRITE_THROUGH = 1u;

    public const uint IGNORE_MERGE_ERRORS = 2u;
}
