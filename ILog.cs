using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System.Diagnostics;
using System.Reflection;
using System.Text;

public class ILog
{
    public class LogMutex
    {
        public static Mutex mutexFile = new Mutex(initiallyOwned: false, "LogMutex");
    }

    private static Mutex mutexFile;

    public static string fileName = "Dota2Mod.log";

    private static Process currentProcess;

    public bool IsDebugEnabled = true;

    private Assembly assembly;

    private string HeadMessage;

    public string Message { get; set; } = "";

    public static void Save(Exception ex)
    {
        string text = "";
        if (fileName == "")
        {
            fileName = Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().ProcessName) + ".log";
        }
        try
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(ex.Message);
            if (ex.InnerException != null)
            {
                stringBuilder.Append("\r\n").Append(ex.InnerException);
            }
            if (ex.StackTrace != null)
            {
                stringBuilder.Append("\r\n").Append(ex.StackTrace);
            }
            text = string.Format($"{stringBuilder.ToString()}:");
            AppendFile(text, fileName);
        }
        catch
        {
        }
    }

    public static void Save(string strs, Exception ex)
    {
        string text = "";
        if (fileName == "")
        {
            fileName = Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().ProcessName) + ".log";
        }
        try
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(ex.Message);
            if (ex.InnerException != null)
            {
                stringBuilder.Append("\r\n").Append(ex.InnerException);
            }
            if (ex.StackTrace != null)
            {
                stringBuilder.Append("\r\n").Append(ex.StackTrace);
            }
            text = string.Format($"{stringBuilder.ToString()}:");
            AppendFile(strs + Environment.NewLine + text, fileName);
        }
        catch
        {
        }
    }

    public static void AppendFile(string s, string filename)
    {
        string path = Path.Combine(modCommon.DataDirectory, filename);
        StreamWriter streamWriter = null;
        try
        {
            mutexFile = LogMutex.mutexFile;
            mutexFile.WaitOne();
            FileStream stream = new FileStream(path, FileMode.Append, FileAccess.Write);
            streamWriter = new StreamWriter(stream);
            streamWriter.BaseStream.Seek(0L, SeekOrigin.End);
            streamWriter.WriteLine(Conversions.ToString(DateAndTime.Now) + ":" + s);
            streamWriter.Close();
        }
        catch (Exception ex)
        {
            ProjectData.SetProjectError(ex);
            Exception ex2 = ex;
            streamWriter?.Close();
            ProjectData.ClearProjectError();
        }
        finally
        {
            mutexFile.ReleaseMutex();
        }
    }

    public ILog(string Head)
    {
        HeadMessage = Head;
    }

    public ILog()
    {
    }

    public static string GetExePatch()
    {
        try
        {
            currentProcess = Process.GetCurrentProcess();
            return new FileInfo(currentProcess.MainModule.FileName).FullName;
        }
        finally
        {
            currentProcess = null;
        }
    }

    public bool IsWarnEnabled()
    {
        return true;
    }

    public void MoreInfo(object v, Exception ex8 = null)
    {
    }
}
