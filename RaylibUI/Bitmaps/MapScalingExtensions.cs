using Civ2engine;
using Civ2engine.Enums;
using Raylib_CSharp.Textures;

namespace RaylibUI;

public static class MapScalingExtensions
{
    /// <summary>
    /// Raylib texture filter for a map scaling mode. Only modes that work without mipmaps are used.
    /// </summary>
    public static TextureFilter ToTextureFilter(this MapScaling scaling) => scaling switch
    {
        MapScaling.Smooth => TextureFilter.Bilinear,
        _ => TextureFilter.Point,
    };

    /// <summary>
    /// Apply the current map scaling setting to a map texture
    /// </summary>
    public static void ApplyMapScaling(this Texture2D texture)
    {
        texture.SetFilter(Settings.MapScaling.ToTextureFilter());
    }
}
