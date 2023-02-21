using Raylib_cs;
using System.Numerics;

namespace RaylibUI;

public class Textbox
{
    private readonly Rectangle _bounds;
    private readonly int _charLimit;

    public Textbox(Rectangle bounds, int charLimit = 25)
    {
        _bounds = bounds;
        _charLimit = charLimit;
    }

    public bool Draw(ref string text, bool editMode)
    {
        bool pressed = false;

        Vector2 mousePoint = Raylib.GetMousePosition();
        int fontSize = 20;
        int textWidth = Raylib.MeasureText(text, fontSize);

        Rectangle cursor = new Rectangle(_bounds.x + 5 + textWidth + 2, _bounds.y + 5, 4, 20);

        if (editMode)
        {
            int keyCount = text.Length;
            int pressedKey = Raylib.GetCharPressed();
            if (pressedKey >= 32 && keyCount < _charLimit)
            {
                text = text.Insert(keyCount, ((char)pressedKey).ToString());
            }

            if (keyCount > 0 && Raylib.IsKeyPressed(KeyboardKey.KEY_BACKSPACE))
            {
                text = text.Remove(keyCount - 1);
            }
        }

        // DRAW
        Raylib.DrawRectangleRec(_bounds, Color.WHITE);
        Raylib.DrawRectangleLinesEx(_bounds, 1.0f, new Color(100, 100, 100, 255));

        Raylib.DrawText(text, (int)_bounds.x + 5, (int)_bounds.y + 5, fontSize, Color.BLACK);

        // Cursor
        if (editMode)
        {
            Raylib.DrawRectangleRec(cursor, Color.BLACK);
        }

        if (editMode)
        {
            if (!Raylib.CheckCollisionPointRec(mousePoint, _bounds) && Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT)) pressed = true;
        }
        else
        {
            if (Raylib.CheckCollisionPointRec(mousePoint, _bounds) && Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT)) pressed = true;
        }

        return pressed;
    }
}
