using Civ2engine;
using Model;
using Model.InterfaceActions;

namespace Civ2.Dialogs;

public interface ICivDialogHandler
{
    string Name { get; }
    ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox> popup);
    MenuElements Dialog { get; }
    IInterfaceAction HandleDialogResult(DialogResult result, Dictionary<string, ICivDialogHandler> civDialogHandlers,
        Civ2Interface activeInterface);
    IInterfaceAction Show(Civ2Interface activeInterface);
}