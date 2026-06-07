namespace SKYNET;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
            var ex = e.ExceptionObject as Exception;
            string logPath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "dota2mods_crash.log");

            try
            {
                System.IO.File.WriteAllText(logPath,
                    $"FATAL: {ex?.GetType().Name}\n{ex?.Message}\n\n{ex?.StackTrace}");
            }
            catch { }

            MessageBox.Show($"Error: {ex?.Message}\n\n{ex?.StackTrace}", "Crash detectado");
        };

        Application.ThreadException += (s, e) =>
        {
            string logPath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "dota2mods_crash.log");

            try
            {
                System.IO.File.WriteAllText(logPath,
                    $"THREAD: {e.Exception.GetType().Name}\n{e.Exception.Message}\n\n{e.Exception.StackTrace}");
            }
            catch { }

            MessageBox.Show($"Error de hilo: {e.Exception.Message}\n\n{e.Exception.StackTrace}", "Crash detectado");
        };

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(defaultValue: false);
        FontService.Initialize();
        Application.Run(new frmMain());
    }
}