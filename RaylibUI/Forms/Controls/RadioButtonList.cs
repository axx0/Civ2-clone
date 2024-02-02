using Raylib_cs;

namespace RaylibUI.Forms;

public class RadioButtonList : Control
{
    public int Selected { get; set; } = 0;
    public int X { get; set; } = 0;
    public int Y { get; set; } = 0;

    private readonly List<RadioButton> _buttons;
    private readonly IList<string> _texts;
    private readonly int _columns;

    public RadioButtonList(IList<string> texts, int columns)
    {
        _buttons = new();
        _texts = texts;
        _columns = columns;

        for (int i = 0; i < _texts.Count; i++)
        {
            _buttons.Add(new RadioButton { Text = _texts[i] });
        }
        _buttons[Selected].IsSelected = true;
    }

    public void Draw(int X, int Y, int Width)
    {
        // Mouse click
        for (int i = 0; i < _buttons.Count; i++)
        {
            if (_buttons[i].IsPressed)
            {
                Selected = i;
            }
        }

        // Keys
        if (Enabled && KeyPressed != 0)
        {
            switch (KeyPressed)
            {
                case (int)KeyboardKey.KEY_DOWN or (int)KeyboardKey.KEY_RIGHT:
                    Selected = Selected + 1 == _buttons.Count ? 0 : Selected + 1;
                    break;
                case (int)KeyboardKey.KEY_UP or (int)KeyboardKey.KEY_LEFT:
                    Selected = Selected - 1 < 0 ? _buttons.Count - 1 : Selected - 1;
                    break;
            }
            KeyPressed = 0;
        }

        _buttons.ForEach(b => b.IsSelected = false);
        _buttons[Selected].IsSelected = true;

        // Draw buttons
        int rowsPerColumn = _texts.Count / _columns;
        int colWidth = Width / _columns;
        for (int col = 0; col < _columns; col++)
        {
            for (int row = 0; row < rowsPerColumn; row++)
            {
                int i = row + col * rowsPerColumn;
                _buttons[i].Draw(X + colWidth * col, Y + 32 * row, colWidth);
            }
        }
    }
}
