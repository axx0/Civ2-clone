using Civ2.Dialogs.Scenario;
using Civ2.Rules;
using Civ2engine;
using Model;
using Model.InterfaceActions;

namespace Civ2.Dialogs;

public class ScenCustomIntro : ICivDialogHandler
{
    public const string Title = "SCENCUSTOMINTRO";

    public string Name { get; } = Title;
    public ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox?> popups)
    {
        Dialog = new DialogElements
        {
            Dialog = popups[Name],
            DialogPos = new Point(0, 0),
        };
        Dialog.Dialog.Name = Title;
        Dialog.Dialog.Button = new List<string> { Labels.Ok, Labels.Cancel };
        return this;
    }

    public DialogElements Dialog { get; private set; }

    public IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers, Civ2Interface civ2Interface)
    {
        if (result.SelectedButton == Labels.Cancel)
        {
            return civDialogHandlers[MainMenu.Title].Show(civ2Interface);
        }

        return civDialogHandlers[ScenChoseCiv.Title].Show(civ2Interface); ;
    }

    public IInterfaceAction Show(Civ2Interface activeInterface)
    {
        var game = Initialization.GameInstance;

        Dialog.ReplaceNumbers = new List<int> { game.ScenarioData.TechParadigm };
        Dialog.ReplaceStrings = new List<string>
        {
            game.ScenarioData.Name, game.Date.GameYearString(0),
            game.Date.GameYearString(game.ScenarioData.MaxTurns),
        };

        return new MenuAction(Dialog);
    }
}