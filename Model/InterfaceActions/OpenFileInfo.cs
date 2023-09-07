namespace Model.InterfaceActions;

public class OpenFileInfo
{
    public string Title { get; init; }
    public IList<FileFilter> Filters { get; init; }
}