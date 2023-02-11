using Civ2.Rules;
using Civ2engine;
using Model;

namespace Civ2.Dialogs.NewGame.CustomWorldDialogs;

public class CustomTemp: SimpleSettingsDialog
{
    public const string Title = "CUSTOMTEMP";

    public CustomTemp() : base(Title, CustomAge.Title)
    {
    }

    protected override void SetConfigValue(DialogResult result, PopupBox popupBox)
    {
        Initialization.ConfigObject.Temperature = result.SelectedButton == popupBox.Button[0]
            ? Initialization.ConfigObject.Random.Next(popupBox.Options.Count)
            : result.SelectedIndex;
    }
}