using Civ2.Rules;
using Civ2engine;
using Model.Dialog;
using Model.InterfaceActions;

namespace Civ2.Dialogs.NewGame;

// ReSharper disable once ClassNeverInstantiated.Global
public class SelectGameVersionHandler : BaseDialogHandler
{
    public const string Title = "AXX-Select-Game";

    public SelectGameVersionHandler() : base(Title) {}
    public override ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox?> popups)
    {
        popups[Name] = new PopupBox
        {
            Name = Title,
            Title = "Select game version", 
            Button = new List<string> {"Quick Start", "OK", Labels.Cancel}
        };
        return base.UpdatePopupData(popups);
    }

    public override IInterfaceAction Show(Civ2Interface activeInterface)
    {
        Dialog.Dialog.Options = activeInterface.MainApp.AllRuleSets.Select(r => r.Name).ToList();
        return base.Show(activeInterface);
    }

    public override IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers, Civ2Interface civ2Interface)
    {                    
        if (result.SelectedButton == Labels.Cancel)
        {
            return civDialogHandlers[MainMenu.Title].Show(civ2Interface);
        }

        var activeInterface = civ2Interface.MainApp.SetActiveRuleSet(result.SelectedIndex);

        return activeInterface.InitNewGame(result.SelectedButton == "Quick Start");
    }
}