using System.Numerics;
using Model;
using Model.Interface;
using Raylib_CSharp;
using Raylib_CSharp.Fonts;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;

namespace RaylibUI.BasicTypes.Controls;

public class LabelControl : BaseControl
{
    public string Text { get; set; }
    
    public readonly TextAlignment Alignment;
    public readonly bool WrapText;

    private readonly int _minWidth;
    private readonly int _defaultHeight;
    private readonly float _spacing;
    private List<string>? _wrappedText;
    private readonly IUserInterface _active;
    private readonly Timer _timer;
    private bool _switch;
    private readonly Color[]? _switchColors;

    public LabelControl(IControlLayout controller,
        string text,
        bool eventTransparent,
        int minWidth = -1,
        Padding padding = default,
        TextAlignment alignment = TextAlignment.Left,
        int defaultHeight = 32,
        bool wrapText = false,
        Font? font = null,
        int fontSize = 20,
        float spacing = 1.0f,
        Color? colorFront = null,
        Color? colorShadow = null,
        Vector2? shadowOffset = null,
        Color[]? switchColors = null,
        Color? colorBack = null,
        int switchTime = 0) : base(controller,
        eventTransparent: eventTransparent)
    {
        Padding = padding;
        Text = text;
        Alignment = alignment;
        WrapText = wrapText;
        _minWidth = minWidth;
        _defaultHeight = defaultHeight;
        FontSize = fontSize;
        _spacing = spacing;
        Font = font ?? controller.MainWindow.ActiveInterface?.Look.LabelFont ?? Fonts.Tnr;
        ColorFront = colorFront ?? Color.Black;
        ColorShadow = colorShadow ?? Color.Black;
        ShadowOffset = shadowOffset ?? Vector2.Zero;

        _active = controller.MainWindow.ActiveInterface;
        _timer = new Timer(_ => _switch = !_switch, null, 0, switchTime);
        _switchColors = switchColors;
        BackgroundColor = colorBack;
    }

    public Vector2 TextSize => TextManager.MeasureTextEx(Font, Text, FontSize, _spacing);

    public Font Font { get; set; }
    public int FontSize { get; set; }
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
        if (WrapText)
        {
            return Width;
        }

        return Math.Max(_minWidth, (int)TextSize.X + Padding.Left + Padding.Right + (Alignment == TextAlignment.Center ? 10 : 0));
    }

    public override int GetPreferredHeight()
    {
        if (!WrapText) return _defaultHeight + Padding.Top + Padding.Bottom;
        
        _wrappedText = DialogUtils.GetWrappedTexts(_active, Text, Width, Font, FontSize);
        return (int)(_wrappedText.Count * TextSize.Y + Padding.Top + Padding.Bottom) ;
    }

    public override void Draw(bool pulse)
    {
        if (!Visible) return;

        if (BackgroundColor != null)
        {
            Graphics.DrawRectangleRec(Bounds, BackgroundColor.Value);
        }
        if (WrapText && _wrappedText?.Count > 1)
        {
            var unitHeight = (Height - Padding.Top - Padding.Bottom) / _wrappedText.Count;
            var y = Bounds.Y + Padding.Top + unitHeight / 2f - TextSize.Y / 2f;
            for (var i = 0; i < _wrappedText.Count; i++)
            {
                var textPosition = new Vector2(Bounds.X + Padding.Left, y);
                Graphics.DrawTextEx(Font, _wrappedText[i], textPosition + ShadowOffset, FontSize, _spacing, ColorShadow);
                Graphics.DrawTextEx(Font, _wrappedText[i], textPosition, FontSize, _spacing, ColorFront);
                y += unitHeight;
            }
        }
        else
        {
            var textPosition = new Vector2(Bounds.X + Padding.Left, Bounds.Y + Padding.Top + (Height - Padding.Top - Padding.Bottom) / 2f - TextSize.Y / 2f);

            if (Alignment == TextAlignment.Center)
            {
                textPosition.X += (Width - Padding.Left - Padding.Right) / 2f - TextSize.X / 2f;
            }
            else if (Alignment == TextAlignment.Right)
            {
                textPosition.X += Width - Padding.Left - Padding.Right - TextSize.X;
            }

            Color colorFront, colorShadow;
            if (_switchColors is not null)
            {
                colorFront = _switch ? _switchColors[0] : _switchColors[1];
                colorShadow = Color.Black;
            }
            else
            {
                colorFront = ColorFront;
                colorShadow = ColorShadow;
            }
            Graphics.DrawTextEx(Font, Text, textPosition + ShadowOffset, FontSize, _spacing, colorShadow);
            Graphics.DrawTextEx(Font, Text, textPosition, FontSize, _spacing, colorFront);
        }

        // Draw control's bounds
        //Graphics.DrawRectangleLinesEx(Bounds, 1f, Color.Magenta);

        base.Draw(pulse);
    }
}