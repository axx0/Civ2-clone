using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace Civ2engine
{
    public static class Settings
    {
        // Game settings from App.config
        public static string Civ2Path { get; private set; }

        public static void LoadConfigSettings()
        {
            const int ERROR_INVALID_NAME = 123;

            var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json").Build();

            // Load settings from App.config
            try
            {
                // Read from config file
                Civ2Path = config.GetSection(nameof(Civ2Path)).Value;
                if (!Directory.Exists(Civ2Path))
                {
                    Debug.WriteLine("Civ2 directory doesn't exist!");
                    Environment.Exit(ERROR_INVALID_NAME);
                }
            }
            catch
            {
                Debug.WriteLine("Error reading app settings");
            }
        }
    }
}
