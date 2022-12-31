using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Civ.Dialogs.NewGame;
using Civ.Rules;
using Civ2engine;
using Model;

namespace Civ.Dialogs;

public class MainMenu : ICivDialogHandler
{
    internal const string Title = "MAINMENU";
    public string Name => Title;
    public ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox> popups)
    {
        Dialog = new MenuElements
        {
            Dialog = popups[Name],
            DialogPos = new Point(156, 72)
        };
        return this;
    }

    public MenuElements Dialog { get; private set; }

    public IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers)

    {
        switch (result.SelectedIndex)
        {
            case 0:
                Initialization.ConfigObject.CustomizeWorld = false;
                return Initialization.RuleSets.Count > 1
                    ? civDialogHandlers[SelectGameVersionHandler.Title].Show()
                    : civDialogHandlers[WorldSizeHandler.Title].Show();
            case 1:
                return new FileAction(new OpenFileInfo{ Filters = })
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

    public IInterfaceAction Show()
         {
             return new MenuAction(Dialog);
         }
}