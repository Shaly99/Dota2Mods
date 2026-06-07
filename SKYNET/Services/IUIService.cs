namespace SKYNET;

/// <summary>
/// Interfaz para servicios de UI: mensajes, logs, diálogos.
/// Permite desacoplar el código del formulario principal.
/// </summary>
public interface IUIService
{
    /// <summary>
    /// Muestra un mensaje de carga/progreso en la UI.
    /// </summary>
    /// <param name="message">Mensaje a mostrar</param>
    /// <param name="loading">Si es true, muestra en el indicador de carga</param>
    void WriteLine(string message, bool loading = false);

    /// <summary>
    /// Muestra un mensaje al usuario en un diálogo modal.
    /// </summary>
    void ShowMessage(string message);

    /// <summary>
    /// Muestra un mensaje de error al usuario.
    /// </summary>
    void ShowError(string message);

    /// <summary>
    /// Loguea una excepción (a archivo, consola, etc.).
    /// </summary>
    void LogException(Exception ex);

    /// <summary>
    /// Loguea una excepción con un mensaje adicional.
    /// </summary>
    void LogException(string message, Exception ex);
}