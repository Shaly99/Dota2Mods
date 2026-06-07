namespace SKYNET.Interop.Windows;

[Flags]
public enum MK : uint
{
    LBUTTON = 1u,
    RBUTTON = 2u,
    SHIFT = 4u,
    CONTROL = 8u,
    MBUTTON = 0x10u,
    XBUTTON1 = 0x20u,
    XBUTTON2 = 0x40u
}
