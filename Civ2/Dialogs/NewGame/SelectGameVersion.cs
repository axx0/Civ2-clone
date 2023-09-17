using Civ2.Rules;
using Civ2engine;
using Model;
using Model.InterfaceActions;

namespace Civ2.Dialogs.NewGame;

// ReSharper disable once ClassNeverInstantiated.Global
public class SelectGameVersionHandler : BaseDialogHandler
{
    internal const string Title = "AXX-Select-Game";

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
            return civDialogHandlers[MainMenu.Title].Show();
        }

        Initialization.ConfigObject.RuleSet = Initialization.RuleSets[result.SelectedIndex];
        
        Initialization.LoadGraphicsAssets(civ2Interface);
        
        if (result.SelectedButton == "Quick Start")
        {
            Initialization.ConfigObject.RuleSet.QuickStart = true;
        }

        return civDialogHandlers[WorldSizeHandler.Title].Show();
    }
}