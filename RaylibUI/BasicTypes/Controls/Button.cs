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
    private Vector2 _textSize;
    private readonly Font _font;
    private int _fontSize;
    private readonly IImageSource? _backgroundImage;
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
        _backgroundImage = backgroundImage;
        Scale = imageScale;
    }

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

    public override void Draw(bool pulse)
    {
        if (!Visible) return;

        if (_backgroundImage == null)
        {
            if (_active != null)
            {
                _active.DrawButton(Texture, Bounds);
            }
            else
            {
                Graphics.DrawTexture(Texture, (int)Bounds.X, (int)Bounds.Y, Color.White);
            }

            if (_hovered)
            {
                Graphics.DrawRectangleLinesEx(Bounds, 0.5f, Color.Magenta);
            }
        }
        else
        {
            Graphics.DrawTextureEx(TextureCache.GetImage(_backgroundImage), 
                new Vector2(Bounds.X, Bounds.Y), 0.0f, Scale, Color.White);
        }

        Graphics.DrawTextEx(_font, Text, new Vector2(Bounds.X + Width / 2 - (int)_textSize.X / 2, Bounds.Y + Height / 2 - (int)_textSize.Y / 2), _fontSize, 1f, Enabled ? _textColour : Color.Gray);

        base.Draw(pulse);
    }

    public int FontSize
    {
        get { return _fontSize; }
        set
        { 
            _fontSize = value;
            _textSize = TextManager.MeasureTextEx(_font, _text, _fontSize, 1f);
        }
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

    public float Scale { get; set; }

    public override void OnMouseEnter()
    {
        if (!Visible) return;

        _hovered = true;
        base.OnMouseEnter();
    }

    public override void OnMouseLeave()
    {
        if (!Visible) return;

        _hovered = false;
        base.OnMouseLeave();
    }

    public override int GetPreferredHeight()
    {
        return _backgroundImage == null ? 36 : Images.GetImageHeight(_backgroundImage, _active, Scale);
    }

    public override int GetPreferredWidth()
    {
        return _backgroundImage == null ? Math.Max((int)_textSize.X + 10, 160) :
            Math.Max((int)_textSize.X, Images.GetImageWidth(_backgroundImage, _active, Scale));
    }
}