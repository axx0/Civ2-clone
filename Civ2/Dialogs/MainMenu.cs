using Civ2.Dialogs.FileDialogs;
using Civ2.Dialogs.NewGame;
using Civ2.Rules;
using Civ2engine;
using Model;
using Model.InterfaceActions;

namespace Civ2.Dialogs;

public class MainMenu : BaseDialogHandler
{
    internal const string Title = "MAINMENU";
    public MainMenu() : base(Title, -0.08, -0.07) { }

    public override IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers)

    {
        if (result.SelectedButton == Dialog.Dialog.Button[1])
        {
            return ExitAction.Exit;
        }
        switch (result.SelectedIndex)
        {
            case 0:
            case 2:
                Initialization.ConfigObject.CustomizeWorld = result.SelectedIndex == 2;
                return Initialization.RuleSets.Count > 1
                    ? civDialogHandlers[SelectGameVersionHandler.Title].Show()
                    : civDialogHandlers[WorldSizeHandler.Title].Show();
            
            
             case 1:
                 return civDialogHandlers[LoadMap.DialogTitle].Show();
            case 3:
                return civDialogHandlers[LoadScenario.DialogTitle].Show();
            case 4:
                return civDialogHandlers[LoadGame.DialogTitle].Show();
        }
        /*var mainMenuDialog = new Civ2dialog(this, popupBoxList["MAINMENU"]);
                   mainMenuDialog.Location = new Point((int)(Screen.PrimaryScreen.Bounds.Width - mainMenuDialog.Width - 156),
                                                       (int)(Screen.PrimaryScreen.Bounds.Height - mainMenuDialog.Height - 72));
                   mainMenuDialog.ShowModal(this);
       
                   switch (mainMenuDialog.SelectedIndex)
                   {
                       //New Game
                       case 0:
                           {
                               NewGame.Start(this, false);
                               break;
                           }
       
                       // Start premade
                       case 1:
                           {
                               LocateStartingFiles("Select Map To Load",
                                   new FileFilter("Save Files (*.mp)", ".mp"), StartPremadeInit);
                               break;
                           }
       
                       //Customise World
                       case 2:
                           {
                               NewGame.Start(this, true);
                               break;
                           }
       
                       // Load scenario
                       case 3:
                           {
                               LocateStartingFiles("Select Scenario To Load",
                                   new FileFilter("Save Files (*.scn)", ".scn"), LoadScenarioInit);
                               break;
                           }
       
                       // Load game
                       case 4:
                           {
                               LocateStartingFiles("Select Game To Load", new FileFilter("Save Files (*.sav)", ".SAV"),
                                   LoadGameInitialization
                               );
                               break;
                           }
                   }break*/
        
        return new MenuAction(Dialog);
    }
}