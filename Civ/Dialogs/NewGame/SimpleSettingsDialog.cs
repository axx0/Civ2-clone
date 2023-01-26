using Civ2engine;
using Model;

namespace Civ.Dialogs.NewGame;

public abstract class SimpleSettingsDialog : BaseDialogHandler
{
    private readonly string _next;

    protected SimpleSettingsDialog(string name, string next) : base(name)
    {
        _next = next;
    }

    public override IInterfaceAction HandleDialogResult(DialogResult result, Dictionary<string, ICivDialogHandler> civDialogHandlers)
    {
        if (result.SelectedButton == Labels.Cancel)
        {
            return civDialogHandlers[MainMenu.Title].Show();
        }

        var popupBox = Dialog.Dialog;
        SetConfigValue(result, popupBox);

        return civDialogHandlers[_next].Show();
    }

    protected abstract void SetConfigValue(DialogResult result, PopupBox popupBox);

}