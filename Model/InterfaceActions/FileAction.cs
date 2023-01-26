namespace Model;

public class FileAction : IInterfaceAction
{
    public FileAction(OpenFileInfo fileInfo)
    {
        FileInfo = fileInfo;
    }

    public EventType ActionType => EventType.OpenFile;
    public MenuElements? MenuElement => null;
    public OpenFileInfo? FileInfo { get; }
}