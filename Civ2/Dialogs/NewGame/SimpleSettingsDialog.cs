using Civ2engine;
using Model;
using Model.InterfaceActions;

namespace Civ2.Dialogs.NewGame;

public abstract class SimpleSettingsDialog : BaseDialogHandler
{
    private readonly string _next;

    protected SimpleSettingsDialog(string name, string next, double x = 0, double y = 0) : base(name, x, y)
    {
        _next = next;
    }

    public override IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers, Civ2Interface civ2Interface)
    {
        if (result.SelectedButton == Labels.Cancel)
        {
            return civDialogHandlers[MainMenu.Title].Show(civ2Interface);
        }

        var popupBox = Dialog.Dialog;
        var next = SetConfigValue(result, popupBox);

        return civDialogHandlers[next].Show(civ2Interface);
    }

    protected abstract string SetConfigValue(DialogResult result, PopupBox popupBox);

}