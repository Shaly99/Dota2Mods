using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Text;

namespace SKYNET.Interop.Windows;

public class Kernel32
{
    public delegate bool CodePageDelegate([MarshalAs(UnmanagedType.LPTStr)] string codePageName);

    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct DRIVE
    {
        public const int UNKNOWN = 0;

        public const int NO_ROOT_DIR = 1;

        public const int REMOVABLE = 2;

        public const int FIXED = 3;

        public const int REMOTE = 4;

        public const int CDROM = 5;

        public const int RAMDISK = 6;
    }

    public static readonly uint INFINITE = uint.MaxValue;

    public static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

    public const uint CP_INSTALLED = 1u;

    public const uint CP_SUPPORTED = 2u;

    public const int MAX_PATH = 260;

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool SetDllDirectory(string pathName);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern uint SetErrorMode(uint uMode);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern void ExitProcess(uint uExitCode);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr GetCurrentProcess();

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern uint GetCurrentProcessId();

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr GetModuleHandle(IntPtr lpModuleName);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern uint GetModuleFileName(IntPtr hModule, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder lpFilename, uint nSize);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool SetProcessWorkingSetSize(IntPtr hProcess, int dwMinimumWorkingSetSize, int dwMaximumWorkingSetSize);

    [DllImport("kernel32.dll")]
    public static extern bool Beep(int frequency, int duration);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetLongPathName([In] string lpFileName, [Out] StringBuilder lpBuffer, [In] int nBufferLength);

    public static string GetLongPathName(string fileName)
    {
        return GetLongPathName(fileName, 260);
    }

    protected static string GetLongPathName(string fileName, int bufferLen)
    {
        StringBuilder stringBuilder = new StringBuilder(bufferLen);
        int longPathName = GetLongPathName(fileName, stringBuilder, bufferLen);
        if (longPathName == 0)
        {
            return null;
        }
        if (longPathName > bufferLen)
        {
            return GetLongPathName(fileName, longPathName);
        }
        return stringBuilder.ToString();
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetShortPathName([In] string lpFileName, [Out] StringBuilder lpBuffer, [In] int nBufferLength);

    public static string GetShortPathName(string fileName)
    {
        return GetShortPathName(fileName, 260);
    }

    protected static string GetShortPathName(string fileName, int bufferLen)
    {
        StringBuilder stringBuilder = new StringBuilder(bufferLen);
        int shortPathName = GetShortPathName(fileName, stringBuilder, bufferLen);
        if (shortPathName == 0)
        {
            return null;
        }
        if (shortPathName > bufferLen)
        {
            return GetShortPathName(fileName, shortPathName);
        }
        return stringBuilder.ToString();
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool SetThreadLocale(int lcid);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool EnumSystemCodePages(CodePageDelegate lpCodePageEnumProc, uint dwFlags);

    [DllImport("Kernel32.dll", SetLastError = true)]
    public static extern IntPtr LocalFree(IntPtr hMem);

    [DllImport("kernel32.dll")]
    public static extern bool ProcessIdToSessionId([In] uint dwProcessId, out uint pSessionId);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr CreateFileMapping(IntPtr hFile, IntPtr lpAttributes, uint flProtect, uint dwMaximumSizeHigh, uint dwMaximumSizeLow, [In][MarshalAs(UnmanagedType.LPTStr)] string lpName);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, uint dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, uint dwNumberOfBytesToMap);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern void ZeroMemory(IntPtr Destination, uint Length);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern int UnmapViewOfFile(IntPtr lpBaseAddress);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool ReplaceFile([MarshalAs(UnmanagedType.LPTStr)] string lpReplacedFileName, [MarshalAs(UnmanagedType.LPTStr)] string lpReplacementFileName, [MarshalAs(UnmanagedType.LPTStr)] string lpBackupFileName, uint dwReplaceFlags, IntPtr lpExclude, IntPtr lpReserved);

    [DllImport("kernel32.dll")]
    [Obsolete("Use Marshal.GetLastWin32Error() with [DllImport(SetLastError = true)] instead!", true)]
    public static extern uint GetLastError();

    [DllImport("Kernel32.dll", SetLastError = true)]
    public static extern IntPtr GlobalLock(IntPtr hMem);

    [DllImport("Kernel32.dll", SetLastError = true)]
    public static extern bool GlobalUnlock(IntPtr hMem);

    [DllImport("Kernel32.dll", SetLastError = true)]
    public static extern UIntPtr GlobalSize(IntPtr hMem);

    [DllImport("Kernel32.dll", SetLastError = true)]
    public static extern IntPtr GlobalAlloc(uint uFlags, UIntPtr dwBytes);

    [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = true)]
    public static extern void CopyMemory(IntPtr Destination, IntPtr Source, UIntPtr Length);

