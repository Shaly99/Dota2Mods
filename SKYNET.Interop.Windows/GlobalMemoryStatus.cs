using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

public class GlobalMemoryStatus
{
    private struct MEMORYSTATUSEX
    {
        public int Length;

        public int MemoryLoad;

        public long TotalPhysical;

        public long AvailablePhysical;

        public long TotalPageFile;

        public long AvailablePageFile;

        public long TotalVirtual;

        public long AvailableVirtual;

        public long AvailableExtendedVirtual;
    }

    private MEMORYSTATUSEX _memoryStatus = default(MEMORYSTATUSEX);

    public int MemoryLoad => _memoryStatus.MemoryLoad;

    public long TotalPhysical => _memoryStatus.TotalPhysical;

    public long AvailablePhysical => _memoryStatus.AvailablePhysical;

    public long TotalPageFile => _memoryStatus.TotalPageFile;

    public long AvailablePageFile => _memoryStatus.AvailablePageFile;

    public long TotalVirtual => _memoryStatus.TotalVirtual;

    public long AvailableVirtual => _memoryStatus.AvailableVirtual;

    public long AvailableExtendedVirtual => _memoryStatus.AvailableExtendedVirtual;

    public GlobalMemoryStatus()
    {
        _memoryStatus.Length = Marshal.SizeOf(typeof(MEMORYSTATUSEX));
        if (!GlobalMemoryStatusEx(ref _memoryStatus))
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }

    [DllImport("Kernel32.dll", SetLastError = true)]
    private static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);
}
