using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using RTciv2.Imagery;
using RTciv2.Sounds;
using CommandLine;

namespace RTciv2
{
    static class Program
    {
        public static string Path = null;   //Path to Civ2 directory
        public static bool QuickLoad = false;   //Quickload enabled/disabled
        public static string SAVName = "";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {

            //Get input arguments
            ProgramArguments arguments = new ProgramArguments();
            if (Parser.Default.ParseArguments(args, arguments))
            {
                if (arguments.SAVName != null)
                {
                    QuickLoad = true;
                    SAVName = arguments.SAVName;
                }
                else SAVName = "";

                if (arguments.Path != null) Path = arguments.Path;
                else Path = @"C:\DOS\CIV 2\Civ2\";

                if (arguments.Verbose)
                {
                    Console.WriteLine("Path = {0}", Path);
                    Console.WriteLine("SAV File = {0}.SAV", SAVName);
                }
            }

            //Read original Civ2 files
            //ReadFiles.ReadRULES(String.Concat(Path, "RULES.TXT"));
            //Game.LoadGame(String.Concat(String.Concat(Path, SAVName), ".SAV"));
            //Images.LoadTerrain(String.Concat(Path, "TERRAIN1.GIF"), String.Concat(Path, "TERRAIN2.GIF"));
            //Images.LoadCities(String.Concat(Path, "CITIES.GIF"));
            //Images.LoadUnits(String.Concat(Path, "UNITS.GIF"));
            //Images.LoadPeople(String.Concat(Path, "PEOPLE.GIF"));
            Images.LoadIcons(String.Concat(Path, "ICONS.GIF"));
            //Images.LoadCityWallpaper(String.Concat(Path, "CITY.GIF"));
            //Sound.LoadSounds(String.Concat(Path, @"\SOUND\"));
            Images.LoadDLLimages(@"C:\DOS\CIV 2\DLLs\");

            //Game.StartGame();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            HelpConsole.CreateConsole();
            Application.Run(new Forms.MainCiv2Window());
        }
    }
}
