namespace SKYNET;

/// <summary>
/// Implementación de IAudioService usando AudioPlayer existente.
/// </summary>
public class AudioService : IAudioService
{
    public bool IsPlaying => AudioPlayer.Playing;

    public void PlayHeroName(string heroName)
    {
        AudioPlayer.PlayHeroName(heroName);
    }

    public void PlaySound(string filePath, bool toStop, int x = 0)
    {
        AudioPlayer.PlaySound(filePath, toStop, x);
    }

    public void StopSounds()
    {
        AudioPlayer.StopSounds();
    }
}