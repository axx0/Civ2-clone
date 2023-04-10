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

    private string _text;
    public string Text 
    {
        get { return _text; }
        set { _text = value; }
    }
    public int? MinValue { get; set; } = 0;

    public bool Draw(int x, int y)
    {
        bool pressed = false;

        Vector2 mousePoint = Raylib.GetMousePosition();
        int textWidth = Raylib.MeasureText(Text, FontSize);

        Rectangle cursor = new Rectangle(x + 5 + textWidth + 2, y + 5, 4, 20);
        Rectangle clickBounds = new Rectangle(x, y, Width, Height);

        if (EditMode && Enabled)
        {
            int keyCount = Text.Length;
            int pressedKey = Raylib.GetCharPressed();
            if (pressedKey >= 32 && keyCount < CharLimit)
            {
                Text = Text.Insert(keyCount, ((char)pressedKey).ToString());
            }

            if (keyCount > 0 && Raylib.IsKeyPressed(KeyboardKey.KEY_BACKSPACE))
            {
                Text = Text.Remove(keyCount - 1);
            }
        }

        // DRAW
        Raylib.DrawRectangleRec(clickBounds, Color.WHITE);
        Raylib.DrawRectangleLinesEx(clickBounds, 1.0f, new Color(100, 100, 100, 255));

        Raylib.DrawText(Text, x + 5, y + 5, FontSize, Color.BLACK);

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
