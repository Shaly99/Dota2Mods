namespace SKYNET;

/// <summary>
/// Helpers thread-safe para operaciones de UI.
/// Garantiza que el código se ejecute en el thread correcto sin crashes.
/// </summary>
public static class UIHelpers
{
    /// <summary>
    /// Ejecuta una acción en el thread de UI del control.
    /// Si ya estamos en el thread correcto, ejecuta directamente.
    /// Si estamos en otro thread, hace Invoke seguro.
    /// </summary>
    public static void InvokeIfRequired(this Control control, Action action)
    {
        if (control == null || action == null) return;

        // Si el control está siendo disposed o ya está disposed, no hacer nada
        if (control.IsDisposed || control.Disposing)
        {
            return;
        }

        try
        {
            if (control.InvokeRequired)
            {
                // Estamos en otro thread → invocar al thread de UI
                control.Invoke(action);
            }
            else
            {
                // Ya estamos en el thread de UI → ejecutar directo
                action();
            }
        }
        catch (ObjectDisposedException)
        {
            // El control se disposed mientras estábamos por invocar - ignorar
        }
        catch (InvalidOperationException)
        {
            // El handle aún no fue creado - ignorar
        }
    }

    /// <summary>
    /// Versión asincrónica: no espera a que termine la acción.
    /// Más rápido pero no podés saber cuándo terminó.
    /// </summary>
    public static void BeginInvokeIfRequired(this Control control, Action action)
    {
        if (control == null || action == null) return;
        if (control.IsDisposed || control.Disposing) return;

        try
        {
            if (control.InvokeRequired)
            {
                control.BeginInvoke(action);
            }
            else
            {
                action();
            }
        }
        catch (ObjectDisposedException) { }
        catch (InvalidOperationException) { }
    }

    /// <summary>
    /// Obtiene una propiedad de un control desde cualquier thread.
    /// Útil cuando necesitás leer (ej: el Text de un Label) desde un Task.
    /// </summary>
    public static T GetProperty<T>(this Control control, Func<T> getter)
    {
        if (control == null || getter == null) return default;
        if (control.IsDisposed) return default;

        try
        {
            if (control.InvokeRequired)
            {
                return (T)control.Invoke(getter);
            }
            return getter();
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// Verifica si estamos en el thread de UI principal.
    /// Útil para asserts o debugging.
    /// </summary>
    public static bool IsOnUIThread(this Control control)
    {
        if (control == null) return false;
        return !control.InvokeRequired;
    }
}