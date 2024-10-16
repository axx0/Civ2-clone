using Model;
using Model.Images;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Textures;
using Raylib_CSharp.Windowing;

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
            x = (int)((1 + Location.X) * Window.GetScreenWidth()) - _texture.Width;
        }
        else if (Location.X > 0)
        {
            x = (int)(Location.X * Window.GetScreenWidth());
        }
        else // =0 (center on screen)
        {
            x = (int)(Window.GetScreenWidth() * 0.5 - _texture.Width * 0.5);
        }

        if (Location.Y < 0)
        {
            y = (int)((1 + Location.Y) * Window.GetScreenHeight()) - _texture.Height;
        }
        else if (Location.Y > 0)
        {
            y = (int)(Location.Y * Window.GetScreenHeight());
        }
        else
        {
            y = (int)(Window.GetScreenHeight() * 0.5 - _texture.Height * 0.5);
        }

        Graphics.DrawTexture(_texture, x, y, Color.White);
    }
}