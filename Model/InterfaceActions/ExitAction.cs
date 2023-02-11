namespace Model;

public class ExitAction : IInterfaceAction
{
    public static ExitAction Exit = new();
    private ExitAction()
    {
        
    }

    public EventType ActionType => EventType.Exit;
    public MenuElements? MenuElement => null;
    public OpenFileInfo? FileInfo => null;
}