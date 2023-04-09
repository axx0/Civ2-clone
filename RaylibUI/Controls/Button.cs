using Raylib_cs;
using System.Numerics;

namespace RaylibUI.Controls;

public class Button : Control
{
    public string Text { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    private bool _pressed;
    public bool Pressed => _pressed;

    public void Draw(int x, int y)
    {
        _pressed = false;
        Vector2 mousePos = Raylib.GetMousePosition();

        Raylib.DrawRectangleLinesEx(new Rectangle(x, y, Width, Height), 1.0f, new Color(100, 100, 100, 255));
        Raylib.DrawRectangleRec(new Rectangle(x + 1, y + 1, Width - 2, Height - 2), Color.WHITE);
        Raylib.DrawRectangleRec(new Rectangle(x + 3, y + 3, Width - 6, Height - 6), new Color(192, 192, 192, 255));
        Raylib.DrawLine(x + 2, y + Height - 2, x + Width - 2, y + Height - 2, new Color(128, 128, 128, 255));
        Raylib.DrawLine(x + 3, y + Height - 3, x + Width - 2, y + Height - 3, new Color(128, 128, 128, 255));
        Raylib.DrawLine(x + Width - 1, y + 2, x + Width - 1, y + Height - 1, new Color(128, 128, 128, 255));
        Raylib.DrawLine(x + Width - 2, y + 3, x + Width - 2, y + Height - 1, new Color(128, 128, 128, 255));

        var textSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), Text, 18, 1.0f);
        Raylib.DrawText(Text, x + Width / 2 - (int)textSize.X / 2, y + Height / 2 - (int)textSize.Y / 2, 18, Color.BLACK);

        if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT) && Raylib.CheckCollisionPointRec(mousePos, new Rectangle(x, y, Width, Height)) && Enabled)
        {
            _pressed = true;
        }
    }
}
