using Civ2.Rules;
using Civ2engine;
using Model;
using Model.Interface;

namespace Civ2.Dialogs.NewGame;

public class CustomTribe : BaseDialogHandler
{
    public const string Title = "CUSTOMTRIBE";

    public CustomTribe() : base(Title, 0, -0.03)
    {
    }

    public override ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox> popups)
    {
        var res = base.UpdatePopupData(popups);

        res.Dialog.TextBoxes = new List<TextBoxDefinition>
        {
            new()
            {
                index = 0, Name = "Leader", CharLimit = 23,
                InitialValue = "Montezuma", Width = 300
            },
            new()
            {
                index = 1, Name = "Tribe", CharLimit = 23,
                InitialValue = "Aztecs", Width = 300
            },
            new()
            {
                index = 2, Name = "Adjective", CharLimit = 23,
                InitialValue = "Aztec", Width = 300
            }
        };
        if (res.Dialog.Dialog.Options is not null)
        {
            res.Dialog.TextBoxes[0].Description = res.Dialog.Dialog.Options[0];
            res.Dialog.TextBoxes[1].Description = res.Dialog.Dialog.Options[1];
            res.Dialog.TextBoxes[2].Description = res.Dialog.Dialog.Options[2];
            res.Dialog.Dialog.Options = null;
        }
        return res;
    }

    public override IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers)
    {
        if (result.SelectedButton == Labels.Cancel)
        {
            return civDialogHandlers[SelectGender.Title].Show();
        }

        //Initialization.ConfigObject.PlayerCiv.LeaderName = result.TextValues["Leader"];
        //Initialization.ConfigObject.PlayerCiv.TribeName = result.TextValues["Tribe"];
        //Initialization.ConfigObject.PlayerCiv.Adjective = result.TextValues["Adjective"];

        if (result.SelectedButton == "Titles")
        {
            return civDialogHandlers[CustomTribe2.Title].Show();
        }

        return civDialogHandlers[SelectCityStyle.Title].Show();
    }
}