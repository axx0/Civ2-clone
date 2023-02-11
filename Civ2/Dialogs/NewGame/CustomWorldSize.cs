using Civ.Rules;
using Civ2engine;
using Model;
using Model.Interface;

namespace Civ.Dialogs.NewGame;

public class CustomWorldSize : BaseDialogHandler
{
    public static string Title = "CUSTOMSIZE"; 
    public CustomWorldSize() : base(Title)
    {
    }

    public override ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox> popups)
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
                index = 0, Name = "Width", MinValue = 20,
                InitialValue = Initialization.ConfigObject.WorldSize[0].ToString(), Width = 75
            },
            new()
            {
                index = 1, Name = "Height", MinValue = 20,
                InitialValue = Initialization.ConfigObject.WorldSize[1].ToString(), Width = 75
            }
        };
        return res;
    }

    public override IInterfaceAction HandleDialogResult(DialogResult result, Dictionary<string, ICivDialogHandler> civDialogHandlers)
    {
        if (result.SelectedButton == Labels.Cancel)
        {
            return civDialogHandlers[WorldSizeHandler.Title].Show();
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
                         Initialization.ConfigObject.CustomizeWorld ? CustomisePercentageLand.Title : Difficulty.Title].Show();
    }
}