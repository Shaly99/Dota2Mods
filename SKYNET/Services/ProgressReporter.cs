namespace SKYNET;

/// <summary>
/// Implementación de IProgressReporter que reporta a frmMain.
/// Thread-safe para uso desde múltiples threads paralelos.
/// </summary>
public class ProgressReporter : IProgressReporter
{
    // 🔑 Contador thread-safe
    private int _processedItems;
    private int _totalItems;

    private string _operationName = "";
    private string _currentTask = "";
    private DateTime _startTime;
    private readonly object _stateLock = new object();

    // Throttling para no saturar la UI
    private DateTime _lastUIUpdate = DateTime.MinValue;
    private const int UI_UPDATE_INTERVAL_MS = 100; // Actualizar UI máx cada 100ms

    public int ProcessedItems => _processedItems;
    public int TotalItems => _totalItems;

    // ════════════════════════════════════════════════════════
    // API PÚBLICA
    // ════════════════════════════════════════════════════════

    public void Start(string operationName, int totalItems)
    {
        lock (_stateLock)
        {
            _operationName = operationName ?? "";
            _totalItems = totalItems;
            _processedItems = 0;
            _startTime = DateTime.Now;
            _currentTask = "";
            _lastUIUpdate = DateTime.MinValue;
        }

        UpdateUI(force: true);
    }

    public void IncrementProgress()
    {
        IncrementProgress(1);
    }

    public void IncrementProgress(int count)
    {
        Interlocked.Add(ref _processedItems, count);
        UpdateUI(force: false);
    }

    public void SetCurrentTask(string task)
    {
        lock (_stateLock)
        {
            _currentTask = task ?? "";
        }
        UpdateUI(force: true);
    }

    public void Complete()
    {
        lock (_stateLock)
        {
            _processedItems = _totalItems;
        }
        UpdateUI(force: true);
    }

    // ════════════════════════════════════════════════════════
    // ACTUALIZACIÓN DE UI
    // ════════════════════════════════════════════════════════

    private void UpdateUI(bool force)
    {
        // Throttling: no actualizar UI más de 10 veces por segundo
        if (!force)
        {
            var now = DateTime.Now;
            lock (_stateLock)
            {
                if ((now - _lastUIUpdate).TotalMilliseconds < UI_UPDATE_INTERVAL_MS)
                {
                    return;
                }
                _lastUIUpdate = now;
            }
        }

        try
        {
            string message = BuildProgressMessage();

            if (frmMain.frm != null)
            {
                modCommon.InvokeAction(frmMain.frm, () =>
                {
                    frmMain.frm.MostrarLabel(message);
                });
            }
        }
        catch
        {
            // Ignorar errores de UI (puede pasar al cerrar app)
        }
    }

    private string BuildProgressMessage()
    {
        int processed, total;
        string operationName, currentTask;
        DateTime startTime;

        lock (_stateLock)
        {
            processed = _processedItems;
            total = _totalItems;
            operationName = _operationName;
            currentTask = _currentTask;
            startTime = _startTime;
        }

        if (total <= 0)
        {
            return string.IsNullOrEmpty(currentTask) ? operationName : currentTask;
        }

        // Calcular porcentaje
        int percent = (int)((double)processed / total * 100);
        if (percent > 100) percent = 100;

        // Calcular tiempo restante
        string timeRemaining = "";
        var elapsed = DateTime.Now - startTime;
        if (processed > 10 && elapsed.TotalSeconds > 1)
        {
            double itemsPerSecond = processed / elapsed.TotalSeconds;
            if (itemsPerSecond > 0)
            {
                int remaining = total - processed;
                var secondsRemaining = remaining / itemsPerSecond;

                if (secondsRemaining < 60)
                {
                    timeRemaining = $" - {(int)secondsRemaining}s restantes";
                }
                else
                {
                    int minutes = (int)(secondsRemaining / 60);
                    int seconds = (int)(secondsRemaining % 60);
                    timeRemaining = $" - {minutes}m {seconds}s restantes";
                }
            }
        }

        // Construir mensaje
        string taskInfo = string.IsNullOrEmpty(currentTask) ? "" : $"\n{currentTask}";
        return $"{operationName} - {percent}% ({processed:N0}/{total:N0}){taskInfo}{timeRemaining}";
    }
}