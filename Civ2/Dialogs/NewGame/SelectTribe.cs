using Civ2.Rules;
using Civ2engine;
using Model;
using Model.InterfaceActions;

namespace Civ2.Dialogs.NewGame;

public class SelectTribe : BaseDialogHandler
{
    public const string Title = "TRIBE";

    public SelectTribe() : base(Title, 0, -0.03)
    {
    }

    public override ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox> popups)
    {
        var res = base.UpdatePopupData(popups);

        if (!res.Dialog.Dialog.Button.Contains(Labels.Cancel))
        {
            res.Dialog.Dialog.Button.Add(Labels.Cancel);
        }

        //TODO: load from rules
        res.Dialog.Dialog.Options = new[]
        {
            "Romans", "Babylonians", "Germans", "Egyptians", "Americans", "Greeks", "Indians",
            "Russians", "Zulus", "French", "Aztecs", "Chinese", "English", "Mongols",
            "Celts", "Japanese", "Vikings", "Spanish", "Persians", "Carthaginians", "Sioux",
        };
        res.Dialog.OptionsCols = 3;
        return res;
    }

    public override IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers)
    {
        if (result.SelectedButton == Labels.Cancel)
        {
            return civDialogHandlers[SelectGender.Title].Show();
        }
        else if (result.SelectedButton == Labels.Custom)
        {
            return civDialogHandlers[CustomTribe.Title].Show();
        }

        // TODO: make civilizations

        return civDialogHandlers[SelectCityStyle.Title].Show();
    }
}