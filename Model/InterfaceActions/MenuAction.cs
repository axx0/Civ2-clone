namespace Model;

public class MenuAction : IInterfaceAction
{
    public MenuAction(MenuElements menu)
    {
        MenuElement = menu;
    }
    public EventType ActionType => EventType.Dialog;
    public MenuElements? MenuElement { get; }
    public OpenFileInfo? FileInfo => null;
}