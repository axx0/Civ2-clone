using System.Configuration;

namespace civ2
{
    public class Settings
    {
        //Game settings from App.config
        public static string Civ2Path { get; private set; }
        public static string SAVname { get; private set; }
        public static string WindowSize { get; private set; }

        public static void UpdateConfigSettings(string civ2Path, string savName, string windowSize)
        {
            //First update the App.config file
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["path"].Value = civ2Path;
            config.AppSettings.Settings["SAV file"].Value = savName;
            config.AppSettings.Settings["window size"].Value = windowSize;
            config.AppSettings.SectionInformation.ForceSave = true;
            config.Save(ConfigurationSaveMode.Modified);

            //Then update settings variables for the game
            Civ2Path = civ2Path;
            SAVname = savName;
            WindowSize = windowSize;
        }
    }
}
