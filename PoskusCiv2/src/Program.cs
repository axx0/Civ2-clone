using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using PoskusCiv2.Imagery;

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
            //Game game = new Game();
            //Game.CreateGame();
            //Game.defineStartGameParameters();
            Game.LoadGame("C:/DOS/CIV 2/Civ2/Rome01.SAV");
            Images.LoadTerrain(@"C:\DOS\CIV 2\Civ2\TERRAIN1.GIF", @"C:\DOS\CIV 2\Civ2\TERRAIN2.GIF");
            Images.LoadCities(@"C:\DOS\CIV 2\Civ2\CITIES.GIF");
            Images.LoadUnits(@"C:\DOS\CIV 2\Civ2\UNITS.GIF");
            Images.LoadIcons(@"C:\DOS\CIV 2\Civ2\ICONS.GIF");
            Images.LoadWallpapers(@"C:\DOS\CIV 2\Civ2\CITY.GIF");
            Game.StartGame();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Forms.MainCiv2Window());


        }
    }

}
