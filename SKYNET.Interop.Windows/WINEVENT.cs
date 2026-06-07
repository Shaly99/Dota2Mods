namespace SKYNET.Interop.Windows;

[Flags]
public enum WINEVENT : uint
{
    OUTOFCONTEXT = 0u,
    SKIPOWNTHREAD = 1u,
    SKIPOWNPROCESS = 2u,
    INCONTEXT = 4u
}
