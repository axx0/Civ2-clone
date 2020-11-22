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
            string rulesPath1 = savDirectoryPath + "\\RULES.TXT";
            string rulesPath2 = Settings.Civ2Path + "RULES.TXT";
            if (File.Exists(rulesPath1))
            {
                Rules.ReadRULES(rulesPath1);
            }
            else
            {
                Rules.ReadRULES(rulesPath2);
            }

            // Import SAV
            ImportSAV(savDirectoryPath + "\\" + SAVname);

            // Import graphical assets from SAV directory. If they don't exist import from root civ2 directory.
            Images.LoadGraphicsAssetsFromFiles(savDirectoryPath);
        }
    }
}
