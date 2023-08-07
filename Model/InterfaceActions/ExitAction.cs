namespace Model.InterfaceActions;

public class ExitAction : IInterfaceAction
{
    public static readonly ExitAction Exit = new();
    private ExitAction()
    {
        
    }

    public string Name => "Exit";
    public EventType ActionType => EventType.Exit;
}