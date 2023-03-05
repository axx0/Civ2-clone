using Raylib_cs;
using System.Numerics;

namespace RaylibUI;

public class Panel
{
    private bool dragging = false;

    public int Width { get; set; }
    public int Height { get; set; }
    public int X { get; set; } = 0;
    public int Y { get; set; } = 0;
    public Padding Padding { get; set; } = new Padding(11, 11, 38, 11);
    public FormattedText Title { get; set; }

    public void Draw()
    {
        int x = X;
        int y = Y;

        Vector2 mousePos = Raylib.GetMousePosition();
        Vector2 delta;

        // Drag/move panel
        if (Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT) && Raylib.CheckCollisionPointRec(mousePos, new Rectangle(x, y, Width, Padding.T)))
        {
            dragging = true;
        }

        if (dragging && Raylib.IsMouseButtonUp(MouseButton.MOUSE_BUTTON_LEFT))
        {
            dragging = false;
        }

        if (dragging) 
        {
            delta = Raylib.GetMouseDelta();
            X += (int)delta.X;
            Y += (int)delta.Y;
        }

        ImageUtils.PaintDialogBase(x, y, Width, Height, Padding);
        Title.Draw(x + Width / 2, y + Padding.T / 2);
    }
}
