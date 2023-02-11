using Model.Images;
using Raylib_cs;

namespace RaylibUI;

public static class TextureCache
{
    private static readonly Dictionary<string, Texture2D> Textures = new();
    public static Texture2D GetBordered(string name, IImageSource source)
    {
        if (!Textures.ContainsKey(name))
        {
            var original = Images.ExtractBitmap(source);
            var origRect = new Rectangle(0, 0, original.width, original.height);
            var copy = Raylib.ImageFromImage(original, origRect);
            Raylib.ImageResizeCanvas(ref copy, copy.width + 22, copy.height + 22, 11, 11, Color.WHITE);
            ImageUtils.PaintPanelBorders(ref copy, copy.width, copy.height, 11,11);
            Raylib.ImageDraw(ref copy, original, origRect, new Rectangle(11,11,origRect.width, origRect.height),Color.WHITE);
            Textures[name] = Raylib.LoadTextureFromImage(copy);
        }

        return Textures[name];
    }

    public static Texture2D GetImage(IImageSource source)
    {
        if (!Textures.ContainsKey(source.Key))
        {
            var img = Images.ExtractBitmap(source);
            Textures[source.Key] = Raylib.LoadTextureFromImage(img);
        }
        return Textures[source.Key];
    }
}