using Civ2.Rules;
using Civ2engine;
using Model.Dialog;
using Model.InterfaceActions;

namespace Civ2.Dialogs.NewGame;

public class SelectStartingYear : BaseDialogHandler
{
    public const string Title = "ACCELERATED";

    public SelectStartingYear() : base(Title, -0.085, -0.03)
    {
    }

    public override ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox?> popups)
    {
        var res = base.UpdatePopupData(popups);
        res.Dialog.ReplaceNumbers = new[] { 4000, 3000, 2000 };
        return res;
    }

    public override IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers, Civ2Interface civ2Interface)
    {
        if (result.SelectedButton == Labels.Cancel)
        {
            return civDialogHandlers[SelectRules.Title].Show(civ2Interface);
        }

        Initialization.ConfigObject.AcceleratedStartup = result.SelectedIndex;

        return civDialogHandlers[SelectGender.Title].Show(civ2Interface);
    }
}