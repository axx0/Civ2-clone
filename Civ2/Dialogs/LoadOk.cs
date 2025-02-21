using Civ2.Rules;
using Civ2engine;
using Model;
using Model.Dialog;
using Model.InterfaceActions;

namespace Civ2.Dialogs;

public class LoadOk : ICivDialogHandler
{
    public const string Title = "LOADOK";

    public string Name { get; } = Title;
    public ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox?> popups)
    {   
        Dialog = new DialogElements
        {
            Dialog = popups[Name],
            DialogPos = new Point(0,0)
        };
        return this;
    }

    public DialogElements Dialog { get; private set; }

    public IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers, Civ2Interface civ2Interface)
    {
        return new StartGame(Initialization.GameInstance, Initialization.ViewData);
    }

    public IInterfaceAction Show(Civ2Interface activeInterface)
    {
        var game = Initialization.GameInstance;
        var playerCiv = game.GetPlayerCiv;
    
        Dialog.ReplaceStrings = new List<string>
        {
            playerCiv.LeaderTitle, playerCiv.LeaderName,
            playerCiv.TribeName, game.Date.GameYearString(game.TurnNumber),
            game.DifficultyLevel.ToString()
        };
        return new MenuAction(Dialog);
    }
}