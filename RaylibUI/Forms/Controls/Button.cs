using Civ2engine;
using Raylib_cs;
using System.Numerics;

namespace RaylibUI.Forms;

public class Button : Control
{
    public string Text { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    private bool _pressed;
    public bool Pressed => _pressed;

    public void Draw(int X, int Y)
    {
        _pressed = false;

        // Keys
        if (Enabled && KeyPressed != 0 )
        {
            switch (KeyPressed)
            {
                case (int)KeyboardKey.KEY_ENTER or (int)KeyboardKey.KEY_KP_ENTER:
                    if (Text == Labels.Ok)
                        _pressed = true;
                    break;
                case (int)KeyboardKey.KEY_ESCAPE:
                    if (Text == Labels.Cancel)
                        _pressed = true;
                    break;
            }
            KeyPressed = 0;
        }

        Raylib.DrawRectangleLinesEx(new Rectangle(X, Y, Width, Height), 1.0f, new Color(100, 100, 100, 255));
        Raylib.DrawRectangleRec(new Rectangle(X + 1, Y + 1, Width - 2, Height - 2), Color.WHITE);
        Raylib.DrawRectangleRec(new Rectangle(X + 3, Y + 3, Width - 6, Height - 6), new Color(192, 192, 192, 255));
        Raylib.DrawLine(X + 2, Y + Height - 2, X + Width - 2, Y + Height - 2, new Color(128, 128, 128, 255));
        Raylib.DrawLine(X + 3, Y + Height - 3, X + Width - 2, Y + Height - 3, new Color(128, 128, 128, 255));
        Raylib.DrawLine(X + Width - 1, Y + 2, X + Width - 1, Y + Height - 1, new Color(128, 128, 128, 255));
        Raylib.DrawLine(X + Width - 2, Y + 3, X + Width - 2, Y + Height - 1, new Color(128, 128, 128, 255));

        //var textSize = Raylib.MeasureTextEx(, Text, 18, 1.0f);
        //Raylib.DrawText(Text, X + Width / 2 - (int)textSize.X / 2, Y + Height / 2 - (int)textSize.Y / 2, 18, Color.BLACK);

        Vector2 mousePos = Raylib.GetMousePosition();
        if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT) && Raylib.CheckCollisionPointRec(mousePos, new Rectangle(X, Y, Width, Height)) && Enabled)
        {
            _pressed = true;
        }
    }
}
