using System.Numerics;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Transformations;
using Raylib_CSharp.Colors;
using RaylibUI.BasicTypes;
using Model;
using Model.Interface;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Fonts;

namespace RaylibUI.Controls;

/// <summary>
/// One-line edit box. It asks for its min width and one line of text in height, and uses whatever
/// size it's given. Text longer than the box scrolls sideways so the caret stays in view.
/// </summary>
public class TextBox : BaseControl
{
    public event EventHandler<EventArgs>? TextChanged;
    public override bool CanFocus => true;

    private const int TextMargin = 5;
    private const float Spacing = 1f;

    private readonly IControlLayout _controller;
    private readonly int _minWidth;
    private readonly Action<string>? _acceptAction;
    private readonly Font _font;
    private readonly int _fontSize;
    private readonly int _lineHeight;

    private string _text = string.Empty;
    private int _editPosition;
    private int _scrollOffset;      // how far the text is scrolled left, in pixels
    private bool _editMode;

    public string Text => _text;

    public TextBox(IControlLayout controller, string initialValue, int minWidth, Action<string>? acceptAction = null) : base(controller)
    {
        _controller = controller;
        _minWidth = minWidth;
        _acceptAction = acceptAction;
        _font = controller.MainWindow.ActiveInterface?.Look.DefaultFont ?? Fonts.Tnr;
        _fontSize = Styles.BaseFontSize;
        // One line's height, the same for any text (an empty text measures 0 high)
        _lineHeight = (int)TextManager.MeasureTextEx(_font, "Ag", _fontSize, Spacing).Y;

        // A size until a parent lays it out
        Width = GetPreferredWidth();
        Height = GetPreferredHeight();

        SetText(initialValue);
    }

    public void SetText(string text)
    {
        if (_text == text) return;

        _text = text;
        SetEditPosition(_text.Length);
        TextChanged?.Invoke(this, EventArgs.Empty);
    }

    public override int GetPreferredWidth() => _minWidth;

    public override int GetPreferredHeight() => _lineHeight + 2 * TextMargin;

    private int TextWidth(string text) => (int)TextManager.MeasureTextEx(_font, text, _fontSize, Spacing).X;

    private int InnerWidth => Math.Max(1, Width - 2 * TextMargin);

    public override void Draw(bool pulse)
    {
        if (!Visible) return;

        Graphics.DrawRectangleRec(Bounds, Color.White);
        Graphics.DrawRectangleLinesEx(Bounds, 1f, Color.Black);

        // Text and caret are cut off at the box's inner edge
        var textX = Bounds.X + TextMargin - _scrollOffset;
        var textY = Bounds.Y + (Height - _lineHeight) / 2;
        Graphics.BeginScissorMode((int)Bounds.X + 1, (int)Bounds.Y + 1, Math.Max(0, Width - 2), Math.Max(0, Height - 2));
        Graphics.DrawTextEx(_font, _text, new Vector2(textX, textY), _fontSize, Spacing, Color.Black);
        if (_editMode && pulse)
        {
            var caretX = textX + TextWidth(_text[.._editPosition]) + 1;
            Graphics.DrawRectangleRec(new Rectangle(caretX, textY, 1, _lineHeight), Color.Black);
        }
        Graphics.EndScissorMode();

        base.Draw(pulse);
    }

    public override void OnResize()
    {
        base.OnResize();
        EnsureCaretVisible();
    }

    /// <summary>
    /// A press focuses the box and puts the caret at the mouse (as in Windows).
    /// </summary>
    public override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (e.Button != MouseButton.Left) return;

        if (_controller.Focused != this)
        {
            _controller.Focused = this;
        }
        SetEditPosition(PositionAt(e.Location.X));
    }

    /// <summary>
    /// The caret position nearest to this x (relative to the box).
    /// </summary>
    private int PositionAt(float x)
    {
        var target = x - TextMargin + _scrollOffset;
        var best = 0;
        var bestDistance = float.MaxValue;
        for (var i = 0; i <= _text.Length; i++)
        {
            var distance = Math.Abs(TextWidth(_text[..i]) - target);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                best = i;
            }
        }
        return best;
    }

    public override void OnFocus()
    {
        _editMode = true;
    }

    public override void OnBlur()
    {
        _editMode = false;
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
        TextChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public override bool OnKeyPressed(KeyboardKey key)
    {
        switch (key)
        {
            case KeyboardKey.Left:
                if (_editPosition > 0)
                {
                    SetEditPosition(_editPosition - 1);
                }
                return true;
            case KeyboardKey.Right:
                if (_editPosition < _text.Length)
                {
                    SetEditPosition(_editPosition + 1);
                }
                return true;
            case KeyboardKey.Home:
                SetEditPosition(0);
                return true;
            case KeyboardKey.End:
                SetEditPosition(_text.Length);
                return true;
            case KeyboardKey.Backspace:
                if (_editPosition > 0)
                {
                    _text = _text.Remove(_editPosition - 1, 1);
                    SetEditPosition(_editPosition - 1);
                    TextChanged?.Invoke(this, EventArgs.Empty);
                }
                return true;
            case KeyboardKey.Delete:
                if (_editPosition < _text.Length)
                {
                    _text = _text.Remove(_editPosition, 1);
                    SetEditPosition(_editPosition);
                    TextChanged?.Invoke(this, EventArgs.Empty);
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
        _editPosition = Math.Clamp(newEditPosition, 0, _text.Length);
        EnsureCaretVisible();
    }

    /// <summary>
    /// Scroll the text sideways so the caret is inside the box, and don't leave empty space
    /// on the right while there's hidden text on the left.
    /// </summary>
    private void EnsureCaretVisible()
    {
        var caretX = TextWidth(_text[.._editPosition]);
        if (caretX - _scrollOffset > InnerWidth)
        {
            _scrollOffset = caretX - InnerWidth;
        }
        else if (caretX < _scrollOffset)
        {
            _scrollOffset = caretX;
        }

        _scrollOffset = Math.Clamp(_scrollOffset, 0, Math.Max(0, TextWidth(_text) - InnerWidth));
    }
}
