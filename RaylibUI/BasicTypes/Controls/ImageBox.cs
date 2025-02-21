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
    private readonly IImageSource[] _image;
    private readonly float _scale;
    private readonly int[,] _coords;

    public ImageBox(IControlLayout controller, DialogImageElements image) : base(controller)
    {
        _image = image.Image;
        _scale = image.Scale;
        _coords = image.Coords;
        _active = controller.MainWindow.ActiveInterface;
    }

    public override int GetPreferredWidth()
    {
        return _image.Select(img => Images.GetImageWidth(img, _active, _scale)).Max();
    }

    public override int GetPreferredHeight()
    {
        return _image.Select(img => Images.GetImageHeight(img, _active, _scale)).Max();
    }

    public override void Draw(bool pulse)
    {
        for (int i = 0; i < _image.Length; i++)
        {
            Graphics.DrawTextureEx(TextureCache.GetImage(_image[i]), 
                new System.Numerics.Vector2((int)Location.X + _coords[i, 0] * _scale, (int)Location.Y + _coords[i, 1] * _scale), 
                0f, _scale, Color.White);
        }

        base.Draw(pulse);
    }
}