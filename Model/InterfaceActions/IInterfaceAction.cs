namespace Model;

public interface IInterfaceAction
{
    EventType ActionType { get; }
    MenuElements? MenuElement { get; }
    OpenFileInfo? FileInfo { get; }
}

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

public class OpenFileInfo
{
    public string Title { get; init; }
    public IList<FileFilter> Filters { get; init; }
}

public class FileFilter
{
    public string Name { get; }
    
    public string[] Extensions { get; }
}