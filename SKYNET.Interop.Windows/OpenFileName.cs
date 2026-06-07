namespace SKYNET.Interop.Windows;

public struct OpenFileName
{
    public int lStructSize;

    public IntPtr hwndOwner;

    public IntPtr hInstance;

    public IntPtr lpstrFilter;

    public IntPtr lpstrCustomFilter;

    public int nMaxCustFilter;

    public int nFilterIndex;

    public IntPtr lpstrFile;

    public int nMaxFile;

    public IntPtr lpstrFileTitle;

    public int nMaxFileTitle;

    public IntPtr lpstrInitialDir;

    public IntPtr lpstrTitle;

    public int Flags;

    public short nFileOffset;

    public short nFileExtension;

    public IntPtr lpstrDefExt;

    public int lCustData;

    public OfnHookProc lpfnHook;

    public IntPtr lpTemplateName;

    public IntPtr pvReserved;

    public int dwReserved;

    public int FlagsEx;
}
