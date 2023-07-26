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
                        foreach (var extension in extensions)
                        {
                            var searchPath = FileExists(path, filename + "." + extension);
                            if (searchPath is not null)
                            {
                                return searchPath;
                            }
                        }
                    }
                }
                else
                {
                    foreach (var path in searchPaths)
                    {
                        var searchPath = FileExists(path, filename);
                        if (searchPath is not null)
                        {
                            return searchPath;
                        }
                    }
                }
            }

            var rootPath = FileExists(Settings.Civ2Path, filename);

            if (rootPath is not null)
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

        // Check if file exists in directory (ignoring case). 
        // If file is found, return file path with correct case.
        public static string FileExists(string path, string file)
        {
            var d = new DirectoryInfo(path); //Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles(); //Getting Text files

            foreach(FileInfo dirFile in Files )
            {
                if (String.Equals(dirFile.Name, file, StringComparison.OrdinalIgnoreCase))
                    return path + Path.DirectorySeparatorChar + dirFile.Name;
            }
            return null;
        }
    }
}