using System.Numerics;
using Model.Images;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Fonts;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Transformations;
using RaylibUI.BasicTypes;
using RaylibUtils;

namespace RaylibUI;

public class OptionControl : BaseControl
{
    private readonly IImageSource[] _images;
    private readonly int _imageWidth, _imageHeight;
    private readonly OptionsPanel _parent;
    private readonly Font _font;
    private readonly Color _textColor;
    private readonly string _text;
    private const int MaxFontSize = 22;
    private const int MinFontSize = 13;
    private const int TextGap = 9;

    public override bool CanFocus => false;

    public OptionControl(IControlLayout controller, OptionsPanel parent, string text, int index, bool isChecked, IImageSource[] images) :
        base(controller, eventTransparent: false)
    {
        Index = index;
        Checked = isChecked;
        _parent = parent;
        _text = text;
        _images = images;
        _imageWidth = Images.GetImageWidth(images[0], controller.MainWindow.ActiveInterface);
        _imageHeight = Images.GetImageHeight(images[0], controller.MainWindow.ActiveInterface);
        _font = controller.MainWindow.ActiveInterface.Look.MenuFont;
        _textColor = Color.Black;
    }

    public int Index { get; }
    public bool Checked { get; set; }

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
        set => _width = value;
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
        set => _height = value;
    }

    public void Clear()
    {
        Checked = false;
    }

    public override int GetPreferredWidth()
    {
        return _imageWidth + TextGap + (int)TextManager.MeasureTextEx(_font, _text, MaxFontSize, 0f).X;
    }

    public override int GetPreferredHeight()
    {
        var textHeight = TextManager.MeasureTextEx(_font, _text, MaxFontSize, 0f).Y;
        return Math.Max(_imageHeight, (int)Math.Ceiling(textHeight)) + 6;
    }

    public override void Draw(bool pulse)
    {
        if (!Visible)
        {
            return;
        }

        var icon = TextureCache.GetImage(_images[Checked || _images.Length == 1 ? 0 : 1]);
        Graphics.DrawTexture(icon, (int)Bounds.X, (int)(Bounds.Y + (Height - _imageHeight) / 2f), Color.White);

        var fontSize = GetFittedFontSize();
        var textSize = TextManager.MeasureTextEx(_font, _text, fontSize, 0f);
        var textPosition = new Vector2(
            MathF.Round(Bounds.X + _imageWidth + TextGap),
            MathF.Round(Bounds.Y + (Height - textSize.Y) / 2f));

        Graphics.DrawTextEx(_font, _text, textPosition, fontSize, 0f, _textColor);

        if (_parent.SelectedId == Index)
        {
            Graphics.DrawRectangleLinesEx(
                new Rectangle(Bounds.X + _imageWidth + 2, Bounds.Y + 2, Bounds.Width - _imageWidth - 4, Bounds.Height - 4),
                0.5f,
                new Color(48, 48, 48, 210));
        }

        base.Draw(pulse);
    }

    private int GetFittedFontSize()
    {
        var availableTextWidth = Width - _imageWidth - TextGap - 6;
        if (availableTextWidth <= 0)
        {
            return MinFontSize;
        }

        var fontSize = MaxFontSize;
        while (fontSize > MinFontSize &&
               TextManager.MeasureTextEx(_font, _text, fontSize, 0f).X > availableTextWidth)
        {
            fontSize--;
        }

        return fontSize;
    }
}
