using Civ2.Rules;
using Civ2engine;
using Model;
using Model.Interface;

namespace Civ2.Dialogs.NewGame;

public class EnerName : BaseDialogHandler
{
    public const string Title = "NAME";

    public EnerName() : base(Title, 0, -0.03)
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
        Dictionary<string, ICivDialogHandler> civDialogHandlers)
    {
        if (result.SelectedButton == Labels.Cancel)
        {
            return civDialogHandlers[SelectGender.Title].Show();
        }

        return civDialogHandlers[SelectCityStyle.Title].Show();
    }
}