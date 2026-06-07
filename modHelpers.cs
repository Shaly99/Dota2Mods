namespace SKYNET;

/// <summary>
/// Facade (wrapper) que delega al IModBuilderService registrado en ServiceContainer.
/// Mantiene la API estática vieja para que el código existente siga funcionando.
/// </summary>
internal class modHelpers
{
    private static IModBuilderService Service => ServiceContainer.Get<IModBuilderService>();

    public static void ModWorker_DoWork() => Service.BuildMod();

    public static void ClearGameInfo() => Service.ClearGameInfo();

    public static void ModWorker_RunWorkerCompleted()
    {
        // Este método ya no se usa directamente, BuildMod lo maneja internamente.
        // Lo dejamos por compatibilidad por si alguien lo llama.
    }
}