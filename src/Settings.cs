using Microsoft.Extensions.Configuration;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;

namespace civ2
{
    public class Settings
    {
        // Game settings from App.config
        public static string Civ2Path { get; private set; }

        public static void LoadConfigSettings()
        {
            const int ERROR_INVALID_NAME = 123;

            var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json").Build();

            //Civ2Path = config.GetSection(nameof(Civ2Path)).Value;

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
            catch (ConfigurationErrorsException)
            {
                Debug.WriteLine("Error reading app settings");
            }
        }
    }
}
