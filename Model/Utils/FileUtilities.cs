namespace Model.Utils;

public static class FileUtilities
{
    public static string? GetFile(string path, string fileName)
    {
        var filePath = Path.Combine(path, fileName);
        if (File.Exists(filePath))
            return filePath;

        return (from file in Directory.EnumerateFiles(path)
            let actualFileName = Path.GetFileName(file)
            where actualFileName.Equals(fileName, StringComparison.OrdinalIgnoreCase)
            select file).FirstOrDefault();
    }

}