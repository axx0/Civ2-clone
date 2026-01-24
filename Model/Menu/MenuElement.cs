using Model.Input;

namespace Model.Menu;

public class MenuElement(
    string menuText,
    Shortcut shortcut,
    Key hotkey,
    string commandId = "",
    bool omitIfNoCommand = false,
    bool repeat = false)
{
    public bool Repeat { get; } = repeat;

    public bool OmitIfNoCommand { get; } = omitIfNoCommand;

    public string MenuText { get; } = menuText;
    public Shortcut Shortcut { get; } = shortcut;
    public Key Hotkey { get; } = hotkey;
    public string CommandId { get; } = commandId;
}