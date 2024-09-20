using Civ2.Rules;
using Civ2engine;
using Model.Dialog;

namespace Civ2.Dialogs.NewGame;

public class Barbarity : SimpleSettingsDialog
{
    public const string Title = "BARBARITY";
    
    public Barbarity() : base(Title, 0.085, -0.03)
    {
    }

    protected override string SetConfigValue(DialogResult result, PopupBox? popupBox)
    {
        Initialization.ConfigObject.BarbarianActivity = result.SelectedIndex;
        return SelectRules.Title;
    }
}