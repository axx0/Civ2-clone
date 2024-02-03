using Civ2.Dialogs.FileDialogs;
using Civ2engine;
using Model;
using Model.InterfaceActions;

namespace Civ2.Dialogs.Scenario;

public class ChoseScenCiv : ICivDialogHandler
{
    public const string Title = "SCENCHOSECIV";

    public string Name { get; } = Title;
    public ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox?> popups)
    {
        Dialog = new DialogElements
        {
            Dialog = popups[Name],
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
        return civDialogHandlers[ScenDifficulty.Title].Show(civ2Interface); ;
    }

    public IInterfaceAction Show(Civ2Interface activeInterface)
    {
        Dialog.Dialog.Options = Game.Instance.AllCivilizations.Skip(1).Select(c => c.TribeName + " (" + c.LeaderName + ")").ToList();
        return new MenuAction(Dialog);
    }
}