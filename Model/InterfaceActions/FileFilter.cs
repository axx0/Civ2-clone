namespace Model;

public class FileFilter
{
    private readonly string _extension;

    public FileFilter(string extension)
    {
        _extension = extension;
    }
}