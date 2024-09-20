using Civ2.Rules;
using Civ2engine;
using Model.Dialog;

namespace Civ2.Dialogs.NewGame.CustomWorldDialogs;

public class CustomAge: SimpleSettingsDialog
{
    public const string Title = "CUSTOMAGE";

    public CustomAge() : base(Title)
    {
    }

    protected override string SetConfigValue(DialogResult result, PopupBox? popupBox)
    {
        Initialization.ConfigObject.Age = result.SelectedButton == popupBox.Button[0]
            ? Initialization.ConfigObject.Random.Next(popupBox.Options.Count)
            : result.SelectedIndex;
        return DifficultyHandler.Title;
    }
}