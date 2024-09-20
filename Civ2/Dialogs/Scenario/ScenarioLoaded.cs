using Civ2.Dialogs.Scenario;
using Civ2.Rules;
using Civ2engine;
using Civ2engine.Enums;
using Model;
using Model.Dialog;
using Model.InterfaceActions;

namespace Civ2.Dialogs;

public class ScenarioLoadedDialog : ICivDialogHandler
{
    public const string Title = "SCENARIOLOADED";

    public string Name { get; } = Title;
    public ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox?> popups)
    {
        Dialog = new DialogElements
        {
            Dialog = popups[Name],
            DialogPos = new Point(0, 0)
        };
        return this;
    }

    public DialogElements Dialog { get; private set; }

    public IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers, Civ2Interface civ2Interface)
    {
        return civDialogHandlers[ScenChoseCiv.Title].Show(civ2Interface);
    }

    public IInterfaceAction Show(Civ2Interface activeInterface)
    {
        var config = Initialization.ConfigObject;
        var date = new Date(config.StartingYear, config.TurnYearIncrement, config.DifficultyLevel);

        Dialog.ReplaceNumbers = new List<int> { config.TechParadigm };
        Dialog.ReplaceStrings = new List<string>
        {
            config.ScenarioName, date.GameYearString(1),
            date.GameYearString(config.MaxTurns),
        };
        
        return new MenuAction(Dialog);
    }
}