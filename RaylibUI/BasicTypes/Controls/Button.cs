using System.Numerics;
using Model;
using Model.Interface;
using Raylib_cs;
using RaylibUI.BasicTypes;

namespace RaylibUI.Controls;

public class Button : BaseControl
{
    private readonly string _text;
    private readonly Vector2 _textSize;
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
        _textSize = Raylib.MeasureTextEx(_font, text, _fontSize, 1f);
        Height = (int)(_textSize.Y + 10f);
    }

    public override void Draw(bool pulse)
    {
        var x = (int)Location.X;
        var y = (int)Location.Y;
        var w = Width;
        var h = Height;

        _active.DrawButton(Texture, x, y, w, h);

        Raylib.DrawTextEx(_font, Text, new Vector2(x + w / 2 - (int)_textSize.X / 2, y + h / 2 - (int)_textSize.Y / 2), _fontSize, 1f, _colour);

        if (_hovered)
        {
            Raylib.DrawRectangleLinesEx(new Rectangle(x, y, w, h), 0.5f, Color.Magenta);
        }
        base.Draw(pulse);
    }

    private Texture2D _texture;
    public Texture2D Texture
    {
        get
        {
            if (_texture.Width == 0 && ImageUtils.Wallpaper.Button is not null)
            {
                _texture = ImageUtils.PaintButtonBase(Width, Height);
            }
            return _texture;
        }
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
        return Math.Max((int)_textSize.X + 10, 160);
    }
}