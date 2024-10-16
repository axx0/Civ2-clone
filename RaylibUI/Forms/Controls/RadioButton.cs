using Microsoft.VisualBasic;
using Raylib_CSharp.Collision;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Transformations;

namespace RaylibUI.Forms;

public class RadioButton : Control
{
    public string Text { get; set; }
    public bool IsPressed => Enabled && 
        Input.IsMouseButtonPressed(MouseButton.Left) &&
        ShapeHelper.CheckCollisionPointRec(Input.GetMousePosition(), new Rectangle(_x + 6, _y + 3, _width - 4, 32));
    public bool IsSelected { get; set; }

    private int _x, _y, _width;
    public void Draw(int x, int y, int width)
    {
        _x = x;
        _y = y;
        _width = width;

        ImageUtils.PaintRadioButton(_x + 8, _y + 7, IsSelected);
        Graphics.DrawText(Text, _x + 38, _y + 5, 20, Color.Black);

        if (IsSelected)
        {
            Graphics.DrawRectangleLines(_x + 32, _y + 3,
                _width - 30 - 2, 26, new Color(64, 64, 64, 255));
        }
    }
}
