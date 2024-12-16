using Civ2.Rules;
using Civ2engine;
using Model.Dialog;

namespace Civ2.Dialogs.NewGame.PremadeWorld;

public class StartLoc : SimpleSettingsDialog
{
    public const string StartLocKey = "USESTARTLOC";

    public StartLoc() : base(StartLocKey)
    {
    }
    
    protected override string SetConfigValue(DialogResult result, PopupBox? popupBox)
    {
        if (result.SelectedIndex == 0)
        {
            Initialization.ConfigObject.StartPositions = [];
        }

        return DifficultyHandler.Title;
    }
}