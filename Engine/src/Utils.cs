using System;
using System.IO;

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
        public static string GetFilePath(string filename, params string[] searchPaths)
        {
            foreach (var path in searchPaths)
            {
                var searchPath = path + Path.DirectorySeparatorChar + filename;
                if (File.Exists(searchPath))
                {
                    return searchPath;
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