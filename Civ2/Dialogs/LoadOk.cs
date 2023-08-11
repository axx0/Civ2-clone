using Civ2.Rules;
using Civ2engine;
using Model;
using Model.InterfaceActions;

namespace Civ2.Dialogs;

public class LoadOk : ICivDialogHandler
{
    public const string Title = "LOADOK";

    public string Name { get; } = Title;
    public ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox> popups)
    {        Dialog = new MenuElements
        {
            Dialog = popups[Name],
            DialogPos = new Point(0,0)
        };
        return this;
    }

    public MenuElements Dialog { get; private set; }

    public IInterfaceAction HandleDialogResult(DialogResult result, Dictionary<string, ICivDialogHandler> civDialogHandlers)
    {
        return new StartGame(Initialization.SelectedRuleSet, Initialization.GameInstance);
    }

    public IInterfaceAction Show()
    {
        var game = Initialization.GameInstance;
        var playerCiv = game.GetPlayerCiv;
    
        Dialog.ReplaceStrings = new List<string>
        {
            playerCiv.LeaderTitle, playerCiv.LeaderName,
            playerCiv.TribeName, game.GetGameYearString,
            game.DifficultyLevel.ToString()
        };
        return new MenuAction(Dialog);
    }
}