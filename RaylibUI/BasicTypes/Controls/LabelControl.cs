using System.Numerics;
using Raylib_cs;

namespace RaylibUI.BasicTypes.Controls;

public class LabelControl : BaseControl
{
    public int Offset { get; protected set; }
    private readonly string _text;
    private readonly TextAlignment _alignment;
    private readonly Vector2 _textSize;

    public LabelControl(IControlLayout controller, string text, int offset = 0, TextAlignment alignment = TextAlignment.Left) : base(controller)
    {
        Offset = offset;
        _text = text;
        _alignment = alignment;
        _textSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), text, 20, 1.0f);
    }

    public override Size GetPreferredSize(int width, int height)
    {
        return new Size((int)_textSize.X + Offset, 38);
    }

    public override void Draw(bool pulse)
    {
        var textPosition = new Vector2(Location.X + Offset + (_alignment == TextAlignment.Center ? Width / 2f - _textSize.X / 2f : 0),
            Location.Y + Height / 2f - _textSize.Y / 2f);
        Raylib.DrawTextEx(Raylib.GetFontDefault(),_text ,textPosition, 20, 1.0f, Color.BLACK);
        base.Draw(pulse);
    }
}

public enum TextAlignment
{
    Left,
    Center
}