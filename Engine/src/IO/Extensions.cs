using System;
using System.IO;

namespace Civ2engine
{
    public static class Utils
    {
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