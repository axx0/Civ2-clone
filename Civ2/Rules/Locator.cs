using Civ2engine;

namespace Civ2.Rules;

public static class Locator
{
    
    internal static IList<Ruleset> LocateRules(params string[] searchPaths)
    {
        var foundRules = new List<Ruleset>();
        foreach (var searchPath in searchPaths)
        {
            ValidateFolder(searchPath, searchPath, foundRules);
            foreach (var directory in Directory.EnumerateDirectories(searchPath))
            {
                ValidateFolder(directory, searchPath, foundRules);
            }
        }
        
        return foundRules;
    }

    private static void ValidateFolder(string searchPath, string rootPath, List<Ruleset> foundRules)
    {
        var rules = searchPath + Path.DirectorySeparatorChar + "rules.txt";
        if (File.Exists(rules))
        {
            var game = searchPath + Path.DirectorySeparatorChar + "game.txt";
            var name = "";
            if (File.Exists(game))
            {
                foreach (var line in File.ReadLines(game))

                {
                    if (!line.StartsWith("@title")) continue;
                    name = line[7..];
                    break;
                }
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                name = Path.GetFileName(searchPath);
            }
            else if (searchPath != rootPath)
            {
                name += " - " + Path.GetFileName(searchPath);
            }

            foundRules.Add(new Ruleset {Name = name, FolderPath = searchPath, Root = rootPath});
        }
    }
}