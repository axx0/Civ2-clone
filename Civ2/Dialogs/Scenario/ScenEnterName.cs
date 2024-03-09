using Civ2.Dialogs.FileDialogs;
using Civ2.Dialogs.NewGame;
using Civ2.Rules;
using Civ2engine;
using Model;
using Model.Interface;
using Model.InterfaceActions;

namespace Civ2.Dialogs.Scenario;

public class ScenEnterName : ICivDialogHandler
{
    public const string Title = "SCENENTERNAME";

    public string Name { get; } = Title;
    public ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox?> popups)
    {
        Dialog = new DialogElements
        {
            Dialog = new PopupBox()
            {
                Button = new List<string> { Labels.Ok, Labels.Cancel },
                Title = "Please enter your name",
                Name = Title,
                Width = 440
            },
            DialogPos = new Point(0, 0),
        };

        Dialog.TextBoxes = new List<TextBoxDefinition>
        {
            new()
            {
                Index = 0, Name = "Name",
                Width = 400
            },
        };
        return this;
    }

    public DialogElements Dialog { get; private set; }

    public IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers, Civ2Interface civ2Interface)
    {
        if (result.SelectedButton == Labels.Cancel)
        {
            return civDialogHandlers[ScenDifficulty.Title].Show(civ2Interface);
        }

        Game.Instance.GetPlayerCiv.LeaderName = result.TextValues["Name"];
        return new StartGame(Initialization.GameInstance);
    }

    public IInterfaceAction Show(Civ2Interface activeInterface)
    {
        Dialog.TextBoxes[0].InitialValue = Game.Instance.GetPlayerCiv.LeaderName;
        return new MenuAction(Dialog);
    }
}