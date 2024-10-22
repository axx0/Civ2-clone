using Civ2engine;
using Model;
using Model.Dialog;
using Model.Images;
using Model.Menu;
using Raylib_CSharp.Interact;

namespace RaylibUI.RunGame.Commands;

public class CityReportOptions : IGameCommand
{
    private readonly GameScreen _gameScreen;
    private readonly Options _options;

    public CityReportOptions(GameScreen gameScreen)
    {
        _gameScreen = gameScreen;
        _options = gameScreen.Game.Options;
    }
    
    public string Id => CommandIds.CityReportOptions;
    public Shortcut[] ActivationKeys { get; set; } = { new(KeyboardKey.E, ctrl: true) };
    public CommandStatus Status => CommandStatus.Normal;

    public bool Update()
    {
        return true;
    }

    public void Action()
    {
        // ReSharper disable once StringLiteralTypo
        _gameScreen.ShowPopup("MESSAGEOPTIONS", DialogClick, 
            checkboxStates: new List<bool> { _options.WarnWhenCityGrowthHalted, _options.ShowCityImprovementsBuilt, _options.ShowNonCombatUnitsBuilt, _options.ShowInvalidBuildInstructions, _options.AnnounceCitiesInDisorder, _options.AnnounceOrderRestored, _options.AnnounceWeLoveKingDay, _options.WarnWhenFoodDangerouslyLow, _options.WarnWhenPollutionOccurs, _options.WarnChangProductWillCostShields, _options.ZoomToCityNotDefaultAction });
    }

    private void DialogClick(string button, int _, IList<bool>? checkboxes, IDictionary<string, string>? _2)
    {
        if (button == Labels.Ok)
        {
            _options.WarnWhenCityGrowthHalted = checkboxes[0];
            _options.ShowCityImprovementsBuilt = checkboxes[1];
            _options.ShowNonCombatUnitsBuilt = checkboxes[2];
            _options.ShowInvalidBuildInstructions = checkboxes[3];
            _options.AnnounceCitiesInDisorder = checkboxes[4];
            _options.AnnounceOrderRestored = checkboxes[5];
            _options.AnnounceWeLoveKingDay = checkboxes[6];
            _options.WarnWhenFoodDangerouslyLow = checkboxes[7];
            _options.WarnWhenPollutionOccurs = checkboxes[8];
            _options.WarnChangProductWillCostShields = checkboxes[9];
            _options.ZoomToCityNotDefaultAction = checkboxes[10];
        }
    }

    public MenuCommand? Command { get; set; }
    public string ErrorDialog { get; } = string.Empty;
    public DialogImageElements? ErrorImage { get; } = null;
    public string? Name { get; }
}