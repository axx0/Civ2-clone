using Civ2.Dialogs.NewGame.CustomWorldDialogs;
using Civ2.Rules;
using Civ2engine;
using Model.Dialog;
using Model.Interface;
using Model.InterfaceActions;

namespace Civ2.Dialogs.NewGame;

public class CustomWorldSize : BaseDialogHandler
{
    public static string Title = "CUSTOMSIZE"; 
    public CustomWorldSize() : base(Title, 0, -0.03)
    {
    }

    public override ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox?> popups)
    {
        var res = base.UpdatePopupData(popups);
        if(!res.Dialog.Dialog.Button.Contains(Labels.Cancel))
        {
            res.Dialog.Dialog.Button.Add(Labels.Cancel);
        }
        res.Dialog.TextBoxes = new List<TextBoxDefinition>
        {
            new()
            {
                Index = 0, Name = "Width", MinValue = 20, CharLimit = 5,
                InitialValue = Initialization.ConfigObject.WorldSize[0].ToString(), Width = 75
            },
            new()
            {
                Index = 1, Name = "Height", MinValue = 20, CharLimit = 5,
                InitialValue = Initialization.ConfigObject.WorldSize[1].ToString(), Width = 75
            }
        };
        if (res.Dialog.Dialog.Options is not null)
        {
            res.Dialog.TextBoxes[0].Description = res.Dialog.Dialog.Options[0];
            res.Dialog.TextBoxes[1].Description = res.Dialog.Dialog.Options[1];
            res.Dialog.Dialog.Options = null;
        }
        return res;
    }

    public override IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers, Civ2Interface civ2Interface)
    {
        if (result.SelectedButton == Labels.Cancel)
        {
            return civDialogHandlers[WorldSizeHandler.Title].Show(civ2Interface);
        }

        if (int.TryParse(result.TextValues["Width"], out var width))
        {
            Initialization.ConfigObject.WorldSize[0] = width;
        }

        if (int.TryParse(result.TextValues["Height"], out var height))
        {
            Initialization.ConfigObject.WorldSize[1] = height;
        }

        return civDialogHandlers[
                         Initialization.ConfigObject.CustomizeWorld ? CustomisePercentageLand.Title : DifficultyHandler.Title].Show(civ2Interface);
    }
}