namespace SKYNET.Interop.Windows;

[Flags]
public enum AW : uint
{
    HOR_POSITIVE = 1u,
    HOR_NEGATIVE = 2u,
    VER_POSITIVE = 4u,
    VER_NEGATIVE = 8u,
    CENTER = 0x10u,
    HIDE = 0x10000u,
    ACTIVATE = 0x20000u,
    SLIDE = 0x40000u,
    BLEND = 0x80000u
}
