using System.Numerics;
using Raylib_cs;
using RaylibUI.Forms;

namespace RaylibUI.BasicTypes.Controls;

public class LabelControl : BaseControl
{
    public int Offset { get; }
    protected readonly string _text;
    private readonly int _minWidth;
    private readonly TextAlignment _alignment;
    private readonly Vector2 _textSize;
    private readonly int _defaultHeight;
    private readonly bool _wrapText;
    private List<string>? _wrappedText;

    public LabelControl(IControlLayout controller, string text, int minWidth = -1, int offset = 2,
        TextAlignment alignment = TextAlignment.Left, int defaultHeight = 32, bool wrapText = false) : base(controller)
    {
        Offset = offset;
        _text = text;
        _minWidth = minWidth;
        _defaultHeight = defaultHeight;
        _wrapText = wrapText;
        _alignment = alignment;
        _textSize = Raylib.MeasureTextEx(Fonts.DefaultFont, text, 20, 1.0f);
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
        
        _wrappedText = CtrlHelpers.GetWrappedTexts(_text, Width, Fonts.FontSize);
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
                Raylib.DrawTextEx(Fonts.DefaultFont, _wrappedText[i], textPosition, 20, 1.0f, Color.BLACK);
                y += unitHeight;
            }
        }
        else
        {
            var textPosition = new Vector2(
                Location.X + Offset + (_alignment == TextAlignment.Center ? Width / 2f - _textSize.X / 2f : 0),
                Location.Y + Height / 2f - _textSize.Y / 2f);
            Raylib.DrawTextEx(Fonts.DefaultFont, _text, textPosition, 20, 1.0f, Color.BLACK);
        }

        base.Draw(pulse);
    }
}

public enum TextAlignment
{
    Left,
    Center
}