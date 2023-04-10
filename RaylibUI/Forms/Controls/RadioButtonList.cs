using Raylib_cs;

namespace RaylibUI.Forms;

public class RadioButtonList : Control
{
    public IList<string> Texts { get; set; }
    public int Selected { get; set; }

    public void Draw(int x, int y, Padding padding, Size size)
    {
        // Detect mouse click on option
        var mousePos = Raylib.GetMousePosition();
        if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT) && Raylib.CheckCollisionPointRec(mousePos, new Rectangle(x + padding.L + 8, y + padding.T + 5, size.width - padding.L - padding.R - 8, 32 * Texts.Count)) && Enabled)
        {
            Selected = ((int)mousePos.Y - y - padding.T - 5) / 32;
        }

        for (int i = 0; i < Texts.Count; i++)
        {
            ImageUtils.PaintRadioButton(x + padding.L + 10, y + padding.T + 9 + 32 * i, Selected == i);
            Raylib.DrawText(Texts[i], x + padding.L + 40, y + padding.T + 10 + 32 * i, 20, Color.BLACK);

            if (Selected == i)
            {
                Raylib.DrawRectangleLines(x + padding.L + 34, y + padding.T + 5 + 32 * i, size.width - padding.L - padding.R - 34 - 2, 26, new Color(64, 64, 64, 255));
            }
        }
    }
}
