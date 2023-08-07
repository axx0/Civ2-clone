namespace Model.InterfaceActions;

public class MenuAction : IInterfaceAction
{
    public MenuAction(MenuElements menu)
    {
        MenuElement = menu;
        Name = menu.Dialog.Name;
    }

    public string Name { get; }
    public EventType ActionType => EventType.Dialog;
    public MenuElements MenuElement { get; }
}