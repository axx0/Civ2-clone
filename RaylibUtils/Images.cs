using Civ2engine;
using Raylib_cs;

namespace RayLibUtils;

public static class Images
{
    public static Image LoadImage(string filename, string[] searchPaths, params string[] extensions)
    {
        var path = Utils.GetFilePath(filename, searchPaths, extensions);
        return Raylib.LoadImageFromMemory(Path.GetExtension(path).ToLowerInvariant(), File.ReadAllBytes(path));
    }
}