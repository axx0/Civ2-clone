using System;
using System.Configuration;

namespace civ2
{
    public class Settings
    {
        //Game settings from App.config
        public static string Civ2Path { get; private set; }

        public static void LoadConfigSettings()
        {
            //Load settings from App.config
            try
            {
                //Read from config file
                Civ2Path = ConfigurationManager.AppSettings.Get("path");
                // TODO: check if path exists
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app settings");
            }
        }

        public static void UpdateConfigSettings(string civ2Path)
        {
            //First update the App.config file
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["path"].Value = civ2Path;

            config.AppSettings.SectionInformation.ForceSave = true;
            config.Save(ConfigurationSaveMode.Modified);

            //Then update settings variables for the game
            Civ2Path = civ2Path;
        }
    }
}
