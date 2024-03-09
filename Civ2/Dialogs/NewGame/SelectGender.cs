using Civ2.Rules;
using Civ2engine;
using Model;
using Model.InterfaceActions;

namespace Civ2.Dialogs.NewGame;

public class SelectGender : BaseDialogHandler
{
    public const string Title = "GENDER";

    public SelectGender() : base(Title, 0, -0.03)
    {
    }

    public override IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers, Civ2Interface civ2Interface)
    {
        if (result.SelectedButton == Labels.Cancel)
        {
            return civDialogHandlers[Difficulty.Title].Show(civ2Interface);
        }

        var config = Initialization.ConfigObject;
        config.Gender = result.SelectedIndex;
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