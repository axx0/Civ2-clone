
using Civ2engine;
using Raylib_CSharp.Images;

namespace RaylibUI;

/// <summary>
/// This screen should be shown when there is no Civ2Path set to show a file dialog so the user cna set it
/// </summary>
public class GameFileLocatorScreen : BaseScreen
{
    public GameFileLocatorScreen(Main host, Action onSelect, Action shutdownApp) : base(host)
    {
        ImageUtils.InnerWallpaper = Image.Load("stripe.png");
        ImageUtils.OuterWallpaper = Image.Load("SteelGrey.png");
        ShowDialog(new FileDialog(host,"Please select Civ 2 data folder", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Settings.IsValidRoot, (fileName) =>
        {
            if (fileName is null)
            {
                shutdownApp();
                return false;
            }
            if (!Settings.AddPath(fileName)) return false;
            onSelect();
            return true;
        }));
    }

    public override void InterfaceChanged(Sound sound)
    {
        MainWindow.ReloadMain();
    }
}