    [DllImport("Kernel32.dll", SetLastError = true)]
    public static extern IntPtr CreateMutex(IntPtr lpMutexAttributes, int bInitialOwner, [MarshalAs(UnmanagedType.LPTStr)] string lpName);

    [DllImport("Kernel32.dll", SetLastError = true)]
    public static extern int ReleaseMutex(IntPtr hMutex);

    [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern IntPtr CreateEvent(IntPtr lpEventAttributes, int bManualReset, int bInitialState, [MarshalAs(UnmanagedType.LPTStr)] string lpName);

    [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern IntPtr OpenEvent(uint dwDesiredAccess, int bInheritHandle, [MarshalAs(UnmanagedType.LPTStr)] string lpName);

    [DllImport("Kernel32.dll", SetLastError = true)]
    public static extern int SetEvent(IntPtr hEvent);

    [DllImport("Kernel32.dll", SetLastError = true)]
    public static extern int ResetEvent(IntPtr hEvent);

    [DllImport("Kernel32.dll", SetLastError = true)]
    public static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

    [DllImport("Kernel32.dll", SetLastError = true)]
    public static extern int CloseHandle(IntPtr hObject);

    [DllImport("Kernel32.dll", SetLastError = true)]
    public static extern uint WaitForMultipleObjects(uint nCount, IntPtr[] pHandles, bool bWaitAll, uint dwMilliseconds);

    [DllImport("kernel32.dll")]
    public static extern long GetDriveType(string driveLetter);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    public static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpdefault, StringBuilder sbout, int nsize, string lpFileName);

    [DllImport("kernel32.dll")]
    public static extern uint GetCurrentThreadId();

    [DllImport("kernel32.dll")]
    public static extern int GetTempFileName(string lpPathName, string lpPrefixString, int uUnique, StringBuilder lpTempFileName);

    [DllImport("Kernel32.dll", SetLastError = true)]
    public static extern bool WriteFile(SafeFileHandle hFile, IntPtr lpBuffer, uint nNumberOfBytesToWrite, out uint lpNumberOfBytesWritten, IntPtr lpOverlapped);

    [DllImport("kernel32.dll")]
    public static extern bool QueryPerformanceCounter(ref long performanceCount);

    [DllImport("kernel32.dll")]
    public static extern bool QueryPerformanceFrequency(ref long frequency);

    [DllImport("kernel32.dll")]
    public static extern uint GetTickCount();

    [DllImport("kernel32.dll")]
    public static extern bool FileTimeToLocalFileTime([In] ref System.Runtime.InteropServices.ComTypes.FILETIME lpFileTime, out System.Runtime.InteropServices.ComTypes.FILETIME lpLocalFileTime);

    [DllImport("kernel32.dll")]
    public static extern bool LocalFileTimeToFileTime([In] ref System.Runtime.InteropServices.ComTypes.FILETIME lpLocalFileTime, out System.Runtime.InteropServices.ComTypes.FILETIME lpFileTime);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern IntPtr LoadLibrary(string lpFileName);

    [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
    public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
}
