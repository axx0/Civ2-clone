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
    private Vector2 _textSize;
    private readonly Font _font;
    private int _fontSize;
    private readonly IImageSource? _backgroundImage;
    private readonly Color _textColour;
    private readonly IUserInterface? _active;
    private bool _hovered;
    public override bool CanFocus => true;

    public Button(IControlLayout controller, string text, Font? font = null, int? fontSize = null, IImageSource? backgroundImage = null, float imageScale = 1.0f) : base(controller)
    {
        _active = controller.MainWindow.ActiveInterface;
        _font = _active == null ? font ?? Fonts.Tnr : font ?? _active.Look.ButtonFont;        
        _fontSize = fontSize ?? _active?.Look.ButtonFontSize ?? 20;
        Text = text;
        _textColour = _active?.Look.ButtonColour ?? Color.Black;
        _backgroundImage = backgroundImage;
        Scale = imageScale;
    }

    private string _text = string.Empty;
    public string Text
    {
        get => _text;
        set
        {
            _text = value;
            _textSize = TextManager.MeasureTextEx(_font, _text, _fontSize, 0f);
        }
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
        set
        {
            if (_width == value)
            {
                return;
            }

            _width = value;
            InvalidateTexture();
        }
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
        set
        {
            if (_height == value)
            {
                return;
            }

            _height = value;
            InvalidateTexture();
        }
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
                DrawPressedOverlay();
            }
        }
        else
        {
            Graphics.DrawTextureEx(TextureCache.GetImage(_backgroundImage), 
                new Vector2(Bounds.X, Bounds.Y), 0.0f, Scale, Color.White);
        }

        var drawFontSize = GetDrawFontSize();
        var drawTextSize = drawFontSize == _fontSize ? _textSize : TextManager.MeasureTextEx(_font, Text, drawFontSize, 0f);
        var textOffset = _hovered && _backgroundImage == null ? Vector2.One : Vector2.Zero;
        Graphics.DrawTextEx(_font, Text, new Vector2(Bounds.X + Width / 2 - drawTextSize.X / 2, Bounds.Y + Height / 2 - drawTextSize.Y / 2) + textOffset, drawFontSize, 0f, Enabled ? _textColour : Color.Gray);

        base.Draw(pulse);
    }

    private void DrawPressedOverlay()
    {
        var x = (int)Bounds.X;
        var y = (int)Bounds.Y;
        var w = (int)Bounds.Width;
        var h = (int)Bounds.Height;
        if (w <= 2 || h <= 2)
        {
            return;
        }

        Graphics.DrawRectangle(x + 2, y + 2, w - 4, h - 4, new Color(176, 176, 176, 90));
        Graphics.DrawLine(x + 1, y + 1, x + w - 2, y + 1, new Color(96, 96, 96, 255));
        Graphics.DrawLine(x + 1, y + 1, x + 1, y + h - 2, new Color(96, 96, 96, 255));
        Graphics.DrawLine(x + 2, y + 2, x + w - 3, y + 2, new Color(128, 128, 128, 255));
        Graphics.DrawLine(x + 2, y + 2, x + 2, y + h - 3, new Color(128, 128, 128, 255));
        Graphics.DrawLine(x + 1, y + h - 2, x + w - 2, y + h - 2, Color.White);
        Graphics.DrawLine(x + w - 2, y + 1, x + w - 2, y + h - 2, Color.White);
    }

    public int FontSize
    {
        get { return _fontSize; }
        set
        { 
            _fontSize = value;
            _textSize = TextManager.MeasureTextEx(_font, _text, _fontSize, 0f);
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

    private void InvalidateTexture()
    {
        if (_texture.Width > 0)
        {
            _texture.Unload();
            _texture = default;
        }
    }

    public bool Enabled { get; set; } = true;

    public float Scale { get; set; }

    private int GetDrawFontSize()
    {
        if (_backgroundImage != null)
        {
            return _fontSize;
        }

        var availableWidth = Math.Max(1, Width - 14);
        var availableHeight = Math.Max(1, Height - 8);
        var fontSize = _fontSize;
        var textSize = _textSize;

        while (fontSize > 8 && (textSize.X > availableWidth || textSize.Y > availableHeight))
        {
            fontSize--;
            textSize = TextManager.MeasureTextEx(_font, Text, fontSize, 0f);
        }

        return fontSize;
    }

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
        return _backgroundImage == null ? 36 : Images.GetImageHeight(_backgroundImage, _active!, Scale);
    }

    public override int GetPreferredWidth()
    {
        return _backgroundImage == null ? Math.Max((int)_textSize.X + 10, 160) :
            Math.Max((int)_textSize.X, Images.GetImageWidth(_backgroundImage, _active!, Scale));
    }
}
