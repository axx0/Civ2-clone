using System.Numerics;
using Model;
using Model.Interface;
using Raylib_CSharp.Fonts;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;
using Model.Controls;

namespace RaylibUI.BasicTypes.Controls;

public class LabelControl : BaseControl
{
    private readonly int _minWidth;
    private readonly float _spacing;

    public LabelControl(IControlLayout controller,
        string text,
        bool eventTransparent,
        int minWidth = -1,
        Padding padding = default,
        HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left,
        VerticalAlignment verticalAlignment = VerticalAlignment.Top,
        Font? font = null,
        int fontSize = 20,
        float spacing = 0f,
        Color? colorFront = null,
        Color? colorShadow = null,
        Vector2? shadowOffset = null,
        Color? colorBack = null) : 
        base(controller, eventTransparent: eventTransparent)
    {
        Padding = padding;
        _text = text;
        HorizontalAlignment = horizontalAlignment;
        VerticalAlignment = verticalAlignment;
        _minWidth = minWidth;
        _fontSize = fontSize;
        _spacing = spacing;
        _font = font ?? controller.MainWindow.ActiveInterface?.Look.LabelFont ?? Fonts.Tnr;
        ColorFront = colorFront ?? Color.Black;
        ColorShadow = colorShadow ?? Color.Black;
        ShadowOffset = shadowOffset ?? Vector2.Zero;

        BackgroundColor = colorBack;
        _textSize = TextManager.MeasureTextEx(_font, _text, _fontSize, _spacing);
    }

    private Vector2 _textSize;
    public Vector2 TextSize => _textSize;

    private string _text;
    public string Text
    {
        get => _text;
        set
        {
            _text = value;
            _textSize = TextManager.MeasureTextEx(_font, _text, _fontSize, _spacing);
        }
    }

    private Font _font;
    public Font Font 
    {
        get => _font;
        set 
        {
            _font = value;
            _textSize = TextManager.MeasureTextEx(_font, _text, _fontSize, _spacing);
        }
    }

    private int _fontSize;
    public int FontSize
    {
        get => _fontSize;
        set
        {
            _fontSize = value;
            _textSize = TextManager.MeasureTextEx(_font, _text, _fontSize, _spacing);
        }
    }

    public HorizontalAlignment HorizontalAlignment { get; set; }
    public VerticalAlignment VerticalAlignment { get; set; }

    public Color? BackgroundColor { get; set; }
    public Color ColorFront { get; set; }
    public Color ColorShadow { get; set; }
    public Vector2 ShadowOffset { get; set; }

    public Padding Padding { get; set; }

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


    public override int GetPreferredWidth()
    {
        return Math.Max(_minWidth, (int)_textSize.X + Padding.Left + Padding.Right + (HorizontalAlignment == HorizontalAlignment.Center ? 10 : 0));
    }

    public override int GetPreferredHeight() => (int)_textSize.Y + Padding.Top + Padding.Bottom;

    public override void Draw(bool pulse)
    {
        if (!Visible) return;

        if (BackgroundColor != null)
        {
            Graphics.DrawRectangleRec(Bounds, BackgroundColor.Value);
        }
        
        var textPosition = new Vector2(Bounds.X + Padding.Left, Bounds.Y + Padding.Top);

        if (HorizontalAlignment == HorizontalAlignment.Center)
        {
            textPosition.X += (Width - Padding.Left - Padding.Right) / 2f - _textSize.X / 2f;
        }
        else if (HorizontalAlignment == HorizontalAlignment.Right)
        {
            textPosition.X += Width - Padding.Left - Padding.Right - _textSize.X;
        }

        if (VerticalAlignment == VerticalAlignment.Center)
        {
            textPosition.Y += (Height - Padding.Top - Padding.Bottom) / 2f - _textSize.Y / 2f;
        }
        else if (VerticalAlignment == VerticalAlignment.Bottom)
        {
            textPosition.Y += Height - Padding.Top - Padding.Bottom - _textSize.Y;
        }

        Color colorFront, colorShadow;
        colorFront = ColorFront;
        colorShadow = ColorShadow;
        Graphics.DrawTextEx(_font, _text, textPosition + ShadowOffset, _fontSize, _spacing, colorShadow);
        Graphics.DrawTextEx(_font, _text, textPosition, _fontSize, _spacing, colorFront);

        // Draw control's bounds
        //Graphics.DrawRectangleLinesEx(Bounds, 1f, Color.Blue);

        base.Draw(pulse);
    }
}