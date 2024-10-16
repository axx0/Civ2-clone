using Raylib_CSharp.Images;

namespace RaylibUI.Forms;

public class ListBox : Control
{
    /// <summary>
    /// List of left-aligned texts
    /// </summary>
    public List<string> LeftText { get; set; }

    /// <summary>
    /// List of right-aligned texts
    /// </summary>
    public List<string> RightText { get; set; }

    /// <summary>
    /// List of icons left of text
    /// </summary>
    public List<Image> Icons { get; set; }
    public int Selected { get; set; } = 0;
    public int Columns { get; set; }
    public ListboxType Type { get; set; } = ListboxType.Standard;

    public void Draw(int x, int y, Size size)
    {
        //// Keys
        //if (Enabled && KeyPressed != 0)
        //{
        //    switch (KeyPressed)
        //    {
        //        case (int)KeyboardKey.SPACE:
        //            Checked[Selected] = !Checked[Selected];
        //            break;
        //        case (int)KeyboardKey.DOWN or (int)KeyboardKey.RIGHT:
        //            Selected = Selected + 1 == Texts.Count ? 0 : Selected + 1;
        //            break;
        //        case (int)KeyboardKey.UP or (int)KeyboardKey.LEFT:
        //            Selected = Selected - 1 < 0 ? Texts.Count - 1 : Selected - 1;
        //            break;
        //    }
        //    KeyPressed = 0;
        //}

        //// Detect mouse click on box
        //var mousePos = Raylib.GetMousePosition();
        //if (Raylib.IsMouseButtonPressed(MouseButton.Left) && Raylib.CheckCollisionPointRec(mousePos, new Rectangle(X + 6, Y + 3, size.Width - 4, 32 * Texts.Count)) && Enabled)
        //{
        //    Selected = ((int)mousePos.Y - Y - 3) / 32;
        //    Checked[Selected] = !Checked[Selected];
        //}

        //for (int i = 0; i < Texts.Count; i++)
        //{
        //    ImageUtils.PaintCheckbox(X + 8, Y + 7 + 32 * i, Checked[i]);
        //    Raylib.DrawText(Texts[i], X + 38, Y + 8 + 32 * i, 20, Color.Black);

        //    if (Selected == i)
        //    {
        //        Graphics.DrawRectangleLines(X + 32, Y + 3 + 32 * i, size.Width - 30 - 2, 26, new Color(64, 64, 64, 255));
        //    }
        //}
    }

    public enum ListboxType
    {
        Standard,
        White
    }
}
