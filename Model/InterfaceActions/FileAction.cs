namespace Model.InterfaceActions;

public class FileAction : IInterfaceAction
{
    public FileAction(OpenFileInfo fileInfo, string name)
    {
        FileInfo = fileInfo;
        Name = name;
    }

    public string Name { get; }
    public EventType ActionType => EventType.OpenFile;
    public OpenFileInfo FileInfo { get; }
}