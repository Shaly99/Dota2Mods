using Microsoft.VisualBasic;
using System.Globalization;
using System.Text;

namespace SKYNET;

public class ThreadManager
{
    public static List<Thread> ActiveThreads { get; set; }

    public static void RunThread(Action startThread)
    {
        Thread thread = new Thread(startThread.Invoke);
        thread.IsBackground = true;
        thread.Name = CreateName();
        thread.Start();
        ActiveThreads.Add(thread);
    }

    private static void NewThreadCore(Thread t, bool inheritCulture, bool singleThreadedApartment, bool background)
    {
        t.Name = CreateName();
        modCommon.Show(t.Name);
        if (inheritCulture)
        {
            t.CurrentCulture = CultureInfo.CurrentCulture;
            t.CurrentUICulture = CultureInfo.CurrentUICulture;
        }
        if (singleThreadedApartment)
        {
            t.SetApartmentState(ApartmentState.STA);
        }
        t.IsBackground = background;
    }

    public static void StopThreads()
    {
        for (int i = 0; i < ActiveThreads.Count; i++)
        {
            Thread thread = ActiveThreads[i];
            try
            {
                thread?.Join();
            }
            catch
            {
            }
        }
    }

    static ThreadManager()
    {
        ActiveThreads = new List<Thread>();
    }

    private static string CreateName()
    {
        string result = "";
        checked
        {
            try
            {
                string text = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                short maxValue = (short)text.Length;
                StringBuilder stringBuilder = new StringBuilder();
                int num = 1;
                Random random = new Random();
                do
                {
                    int startIndex = random.Next(0, maxValue);
                    stringBuilder.Append(text.Substring(startIndex, 1));
                    num++;
                }
                while (num <= 6);
                stringBuilder.Append(DateAndTime.Now.ToString("HHmmss"));
                result = stringBuilder.ToString();
            }
            catch (Exception)
            {
            }
            return result;
        }
    }
}
