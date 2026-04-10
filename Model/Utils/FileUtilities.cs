using Civ2engine.IO;

namespace Model.Utils;

public static class FileUtilities
{
    /// <summary>
    /// Returns path to file ignoring its case or null if file doesn't exist.
    /// </summary>
    public static string? GetFile(string path, string fileName)
    {
        return Directory.EnumerateFiles(path, fileName, 
            new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive }).FirstOrDefault();

        //var filePath = Path.Combine(path, fileName);
        //if (File.Exists(filePath))
        //    return filePath;

        //return (from file in Directory.EnumerateFiles(path)
        //    let actualFileName = Path.GetFileName(file)
        //    where actualFileName.Equals(fileName, StringComparison.OrdinalIgnoreCase)
        //    select file).FirstOrDefault();
    }

}