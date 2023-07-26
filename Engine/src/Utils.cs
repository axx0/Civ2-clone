using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

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
                if (extensions.Length > 0)
                {
                    foreach (var path in searchPaths)
                    {
                        var searchPath = path + Path.DirectorySeparatorChar + filename;

                        foreach (var extension in extensions)
                        {
                            var filePath = searchPath + "." + extension;
                            if (File.Exists(filePath))
                            {
                                return filePath;
                            }
                        }
                    }
                }
                else
                {
                    foreach (var path in searchPaths)
                    {
                        var searchPath = path + Path.DirectorySeparatorChar + filename;

                        if (File.Exists(searchPath))
                        {
                            return searchPath;
                        }
                    }
                }
            }

            var rootPath = Settings.Civ2Path + Path.DirectorySeparatorChar + filename;

            if (File.Exists(rootPath))
            {
                return rootPath;
            }

            var appPath = Settings.BasePath  + filename;

            if (File.Exists(appPath))
            {
                return appPath;
            }
            
            foreach (var extension in extensions)
            {
                var filePath = appPath + "." + extension;
                if (File.Exists(filePath))
                {
                    return filePath;
                }
            }

            Console.WriteLine(filename + " not found!");
            
            return null;
        }
    }
}