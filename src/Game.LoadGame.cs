using System.Collections.Generic;
using System.IO;
using civ2.Bitmaps;
using civ2.Units;

namespace civ2
{
    public partial class Game : BaseInstance
    {
        public static void LoadGame(string savDirectoryPath, string SAVname)
        {
            _instance = new Game();

            // Import graphical assets from SAV directory. If they don't exist import from root civ2 directory.
            Images.LoadGraphicsAssetsFromFiles(savDirectoryPath);

            // Read RULES.txt
            Rules.ReadRULES(savDirectoryPath);
            
            // Import SAV
            _instance.ImportSAV(savDirectoryPath + "\\" + SAVname);


            
            //_instance.ActiveUnit = Data.SelectedUnitIndex == -1 ? null : Units.Find(unit => unit.Id == Data.SelectedUnitIndex);    //null means all units have ended turn
            //_instance.ActiveCiv = Civs[Data.HumanPlayer];
            //_instance.ActiveUnit = Game.Units[0];   //temp!!!
            //_instance.ActiveCiv = Game.Civs[0];   //temp!!!

        }

        private Game()
        {
            _units = new List<IUnit>();
            _casualties = new List<IUnit>();
            _cities = new List<City>();
            _civs = new List<Civilization>();
            Options = new Options();
        }

        private void Okg()
        {
            
        }

    
    }
}
