using System;
using System.Collections.Generic;
using System.IO;

namespace SKYNET;

/// <summary>
/// Implementación del servicio de videos de héroes.
/// Cachea las rutas para acceso O(1).
/// </summary>
public class HeroVideoService : IHeroVideoService
{
    private readonly Dictionary<string, string> _videoPaths
        = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    public bool HasVideos => _videoPaths.Count > 0;

    public void Load(string videoFolder)
    {
        _videoPaths.Clear();

        if (string.IsNullOrEmpty(videoFolder)) return;
        if (!Directory.Exists(videoFolder)) return;

        try
        {
            var files = Directory.GetFiles(videoFolder, "npc_dota_hero_*.webm");
            foreach (var file in files)
            {
                string heroName = Path.GetFileNameWithoutExtension(file);
                _videoPaths[heroName] = file;
            }
        }
        catch { }
    }

    public string GetVideoPath(string heroName)
    {
        if (string.IsNullOrEmpty(heroName)) return null;
        return _videoPaths.TryGetValue(heroName, out var path) ? path : null;
    }
}