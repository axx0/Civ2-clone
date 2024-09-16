using Civ2.Rules;
using Civ2engine;
using Model.Dialog;
using Model.InterfaceActions;

namespace Civ2.Dialogs.NewGame;

public class AdvancedRules : BaseDialogHandler
{
    public const string Title = "ADVANCED";

    public AdvancedRules() : base(Title, 0.085, -0.03)
    {
    }

    public override ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox?> popups)
    {
        var res = base.UpdatePopupData(popups);
        res.Dialog.CheckboxStates = new[]
        {
            Initialization.ConfigObject.SimplifiedCombat,
            Initialization.ConfigObject.FlatWorld, 
            Initialization.ConfigObject.SelectComputerOpponents,
            Initialization.ConfigObject.AcceleratedStartup > 0,
            Initialization.ConfigObject.Bloodlust,
            Initialization.ConfigObject.DontRestartEliminatedPlayers
        };
        return res;
    }

    public override IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers, Civ2Interface civ2Interface)
    {
        if (result.SelectedButton == Labels.Cancel)
        {
            return civDialogHandlers[SelectGender.Title].Show(civ2Interface);
        }

        Initialization.ConfigObject.SimplifiedCombat = result.CheckboxReturnStates[0];
        Initialization.ConfigObject.FlatWorld = result.CheckboxReturnStates[1];
        Initialization.ConfigObject.SelectComputerOpponents = result.CheckboxReturnStates[2];
        Initialization.ConfigObject.Bloodlust = result.CheckboxReturnStates[4];
        Initialization.ConfigObject.DontRestartEliminatedPlayers = result.CheckboxReturnStates[5];

        if (result.CheckboxReturnStates[3])
        {
            return civDialogHandlers[SelectStartingYear.Title].Show(civ2Interface);
        }

        return civDialogHandlers[SelectGender.Title].Show(civ2Interface);
    }
}