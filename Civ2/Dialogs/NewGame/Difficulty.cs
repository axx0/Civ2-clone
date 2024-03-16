using Civ2.Dialogs.FileDialogs;
using Civ2.Dialogs.Scenario;
using Civ2.Rules;
using Civ2engine;
using Civ2engine.Enums;
using Model;
using Model.InterfaceActions;

namespace Civ2.Dialogs.NewGame;

public class Difficulty : BaseDialogHandler
{
    public const string Title = "DIFFICULTY";
    
    public Difficulty() : base(Title, 0.085, -0.03)
    {
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

        if (config.IsScenario)
        {
            Game.Instance.SetDifficultyLevel((DifficultyType)result.SelectedIndex);
            return civDialogHandlers[SelectGender.Title].Show(civ2Interface);
        }
        else
        {
            config.DifficultlyLevel = result.SelectedIndex; 
            return civDialogHandlers[NoOfCivs.Title].Show(civ2Interface);
        }
    }
}