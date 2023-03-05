using Raylib_cs;

namespace RaylibUI;

public class MenuBarItem
{
    public string Text;
    public MenuStripItem[] Items;
    public bool IsEnabled = true;
    public bool IsActivated = false;
    public Rectangle Bounds;
}
