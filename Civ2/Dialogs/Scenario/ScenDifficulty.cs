using Civ2.Dialogs.FileDialogs;
using Civ2.Dialogs.NewGame;
using Civ2.Rules;
using Civ2engine;
using Civ2engine.Enums;
using Model;
using Model.InterfaceActions;

namespace Civ2.Dialogs.Scenario;

public class ScenDifficulty : ICivDialogHandler
{
    public const string Title = "SCENDIFFICULTY";

    public string Name { get; } = Title;
    public ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox?> popups)
    {
        Dialog = new DialogElements
        {
            Dialog = new PopupBox()
            {
                Button = new List<string> { Labels.Ok, Labels.Cancel },
                Options = new List<string> { Labels.For(LabelIndex.Chieftan) + " (easiest)", Labels.For(LabelIndex.Warlord),
                        Labels.For(LabelIndex.Prince), Labels.For(LabelIndex.King), Labels.For(LabelIndex.Emperor),
                        Labels.For(LabelIndex.Deity) + " (toughest)"},
                Title = "Select " + Labels.For(LabelIndex.Difficulty) + " Level",
                Name = Title,
                Width = 320
            },
            DialogPos = new Point(0, 0),
        };
        return this;
    }

    public DialogElements Dialog { get; private set; }

    public IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers, Civ2Interface civ2Interface)
    {
        if (result.SelectedButton == Labels.Cancel)
        {
            return civDialogHandlers[LoadScenario.DialogTitle].Show(civ2Interface);
        }
        
        //Game.Instance.DifficultyLevel = (DifficultyType)result.SelectedIndex;
        return civDialogHandlers[ScenGender.Title].Show(civ2Interface); ;
    }

    public IInterfaceAction Show(Civ2Interface activeInterface)
    {
        return new MenuAction(Dialog);
    }
}