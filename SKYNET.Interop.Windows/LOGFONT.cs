using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public struct LOGFONT
{
    public int lfHeight;
    public int lfWidth;
    public int lfEscapement;
    public int lfOrientation;
    public int lfWeight;
    public byte lfItalic;
    public byte lfUnderline;
    public byte lfStrikeOut;
    public byte lfCharSet;
    public byte lfOutPrecision;
    public byte lfClipPrecision;
    public byte lfQuality;
    public byte lfPitchAndFamily;

    private const int LF_FACESIZE = 32;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = LF_FACESIZE)]
    public string lfFaceName;

    public LOGFONT(string lfFaceName)
    {
        lfHeight = 0;
        lfWidth = 0;
        lfEscapement = 0;
        lfOrientation = 0;
        lfWeight = 0;
        lfItalic = 0;
        lfUnderline = 0;
        lfStrikeOut = 0;
        lfCharSet = 0;
        lfOutPrecision = 0;
        lfClipPrecision = 0;
        lfQuality = 0;
        lfPitchAndFamily = 0;
        this.lfFaceName = lfFaceName;
    }
}