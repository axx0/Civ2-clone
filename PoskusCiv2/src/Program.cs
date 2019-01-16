using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using PoskusCiv2.Imagery;
using PoskusCiv2.Sounds;

namespace PoskusCiv2
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ReadFiles.ReadRULES(@"C:\DOS\CIV 2\Civ2\RULES.TXT");
            //Game.LoadGame("C:/DOS/CIV 2/Civ2/Rome01.SAV");
            Game.LoadGame("C:/DOS/CIV 2/Civ2/Persia01.SAV");
            Images.LoadTerrain(@"C:\DOS\CIV 2\Civ2\TERRAIN1.GIF", @"C:\DOS\CIV 2\Civ2\TERRAIN2.GIF");
            Images.LoadCities(@"C:\DOS\CIV 2\Civ2\CITIES.GIF");
            Images.LoadUnits(@"C:\DOS\CIV 2\Civ2\UNITS.GIF");
            Images.LoadPeople(@"C:\DOS\CIV 2\Civ2\PEOPLE.GIF");
            Images.LoadIcons(@"C:\DOS\CIV 2\Civ2\ICONS.GIF");
            Images.LoadCityWallpaper(@"C:\DOS\CIV 2\Civ2\CITY.GIF");
            Sound.LoadSounds(@"C:\DOS\CIV 2\Civ2");
            Images.LoadDLLimages(@"C:\DOS\CIV 2\DLLs\");

            Game.StartGame();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Forms.MainCiv2Window());
        }
    }

}
