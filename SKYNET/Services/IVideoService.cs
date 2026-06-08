using System.Windows.Forms;

namespace SKYNET;

public interface IVideoService
{
    void Initialize();

    /// <summary>API vieja para HeroAvatar — pinta el video automáticamente en el container.</summary>
    bool PlayLoop(string videoPath, Control container);
    void Stop(Control container);
    void Release(Control container);
    void StopAll();
    VlcFrameRenderer? GetRenderer(Control container);

    /// <summary>API nueva para overlay — devuelve el renderer para pintar manualmente.</summary>
    VlcFrameRenderer? PlayForRenderer(string videoPath, object owner);
    void StopForOwner(object owner);
}