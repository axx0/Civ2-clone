using Civ2.Rules;
using Civ2engine;
using Model.Dialog;

namespace Civ2.Dialogs.NewGame.PremadeWorld;

public class RandomiseResourceSeed : SimpleSettingsDialog
{
    public const string Title = "USESEED";
    public RandomiseResourceSeed() : base(Title)
    {
    }

    protected override string SetConfigValue(DialogResult result, PopupBox? popupBox)
    {
        if (result.SelectedIndex == 0)
        {
            Initialization.ConfigObject.ResourceSeed = 0;
        }

        if (Initialization.ConfigObject.StartPositions.Length > 0)
        {
            return StartLoc.StartLocKey;
        }

        return DifficultyHandler.Title;
    }
}