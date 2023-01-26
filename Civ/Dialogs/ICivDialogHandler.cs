using Civ2engine;
using Model;

namespace Civ.Dialogs;

public interface ICivDialogHandler
{
    string Name { get; }
    ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox> popup);
    MenuElements Dialog { get; }
    IInterfaceAction HandleDialogResult(DialogResult result, Dictionary<string, ICivDialogHandler> civDialogHandlers);
    IInterfaceAction Show();
}