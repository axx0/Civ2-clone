namespace Model.InterfaceActions;

public class FileFilter
{
    private readonly string _extension;

    public FileFilter(string extension)
    {
        _extension = extension;
    }

    public bool IsMatch(string fileName)
    {
        return fileName.EndsWith(_extension, StringComparison.InvariantCultureIgnoreCase);
    }
}