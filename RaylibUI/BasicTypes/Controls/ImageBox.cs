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

    public ImageBox(IControlLayout controller, IImageSource? image) : base(controller, true)
    {
        Image = [image];
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

    public override void Draw(bool pulse)
    {
        if (Image != null)
        {
            for (int i = 0; i < Image.Length; i++)
            {
                //Graphics.DrawTextureEx(TextureCache.GetImage(_image[i]),
                //    new System.Numerics.Vector2((int)Location.X + Coords[i, 0] * Scale, (int)Location.Y + Coords[i, 1] * Scale),
                //    0f, Scale, Color.White);
                Graphics.DrawTextureEx(TextureCache.GetImage(Image[i]),
                    new System.Numerics.Vector2((int)Location.X + Coords[i, 0], (int)Location.Y + Coords[i, 1]),
                    0f, Scale, Color.White);
            }
        }

        base.Draw(pulse);
    }
}