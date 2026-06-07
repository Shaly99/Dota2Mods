using System.Collections.Concurrent;
using System.Diagnostics;

namespace SKYNET;

/// <summary>
/// Logger que detecta accesos a controles desde threads incorrectos.
/// Guarda un log para identificar lugares problemáticos sin crashear la app.
/// </summary>
public static class ThreadViolationLogger
{
    private static readonly ConcurrentDictionary<string, int> _violations = new();
    private static readonly object _fileLock = new();
    private static string _logPath;

    static ThreadViolationLogger()
    {
        try
        {
            _logPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "dota2mods_thread_violations.log");
        }
        catch
        {
            _logPath = "thread_violations.log";
        }
    }

    /// <summary>
    /// Registra una violación de thread (acceso a UI desde thread incorrecto).
    /// Solo loguea cada violación única una vez para no saturar el archivo.
    /// </summary>
    public static void LogViolation(string operation)
    {
        try
        {
            // Capturar el stack trace para saber dónde se produjo
            var stackTrace = new StackTrace(skipFrames: 2, fNeedFileInfo: true);
            var frame = stackTrace.GetFrame(0);
            var methodInfo = frame?.GetMethod();

            string key = $"{methodInfo?.DeclaringType?.Name}.{methodInfo?.Name}#{operation}";

            // Solo loguear cada violación única una vez
            int count = _violations.AddOrUpdate(key, 1, (k, v) => v + 1);

            if (count == 1) // Solo primera vez
            {
                WriteToFile(key, stackTrace.ToString());
            }
        }
        catch
        {
            // Nunca crashear por el logger
        }
    }

    private static void WriteToFile(string violation, string stack)
    {
        try
        {
            lock (_fileLock)
            {
                File.AppendAllText(_logPath,
                    $"=== {DateTime.Now:yyyy-MM-dd HH:mm:ss} ===\n" +
                    $"Operación: {violation}\n" +
                    $"Thread ID: {Thread.CurrentThread.ManagedThreadId}\n" +
                    $"Stack:\n{stack}\n\n");
            }
        }
        catch { }
    }

    /// <summary>
    /// Cantidad total de violaciones detectadas (todas las únicas).
    /// </summary>
    public static int UniqueViolationsCount => _violations.Count;

    /// <summary>
    /// Cantidad total de veces que se llamó (incluyendo repetidas).
    /// </summary>
    public static int TotalViolations
    {
        get
        {
            int total = 0;
            foreach (var v in _violations.Values) total += v;
            return total;
        }
    }

    /// <summary>
    /// Limpia el log (útil para tests).
    /// </summary>
    public static void Clear()
    {
        _violations.Clear();
        try
        {
            if (File.Exists(_logPath)) File.Delete(_logPath);
        }
        catch { }
    }
}