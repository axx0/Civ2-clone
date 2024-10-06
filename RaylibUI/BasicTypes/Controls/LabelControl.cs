using System.Numerics;
using Model;
using Model.Interface;
using Raylib_cs;
using RaylibUI.Forms;

namespace RaylibUI.BasicTypes.Controls;

public class LabelControl : BaseControl
{
    public string Text { get; }
    
    public readonly TextAlignment Alignment;
    public readonly bool WrapText;

    private readonly int _minWidth;
    private readonly int _defaultHeight;
    private int _fontSize, _offset;
    private readonly float _spacing;
    private List<string>? _wrappedText;
    private readonly Font _labelFont;
    private readonly Color _colorFront;
    private readonly Color _colorShadow;
    private readonly Vector2 _shadowOffset;
    private readonly IUserInterface _active;
    private readonly Timer _timer;
    private bool _switch;
    private readonly Color[]? _switchColors;

    public LabelControl(IControlLayout controller, 
        string text, 
        bool eventTransparent, 
        int minWidth = -1,
        int offset = 2, 
        TextAlignment alignment = TextAlignment.Left, 
        int defaultHeight = 32, 
        bool wrapText = false,
        Font? font = null, 
        int fontSize = 20, 
        float spacing = 1.0f, 
        Color? colorFront = null, 
        Color? colorShadow = null,
        Vector2? shadowOffset = null, 
        Color[]? switchColors = null, 
        int switchTime = 0) : base(controller,
        eventTransparent: eventTransparent)
    {
        Text = text;
        Alignment = alignment;
        WrapText = wrapText;
        _offset = offset;
        _minWidth = minWidth;
        _defaultHeight = defaultHeight;
        _fontSize = fontSize;
        _spacing = spacing;
        _labelFont = font ?? controller.MainWindow.ActiveInterface?.Look.LabelFont ?? Fonts.Tnr;
        _colorFront = colorFront ?? Color.Black;
        _colorShadow = colorShadow ?? Color.Black;
        _shadowOffset = shadowOffset ?? Vector2.Zero;

        _active = controller.MainWindow.ActiveInterface;
        _timer = new Timer(_ => _switch = !_switch, null, 0, switchTime);
        _switchColors = switchColors;
    }

    public Vector2 TextSize => Raylib.MeasureTextEx(_labelFont, Text, _fontSize, _spacing);

    public int FontSize
    {
        get { return _fontSize; }
        set { _fontSize = value; }
    }

    public int Offset 
    {
        get { return _offset; }
        set { _offset = value; }
    }


    public override int GetPreferredWidth()
    {
        if (WrapText)
        {
            return Width;
        }

        return Math.Max(_minWidth, (int)TextSize.X + Offset + (Alignment == TextAlignment.Center ? 10 : 0));
    }

    public override int GetPreferredHeight()
    {
        if (!WrapText) return _defaultHeight;
        
        _wrappedText = DialogUtils.GetWrappedTexts(_active, Text, Width, _labelFont, _fontSize);
        return (int)(_wrappedText.Count * TextSize.Y) ;
    }

    public override void Draw(bool pulse)
    {
        if (WrapText && _wrappedText?.Count > 1)
        {
            var unitHeight = Height / _wrappedText.Count;
            var y = Location.Y + unitHeight / 2f - TextSize.Y / 2f;
            for (var i = 0; i < _wrappedText.Count; i++)
            {
                var textPosition = new Vector2(Location.X + _offset, y);
                Raylib.DrawTextEx(_labelFont, _wrappedText[i], textPosition + _shadowOffset, _fontSize, _spacing, _colorShadow);
                Raylib.DrawTextEx(_labelFont, _wrappedText[i], textPosition, _fontSize, _spacing, _colorFront);
                y += unitHeight;
            }
        }
        else
        {
            var textPosition = new Vector2(Location.X + _offset, Location.Y + Height / 2f - TextSize.Y / 2f);

            if (Alignment == TextAlignment.Center)
            {
                textPosition.X += Width / 2f - TextSize.X / 2f;
            }
            else if (Alignment == TextAlignment.Right)
            {
                textPosition.X += Width - TextSize.X - 2 * _offset;
            }

            Color colorFront, colorShadow;
            if (_switchColors is not null)
            {
                colorFront = _switch ? _switchColors[0] : _switchColors[1];
                colorShadow = Color.Black;
            }
            else
            {
                colorFront = _colorFront;
                colorShadow = _colorShadow;
            }
            Raylib.DrawTextEx(_labelFont, Text, textPosition + _shadowOffset, _fontSize, _spacing, colorShadow);
            Raylib.DrawTextEx(_labelFont, Text, textPosition, _fontSize, _spacing, colorFront);
        }

        //Raylib.DrawRectangleLines((int)Bounds.X, (int)Bounds.Y, (int)Bounds.Width, (int)Bounds.Height, Color.Red);

        base.Draw(pulse);
    }
}