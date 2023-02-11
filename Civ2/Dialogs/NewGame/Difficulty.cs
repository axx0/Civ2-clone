using Civ.Rules;
using Civ2engine;
using Model;

namespace Civ.Dialogs.NewGame;

public class Difficulty : SimpleSettingsDialog
{
    public const string Title = "DIFFICULTY";
    
    public Difficulty() : base(Title, MainMenu.Title)
    {
    }

    protected override void SetConfigValue(DialogResult result, PopupBox popupBox)
    {
        Initialization.ConfigObject.DifficultlyLevel = result.SelectedIndex;
    }
}