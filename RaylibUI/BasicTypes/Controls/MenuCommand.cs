using Raylib_cs;

namespace RaylibUI.BasicTypes.Controls;

public class MenuCommand
{
    public string MenuText { get; set; }
    public KeyboardKey Shortcut { get; set; }
    public bool Enabled { get; set; }

    public event EventHandler<EventArgs> Executed;
}
