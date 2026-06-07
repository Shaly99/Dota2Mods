using SKYNET.Interop.Windows;
using System.Collections;
using System.Text.RegularExpressions;

namespace SKYNET;

public class KeyboardHelper
{
    private static Hashtable keysToShortcutTable;

    private static Hashtable keysToAcceleratorMnemonicTable;

    private static Regex regexCtrlShiftAlt;

    public static ICollection Shortcuts => keysToShortcutTable.Values;

    static KeyboardHelper()
    {
        regexCtrlShiftAlt = new Regex("(Ctrl|Shift|Alt)");
        keysToShortcutTable = new Hashtable();
        keysToAcceleratorMnemonicTable = new Hashtable();
        keysToShortcutTable[Keys.None] = Shortcut.AltBksp;
        keysToShortcutTable[Keys.Insert] = Shortcut.Ins;
        keysToShortcutTable[Keys.Insert | Keys.Shift] = Shortcut.ShiftIns;
        keysToShortcutTable[Keys.Insert | Keys.Control] = Shortcut.CtrlIns;
        keysToShortcutTable[Keys.Delete] = Shortcut.Del;
        keysToShortcutTable[Keys.Delete | Keys.Shift] = Shortcut.ShiftDel;
        keysToShortcutTable[Keys.Delete | Keys.Control] = Shortcut.CtrlDel;
        keysToShortcutTable[Keys.D0 | Keys.Alt] = Shortcut.Alt0;
        keysToShortcutTable[Keys.D1 | Keys.Alt] = Shortcut.Alt1;
        keysToShortcutTable[Keys.D2 | Keys.Alt] = Shortcut.Alt2;
        keysToShortcutTable[Keys.D3 | Keys.Alt] = Shortcut.Alt3;
        keysToShortcutTable[Keys.D4 | Keys.Alt] = Shortcut.Alt4;
        keysToShortcutTable[Keys.D5 | Keys.Alt] = Shortcut.Alt5;
        keysToShortcutTable[Keys.D6 | Keys.Alt] = Shortcut.Alt6;
        keysToShortcutTable[Keys.D7 | Keys.Alt] = Shortcut.Alt7;
        keysToShortcutTable[Keys.D8 | Keys.Alt] = Shortcut.Alt8;
        keysToShortcutTable[Keys.D9 | Keys.Alt] = Shortcut.Alt9;
        keysToShortcutTable[Keys.D0 | Keys.Control] = Shortcut.Ctrl0;
        keysToShortcutTable[Keys.D1 | Keys.Control] = Shortcut.Ctrl1;
        keysToShortcutTable[Keys.D2 | Keys.Control] = Shortcut.Ctrl2;
        keysToShortcutTable[Keys.D3 | Keys.Control] = Shortcut.Ctrl3;
        keysToShortcutTable[Keys.D4 | Keys.Control] = Shortcut.Ctrl4;
        keysToShortcutTable[Keys.D5 | Keys.Control] = Shortcut.Ctrl5;
        keysToShortcutTable[Keys.D6 | Keys.Control] = Shortcut.Ctrl6;
        keysToShortcutTable[Keys.D7 | Keys.Control] = Shortcut.Ctrl7;
        keysToShortcutTable[Keys.D8 | Keys.Control] = Shortcut.Ctrl8;
        keysToShortcutTable[Keys.D9 | Keys.Control] = Shortcut.Ctrl9;
        keysToShortcutTable[Keys.D0 | Keys.Shift | Keys.Control] = Shortcut.CtrlShift0;
        keysToShortcutTable[Keys.D1 | Keys.Shift | Keys.Control] = Shortcut.CtrlShift1;
        keysToShortcutTable[Keys.D2 | Keys.Shift | Keys.Control] = Shortcut.CtrlShift2;
        keysToShortcutTable[Keys.D3 | Keys.Shift | Keys.Control] = Shortcut.CtrlShift3;
        keysToShortcutTable[Keys.D4 | Keys.Shift | Keys.Control] = Shortcut.CtrlShift4;
        keysToShortcutTable[Keys.D5 | Keys.Shift | Keys.Control] = Shortcut.CtrlShift5;
        keysToShortcutTable[Keys.D6 | Keys.Shift | Keys.Control] = Shortcut.CtrlShift6;
        keysToShortcutTable[Keys.D7 | Keys.Shift | Keys.Control] = Shortcut.CtrlShift7;
        keysToShortcutTable[Keys.D8 | Keys.Shift | Keys.Control] = Shortcut.CtrlShift8;
        keysToShortcutTable[Keys.D9 | Keys.Shift | Keys.Control] = Shortcut.CtrlShift9;
        keysToShortcutTable[Keys.F1] = Shortcut.F1;
        keysToShortcutTable[Keys.F2] = Shortcut.F2;
        keysToShortcutTable[Keys.F3] = Shortcut.F3;
        keysToShortcutTable[Keys.F4] = Shortcut.F4;
        keysToShortcutTable[Keys.F5] = Shortcut.F5;
        keysToShortcutTable[Keys.F6] = Shortcut.F6;
        keysToShortcutTable[Keys.F7] = Shortcut.F7;
        keysToShortcutTable[Keys.F8] = Shortcut.F8;
        keysToShortcutTable[Keys.F9] = Shortcut.F9;
        keysToShortcutTable[Keys.F10] = Shortcut.F10;
        keysToShortcutTable[Keys.F11] = Shortcut.F11;
        keysToShortcutTable[Keys.F12] = Shortcut.F12;
        keysToShortcutTable[Keys.F1 | Keys.Alt] = Shortcut.AltF1;
        keysToShortcutTable[Keys.F2 | Keys.Alt] = Shortcut.AltF2;
        keysToShortcutTable[Keys.F3 | Keys.Alt] = Shortcut.AltF3;
        keysToShortcutTable[Keys.F4 | Keys.Alt] = Shortcut.AltF4;
        keysToShortcutTable[Keys.F5 | Keys.Alt] = Shortcut.AltF5;
        keysToShortcutTable[Keys.F6 | Keys.Alt] = Shortcut.AltF6;
        keysToShortcutTable[Keys.F7 | Keys.Alt] = Shortcut.AltF7;
        keysToShortcutTable[Keys.F8 | Keys.Alt] = Shortcut.AltF8;
        keysToShortcutTable[Keys.F9 | Keys.Alt] = Shortcut.AltF9;
        keysToShortcutTable[Keys.F10 | Keys.Alt] = Shortcut.AltF10;
        keysToShortcutTable[Keys.F11 | Keys.Alt] = Shortcut.AltF11;
        keysToShortcutTable[Keys.F12 | Keys.Alt] = Shortcut.AltF12;
        keysToShortcutTable[Keys.F1 | Keys.Shift] = Shortcut.ShiftF1;
        keysToShortcutTable[Keys.F2 | Keys.Shift] = Shortcut.ShiftF2;
        keysToShortcutTable[Keys.F3 | Keys.Shift] = Shortcut.ShiftF3;
        keysToShortcutTable[Keys.F4 | Keys.Shift] = Shortcut.ShiftF4;
        keysToShortcutTable[Keys.F5 | Keys.Shift] = Shortcut.ShiftF5;
        keysToShortcutTable[Keys.F6 | Keys.Shift] = Shortcut.ShiftF6;
        keysToShortcutTable[Keys.F7 | Keys.Shift] = Shortcut.ShiftF7;
        keysToShortcutTable[Keys.F8 | Keys.Shift] = Shortcut.ShiftF8;
        keysToShortcutTable[Keys.F9 | Keys.Shift] = Shortcut.ShiftF9;
        keysToShortcutTable[Keys.F10 | Keys.Shift] = Shortcut.ShiftF10;
        keysToShortcutTable[Keys.F11 | Keys.Shift] = Shortcut.ShiftF11;
        keysToShortcutTable[Keys.F12 | Keys.Shift] = Shortcut.ShiftF12;
        keysToShortcutTable[Keys.F1 | Keys.Control] = Shortcut.CtrlF1;
        keysToShortcutTable[Keys.F2 | Keys.Control] = Shortcut.CtrlF2;
        keysToShortcutTable[Keys.F3 | Keys.Control] = Shortcut.CtrlF3;
        keysToShortcutTable[Keys.F4 | Keys.Control] = Shortcut.CtrlF4;
        keysToShortcutTable[Keys.F5 | Keys.Control] = Shortcut.CtrlF5;
        keysToShortcutTable[Keys.F6 | Keys.Control] = Shortcut.CtrlF6;
        keysToShortcutTable[Keys.F7 | Keys.Control] = Shortcut.CtrlF7;
        keysToShortcutTable[Keys.F8 | Keys.Control] = Shortcut.CtrlF8;
        keysToShortcutTable[Keys.F9 | Keys.Control] = Shortcut.CtrlF9;
        keysToShortcutTable[Keys.F10 | Keys.Control] = Shortcut.CtrlF10;
        keysToShortcutTable[Keys.F11 | Keys.Control] = Shortcut.CtrlF11;
        keysToShortcutTable[Keys.F12 | Keys.Control] = Shortcut.CtrlF12;
        keysToShortcutTable[Keys.F1 | Keys.Shift | Keys.Control] = Shortcut.CtrlShiftF1;
        keysToShortcutTable[Keys.F2 | Keys.Shift | Keys.Control] = Shortcut.CtrlShiftF2;
        keysToShortcutTable[Keys.F3 | Keys.Shift | Keys.Control] = Shortcut.CtrlShiftF3;
        keysToShortcutTable[Keys.F4 | Keys.Shift | Keys.Control] = Shortcut.CtrlShiftF4;
        keysToShortcutTable[Keys.F5 | Keys.Shift | Keys.Control] = Shortcut.CtrlShiftF5;
        keysToShortcutTable[Keys.F6 | Keys.Shift | Keys.Control] = Shortcut.CtrlShiftF6;
        keysToShortcutTable[Keys.F7 | Keys.Shift | Keys.Control] = Shortcut.CtrlShiftF7;
        keysToShortcutTable[Keys.F8 | Keys.Shift | Keys.Control] = Shortcut.CtrlShiftF8;
        keysToShortcutTable[Keys.F9 | Keys.Shift | Keys.Control] = Shortcut.CtrlShiftF9;
        keysToShortcutTable[Keys.F10 | Keys.Shift | Keys.Control] = Shortcut.CtrlShiftF10;
        keysToShortcutTable[Keys.F11 | Keys.Shift | Keys.Control] = Shortcut.CtrlShiftF11;
        keysToShortcutTable[Keys.F12 | Keys.Shift | Keys.Control] = Shortcut.CtrlShiftF12;
        keysToShortcutTable[Keys.A | Keys.Control] = Shortcut.CtrlA;
        keysToShortcutTable[Keys.B | Keys.Control] = Shortcut.CtrlB;
        keysToShortcutTable[Keys.C | Keys.Control] = Shortcut.CtrlC;
        keysToShortcutTable[Keys.D | Keys.Control] = Shortcut.CtrlD;
        keysToShortcutTable[Keys.E | Keys.Control] = Shortcut.CtrlE;
        keysToShortcutTable[Keys.F | Keys.Control] = Shortcut.CtrlF;
        keysToShortcutTable[Keys.G | Keys.Control] = Shortcut.CtrlG;
        keysToShortcutTable[Keys.H | Keys.Control] = Shortcut.CtrlH;
        keysToShortcutTable[Keys.I | Keys.Control] = Shortcut.CtrlI;
        keysToShortcutTable[Keys.J | Keys.Control] = Shortcut.CtrlJ;
        keysToShortcutTable[Keys.K | Keys.Control] = Shortcut.CtrlK;
        keysToShortcutTable[Keys.L | Keys.Control] = Shortcut.CtrlL;
        keysToShortcutTable[Keys.M | Keys.Control] = Shortcut.CtrlM;
        keysToShortcutTable[Keys.N | Keys.Control] = Shortcut.CtrlN;
        keysToShortcutTable[Keys.O | Keys.Control] = Shortcut.CtrlO;
        keysToShortcutTable[Keys.P | Keys.Control] = Shortcut.CtrlP;
        keysToShortcutTable[Keys.Q | Keys.Control] = Shortcut.CtrlQ;
        keysToShortcutTable[Keys.R | Keys.Control] = Shortcut.CtrlR;
        keysToShortcutTable[Keys.S | Keys.Control] = Shortcut.CtrlS;
        keysToShortcutTable[Keys.T | Keys.Control] = Shortcut.CtrlT;
        keysToShortcutTable[Keys.U | Keys.Control] = Shortcut.CtrlU;
        keysToShortcutTable[Keys.V | Keys.Control] = Shortcut.CtrlV;
        keysToShortcutTable[Keys.W | Keys.Control] = Shortcut.CtrlW;
        keysToShortcutTable[Keys.X | Keys.Control] = Shortcut.CtrlX;
        keysToShortcutTable[Keys.Y | Keys.Control] = Shortcut.CtrlY;
        keysToShortcutTable[Keys.Z | Keys.Control] = Shortcut.CtrlZ;
        keysToShortcutTable[Keys.A | Keys.Shift | Keys.Control] = Shortcut.CtrlShiftA;
        keysToShortcutTable[Keys.B | Keys.Shift | Keys.Control] = Shortcut.CtrlShiftB;
        keysToShortcutTable[Keys.C | Keys.Shift | Keys.Control] = Shortcut.CtrlShiftC;
        keysToShortcutTable[Keys.D | Keys.Shift | Keys.Control] = Shortcut.CtrlShiftD;
        keysToShortcutTable[Keys.E | Keys.Shift | Keys.Control] = Shortcut.CtrlShiftE;
        keysToShortcutTable[Keys.F | Keys.Shift | Keys.Control] = Shortcut.CtrlShiftF;
        keysToShortcutTable[Keys.G | Keys.Shift | Keys.Control] = Shortcut.CtrlShiftG;
        keysToShortcutTable[Keys.H | Keys.Shift | Keys.Control] = Shortcut.CtrlShiftH;
        keysToShortcutTable[Keys.I | Keys.Shift | Keys.Control] = Shortcut.CtrlShiftI;
        keysToShortcutTable[Keys.J | Keys.Shift | Keys.Control] = Shortcut.CtrlShiftJ;
        keysToShortcutTable[Keys.K | Keys.Shift | Keys.Control] = Shortcut.CtrlShiftK;
        keysToShortcutTable[Keys.L | Keys.Shift | Keys.Control] = Shortcut.CtrlShiftL;
        keysToShortcutTable[Keys.M | Keys.Shift | Keys.Control] = Shortcut.CtrlShiftM;
        keysToShortcutTable[Keys.N | Keys.Shift | Keys.Control] = Shortcut.CtrlShiftN;
        keysToShortcutTable[Keys.O | Keys.Shift | Keys.Control] = Shortcut.CtrlShiftO;
        keysToShortcutTable[Keys.P | Keys.Shift | Keys.Control] = Shortcut.CtrlShiftP;
        keysToShortcutTable[Keys.Q | Keys.Shift | Keys.Control] = Shortcut.CtrlShiftQ;
        keysToShortcutTable[Keys.R | Keys.Shift | Keys.Control] = Shortcut.CtrlShiftR;
        keysToShortcutTable[Keys.S | Keys.Shift | Keys.Control] = Shortcut.CtrlShiftS;
        keysToShortcutTable[Keys.T | Keys.Shift | Keys.Control] = Shortcut.CtrlShiftT;
        keysToShortcutTable[Keys.U | Keys.Shift | Keys.Control] = Shortcut.CtrlShiftU;
        keysToShortcutTable[Keys.V | Keys.Shift | Keys.Control] = Shortcut.CtrlShiftV;
        keysToShortcutTable[Keys.W | Keys.Shift | Keys.Control] = Shortcut.CtrlShiftW;
        keysToShortcutTable[Keys.X | Keys.Shift | Keys.Control] = Shortcut.CtrlShiftX;
        keysToShortcutTable[Keys.Y | Keys.Shift | Keys.Control] = Shortcut.CtrlShiftY;
        keysToShortcutTable[Keys.Z | Keys.Shift | Keys.Control] = Shortcut.CtrlShiftZ;
        keysToAcceleratorMnemonicTable[Keys.A | Keys.Alt] = AcceleratorMnemonic.AltA;
        keysToAcceleratorMnemonicTable[Keys.B | Keys.Alt] = AcceleratorMnemonic.AltB;
        keysToAcceleratorMnemonicTable[Keys.C | Keys.Alt] = AcceleratorMnemonic.AltC;
        keysToAcceleratorMnemonicTable[Keys.D | Keys.Alt] = AcceleratorMnemonic.AltD;
        keysToAcceleratorMnemonicTable[Keys.E | Keys.Alt] = AcceleratorMnemonic.AltE;
        keysToAcceleratorMnemonicTable[Keys.F | Keys.Alt] = AcceleratorMnemonic.AltF;
        keysToAcceleratorMnemonicTable[Keys.G | Keys.Alt] = AcceleratorMnemonic.AltG;
        keysToAcceleratorMnemonicTable[Keys.H | Keys.Alt] = AcceleratorMnemonic.AltH;
        keysToAcceleratorMnemonicTable[Keys.I | Keys.Alt] = AcceleratorMnemonic.AltI;
        keysToAcceleratorMnemonicTable[Keys.J | Keys.Alt] = AcceleratorMnemonic.AltJ;
        keysToAcceleratorMnemonicTable[Keys.K | Keys.Alt] = AcceleratorMnemonic.AltK;
        keysToAcceleratorMnemonicTable[Keys.L | Keys.Alt] = AcceleratorMnemonic.AltL;
        keysToAcceleratorMnemonicTable[Keys.M | Keys.Alt] = AcceleratorMnemonic.AltM;
        keysToAcceleratorMnemonicTable[Keys.N | Keys.Alt] = AcceleratorMnemonic.AltN;
        keysToAcceleratorMnemonicTable[Keys.O | Keys.Alt] = AcceleratorMnemonic.AltO;
        keysToAcceleratorMnemonicTable[Keys.P | Keys.Alt] = AcceleratorMnemonic.AltP;
        keysToAcceleratorMnemonicTable[Keys.Q | Keys.Alt] = AcceleratorMnemonic.AltQ;
        keysToAcceleratorMnemonicTable[Keys.R | Keys.Alt] = AcceleratorMnemonic.AltR;
        keysToAcceleratorMnemonicTable[Keys.S | Keys.Alt] = AcceleratorMnemonic.AltS;
        keysToAcceleratorMnemonicTable[Keys.T | Keys.Alt] = AcceleratorMnemonic.AltT;
        keysToAcceleratorMnemonicTable[Keys.U | Keys.Alt] = AcceleratorMnemonic.AltU;
        keysToAcceleratorMnemonicTable[Keys.V | Keys.Alt] = AcceleratorMnemonic.AltV;
        keysToAcceleratorMnemonicTable[Keys.W | Keys.Alt] = AcceleratorMnemonic.AltW;
        keysToAcceleratorMnemonicTable[Keys.X | Keys.Alt] = AcceleratorMnemonic.AltX;
        keysToAcceleratorMnemonicTable[Keys.Y | Keys.Alt] = AcceleratorMnemonic.AltY;
        keysToAcceleratorMnemonicTable[Keys.Z | Keys.Alt] = AcceleratorMnemonic.AltZ;
        keysToAcceleratorMnemonicTable[Keys.Oemplus | Keys.Control] = AcceleratorMnemonic.CtrlOEMPLUS;
        keysToAcceleratorMnemonicTable[Keys.Oemplus | Keys.Shift | Keys.Control] = AcceleratorMnemonic.CtrlShiftOEMPLUS;
    }

