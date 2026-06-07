namespace SKYNET;

/// <summary>
/// Interfaz para reportar progreso de operaciones largas.
/// Permite mostrar al usuario el avance de extracciones, etc.
/// </summary>
public interface IProgressReporter
{
    /// <summary>
    /// Inicia una nueva operación con un total esperado de items.
    /// </summary>
    void Start(string operationName, int totalItems);

    /// <summary>
    /// Reporta que se procesó un item.
    /// </summary>
    void IncrementProgress();

    /// <summary>
    /// Reporta que se procesaron N items de una vez.
    /// </summary>
    void IncrementProgress(int count);

    /// <summary>
    /// Cambia la categoría/subtarea actual.
    /// </summary>
    void SetCurrentTask(string task);

    /// <summary>
    /// Finaliza la operación actual.
    /// </summary>
    void Complete();

    /// <summary>
    /// Cantidad procesada hasta el momento.
    /// </summary>
    int ProcessedItems { get; }

    /// <summary>
    /// Cantidad total esperada.
    /// </summary>
    int TotalItems { get; }
}