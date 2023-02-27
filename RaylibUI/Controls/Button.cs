using Raylib_cs;
using System.Numerics;

namespace RaylibUI;

public class Button
{
    private readonly int _width, _height;
    private readonly string _text;

    public Button(int width, int height, string text)
    {
        _width = width;
        _height = height;
        _text = text;
    }

    // Return true when button pressed
    public bool Draw(int x, int y)
    {
        bool pressed = false;
        Vector2 mousePos = Raylib.GetMousePosition();

        int w = _width;
        int h = _height;

        Raylib.DrawRectangleLinesEx(new Rectangle(x, y, w, h), 1.0f, new Color(100, 100, 100, 255));
        Raylib.DrawRectangleRec(new Rectangle(x + 1, y + 1, w - 2, h - 2), Color.WHITE);
        Raylib.DrawRectangleRec(new Rectangle(x + 3, y + 3, w - 6, h - 6), new Color(192, 192, 192, 255));
        Raylib.DrawLine(x + 2, y + h - 2, x + w - 2, y + h - 2, new Color(128, 128, 128, 255));
        Raylib.DrawLine(x + 3, y + h - 3, x + w - 2, y + h - 3, new Color(128, 128, 128, 255));
        Raylib.DrawLine(x + w - 1, y + 2, x + w - 1, y + h - 1, new Color(128, 128, 128, 255));
        Raylib.DrawLine(x + w - 2, y + 3, x + w - 2, y + h - 1, new Color(128, 128, 128, 255));

        var textSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), _text, 18, 1.0f);
        Raylib.DrawText(_text, x + w / 2 - (int)textSize.X / 2, y + h / 2 - (int)textSize.Y / 2, 18, Color.BLACK);

        if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT) && Raylib.CheckCollisionPointRec(mousePos, new Rectangle(x, y, w, h)))
        {
            pressed = true;
        }

        return pressed;
    }
}
