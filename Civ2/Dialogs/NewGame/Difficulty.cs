using Civ2.Dialogs.FileDialogs;
using Civ2.Dialogs.Scenario;
using Civ2.Rules;
using Civ2engine;
using Civ2engine.Enums;
using Model.Dialog;
using Model.Interface;
using Model.InterfaceActions;

namespace Civ2.Dialogs.NewGame;

public class DifficultyHandler : BaseDialogHandler
{
    public const string Title = "DIFFICULTY";
    
    public DifficultyHandler() : base(Title, 0.085, -0.03)
    {
    }

    public override IInterfaceAction Show(Civ2Interface activeInterface)
    {
        var config = Initialization.ConfigObject;

        if (config.IsScenario)
            Dialog.SelectedOption = config.DifficultyLevel;

        return base.Show(activeInterface);
    }

    public override IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers, Civ2Interface civ2Interface)
    {
        var config = Initialization.ConfigObject;

        if (result.SelectedButton == Labels.Cancel)
        {
            return config.IsScenario ?
                civDialogHandlers[LoadScenario.DialogTitle].Show(civ2Interface) :
                civDialogHandlers[MainMenu.Title].Show(civ2Interface);
        }

        config.DifficultyLevel = result.SelectedIndex;

        return config.IsScenario ?
            civDialogHandlers[SelectGender.Title].Show(civ2Interface) :
            civDialogHandlers[NoOfCivs.Title].Show(civ2Interface);
    }
}