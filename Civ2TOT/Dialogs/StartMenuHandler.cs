using Civ2;
using Civ2.Dialogs;
using Civ2.Dialogs.NewGame;
using Civ2.Rules;
using Model.Dialog;
using Model.InterfaceActions;

namespace TOT.Dialogs;

public class StartMenuHandler : BaseDialogHandler
{
    
    internal const string Title = "STARTMENU";
    public StartMenuHandler() : base(Title)
    {
    }

    public override IInterfaceAction HandleDialogResult(DialogResult result, Dictionary<string, ICivDialogHandler> civDialogHandlers, Civ2Interface civ2Interface)
    {   
        if (result.SelectedButton == Dialog.Dialog.Button[1])
        {
            return ExitAction.Exit;
        }

        switch (result.SelectedIndex)
        {
            case 0:
                return civDialogHandlers[MainMenu.Title].Show(civ2Interface);
            default:
                //TODO: Additional cases
                break;
        }

        return new MenuAction(Dialog);
    }
}