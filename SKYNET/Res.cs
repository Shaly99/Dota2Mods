using System.Resources;

namespace SKYNET;

internal class Res
{
    private static ResourceManager resMan;

    public static bool DebugMode;

    public static string Get(StringId stringId)
    {
        return Get(stringId.ToString());
    }

    public static string Get(string name)
    {
        if (DebugMode)
        {
            return "[" + name + "]";
        }
        return resMan.GetString(name) ?? ("@" + name);
    }
}
