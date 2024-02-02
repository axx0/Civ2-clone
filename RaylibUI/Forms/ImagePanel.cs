using Model;
using Model.Images;
using Raylib_cs;

namespace RaylibUI.Forms;

public class ImagePanel
{
    private readonly Texture2D _texture;

    public ImagePanel(IUserInterface active, string name, IImageSource imageSource, Point location)
    {
        Key = name;
        Location = location;

        _texture = TextureCache.GetBordered(active, name, imageSource);
    }

    public string Key { get; }
    public Point Location { get; set; }

    public void Draw()
    {
        int X, Y;

        // Panel position on screen
        if (Location.X < 0) // offset from right
        {
            X = (int)((1 + Location.X) * Raylib.GetScreenWidth()) - _texture.Width;
        }
        else if (Location.X > 0)
        {
            X = (int)(Location.X * Raylib.GetScreenWidth());
        }
        else // =0 (center on screen)
        {
            X = (int)(Raylib.GetScreenWidth() * 0.5 - _texture.Width * 0.5);
        }

        if (Location.Y < 0)
        {
            Y = (int)((1 + Location.Y) * Raylib.GetScreenHeight()) - _texture.Height;
        }
        else if (Location.Y > 0)
        {
            Y = (int)(Location.Y * Raylib.GetScreenHeight());
        }
        else
        {
            Y = (int)(Raylib.GetScreenHeight() * 0.5 - _texture.Height * 0.5);
        }

        Raylib.DrawTexture(_texture, X, Y, Color.WHITE);
    }
}