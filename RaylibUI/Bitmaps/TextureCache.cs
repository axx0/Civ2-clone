using Civ2engine;
using Model;
using Model.Images;
using Raylib_cs;
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
            var copy = Raylib.ImageCopy(Images.ExtractBitmap(source));
            Raylib.ImageResizeCanvas(ref copy, copy.Width + padding.Left + padding.Right, copy.Height + padding.Left + padding.Right, padding.Left, padding.Top, Color.White);
            ImageUtils.PaintPanelBorders(active, ref copy, copy.Width, copy.Height, padding);
            Textures[name] = Raylib.LoadTextureFromImage(copy);
        }

        return Textures[name];
    }

    public static Texture2D GetImage(IImageSource source)
    {
        return GetImage(source, null, -1);
    }

    public static Texture2D GetImage(IImageSource source, IUserInterface activeInterface = null, int civ = -1)
    {
        var key = source.GetKey( civ);
        if (!Textures.ContainsKey(key))
        {
            var img = Images.ExtractBitmapData(source, activeInterface, civ).Image;
            Textures[key] = Raylib.LoadTextureFromImage(img);
            Raylib.SetTextureFilter(Textures[key], (TextureFilter)Settings.TextureFilter);
        }
        return Textures[key];
    }

    public static void Clear()
    {
        foreach (var texturesValue in Textures.Values)
        {
            Raylib.UnloadTexture(texturesValue);
        }
        Textures.Clear();
        Images.ClearCache();
    }
}