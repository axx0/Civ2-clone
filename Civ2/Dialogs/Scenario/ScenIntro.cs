using Civ2.Dialogs.Scenario;
using Civ2.Rules;
using Civ2engine;
using Model;
using Model.InterfaceActions;

namespace Civ2.Dialogs;

public class ScenIntro : ICivDialogHandler
{
    public const string Title = "SCENINTRO";

    public string Name { get; } = Title;
    public ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox> popups)
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
        return civDialogHandlers[ChoseScenCiv.Title].Show(civ2Interface);
    }

    public IInterfaceAction Show(Civ2Interface activeInterface)
    {
        return new MenuAction(Dialog);
    }
}