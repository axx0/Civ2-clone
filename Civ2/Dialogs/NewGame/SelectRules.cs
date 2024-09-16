using Civ2.Dialogs.NewGame.CustomWorldDialogs;
using Civ2.Rules;
using Civ2engine;
using Model.Dialog;
using Model.InterfaceActions;

namespace Civ2.Dialogs.NewGame;

public class SelectRules : BaseDialogHandler
{
    public const string Title = "RULES";

    public SelectRules() : base(Title, -0.085, -0.03)
    {
    }

    public override IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers, Civ2Interface civ2Interface)
    {
        if (result.SelectedButton == Labels.Cancel)
        {
            return civDialogHandlers[MainMenu.Title].Show(civ2Interface);
        }

        if (result.SelectedButton == Labels.Ok && result.SelectedIndex == 1)
        {
            return civDialogHandlers[AdvancedRules.Title].Show(civ2Interface);
        }

        return civDialogHandlers[SelectGender.Title].Show(civ2Interface);
    }
}