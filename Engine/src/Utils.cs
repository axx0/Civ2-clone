using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Civ2engine
{
    public static class Utils
    {
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
        public static string GetFilePath(string filename, IEnumerable<string> searchPaths = null, params string[] extensions)
        {
            if (searchPaths != null)
            {
                foreach (var path in searchPaths)
                {
                    var searchPath = path + Path.DirectorySeparatorChar + filename;
                    if (extensions.Length > 0)
                    {
                        foreach (var extension in extensions)
                        {
                            var filePath = searchPath + "." + extension;
                            if (File.Exists(filePath))
                            {
                                return filePath;
                            }

                        }
                    }
                    else if (File.Exists(searchPath))
                    {
                        return searchPath;
                    }
                }
            }

            var rootPath = Settings.Civ2Path + filename;

            if (File.Exists(rootPath))
            {
                return rootPath;
            }

            Console.WriteLine(filename + " not found!");
            return null;
        }
    }
}