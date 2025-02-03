using System.Numerics;
using Model;
using Model.Interface;
using Raylib_CSharp.Fonts;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Textures;
using Raylib_CSharp.Transformations;
using RaylibUI.BasicTypes;
using Model.Images;
using JetBrains.Annotations;
using RaylibUtils;

namespace RaylibUI.Controls;

public class Button : BaseControl
{
    private readonly string _text;
    private readonly Vector2 _textSize;
    private readonly Font _font;
    private readonly int _fontSize;
    private readonly IImageSource? _backgroundImage;
    private readonly float _imageScale;
    private readonly Color _textColour;
    private readonly IUserInterface? _active;
    private bool _hovered;
    public override bool CanFocus => true;
    public string Text => _text;

    public Button(IControlLayout controller, string text, Font? font = null, int? fontSize = null, IImageSource? backgroundImage = null, float imageScale = 1.0f) : base(controller)
    {
        _active = controller.MainWindow.ActiveInterface;
        _text = text;
        
        _font = _active == null ? font ?? Fonts.Tnr : font ?? _active.Look.ButtonFont;        
        _fontSize = fontSize ?? _active?.Look.ButtonFontSize ?? 20;
        _textColour = _active?.Look.ButtonColour ?? Color.Black;
        _textSize = TextManager.MeasureTextEx(_font, text, _fontSize, 1f);
        Height = (int)(_textSize.Y + 10f);
        _backgroundImage = backgroundImage;
        _imageScale = imageScale;
    }

    public override void Draw(bool pulse)
    {
        var x = (int)Location.X;
        var y = (int)Location.Y;
        var w = Width;
        var h = Height;

        if (_backgroundImage == null)
        {
            if (_active != null)
            {
                _active.DrawButton(Texture, x, y, w, h);
            }
            else
            {
                Graphics.DrawTexture(Texture, x, y, Color.White);
            }

            if (_hovered)
            {
                Graphics.DrawRectangleLinesEx(new Rectangle(x, y, w, h), 0.5f, Color.Magenta);
            }
        }
        else
        {
            Graphics.DrawTextureEx(TextureCache.GetImage(_backgroundImage), new Vector2(x, y), 0.0f, _imageScale, Color.White);
        }

        Graphics.DrawTextEx(_font, Text, new Vector2(x + w / 2 - (int)_textSize.X / 2, y + h / 2 - (int)_textSize.Y / 2), _fontSize, 1f, Enabled ? _textColour : Color.Gray);

        base.Draw(pulse);
    }

    private Texture2D _texture;
    public Texture2D Texture
    {
        get
        {
            if (_texture.Width == 0)
            {
                _texture = ImageUtils.PaintButtonBase(Width, Height);
            }
            return _texture;
        }
    }

    public bool Enabled { get; set; } = true;

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
        return _backgroundImage == null ? 35 : 
            Images.GetImageHeight(_backgroundImage, _active, _imageScale);
    }

    public override int GetPreferredWidth()
    {
        return _backgroundImage == null ? Math.Max((int)_textSize.X + 10, 160) :
            Math.Max((int)_textSize.X, Images.GetImageWidth(_backgroundImage, _active, _imageScale));
    }
}