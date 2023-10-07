using Civ2.Rules;
using Civ2engine;
using Model;
using Model.Interface;
using Model.InterfaceActions;

namespace Civ2.Dialogs.NewGame;

public class EnterName : BaseDialogHandler
{
    public const string Title = "NAME";

    public EnterName() : base(Title, 0, -0.03)
    {
    }

    public override ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox> popups)
    {
        var res = base.UpdatePopupData(popups);
        res.Dialog.TextBoxes = new List<TextBoxDefinition>
        {
            new()
            {
                index = 0, Name = "Name",
                // TODO: get player civ from config
                //InitialValue = Initialization.ConfigObject.PlayerCiv.LeaderName, Width = 75
                InitialValue = "Shaka", Width = 400
            },
        };
        return res;
    }

    public override IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers, Civ2Interface civ2Interface)
    {
        if (result.SelectedButton == Labels.Cancel)
        {
            return civDialogHandlers[SelectGender.Title].Show(civ2Interface);
        }

        return civDialogHandlers[SelectCityStyle.Title].Show(civ2Interface);
    }
}