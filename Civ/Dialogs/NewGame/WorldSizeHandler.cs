using System.Drawing;
using Civ.Rules;
using Civ2engine;
using Model;

namespace Civ.Dialogs.NewGame;

public class WorldSizeHandler : ICivDialogHandler
{
    public const string Title = "SIZEOFMAP";
    public string Name => Title;
    public ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox> popup)
    {
        Dialog = new MenuElements
        {
            Dialog = popup[Title],
            DialogPos = new Point(0, 75)
        };
        return this;
    }

    public MenuElements Dialog { get; set; }

    public IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers)
    {
        if (result.SelectedIndex == int.MinValue)
        {
            return civDialogHandlers[MainMenu.Title].Show();
        }

        Initialization.ConfigObject.WorldSize = result.SelectedIndex switch
        {
            1 => new[] { 50, 80 },
            2 => new[] { 75, 120 },
            _ => new[] { 40, 50 }
        };

        if (result.SelectedButton != "Custom")
        {
            //TODO: custom ise world 
        }

        return civDialogHandlers[MainMenu.Title].Show();
    }

    public IInterfaceAction Show()
    {
        return new MenuAction(Dialog);
    }
}