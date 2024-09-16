using Model.Dialog;

namespace Model.InterfaceActions;

public class MenuAction : IInterfaceAction
{
    public MenuAction(DialogElements dialog)
    {
        DialogElement = dialog;
        Name = dialog.Dialog.Name;
    }

    public string Name { get; }
    public EventType ActionType => EventType.Dialog;
    public DialogElements DialogElement { get; }
}