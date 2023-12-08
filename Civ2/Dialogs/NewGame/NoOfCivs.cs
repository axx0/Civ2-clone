using Civ2.Rules;
using Civ2engine;
using Model;

namespace Civ2.Dialogs.NewGame;

public class NoOfCivs : SimpleSettingsDialog
{
    public const string Title = "ENEMIES";
    
    public NoOfCivs() : base(Title, -0.085, -0.03)
    {
    }

    protected override string SetConfigValue(DialogResult result, PopupBox popupBox)
    {
        Initialization.ConfigObject.NumberOfCivs = result.SelectedIndex;
        return Barbarity.Title;
    }
}