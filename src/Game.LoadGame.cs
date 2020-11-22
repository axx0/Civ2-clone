using System.IO;
using civ2.Bitmaps;

namespace civ2
{
    public partial class Game : BaseInstance
    {
        public static void LoadGame(string savDirectoryPath, string SAVname)
        {
            // Read files. If they don't exist in SAV directory path, read them from base directory in settings

            // Read RULES.txt
            Rules.ReadRULES(savDirectoryPath);

            // Import SAV
            ImportSAV(savDirectoryPath + "\\" + SAVname);

            // Import graphical assets from SAV directory. If they don't exist import from root civ2 directory.
            Images.LoadGraphicsAssetsFromFiles(savDirectoryPath);

            //_instance.ActiveUnit = Data.SelectedUnitIndex == -1 ? null : Units.Find(unit => unit.Id == Data.SelectedUnitIndex);    //null means all units have ended turn
            //_instance.ActiveCiv = Civs[Data.HumanPlayer];
            _instance.ActiveUnit = Game.Units[0];   //temp!!!
            _instance.ActiveCiv = Game.Civs[0];   //temp!!!
        }
    }
}
