using Civ2.Rules;
using Civ2engine.NewGame;
using Model.Dialog;
using Model.ImageSets;
using Model.InterfaceActions;

namespace Civ2.Dialogs.NewGame;

public class Init : BaseDialogHandler
{
    public const string Title = "INIT";
    public Init() : base(Title)
    {
    }

    public override IInterfaceAction Show(Civ2Interface activeInterface)
    {
        Dialog.Image = new(activeInterface.PicSources["backgroundImageSmall1"][0]);
        
        var config = Initialization.ConfigObject;
        if (config.PlayerCiv.Id >= Initialization.ConfigObject.Civilizations.Count)
        {
            var correctColour = activeInterface.PlayerColours[config.PlayerCiv.Id];
            var correctIndex = Initialization.ConfigObject.Civilizations.Count - 1;
            activeInterface.PlayerColours[config.PlayerCiv.Id] = activeInterface.PlayerColours[correctIndex];
            activeInterface.PlayerColours[correctIndex] = correctColour;
            config.PlayerCiv.Id = correctIndex;
        }
        var maps = config.MapTask.Result;
        Initialization.GameInstance = NewGameInitialisation.StartNewGame(config, maps, config.Civilizations.OrderBy(c=>c.Id).ToList(), activeInterface.MainApp.ActiveRuleSet.Paths);
        
        var playerCiv = Initialization.GameInstance.GetPlayerCiv;
        Dialog.ReplaceStrings = new List<string>
        {   playerCiv.LeaderName,
            playerCiv.TribeName, ""
        };
        return base.Show(activeInterface);
    }

    public override IInterfaceAction HandleDialogResult(DialogResult result, Dictionary<string, ICivDialogHandler> civDialogHandlers, Civ2Interface civ2Interface)
    {
        return new StartGame(Initialization.GameInstance, Initialization.ViewData);
    }
}