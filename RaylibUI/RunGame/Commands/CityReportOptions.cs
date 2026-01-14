using Civ2engine;
using Model;
using Model.Dialog;
using Model.Images;
using Model.Menu;
using Raylib_CSharp.Interact;

namespace RaylibUI.RunGame.Commands;

public class CityReportOptions(GameScreen gameScreen)
    : AlwaysOnCommand(gameScreen, CommandIds.CityReportOptions, [new Shortcut(KeyboardKey.E, ctrl: true)])
{
    public override void Action()
    {
        var options = GameScreen.Game.Options;
        // ReSharper disable once StringLiteralTypo
        GameScreen.ShowPopup("MESSAGEOPTIONS", DialogClick,
            checkboxStates: new List<bool> { options.WarnWhenCityGrowthHalted, options.ShowCityImprovementsBuilt, options.ShowNonCombatUnitsBuilt, options.ShowInvalidBuildInstructions, options.AnnounceCitiesInDisorder, options.AnnounceOrderRestored, options.AnnounceWeLoveKingDay, options.WarnWhenFoodDangerouslyLow, options.WarnWhenPollutionOccurs, options.WarnChangProductWillCostShields, options.ZoomToCityNotDefaultAction });
    }

    private void DialogClick(string button, int _, IList<bool>? checkboxes, IDictionary<string, string>? _2)
    {
        if (button == Labels.Ok)
        {
            var options = GameScreen.Game.Options;
            options.WarnWhenCityGrowthHalted = checkboxes[0];
            options.ShowCityImprovementsBuilt = checkboxes[1];
            options.ShowNonCombatUnitsBuilt = checkboxes[2];
            options.ShowInvalidBuildInstructions = checkboxes[3];
            options.AnnounceCitiesInDisorder = checkboxes[4];
            options.AnnounceOrderRestored = checkboxes[5];
            options.AnnounceWeLoveKingDay = checkboxes[6];
            options.WarnWhenFoodDangerouslyLow = checkboxes[7];
            options.WarnWhenPollutionOccurs = checkboxes[8];
            options.WarnChangProductWillCostShields = checkboxes[9];
            options.ZoomToCityNotDefaultAction = checkboxes[10];
        }
    }
}