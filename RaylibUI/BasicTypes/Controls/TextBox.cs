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

    private readonly Vector2 _textOffsetV = new Vector2(5,5);
    private string _focusText;

    private const int TextMargin = 5;

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
        var size = TextManager.MeasureTextEx(_active?.Look.DefaultFont ?? Fonts.Tnr, _text, Styles.BaseFontSize, 1.0f);
        _editWidth = (int)size.X;
        Height = (int)(size.Y + TextMargin * 2);
        TextChanged?.Invoke(this, EventArgs.Empty);
    }

    public override void Draw(bool pulse)
    {
        Graphics.DrawRectangle((int)Location.X, (int)Location.Y+1, Width, Height -3, Color.White);
        Graphics.DrawRectangleLines((int)Location.X, (int)Location.Y+1, Width, Height -3, Color.Black);
        Graphics.DrawTextEx(_active?.Look.DefaultFont ?? Fonts.Tnr, _text, Location + _textOffsetV, Styles.BaseFontSize,1.0f, Color.Black);
        
        if (_editMode)
        {
            if (pulse)
            {
                Graphics.DrawRectangleRec(new Rectangle(Location.X + 5 + _editWidth + 1, Location.Y + 5, 1, 20), Color.Black);
            }
        }

        base.Draw(pulse);
    }

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
        return -1;
    }

    public override int GetPreferredWidth()
    {
        return _minWidth;
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
            default:
                var charPressed = Input.GetCharPressed();
                if (charPressed > 0)
                {
                    _text = _text.Insert(_editPosition, Convert.ToChar(charPressed).ToString());
                    SetEditPosition(_editPosition + 1);
                    return true;
                }

                break;
        }

        return base.OnKeyPressed(key);
    }

    private void SetEditPosition(int newEditPosition)
    {
        _editPosition = newEditPosition;
        _editWidth = (int)TextManager.MeasureTextEx(_active?.Look.DefaultFont ?? Fonts.Tnr, _text.Substring(0,_editPosition), Styles.BaseFontSize, 1).X;
    }
}