using Civ2.Rules;
using Civ2engine;
using Model.Controls;

namespace Civ2.Dialogs.NewGame.CustomWorldDialogs;

public class CustomClimate: SimpleSettingsDialog
{
    public const string Title = "CUSTOMCLIMATE";

    public CustomClimate() : base(Title)
    {
    }

    protected override string SetConfigValue(DialogResult result, DialogElements? dialog)
    {
        Initialization.ConfigObject.Climate = result.SelectedButton == dialog.Button[0]
            ? Initialization.ConfigObject.Random.Next(dialog.Options.Texts.Count)
            : result.SelectedIndex;
        return CustomTemp.Title;
    }
}