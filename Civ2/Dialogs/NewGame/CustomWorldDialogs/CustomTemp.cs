using Civ2.Rules;
using Civ2engine;
using Model.Dialog;

namespace Civ2.Dialogs.NewGame.CustomWorldDialogs;

public class CustomTemp: SimpleSettingsDialog
{
    public const string Title = "CUSTOMTEMP";

    public CustomTemp() : base(Title)
    {
    }

    protected override string SetConfigValue(DialogResult result, PopupBox? popupBox)
    {
        Initialization.ConfigObject.Temperature = result.SelectedButton == popupBox.Button[0]
            ? Initialization.ConfigObject.Random.Next(popupBox.Options.Count)
            : result.SelectedIndex;
        return CustomAge.Title;
    }
}