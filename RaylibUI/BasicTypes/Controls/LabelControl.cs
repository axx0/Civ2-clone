using System.Numerics;
using Raylib_cs;
using RaylibUI.Forms;

namespace RaylibUI.BasicTypes.Controls;

public class LabelControl : BaseControl
{
    public int Offset { get; }
    protected readonly string Text;
    
    private readonly int _minWidth;
    private readonly TextAlignment _alignment;
    private readonly Vector2 _textSize;
    private readonly int _defaultHeight;
    private readonly bool _wrapText;
    private readonly int _fontSize;
    private readonly float _spacing;
    private List<string>? _wrappedText;
    private readonly Font _labelFont;

    public LabelControl(IControlLayout controller, string text, bool eventTransparent, int minWidth = -1, int offset = 2,
        TextAlignment alignment = TextAlignment.Left, int defaultHeight = 32, bool wrapText = false, Font? font = null, int fontSize = 20, float spacing = 1.0f) : base(controller, eventTransparent: eventTransparent)
    {
        Offset = offset;
        Text = text;
        _minWidth = minWidth;
        _defaultHeight = defaultHeight;
        _wrapText = wrapText;
        _fontSize = fontSize;
        _spacing = spacing;
        _alignment = alignment;
        _labelFont = font ?? Fonts.DefaultFont;
        _textSize = Raylib.MeasureTextEx(_labelFont, text, _fontSize, _spacing);
    }

    public override int GetPreferredWidth()
    {
        if (_wrapText)
        {
            return -1;
        }

        if (_minWidth != -1) return _minWidth;
        return (int)_textSize.X + Offset + (_alignment == TextAlignment.Center ? 10 : 0);
    }

    public override int GetPreferredHeight()
    {
        if (!_wrapText) return _defaultHeight;
        
        _wrappedText = CtrlHelpers.GetWrappedTexts(Text, Width, Fonts.FontSize);
        return (int)(_wrappedText.Count * _textSize.Y) ;
    }

    public override void Draw(bool pulse)
    {
        if (_wrapText && _wrappedText?.Count > 1)
        {
            var unitHeight = Height / _wrappedText.Count;
            var y = Location.Y + unitHeight / 2f - _textSize.Y / 2f;
            for (var i = 0; i < _wrappedText.Count; i++)
            {
                var textPosition = new Vector2(Location.X + Offset, y);
                Raylib.DrawTextEx(_labelFont, _wrappedText[i], textPosition, _fontSize, _spacing, Color.BLACK);
                y += unitHeight;
            }
        }
        else
        {
            var textPosition = new Vector2(
                Location.X + Offset + (_alignment == TextAlignment.Center ? Width / 2f - _textSize.X / 2f : 0),
                Location.Y + Height / 2f - _textSize.Y / 2f);
            Raylib.DrawTextEx(_labelFont, Text, textPosition, _fontSize, _spacing, Color.BLACK);
            Raylib.DrawTextEx(_labelFont, Text, new Vector2(textPosition.X - 1, textPosition.Y - 1), _fontSize, _spacing, new Color(135, 135, 135, 255));
        }

        base.Draw(pulse);
    }
}

public enum TextAlignment
{
    Left,
    Center
}