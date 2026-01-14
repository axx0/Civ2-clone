using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;
using Model.Images;
using RaylibUtils;
using Model.Dialog;
using Model;

namespace RaylibUI.BasicTypes.Controls;

public class ImageBox : BaseControl
{
    private readonly IUserInterface _active;

    public ImageBox(IControlLayout controller, DialogImageElements image) : base(controller, true)
    {
        Image = image.Image;
        Scale = image.Scale;
        Coords = image.Coords;
        _active = controller.MainWindow.ActiveInterface;
    }

    public ImageBox(IControlLayout controller, IImageSource? image, float scale = 1.0f) : base(controller, true)
    {
        Image = [image];
        Scale = scale;
    }

    public IImageSource?[] Image { get; set; }
    public int[,] Coords { get; set; } = new int[,] { { 0, 0 } };
    public float Scale { get; set; } = 1.0f;

    public override int GetPreferredWidth()
    {
        return Image.Select(img => Images.GetImageWidth(img, _active, Scale)).Max();
    }

    public override int GetPreferredHeight()
    {
        return Image.Select(img => Images.GetImageHeight(img, _active, Scale)).Max();
    }

    private int _width;
    public override int Width
    {
        get
        {
            if (_width == 0)
            {
                _width = GetPreferredWidth();
            }

            return _width;
        }
        set { _width = value; }
    }

    private int _height;
    public override int Height
    {
        get
        {
            if (_height == 0)
            {
                _height = GetPreferredHeight();
            }

            return _height;
        }
        set { _height = value; }
    }

    public override void Draw(bool pulse)
    {
        if (!Visible) return;

        if (Image != null)
        {
            for (int i = 0; i < Image.Length; i++)
            {
                Graphics.DrawTextureEx(TextureCache.GetImage(Image[i]),
                    //new System.Numerics.Vector2((int)Bounds.X + Coords[i, 0] * Scale, (int)Bounds.Y + Coords[i, 1] * Scale),
                    new System.Numerics.Vector2((int)Bounds.X + Coords[i, 0], (int)Bounds.Y + Coords[i, 1]),
                    0f, Scale, Color.White);
            }
        }

        // Draw control's bounds
        //Graphics.DrawRectangleLinesEx(Bounds, 1f, Color.Magenta);

        base.Draw(pulse);
    }
}