using Civ2;
using Civ2.Dialogs;
using Civ2.Dialogs.NewGame;
using Civ2.Rules;
using Model;
using Model.InterfaceActions;

namespace TOT.Dialogs;

public class StartMenuHandler : BaseDialogHandler
{
    
    internal const string Title = "STARTMENU";
    public StartMenuHandler() : base(Title)
    {
    }

    public override IInterfaceAction HandleDialogResult(DialogResult result, Dictionary<string, ICivDialogHandler> civDialogHandlers, Civ2Interface civ2Interface)
    {   if (result.SelectedButton == Dialog.Dialog.Button[1])
        {
            return ExitAction.Exit;
        }

        switch (result.SelectedIndex)
        {
            case 0:
            case 1:
                if (Initialization.RuleSets.Count > 1)
                    return civDialogHandlers[SelectGameVersionHandler.Title].Show(civ2Interface);
                return civDialogHandlers[MainMenu.Title].Show(civ2Interface);
        }

        return new MenuAction(Dialog);
    }
}