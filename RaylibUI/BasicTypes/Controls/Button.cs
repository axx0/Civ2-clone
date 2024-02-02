using System.Numerics;
using Model;
using Model.Interface;
using Raylib_cs;
using RaylibUI.BasicTypes;

namespace RaylibUI.Controls;

public class Button : BaseControl
{
    private readonly string _text;
    private readonly Vector2 textSize;
    private readonly Font _font;
    private readonly int _fontSize;
    private readonly Color _colour;
    private readonly IUserInterface _active;
    private bool _hovered;
    public override bool CanFocus => true;
    public string Text => _text;

    public Button(IControlLayout controller, string text, Font? font = null, int? fontSize = null) : base(controller)
    {
        _active = controller.MainWindow.ActiveInterface;
        _text = text;
        _font = font ?? _active.Look.ButtonFont;
        _fontSize = fontSize ?? _active.Look.ButtonFontSize;
        _colour = _active.Look.ButtonColour;
        textSize = Raylib.MeasureTextEx(_font, text, _fontSize, 1f);
        Height = (int)(textSize.Y + 10f);
    }

    public override void Draw(bool pulse)
    {
        var X = (int)Location.X;
        var Y = (int)Location.Y;
        var w = Width;
        var h = Height;
        Raylib.DrawRectangleLinesEx(new Rectangle(X, Y, w, h), 1.0f, new Color(100, 100, 100, 255));
        Raylib.DrawRectangleRec(new Rectangle(X + 1, Y + 1, w - 2, h - 2), Color.White);
        Raylib.DrawRectangleRec(new Rectangle(X + 3, Y + 3, w - 6, h - 6), new Color(192, 192, 192, 255));
        Raylib.DrawLine(X + 2, Y + h - 2, X + w - 2, Y + h - 2, new Color(128, 128, 128, 255));
        Raylib.DrawLine(X + 3, Y + h - 3, X + w - 2, Y + h - 3, new Color(128, 128, 128, 255));
        Raylib.DrawLine(X + w - 1, Y + 2, X + w - 1, Y + h - 1, new Color(128, 128, 128, 255));
        Raylib.DrawLine(X + w - 2, Y + 3, X + w - 2, Y + h - 1, new Color(128, 128, 128, 255));

        Raylib.DrawTextEx(_font, Text, new Vector2(X + w / 2 - (int)textSize.X / 2, Y + h / 2 - (int)textSize.Y / 2), _fontSize, 1f, _colour);

        if (_hovered)
        {
            Raylib.DrawRectangleLinesEx(new Rectangle(X, Y, w, h), 0.5f, Color.Magenta);
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

    public override int GetPreferredHeight()
    {
        return 35;
    }

    public override int GetPreferredWidth()
    {
        return Math.Max((int)textSize.X + 10, 160);
    }
}