    public static string FormatShortcutString(Shortcut shortcut)
    {
        if (shortcut == Shortcut.None)
        {
            return null;
        }
        return regexCtrlShiftAlt.Replace(shortcut.ToString(), ReplaceShortcutPart);
    }

    private static string ReplaceShortcutPart(Match match)
    {
        return match.Value switch
        {
            "Ctrl" => Res.Get(StringId.ShortcutCtrl) + Res.Get(StringId.ShortcutPlus),
            "Shift" => Res.Get(StringId.ShortcutShift) + Res.Get(StringId.ShortcutPlus),
            "Alt" => Res.Get(StringId.ShortcutAlt) + Res.Get(StringId.ShortcutPlus),
            _ => match.Value,
        };
    }

    public static Keys MapToKeys(Shortcut shortcut)
    {
        IDictionaryEnumerator enumerator = keysToShortcutTable.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if ((Shortcut)enumerator.Value == shortcut)
            {
                return (Keys)enumerator.Key;
            }
        }
        return Keys.None;
    }

    public static Shortcut MapToShortcut(Keys keyData)
    {
        object obj = keysToShortcutTable[keyData];
        if (obj == null)
        {
            return Shortcut.None;
        }
        return (Shortcut)obj;
    }

    public static AcceleratorMnemonic MapToAcceleratorMnemonic(Keys keyData)
    {
        object obj = keysToAcceleratorMnemonicTable[keyData];
        if (obj == null)
        {
            return AcceleratorMnemonic.None;
        }
        return (AcceleratorMnemonic)obj;
    }

    public static Keys GetModifierKeys()
    {
        Keys keys = Keys.None;
        if (User32.GetKeyState(16) < 0)
        {
            keys |= Keys.Shift;
        }
        if (User32.GetKeyState(17) < 0)
        {
            keys |= Keys.Control;
        }
        if (User32.GetKeyState(18) < 0)
        {
            keys |= Keys.Alt;
        }
        return keys;
    }

    public static bool IsCtrlRightAlt(Keys keys)
    {
        return (keys & Keys.Control) != Keys.None && (keys & Keys.Alt) != Keys.None && User32.GetKeyState(165) < 0;
    }
}
