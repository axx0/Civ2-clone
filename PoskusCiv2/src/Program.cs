using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using RTciv2.Bitmaps;
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
            //ProgramArguments arguments = new ProgramArguments();
            //if (Parser.Default.ParseArguments(args, arguments))
            //{
            //    if (arguments.SAVName != null)
            //    {
            //        QuickLoad = true;
            //        SAVName = arguments.SAVName;
            //    }
            //    else SAVName = "";

            //    if (arguments.Path != null) Path = arguments.Path;
            //    else Path = @"C:\DOS\CIV 2\Civ2\";

            //    if (arguments.Verbose)
            //    {
            //        Console.WriteLine("Path = {0}", Path);
            //        Console.WriteLine("SAV File = {0}.SAV", SAVName);
            //    }
            //}

            //Read original Civ2 files
            //Images.LoadIcons();
            //Sound.LoadSounds(String.Concat(Path, @"\SOUND\"));
            //Images.LoadDLLimages();

            //Game.StartNewGame();
            //Game.LoadGame();


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //HelpConsole.CreateConsole();
            Application.Run(new Forms.IntroForm());
            //Application.Run(new Forms.MainCiv2Window());
        }
    }
}
