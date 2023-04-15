using Raylib_cs;

namespace RaylibUI.Forms;

public class RadioButton : Control
{
    public string Text { get; set; }
    public bool IsPressed => Enabled && 
        Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT) &&
        Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), new Rectangle(_x + 6, _y + 3, _width - 4, 32));
    public bool IsSelected { get; set; }

    private int _x, _y, _width;
    public void Draw(int x, int y, int width)
    {
        _x = x;
        _y = y;
        _width = width;

        ImageUtils.PaintRadioButton(_x + 8, _y + 7, IsSelected);
        Raylib.DrawText(Text, _x + 38, _y + 5, 20, Color.BLACK);

        if (IsSelected)
        {
            Raylib.DrawRectangleLines(_x + 32, _y + 3,
                _width - 30 - 2, 26, new Color(64, 64, 64, 255));
        }
    }
}
