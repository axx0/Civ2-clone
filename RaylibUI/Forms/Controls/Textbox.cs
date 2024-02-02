using Raylib_cs;
using System.Numerics;

namespace RaylibUI.Forms;

public class Textbox : Control
{
    public string Name { get; set; }
    public int Width { get; set; } = 50;
    public int Height { get; set; } = 30;
    public int CharLimit { get; set; } = 25;
    public int FontSize { get; set; } = 18;
    public bool EditMode { get; set; } = false;
    public string Value { get; set; }
    public int? MinValue { get; set; } = 0;

    public bool Draw(int X, int Y)
    {
        bool pressed = false;

        Vector2 mousePoint = Raylib.GetMousePosition();
        int textWidth = Raylib.MeasureText(Value, FontSize);

        var cursor = new Rectangle(X + 5 + textWidth + 2, Y + 5, 4, 20);
        var clickBounds = new Rectangle(X, Y, Width, Height);
        
        if (EditMode && Enabled)
        {
            int keyCount = Value.Length;
            int pressedKey = Raylib.GetCharPressed();
            if (pressedKey >= 32 && keyCount < CharLimit)
            {
                Value = Value.Insert(keyCount, ((char)pressedKey).ToString());
            }

            if (keyCount > 0 && Raylib.IsKeyPressed(KeyboardKey.KEY_BACKSPACE))
            {
                Value = Value.Remove(keyCount - 1);
            }
        }

        // DRAW
        Raylib.DrawRectangleRec(clickBounds, Color.WHITE);
        Raylib.DrawRectangleLinesEx(clickBounds, 1.0f, new Color(100, 100, 100, 255));
        Raylib.DrawText(Value, X + 5, Y + 5, FontSize, Color.BLACK);

        // Cursor
        if (EditMode && Enabled)
        {
            Raylib.DrawRectangleRec(cursor, Color.BLACK);
        }

        if (EditMode && Enabled)
        {
            if (!Raylib.CheckCollisionPointRec(mousePoint, clickBounds) && Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT)) pressed = true;
        }
        else
        {
            if (Raylib.CheckCollisionPointRec(mousePoint, clickBounds) && Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT)) pressed = true;
        }

        return pressed;
    }
}
