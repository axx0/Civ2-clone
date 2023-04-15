using Raylib_cs;
using System.Numerics;

namespace RaylibUI.Forms;

public abstract class Form : IForm
{
    public List<IControl> Controls = new();

    public int X { get; set; } = 0;
    public int Y { get; set; } = 0;
    public Size Size { get; set; }
    public Padding Padding { get; set; } = new Padding(11, 11, 38, 11);
    public bool Enabled { get; set; } = true;
    public FormattedText Title { get; set; }
    public bool Hover => Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), new Rectangle(X, Y, Size.width, Size.height));
    public bool Focused { get; set; }
    public bool Pressed => Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT) && Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), new Rectangle(X, Y, Size.width, Size.height));
    public bool PressedTop => Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT) && Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), new Rectangle(X, Y, Size.width, Padding.T));

    private bool dragging = false;
    protected int _formPosX, _formPosY;

    public void Focus()
    {
        Focused = true;
        Controls.ForEach(c => c.Enabled = true);
    }

    public void UnFocus()
    {
        Focused = false;
        Controls.ForEach(c => c.Enabled = false);
    }

    public Form()
    {
    }

    public void Draw()
    {
        X = _formPosX;
        Y = _formPosY;

        // Keys
        var key = Raylib.GetKeyPressed();
        if (key != 0 && Enabled && Focused)
        {
            Controls.ForEach(c => c.KeyPressed = key);
        }

        // Drag/move form
        if (PressedTop && Enabled && Focused)
        {
            dragging = true;
        }

        if (dragging && Raylib.IsMouseButtonUp(MouseButton.MOUSE_BUTTON_LEFT))
        {
            dragging = false;
        }

        Vector2 delta;
        if (dragging)
        {
            delta = Raylib.GetMouseDelta();
            _formPosX += (int)delta.X;
            _formPosY += (int)delta.Y;
        }

        ImageUtils.PaintDialogBase(_formPosX, _formPosY, Size.width, Size.height, Padding);

        // Draw title text
        Title?.Draw(_formPosX + Size.width / 2, _formPosY + Padding.T / 2);
    }
}
