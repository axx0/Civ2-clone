using System.Numerics;
using Model;
using Model.Interface;
using Raylib_CSharp.Fonts;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Interact;
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
    public override bool CanFocus => true;

    // Focus rectangle: a thin dark line just inside the button's bevel
    private const int FocusInset = 4;
    private static readonly Color FocusColour = new(80, 80, 80, 255);

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

    private string _text;
    public string Text
    {
        get => _text;
        set
        {
            _text = value;
            _textSize = TextManager.MeasureTextEx(_font, _text, _fontSize, 0f);
            _preferredSize = null;
        }
    }

    // Size set by a parent/Arrange; null = follow the preferred size
    private int? _width;
    public override int Width
    {
        get => _width ?? GetPreferredWidth();
        set => _width = value;
    }

    private int? _height;
    public override int Height
    {
        get => _height ?? GetPreferredHeight();
        set => _height = value;
    }

    // Cached because image buttons have to look up their image size
    private Size? _preferredSize;
    private Size PreferredSize => _preferredSize ??= _backgroundImage == null
        ? new Size(Math.Max((int)_textSize.X + 10, 160), 36)
        : new Size(Math.Max((int)_textSize.X, Images.GetImageWidth(_backgroundImage, _active, Scale)),
            Images.GetImageHeight(_backgroundImage, _active, Scale));

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

            if (Controller.Focused == this && Width > 2 * FocusInset && Height > 2 * FocusInset)
            {
                Graphics.DrawRectangleLinesEx(new Rectangle(Bounds.X + FocusInset, Bounds.Y + FocusInset,
                    Width - 2 * FocusInset, Height - 2 * FocusInset), 1f, FocusColour);
            }
        }
        else
        {
            Graphics.DrawTextureEx(TextureCache.GetImage(_backgroundImage), 
                new Vector2(Bounds.X, Bounds.Y), 0.0f, Scale, Color.White);
        }

        Graphics.DrawTextEx(_font, Text, new Vector2(Bounds.X + Width / 2 - (int)_textSize.X / 2, Bounds.Y + Height / 2 - (int)_textSize.Y / 2), _fontSize, 0f, Enabled ? _textColour : Color.Gray);

        base.Draw(pulse);
    }

    public int FontSize
    {
        get { return _fontSize; }
        set
        {
            _fontSize = value;
            _textSize = TextManager.MeasureTextEx(_font, _text, _fontSize, 0f);
            _preferredSize = null;
        }
    }

    private Texture2D _texture;
    /// <summary>
    /// Button background, repainted whenever the button's size changes.
    /// </summary>
    public Texture2D Texture
    {
        get
        {
            if ((_texture.Width != Width || _texture.Height != Height) && Width > 0 && Height > 0)
            {
                if (_texture.Width != 0)
                {
                    _texture.Unload();
                }
                _texture = ImageUtils.PaintButtonBase(Width, Height);
            }
            return _texture;
        }
    }

    private float _scale;
    public float Scale
    {
        get => _scale;
        set
        {
            _scale = value;
            _preferredSize = null;
        }
    }

    /// <summary>
    /// Enter or Space on the focused button presses it, the same as a mouse click
    /// (so the dialog doesn't treat Enter as OK when e.g. Cancel is focused).
    /// </summary>
    public override bool OnKeyPressed(KeyboardKey key)
    {
        if (key is KeyboardKey.Enter or KeyboardKey.KpEnter or KeyboardKey.Space)
        {
            if (Enabled)
            {
                OnClick(new MouseEventArgs());
            }
            return true;
        }

        return base.OnKeyPressed(key);
    }

    public override int GetPreferredHeight() => PreferredSize.Height;

    public override int GetPreferredWidth() => PreferredSize.Width;

    protected override Size MeasureCore(Size available) => PreferredSize;
}