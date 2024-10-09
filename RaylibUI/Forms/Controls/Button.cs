using Civ2engine;
using Raylib_CSharp.Collision;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Transformations;
using System.Numerics;

namespace RaylibUI.Forms;

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

        // Keys
        if (Enabled && KeyPressed != 0 )
        {
            switch (KeyPressed)
            {
                case (int)KeyboardKey.Enter or (int)KeyboardKey.KpEnter:
                    if (Text == Labels.Ok)
                        _pressed = true;
                    break;
                case (int)KeyboardKey.Escape:
                    if (Text == Labels.Cancel)
                        _pressed = true;
                    break;
            }
            KeyPressed = 0;
        }

        Graphics.DrawRectangleLinesEx(new Rectangle(x, y, Width, Height), 1.0f, new Color(100, 100, 100, 255));
        Graphics.DrawRectangleRec(new Rectangle(x + 1, y + 1, Width - 2, Height - 2), Color.White);
        Graphics.DrawRectangleRec(new Rectangle(x + 3, y + 3, Width - 6, Height - 6), new Color(192, 192, 192, 255));
        Graphics.DrawLine(x + 2, y + Height - 2, x + Width - 2, y + Height - 2, new Color(128, 128, 128, 255));
        Graphics.DrawLine(x + 3, y + Height - 3, x + Width - 2, y + Height - 3, new Color(128, 128, 128, 255));
        Graphics.DrawLine(x + Width - 1, y + 2, x + Width - 1, y + Height - 1, new Color(128, 128, 128, 255));
        Graphics.DrawLine(x + Width - 2, y + 3, x + Width - 2, y + Height - 1, new Color(128, 128, 128, 255));

        //var textSize = Raylib.MeasureTextEx(, Text, 18, 1.0f);
        //Raylib.DrawText(Text, X + Width / 2 - (int)textSize.X / 2, Y + Height / 2 - (int)textSize.Y / 2, 18, Color.Black);

        Vector2 mousePos = Input.GetMousePosition();
        if (Input.IsMouseButtonPressed(MouseButton.Left) && ShapeHelper.CheckCollisionPointRec(mousePos, new Rectangle(x, y, Width, Height)) && Enabled)
        {
            _pressed = true;
        }
    }
}
