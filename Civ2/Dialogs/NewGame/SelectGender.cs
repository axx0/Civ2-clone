using Civ2.Dialogs.Scenario;
using Civ2.Rules;
using Civ2engine;
using Model.Dialog;
using Model.InterfaceActions;

namespace Civ2.Dialogs.NewGame;

public class SelectGender : BaseDialogHandler
{
    public const string Title = "GENDER";

    public SelectGender() : base(Title, 0, -0.03)
    {
    }

    public override IInterfaceAction Show(Civ2Interface activeInterface)
    {
        var config = Initialization.ConfigObject;

        if (config.IsScenario) 
            Dialog.SelectedOption = config.CivGenders[config.ScenPlayerCivId] == 0 ? 0 : 1;

        return base.Show(activeInterface);
    }

    public override IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers, Civ2Interface civ2Interface)
    {
        var config = Initialization.ConfigObject;

        if (result.SelectedButton == Labels.Cancel)
        {
            return civDialogHandlers[DifficultyHandler.Title].Show(civ2Interface);
        }

        config.Gender = result.SelectedIndex;

        if (config.IsScenario)
        {
            return civDialogHandlers[EnterName.Title].Show(civ2Interface);
        }
        else
        {
            if (config.QuickStart)
            {
                var randomTribe = config.Random.ChooseFrom(config.Rules.Leaders);
                config.PlayerCiv = Initialization.MakeCivilization(config, randomTribe, true, randomTribe.Color);

                Initialization.CompleteConfig();
                return civDialogHandlers[Init.Title].Show(civ2Interface);
            }

            Initialization.ConfigObject.MapTask = MapGenerator.GenerateMap(Initialization.ConfigObject);
            return civDialogHandlers[SelectTribe.Title].Show(civ2Interface);
        }
    }
}