using Civ2.Dialogs.FileDialogs;
using Civ2.Dialogs.Scenario;
using Civ2.Rules;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.IO;
using Model;
using Model.Controls;
using Model.InterfaceActions;

namespace Civ2.Dialogs;

public class ScenCustomIntro : ICivDialogHandler
{
    public const string Title = "SCENCUSTOMINTRO";

    public string Name { get; } = Title;
    public ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox> popups)
    {
        Dialog = new DialogElements(popups[Name])
        {
            DialogPos = new Point(0, 0),
        };
        Dialog.Name = Title;
        Dialog.Button = new List<string> { Labels.Ok, Labels.Cancel };
        return this;
    }

    public DialogElements Dialog { get; private set; }

    public IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers, Civ2Interface civ2Interface)
    {
        if (result.SelectedButton == Labels.Cancel)
        {
            civ2Interface.ScenTitleImage = null;
            return civDialogHandlers[LoadScenario.DialogTitle].Show(civ2Interface);
        }

        return civDialogHandlers[ScenChoseCiv.Title].Show(civ2Interface);
    }

    public IInterfaceAction Show(Civ2Interface activeInterface)
    {
        var config = Initialization.ConfigObject;
        var date = new Date(config.StartingYear, config.TurnYearIncrement, config.DifficultyLevel);

        Dialog.ReplaceNumbers = [config.TechParadigm];
        Dialog.ReplaceStrings =
        [
            config.ScenarioName, date.GameYearString(1),
            date.GameYearString(config.MaxTurns),
        ];

        if (config.ObjectivesProtagonist == config.ScenPlayerCivId)
        {
            Dialog.Image = new(activeInterface.PicSources["unit"][config.ActiveUnitType]);
        }

        return new MenuAction(Dialog);
    }
}