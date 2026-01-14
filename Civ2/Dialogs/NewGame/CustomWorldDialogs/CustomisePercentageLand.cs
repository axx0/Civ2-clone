using Civ2.Rules;
using Civ2engine;
using Model.Dialog;

namespace Civ2.Dialogs.NewGame.CustomWorldDialogs;

public class CustomisePercentageLand : SimpleSettingsDialog
{
    public const string Title = "CUSTOMLAND";

    public CustomisePercentageLand() : base(Title)
    {
    }


    protected override string SetConfigValue(DialogResult result, DialogElements? dialog)
    {
        Initialization.ConfigObject.PropLand = result.SelectedButton == dialog.Button[0]
            ? Initialization.ConfigObject.Random.Next(dialog.Options.Texts.Count)
            : result.SelectedIndex;
        return CustomiseLandform.Title;
    }
}