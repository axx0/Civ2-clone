using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Civ2engine
{
    public static class Utils
    {
        public static int WrapNumber(int number, int range) => (number % range + range) % range;

        public static int GreatestCommonFactor(int a, int b)
        {
            while (b != 0)
            {
                var temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        public static int LowestCommonMultiple(int a, int b)
        {
            if (a == b || b < 1) return a;
            return (a / GreatestCommonFactor(a, b)) * b;
        }
        public static string? GetFilePath(string filename, IEnumerable<string>? searchPaths = null, params string[] extensions)
        {
            var paths = searchPaths ?? Settings.SearchPaths;
            
            if (extensions.Length > 0)
            {
                var files = extensions.Select(e => filename + "." + e).ToArray();
                foreach (var path in paths)
                {
                    if(!Directory.Exists(path)) continue;
                    foreach (var file in files)
                    {
                        var filePath = Directory.EnumerateFiles(path, file,
                            new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive }).FirstOrDefault();
                        if (filePath != null)
                        {
                            return filePath;
                        }
                        
                    }
                }
            }
            else
            {
                foreach (var path in paths)
                {
                    if(!Directory.Exists(path)) continue;
                    var filePath = Directory.EnumerateFiles(path, filename,
                        new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive }).FirstOrDefault();
                    if (filePath != null)
                    {
                        return filePath;
                    }
                }
            }

            Console.WriteLine(filename + " not found!");
            
            return null;
        }
    }
}