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
    public string Text { get; }
    
    public readonly TextAlignment Alignment;
    public readonly bool WrapText;

    private readonly int _minWidth;
    private readonly int _defaultHeight;
    private readonly float _spacing;
    private List<string>? _wrappedText;
    private readonly Font _labelFont;
    private readonly Color _colorFront;
    private readonly Color _colorShadow;
    private readonly Vector2 _shadowOffset;
    private readonly IUserInterface _active;
    private readonly Timer _timer;
    private bool _switch;
    private readonly Color[]? _switchColors;
    private readonly Color? _colorBack;

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
        _labelFont = font ?? controller.MainWindow.ActiveInterface?.Look.LabelFont ?? Fonts.Tnr;
        _colorFront = colorFront ?? Color.Black;
        _colorShadow = colorShadow ?? Color.Black;
        _shadowOffset = shadowOffset ?? Vector2.Zero;

        _active = controller.MainWindow.ActiveInterface;
        _timer = new Timer(_ => _switch = !_switch, null, 0, switchTime);
        _switchColors = switchColors;
        _colorBack = colorBack;
    }

    public Vector2 TextSize => TextManager.MeasureTextEx(_labelFont, Text, FontSize, _spacing);

    public int FontSize { get; set; }

    public Padding Padding { get; set; }


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
        
        _wrappedText = DialogUtils.GetWrappedTexts(_active, Text, Width, _labelFont, FontSize);
        return (int)(_wrappedText.Count * TextSize.Y + Padding.Top + Padding.Bottom) ;
    }

    public override void Draw(bool pulse)
    {
        if (_colorBack != null)
        {
            Graphics.DrawRectangle((int)Location.X, (int)Location.Y, Width, Height, _colorBack.Value);
        }
        if (WrapText && _wrappedText?.Count > 1)
        {
            var unitHeight = (Height - Padding.Top - Padding.Bottom) / _wrappedText.Count;
            var y = Location.Y + Padding.Top + unitHeight / 2f - TextSize.Y / 2f;
            for (var i = 0; i < _wrappedText.Count; i++)
            {
                var textPosition = new Vector2(Location.X + Padding.Left, y);
                Graphics.DrawTextEx(_labelFont, _wrappedText[i], textPosition + _shadowOffset, FontSize, _spacing, _colorShadow);
                Graphics.DrawTextEx(_labelFont, _wrappedText[i], textPosition, FontSize, _spacing, _colorFront);
                y += unitHeight;
            }
        }
        else
        {
            var textPosition = new Vector2(Location.X + Padding.Left, Location.Y + Padding.Top + (Height - Padding.Top - Padding.Bottom) / 2f - TextSize.Y / 2f);

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
                colorFront = _colorFront;
                colorShadow = _colorShadow;
            }
            Graphics.DrawTextEx(_labelFont, Text, textPosition + _shadowOffset, FontSize, _spacing, colorShadow);
            Graphics.DrawTextEx(_labelFont, Text, textPosition, FontSize, _spacing, colorFront);
        }

        //Graphics.DrawRectangleLines((int)Bounds.X, (int)Bounds.Y, (int)Bounds.Width, (int)Bounds.Height, Color.Red);

        base.Draw(pulse);
    }
}