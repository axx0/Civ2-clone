using Civ2.Rules;
using Civ2engine;
using Model.Dialog;
using Model.Interface;
using Model.InterfaceActions;

namespace Civ2.Dialogs.NewGame;

public class EnterName : BaseDialogHandler
{
    public const string Title = "NAME";

    public EnterName() : base(Title, 0, -0.03)
    {
    }

    public override IInterfaceAction Show(Civ2Interface activeInterface)
    {
        var config = Initialization.ConfigObject;

        if (Dialog.TextBoxes == null || Dialog.TextBoxes.Count == 0)
        {
            Dialog.TextBoxes = new List<TextBoxDefinition>
            {
                new()
                {
                    Index = 0,
                    Name = "Name",
                    InitialValue = config.IsScenario ?
                        config.LeaderNames[config.ScenPlayerCivId] :
                        config.PlayerCiv.LeaderName,
                    Width = 400,
                    Description = Dialog.Dialog.Options?[0] ?? string.Empty
                },
            };
            Dialog.Dialog.Options = null;
        }
        else
        {
            Dialog.TextBoxes[0].InitialValue = config.IsScenario ?
                config.LeaderNames[config.ScenPlayerCivId] :
                config.PlayerCiv.LeaderName;
        }

        return base.Show(activeInterface);
    }

    public override IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers, Civ2Interface civ2Interface)
    {
        if (result.SelectedButton == Labels.Cancel)
        {
            return Initialization.ConfigObject.IsScenario ?
                civDialogHandlers[DifficultyHandler.Title].Show(civ2Interface) :
                civDialogHandlers[SelectGender.Title].Show(civ2Interface);
        }

        if (result.TextValues != null)
        {
            if (Initialization.ConfigObject.IsScenario)
            {
                Initialization.ConfigObject.LeaderName = result.TextValues["Name"];
            }
            else
            {
                Initialization.ConfigObject.PlayerCiv.LeaderName = result.TextValues["Name"];
            }
        }
        
        if (Initialization.ConfigObject.IsScenario)
        {
            var game = Initialization.UpdateScenarioChoices();
            
            civ2Interface.ScenTitleImage = null;
            Initialization.Start(game);
            return new StartGame(Initialization.GameInstance, Initialization.ViewData);
        }
        else
        {
            return civDialogHandlers[SelectCityStyle.Title].Show(civ2Interface);
        }
    }
}