namespace SKYNET;

/// <summary>
/// Interfaz para acceder a la configuración global de la app.
/// </summary>
public interface ISettingsService
{
    string Dota2Path { get; set; }
    bool GenerateOnStart { get; set; }
    bool OpenDota { get; set; }
    bool ActiveSounds { get; set; }
    string ClientVersion { get; set; }

    /// <summary>
    /// Guarda la configuración en disco.
    /// </summary>
    void Save();

    /// <summary>
    /// Carga la configuración desde disco.
    /// </summary>
    void Load();
}