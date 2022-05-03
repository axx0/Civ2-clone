using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Civ2engine
{
    public static class Settings
    {
        // Game settings from App.config
        public static string Civ2Path { get; private set; }
        
        public static string[] SearchPaths { get; private set; }

        public static void LoadConfigSettings()
        {
            const int ERROR_INVALID_NAME = 123;

            var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json").Build();

            // Load settings from App.config
            try
            {
                SearchPaths = config.GetSection(nameof(SearchPaths))
                    .GetChildren()
                    .Select(c => c.Value.TrimEnd(Path.DirectorySeparatorChar))
                    .Where(Directory.Exists).ToArray();

                // Read from config file
                Civ2Path = config.GetSection(nameof(Civ2Path)).Value.TrimEnd(Path.DirectorySeparatorChar);
                if (!Directory.Exists(Civ2Path))
                {
                    if (SearchPaths.Length == 0)
                    {
                        Debug.WriteLine("Civ2 directory doesn't exist!");
                        Environment.Exit(ERROR_INVALID_NAME);
                    }
                    Civ2Path = SearchPaths[0];
                }else if (!SearchPaths.Contains(Civ2Path))
                {
                    SearchPaths = new[] { Civ2Path }.Concat(SearchPaths).ToArray();
                }
            }
            catch
            {
                Debug.WriteLine("Error reading app settings");
            }
        }
    }
}
