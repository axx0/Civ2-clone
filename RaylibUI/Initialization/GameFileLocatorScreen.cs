
using Civ2engine;
using Raylib_cs;

namespace RaylibUI;

/// <summary>
/// This screen should be shown when there is no Civ2Path set to show a file dialog so the user cna set it
/// </summary>
public class GameFileLocatorScreen : BaseScreen
{
    public GameFileLocatorScreen(Action onSelect)
    {
        ImageUtils.InnerWallpaper = Raylib.LoadImage("stripe.png");
        ImageUtils.OuterWallpaper = Raylib.LoadImage("SteelGrey.png");
        ShowDialog(new FileDialog("Please select Civ 2 data folder", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Settings.IsValidRoot, (fileName) =>
        {
            if (!Settings.AddPath(fileName)) return false;
            onSelect();
            return true;
        }));
    }
}