namespace SKYNET.Interop.Windows;

[Flags]
public enum QS : uint
{
    KEY = 1u,
    MOUSEMOVE = 2u,
    MOUSEBUTTON = 4u,
    POSTMESSAGE = 8u,
    TIMER = 0x10u,
    PAINT = 0x20u,
    SENDMESSAGE = 0x40u,
    HOTKEY = 0x80u,
    ALLPOSTMESSAGE = 0x100u,
    RAWINPUT = 0x400u,
    MOUSE = 6u,
    INPUT = 0x407u,
    ALLEVENTS = 0x4BFu,
    ALLINPUT = 0x4FFu
}
