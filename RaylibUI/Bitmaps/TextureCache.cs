using Civ2engine;
using Model;
using Model.Images;
using Raylib_CSharp.Textures;
using Raylib_CSharp.Colors;
using RaylibUtils;

namespace RaylibUI;

public static class TextureCache
{
    private static readonly Dictionary<string, Texture2D> Textures = new();

    public static Texture2D GetBordered(IUserInterface active, string name, IImageSource source)
    {
        if (!Textures.ContainsKey(name))
        {
            var padding = active.GetPadding(0f, false);
            var copy = Images.ExtractBitmap(source).Copy();
            copy.ResizeCanvas(copy.Width + padding.Left + padding.Right, copy.Height + padding.Left + padding.Right, padding.Left, padding.Top, Color.White);
            ImageUtils.PaintPanelBorders(active, ref copy, copy.Width, copy.Height, padding);
            Textures[name] = Texture2D.LoadFromImage(copy);
            copy.Unload();
        }

        return Textures[name];
    }

    public static Texture2D GetImage(IImageSource source)
    {
        return GetImage(source, null, -1);
    }

    public static Texture2D GetImage(IImageSource source, IUserInterface? activeInterface = null, int civ = -1)
    {
        var key = source.GetKey( civ);
        if (!Textures.ContainsKey(key))
        {
            var img = Images.ExtractBitmapData(source, activeInterface, civ).Image;
            Textures[key] = Texture2D.LoadFromImage(img);
            Textures[key].ApplyMapScaling();
            MapScaled.Add(key);
        }
        return Textures[key];
    }

    /// <summary>
    /// Keys of textures that follow the map scaling setting (bordered panels don't)
    /// </summary>
    private static readonly HashSet<string> MapScaled = new();

    /// <summary>
    /// Re-apply the map scaling setting to the cached textures after it changed
    /// </summary>
    public static void ApplyMapScaling()
    {
        foreach (var key in MapScaled)
        {
            if (Textures.TryGetValue(key, out var texture))
            {
                texture.ApplyMapScaling();
            }
        }
    }

    public static void Clear()
    {
        foreach (var texture in Textures.Where(t => !t.Key.StartsWith("Binary")))
        {
            texture.Value.Unload();
            Textures.Remove(texture.Key);
            MapScaled.Remove(texture.Key);
        }
        Images.ClearCache();
    }
}