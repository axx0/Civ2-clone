using Civ2.Dialogs.FileDialogs;
using Civ2.Dialogs.NewGame;
using Civ2.Rules;
using Civ2engine;
using Model;
using Model.InterfaceActions;

namespace Civ2.Dialogs.Scenario;

public class ScenChoseCiv : ICivDialogHandler
{
    public const string Title = "SCENCHOSECIV";

    public string Name { get; } = Title;
    public ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox?> popups)
    {
        Dialog = new DialogElements
        {
            Dialog = new PopupBox()
            {
                Button = new List<string> { Labels.Ok, Labels.Cancel },
                Title = " ",
                Name = Title,
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

        Game.Instance.AllCivilizations.Find(c => c.PlayerType == PlayerType.Local).PlayerType = PlayerType.Ai;
        Game.Instance.AllCivilizations[result.SelectedIndex + 1].PlayerType = PlayerType.Local;

        civDialogHandlers[Difficulty.Title].Dialog.Dialog.Options = new List<string> { 
            Labels.For(LabelIndex.Chieftan) + " (easiest)", Labels.For(LabelIndex.Warlord),
            Labels.For(LabelIndex.Prince), Labels.For(LabelIndex.King), 
            Labels.For(LabelIndex.Emperor), Labels.For(LabelIndex.Deity) + " (toughest)"};

        civDialogHandlers[Difficulty.Title].Dialog.Dialog.Button = new List<string> { Labels.Ok, Labels.Cancel };

        civDialogHandlers[Difficulty.Title].Dialog.Dialog.Title = $"Select {Labels.For(LabelIndex.Difficulty)} Level";
        
        return civDialogHandlers[Difficulty.Title].Show(civ2Interface); ;
    }

    public IInterfaceAction Show(Civ2Interface activeInterface)
    {
        Dialog.Dialog.Options = Game.Instance.AllCivilizations.Skip(1).Select(c => c.TribeName + " (" + c.LeaderName + ")").ToList();
        return new MenuAction(Dialog);
    }
}