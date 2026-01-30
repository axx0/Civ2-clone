using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;
using Model.Images;
using RaylibUtils;
using Model.Controls;
using Model;

namespace RaylibUI.BasicTypes.Controls;

public class ImageBox : BaseControl
{
    private readonly IUserInterface _active;

    public ImageBox(IControlLayout controller, DialogImageElements image, bool eventTransparent = true) : base(controller, eventTransparent)
    {
        Image = image.Image;
        Scale = image.Scale;
        Coords = image.Coords;
        _active = controller.MainWindow.ActiveInterface;
    }

    public ImageBox(IControlLayout controller, IImageSource? image, float scale = 1.0f, bool eventTransparent = true) : base(controller, eventTransparent)
    {
        Image = [image];
        Scale = scale;
    }

    private IImageSource[] _image;
    public IImageSource?[] Image 
    {
        get => _image;
        set 
        {
            _image = value;
            Width = GetPreferredWidth();
            Height = GetPreferredHeight();
        }
    }

    public int[,] Coords { get; set; } = new int[,] { { 0, 0 } };

    private float _scale = 1.0f;
    public float Scale 
    {
        get => _scale;
        set
        {
            if (value != _scale)
            {
                _scale = value;
                Width = GetPreferredWidth();
                Height = GetPreferredHeight();
            }
        }
    }

    public override int GetPreferredWidth()
    {
        return Image.Select(img => Images.GetImageWidth(img, _active, _scale)).Max();
    }

    public override int GetPreferredHeight()
    {
        return Image.Select(img => Images.GetImageHeight(img, _active, _scale)).Max();
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

        if (_image != null)
        {
            for (int i = 0; i < _image.Length; i++)
            {
                Graphics.DrawTextureEx(TextureCache.GetImage(_image[i]),
                    //new System.Numerics.Vector2((int)Bounds.X + Coords[i, 0] * Scale, (int)Bounds.Y + Coords[i, 1] * Scale),
                    new System.Numerics.Vector2((int)Bounds.X + Coords[i, 0], (int)Bounds.Y + Coords[i, 1]),
                    0f, _scale, Color.White);
            }
        }

        // Draw control's bounds
        //Graphics.DrawRectangleLinesEx(Bounds, 1f, Color.Magenta);

        base.Draw(pulse);
    }
}