namespace SKYNET;

/// <summary>
/// Interfaz para reproducción de audio del juego.
/// </summary>
public interface IAudioService
{
    /// <summary>
    /// Reproduce el nombre del héroe seleccionado.
    /// </summary>
    void PlayHeroName(string heroName);

    /// <summary>
    /// Reproduce un sonido desde el VPK.
    /// </summary>
    /// <param name="filePath">Ruta del sonido dentro del VPK</param>
    /// <param name="toStop">Si es true, permite detener el sonido con el botón</param>
    /// <param name="x">Posición X opcional para el botón de stop</param>
    void PlaySound(string filePath, bool toStop, int x = 0);

    /// <summary>
    /// Detiene todos los sonidos en reproducción.
    /// </summary>
    void StopSounds();

    /// <summary>
    /// Indica si hay algún sonido reproduciéndose.
    /// </summary>
    bool IsPlaying { get; }
}