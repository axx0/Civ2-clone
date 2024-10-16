using Raylib_CSharp.Interact;

namespace Model.Menu;

public class MenuElement
{
    public MenuElement(string menuText, Shortcut shortcut, KeyboardKey hotkey, string commandId = "",
        bool omitIfNoCommand = false, bool repeat = false)
    {
        this.MenuText = menuText;
        this.Shortcut = shortcut;
        Hotkey = hotkey;
        CommandId = commandId;
        OmitIfNoCommand = omitIfNoCommand;
        Repeat = repeat;
    }

    public bool Repeat { get; }
    
    public bool OmitIfNoCommand { get;  }

    public string MenuText { get; }
    public Shortcut Shortcut { get; }
    public KeyboardKey Hotkey { get; }
    public string CommandId { get; }
}