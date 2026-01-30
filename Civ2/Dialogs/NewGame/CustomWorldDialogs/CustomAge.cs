using Civ2.Rules;
using Civ2engine;
using Model.Controls;

namespace Civ2.Dialogs.NewGame.CustomWorldDialogs;

public class CustomAge: SimpleSettingsDialog
{
    public const string Title = "CUSTOMAGE";

    public CustomAge() : base(Title)
    {
    }

    protected override string SetConfigValue(DialogResult result, DialogElements? dialog)
    {
        Initialization.ConfigObject.Age = result.SelectedButton == dialog.Button[0]
            ? Initialization.ConfigObject.Random.Next(dialog.Options.Texts.Count)
            : result.SelectedIndex;
        return DifficultyHandler.Title;
    }
}