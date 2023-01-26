using Civ.Rules;
using Civ2engine;
using Model;

namespace Civ.Dialogs.NewGame;

public class CustomisePercentageLand : SimpleSettingsDialog
{
    public const string Title = "CUSTOMLAND";

    public CustomisePercentageLand() : base(Title, CustomiseLandform.Title)
    {
    }


    protected override void SetConfigValue(DialogResult result, PopupBox popupBox)
    {
        Initialization.ConfigObject.PropLand = result.SelectedButton == popupBox.Button[0]
            ? Initialization.ConfigObject.Random.Next(popupBox.Options.Count)
            : result.SelectedIndex;
    }
}