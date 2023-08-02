namespace Model;

public class FileAction : IInterfaceAction
{
    public FileAction(OpenFileInfo fileInfo, string name)
    {
        FileInfo = fileInfo;
        Name = name;
    }

    public string Name { get; }
    public EventType ActionType => EventType.OpenFile;
    public MenuElements? MenuElement => null;
    public OpenFileInfo? FileInfo { get; }
}