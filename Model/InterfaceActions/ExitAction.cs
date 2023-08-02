namespace Model;

public class ExitAction : IInterfaceAction
{
    public static readonly ExitAction Exit = new();
    private ExitAction()
    {
        
    }

    public string Name => "Exit";
    public EventType ActionType => EventType.Exit;
    public MenuElements? MenuElement => null;
    public OpenFileInfo? FileInfo => null;
}