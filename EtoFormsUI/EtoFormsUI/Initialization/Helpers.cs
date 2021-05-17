using System;
using System.Collections.Generic;
using System.IO;
using Civ2engine;

namespace EtoFormsUI.Initialization
{
    public static class Helpers
    {
        internal static IList<Ruleset> LocateRules(params string[] searchPaths)
                {
                    var foundRules = new List<Ruleset>();
                    foreach (var searchPath in searchPaths)
                    {
                        var rules = searchPath + Path.DirectorySeparatorChar + "rules.txt";
                        if (File.Exists(rules))
                        {
                            var game = searchPath + Path.DirectorySeparatorChar + "game.txt";
                            var name = "Default";
                            if (File.Exists(game))
                            {
                                foreach (var line in File.ReadLines(game))
        
                                {
                                    if (!line.StartsWith("@title")) continue;
                                    name = line[7..];
                                    break;
                                }
                            }
        
                            foundRules.Add( new Ruleset { Name= name, FolderPath = searchPath, Root = searchPath });
                        }
                    }
        
                    return foundRules;
                }
    }
}