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
        int x, y;

        // Panel position on screen
        if (Location.X < 0) // offset from right
        {
            x = (int)((1 + Location.X) * Raylib.GetScreenWidth()) - _texture.width;
        }
        else if (Location.X > 0)
        {
            x = (int)(Location.X * Raylib.GetScreenWidth());
        }
        else // =0 (center on screen)
        {
            x = (int)(Raylib.GetScreenWidth() * 0.5 - _texture.width * 0.5);
        }

        if (Location.Y < 0)
        {
            y = (int)((1 + Location.Y) * Raylib.GetScreenHeight()) - _texture.height;
        }
        else if (Location.Y > 0)
        {
            y = (int)(Location.Y * Raylib.GetScreenHeight());
        }
        else
        {
            y = (int)(Raylib.GetScreenHeight() * 0.5 - _texture.height * 0.5);
        }

        Raylib.DrawTexture(_texture, x, y, Color.WHITE);
    }
}