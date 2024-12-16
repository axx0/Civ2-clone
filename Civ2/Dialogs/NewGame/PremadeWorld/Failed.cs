using Civ2engine;
using Model.Dialog;

namespace Civ2.Dialogs.NewGame.PremadeWorld;

public class Failed : SimpleSettingsDialog
{
    public const string Title = "FAILEDTOLOAD";
    
    public Failed() : base(Title){}
    protected override string SetConfigValue(DialogResult result, PopupBox? popupBox)
    {
        return MainMenu.Title;
    }
}