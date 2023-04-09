using Raylib_cs;

namespace RaylibUI.Controls;

public class MenuStripItem
{
    public string Text { get; set; }
    public string KeyShortcut { get; set; }
    public bool Enabled { get; set; } = true;
    public Rectangle Bounds { get; set; }
}
