using Model;
using Model.Images;
using Raylib_cs;

namespace RaylibUI;

public class ImagePanel
{
    private readonly Texture2D _texture;

    public ImagePanel(string name, IImageSource imageSource, Point location)
    {
        Key = name;
        Location = location;

        _texture = TextureCache.GetBordered(name, imageSource);
    }

    public string Key { get; }
    public Point Location { get; set; }

    public void Draw()
    {
        Raylib.DrawTexture(_texture, Location.X, Location.Y, Color.WHITE);
    }
}