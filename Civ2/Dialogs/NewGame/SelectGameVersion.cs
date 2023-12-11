using Civ2.Rules;
using Civ2engine;
using Model;
using Model.InterfaceActions;

namespace Civ2.Dialogs.NewGame;

// ReSharper disable once ClassNeverInstantiated.Global
public class SelectGameVersionHandler : BaseDialogHandler
{
    public const string Title = "AXX-Select-Game";

    public SelectGameVersionHandler() : base(Title) {}
    public override ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox> popups)
    {
        popups[Name] = new PopupBox
        {
            Name = Title,
            Title = "Select game version", Options = Initialization.RuleSets.Select(f => f.Name).ToList(),
            Button = new List<string> {"Quick Start", "OK", Labels.Cancel}
        };
        return base.UpdatePopupData(popups);
    }

    public override IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers, Civ2Interface civ2Interface)
    {                    
        if (result.SelectedButton == Labels.Cancel)
        {
            return civDialogHandlers[MainMenu.Title].Show(civ2Interface);
        }

        Initialization.ConfigObject.RuleSet = Initialization.RuleSets[result.SelectedIndex];
        
        Initialization.LoadGraphicsAssets(civ2Interface);
        
        if (result.SelectedButton == "Quick Start")
        {
            Initialization.ConfigObject.RuleSet.QuickStart = true;
            Initialization.ConfigObject.WorldSize = new[] { 50, 80 };
            Initialization.ConfigObject.NumberOfCivs = civ2Interface.PlayerColours.Length - 1;
            Initialization.ConfigObject.BarbarianActivity = Initialization.ConfigObject.Random.Next(5);
            
            Initialization.ConfigObject.MapTask = MapGenerator.GenerateMap(Initialization.ConfigObject);
            return civDialogHandlers[Difficulty.Title].Show(civ2Interface);
        }

        return civDialogHandlers[WorldSizeHandler.Title].Show(civ2Interface);
    }
}