using Civ2engine;
using Model;
using Model.Images;
using Raylib_cs;

namespace RaylibUI;

public static class TextureCache
{
    private static readonly Dictionary<string, Texture2D> Textures = new();
    private const int BORDER_WIDTH = 11;
    public const int DOUBLE_WIDTH = BORDER_WIDTH * 2;

    public static Texture2D GetBordered(string name, IImageSource source)
    {
        if (!Textures.ContainsKey(name))
        {
            var copy = Raylib.ImageCopy(Images.ExtractBitmap(source));
            Raylib.ImageResizeCanvas(ref copy, copy.width + DOUBLE_WIDTH, copy.height + DOUBLE_WIDTH, BORDER_WIDTH, BORDER_WIDTH, Color.WHITE);
            ImageUtils.PaintPanelBorders(ref copy, copy.width, copy.height, BORDER_WIDTH,BORDER_WIDTH);
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
            var img = Images.ExtractBitmap(source, activeInterface, civ);
            Textures[key] = Raylib.LoadTextureFromImage(img);
        }
        return Textures[key];
    }
}