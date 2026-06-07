using NAudio.Wave;
using SKYNET;
using ValveResourceFormat;
using ValveResourceFormat.ResourceTypes;

internal class AudioPlayer
{
    private static WaveOutEvent heroPlayer;

    public static WaveOutEvent soundPlayer;

    private static WaveStream waveStream;

    public static bool Playing { get; internal set; }

    /// <summary>
    /// Acceso al servicio del VPK (encapsula el package).
    /// </summary>
    private static IVpkService Vpk => ServiceContainer.Get<IVpkService>();

    static AudioPlayer()
    {
        heroPlayer = new WaveOutEvent();
        soundPlayer = new WaveOutEvent();
        soundPlayer.PlaybackStopped += SoundPlayer_PlaybackStopped;
    }

    private static void SoundPlayer_PlaybackStopped(object sender, StoppedEventArgs e)
    {
        frmMain.frm.stopSounds.Visible = false;
    }

    public static void StopSounds()
    {
        heroPlayer.Stop();
        soundPlayer.Stop();
        Playing = false;
    }

    public static void PlayHeroName(string HeroName)
    {
        if (!Settings.ActiveSounds)
        {
            return;
        }

        HeroName = HeroName.Replace("npc_dota_hero_", "");
        string[] names = HeroName.Split('_');
        HeroName = names[0];

        if (HeroName.ToLower() == "pangolier")
        {
            HeroName = "pangoli";
        }

        // 🔑 Verificar que el VPK esté cargado
        if (!Vpk.IsLoaded)
        {
            return;
        }

        try
        {
            // 🔑 Usar el servicio para obtener entradas
            List<PackageEntry> list = Vpk.GetEntriesByExtension("vsnd_c")
                .Where(l => l.DirectoryName.Contains("sounds/vo/" + HeroName))
                .ToList();

            if (list.Count == 0)
            {
                list = Vpk.GetEntriesByExtension("vsnd_c")
                    .Where(l => l.DirectoryName.Contains("sounds/vo/") && l.DirectoryName.Contains(names[1]))
                    .ToList();
            }

            if (!list.Any())
            {
                return;
            }

            PackageEntry packageEntry = null;
            if (names.Count() > 1)
            {
                packageEntry = list.Find(x => x.FileName.Contains("_spawn_01") && x.DirectoryName.Contains(names[1]));
                if (packageEntry == null)
                {
                    packageEntry = list.Find(x => x.FileName.Contains("spawn") && x.DirectoryName.Contains(names[1]));
                }
                if (packageEntry == null)
                {
                    packageEntry = list.Find(x => x.FileName.Contains("level") && x.DirectoryName.Contains(names[1]));
                }
                if (packageEntry != null)
                {
                    Play(packageEntry, heroPlayer, toStop: false);
                }
                return;
            }

            packageEntry = list.Find(x => x.FileName.Contains("_spawn_01"));
            if (packageEntry == null)
            {
                packageEntry = list.Find(x => x.FileName.Contains("spawn"));
            }
            if (packageEntry == null)
            {
                packageEntry = list.Find(x => x.FileName.Contains("level"));
            }
            if (packageEntry != null)
            {
                Play(packageEntry, heroPlayer, toStop: false);
            }
        }
        catch (Exception)
        {
        }
    }

    public static void PlaySound(string filePath, bool toStop, int x = 0)
    {
        try
        {
            // 🔑 Verificar que el VPK esté cargado
            if (!Settings.ActiveSounds || !Vpk.IsLoaded)
            {
                return;
            }

            // 🔑 Usar el servicio para buscar entrada
            PackageEntry packageEntry = Vpk.FindEntry(filePath + "_c");

            if (packageEntry == null)
            {
                string path = Path.GetDirectoryName(filePath);
                path = path.Replace("\\", "/");

                // 🔑 Usar el servicio para obtener entradas
                List<PackageEntry> list = Vpk.GetEntriesByExtension("vsnd_c")
                    .Where(l => l.DirectoryName.Contains(path.ToLower()))
                    .ToList();

                if (!list.Any() || !filePath.ToLower().Contains("announcer"))
                {
                    return;
                }

                bool flag = false;
                foreach (PackageEntry item in list)
                {
                    if (item.FileName.ToLower().Contains("welcome_01"))
                    {
                        flag = true;
                        Play(item, soundPlayer, toStop, x);
                    }
                }

                if (flag)
                {
                    return;
                }

                foreach (PackageEntry item2 in list)
                {
                    if (item2.FileName.ToLower().Contains("rampage_01"))
                    {
                        Play(item2, soundPlayer, toStop, x);
                        break;
                    }
                }
                return;
            }

            Play(packageEntry, soundPlayer, toStop, x);
        }
        catch (Exception)
        {
        }
    }

    public static void Play(PackageEntry entry, WaveOutEvent waveOut, bool toStop, int x = 0)
    {
        if (toStop)
        {
            frmMain.frm.stopSounds.Visible = true;
            frmMain.frm.stopSounds.Location = new Point(x, frmMain.frm.stopSounds.Location.Y);
        }

        // 🔑 Usar el servicio para leer la entrada
        Vpk.ReadEntry(entry, out var output);

        Resource resource = new Resource();
        resource.Read(new MemoryStream(output));

        // Usamos BlockType.DATA (no Audio)
        Sound sound = resource.DataBlock as Sound;
        MemoryStream soundStream = sound.GetSoundStream();
        waveOut.Volume = 0.5f;

        // ─── Solución definitiva: StreamMediaFoundationReader reproduce todos los formatos comunes en Windows ───
        waveStream = new StreamMediaFoundationReader(soundStream);

        waveOut.Stop();
        waveOut.Init(waveStream);
        waveOut.Play();
    }
}