using System.Numerics;
using System.Xml;
using Raylib_CSharp;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Transformations;
using Raylib_CSharp.Colors;
using RaylibUI.BasicTypes;
using Model;
using Model.Interface;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Fonts;

namespace RaylibUI.Controls;

public class TextBox : BaseControl
{
    public event EventHandler<EventArgs> TextChanged; 
    public override bool CanFocus => true;
    
    private int _editPosition = 0;

    private string _text;
    private readonly IControlLayout _controller;
    private readonly IUserInterface? _active;
    private readonly int _minWidth;
    private readonly Action<string>? _acceptAction;
    private bool _editMode = false;
    private int _editWidth;

    private readonly Vector2 _textOffsetV = new Vector2(8, 0);
    private string _focusText;

    private const int TextMargin = 7;

    public string Text => _text;

    public TextBox(IControlLayout controller, string initialValue, int minWidth, Action<string>? acceptAction = null) : base(controller)
    {
        _controller = controller;
        _minWidth = minWidth;
        _acceptAction = acceptAction;
        _active = controller.MainWindow.ActiveInterface;
        SetText(initialValue);
        Click += OnClick;
    }

    public void SetText(string initialValue)
    {
        if(_text == initialValue) return;
        _text = initialValue;
        _editPosition = _text.Length;
        var fontSize = TextRendering.LegibleUiFontSize(Styles.BaseFontSize);
        var size = TextRendering.Measure(_active?.Look.DefaultFont ?? Fonts.Tnr, _text, fontSize, 1.0f);
        _editWidth = (int)size.X;
        Height = Math.Max(34, (int)(size.Y + TextMargin * 2));
        TextChanged?.Invoke(this, EventArgs.Empty);
    }

    public override void Draw(bool pulse)
    {
        Graphics.DrawRectangleRec(Bounds, Color.White);
        Graphics.DrawRectangleLinesEx(Bounds, 1.5f, Color.Black);
        var font = _active?.Look.DefaultFont ?? Fonts.Tnr;
        var fontSize = TextRendering.LegibleUiFontSize(Styles.BaseFontSize);
        var textSize = TextRendering.Measure(font, _text, fontSize, 1.0f);
        var textPosition = new Vector2(Bounds.X + _textOffsetV.X, Bounds.Y + MathF.Max(2, (Height - textSize.Y) / 2f));
        TextRendering.DrawReadable(font, _text, textPosition, fontSize, 1.0f, TextRendering.StrongBlack);
        
        if (_editMode && pulse)
        {
            Graphics.DrawRectangleRec(new Rectangle(Bounds.X + _textOffsetV.X + _editWidth + 2, Bounds.Y + MathF.Max(4, (Height - fontSize) / 2f), 1.5f, fontSize), Color.Black);
        }

        base.Draw(pulse);
    }

    public override int Width => GetPreferredWidth();

    public void OnClick(object? sender, MouseEventArgs mouseEventArgs)
    {
        if (_controller.Focused != this)
        {
            _controller.Focused = this;
        }
    }

    public override void OnFocus()
    {
        _focusText = _text;
        _editMode = true;
    }

    public override void OnBlur()
    {
        _editMode = false;
        if (_text != _focusText)
        {
            TextChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public override int GetPreferredHeight()
    {
        return Height > 0 ? Height : -1;
    }

    public override int GetPreferredWidth()
    {
        return _minWidth;
    }

    /**
     * Note that TextBox implements BOTH onCharPressed and onKeyPressed.
     * onCharPressed captures the user's inputted text,
     * while onKeyPressed handles editing/navigation within the textbox.
     */
    public override bool OnCharPressed(char charPressed)
    {
        if (charPressed <= char.MinValue)
        {
            return false;
        }
        _text = _text.Insert(_editPosition, charPressed.ToString());
        SetEditPosition(_editPosition + 1);
        return true;
    }

    public override bool OnKeyPressed(KeyboardKey key)
    {
        switch (key)
        {
            case KeyboardKey.Left:
            {
                if (_editPosition > 0)
                {
                    SetEditPosition(_editPosition - 1);
                }

                return true;
            }
            case KeyboardKey.Right:
                if (_editPosition < _text.Length)
                {
                    SetEditPosition(_editPosition + 1);
                }

                return true;
            case KeyboardKey.Backspace:
                if (_editPosition > 0)
                {
                    SetEditPosition(_editPosition - 1);
                    _text = _text.Remove(_editPosition, 1);
                }

                return true;
            case KeyboardKey.Delete:
                if (_editPosition < _text.Length)
                {
                    _text = _text.Remove(_editPosition, 1);
                }

                return true;
            case KeyboardKey.Tab:
                return base.OnKeyPressed(key);
            case KeyboardKey.Enter:
            case KeyboardKey.KpEnter:
                if (_acceptAction != null)
                {
                    _acceptAction(_text);
                    return true;
                }

                break;
        }

        return base.OnKeyPressed(key);
    }

    private void SetEditPosition(int newEditPosition)
    {
        _editPosition = newEditPosition;
        _editWidth = (int)TextRendering.Measure(_active?.Look.DefaultFont ?? Fonts.Tnr, _text.Substring(0, _editPosition), TextRendering.LegibleUiFontSize(Styles.BaseFontSize), 1).X;
    }
}
