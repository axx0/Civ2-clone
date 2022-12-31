using Civ.Rules;
using Civ2engine;
using Model;

namespace Civ.Dialogs.NewGame;

// ReSharper disable once ClassNeverInstantiated.Global
public class SelectGameVersionHandler : ICivDialogHandler
{
    internal static string Title = "AXX-Select-Game";
    public string Name => Title;
    public ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox> popups)
    {
        popups[Name] = new PopupBox
        {
            Title = "Select game version", Options = Initialization.RuleSets.Select(f => f.Name).ToList(),
            Button = new List<string> {"Quick Start", "OK",Labels.Cancel}
        };
        return this;
    }

    public MenuElements Dialog { get; }
    public IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers)
    {                    
        if (result.SelectedIndex == int.MinValue)
        {
            return civDialogHandlers[MainMenu.Title].Show();
        }

        Initialization.ConfigObject.RuleSet = Initialization.RuleSets[result.SelectedIndex];
        
        if (result.SelectedButton == "Quick Start")
        {
            Initialization.ConfigObject.RuleSet.QuickStart = true;
        }

        return civDialogHandlers[WorldSizeHandler.Title].Show();
    }

    public IInterfaceAction Show()
    {
        return new MenuAction(Dialog);
    }
}