using System.Numerics;
using Raylib_cs;
using RaylibUI.BasicTypes;

namespace RaylibUI.Controls;

public class Button : BaseControl
{
    private readonly string _text;
    private readonly Action _onClick;
    private readonly Vector2 textSize;
    private bool _hovered;
    public override bool CanFocus => true;

    public Button(IControlLayout controller, string text, Action onClick) : base(controller)
    {
        _text = text;
        _onClick = onClick;
        textSize = Raylib.MeasureTextEx(Fonts.DefaultFont, text, Styles.BaseFontSize, 1f);
        Height = (int)(textSize.Y + 10f);
    }

    public override void Draw(bool pulse)
    {
        var x = (int)Location.X;
        var y = (int)Location.Y;
        var w = Width;
        var h = Height;
        Raylib.DrawRectangleLinesEx(new Rectangle(x, y, w, h), 1.0f, new Color(100, 100, 100, 255));
        Raylib.DrawRectangleRec(new Rectangle(x + 1, y + 1, w - 2, h - 2), Color.WHITE);
        Raylib.DrawRectangleRec(new Rectangle(x + 3, y + 3, w - 6, h - 6), new Color(192, 192, 192, 255));
        Raylib.DrawLine(x + 2, y + h - 2, x + w - 2, y + h - 2, new Color(128, 128, 128, 255));
        Raylib.DrawLine(x + 3, y + h - 3, x + w - 2, y + h - 3, new Color(128, 128, 128, 255));
        Raylib.DrawLine(x + w - 1, y + 2, x + w - 1, y + h - 1, new Color(128, 128, 128, 255));
        Raylib.DrawLine(x + w - 2, y + 3, x + w - 2, y + h - 1, new Color(128, 128, 128, 255));
        
        Raylib.DrawText(_text, x + w / 2 - (int)textSize.X / 2, y + h / 2 - (int)textSize.Y / 2, 18, Color.BLACK);
        if (_hovered)
        {
            Raylib.DrawRectangleLinesEx(new Rectangle(x, y, w, h), 0.5f, Color.MAGENTA);
        }
        base.Draw(pulse);
    }

    public override void OnMouseEnter()
    {
        _hovered = true;
        base.OnMouseEnter();
    }

    public override void OnMouseLeave()
    {
        _hovered = false;
        base.OnMouseLeave();
    }


    public override Size GetPreferredSize(int width, int height)
    {
        return new Size(Math.Max((int)textSize.X + 10,160), 35);
    }

    public override void OnClick()
    {
        _onClick();
    }
}