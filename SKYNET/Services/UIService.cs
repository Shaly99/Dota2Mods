namespace SKYNET;

/// <summary>
/// Implementación de IUIService que usa los métodos existentes de modCommon.
/// Esto permite migrar gradualmente sin romper lo que ya funciona.
/// </summary>
public class UIService : IUIService
{
    public void WriteLine(string message, bool loading = false)
    {
        modCommon.WriteLine(message, loading);
    }

    public void ShowMessage(string message)
    {
        modCommon.Show(message);
    }

    public void ShowError(string message)
    {
        // Por ahora usa el mismo método
        // Más adelante puede mostrar un mensaje con estilo de error
        modCommon.Show($"Error: {message}");
    }

    public void LogException(Exception ex)
    {
        if (ex == null) return;
        modCommon.Save(ex);
    }

    public void LogException(string message, Exception ex)
    {
        if (ex == null) return;
        modCommon.Save(message, ex);
    }
}