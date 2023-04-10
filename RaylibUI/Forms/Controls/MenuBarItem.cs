using Raylib_cs;

namespace RaylibUI.Forms;

public class MenuBarItem
{
    public string Text { get; set; }
    public MenuStripItem[] Items { get; set; }
    public bool Enabled { get; set; } = true;
    public bool Activated { get; set; } = false;
    public Rectangle Bounds { get; set; }
}
