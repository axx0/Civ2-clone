using Civ2.Rules;
using Civ2engine;
using Model.Dialog;
using Model.Interface;
using Model.InterfaceActions;
using RaylibUtils;

namespace Civ2.Dialogs.NewGame;

public class SelectCityStyle : BaseDialogHandler
{
    public const string Title = "CUSTOMCITY";

    public SelectCityStyle() : base(Title, -0.085, -0.03)
    {
    }

    public override ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox?> popups)
    {
        var res = base.UpdatePopupData(popups);

        if (!res.Dialog.Dialog.Button.Contains(Labels.Cancel))
        {
            res.Dialog.Dialog.Button.Add(Labels.Cancel);
        }
        return res;
    }

    public override IInterfaceAction Show(Civ2Interface activeInterface)
    {
        Dialog.OptionsIcons = activeInterface.CityImages.Sets.Take(4).Select(i => i.Skip(6).First().Image).ToArray();
        Dialog.Dialog.Default = Initialization.ConfigObject.PlayerCiv.CityStyle;
        Dialog.Dialog.Options ??= Labels.Items[247..251];
        return base.Show(activeInterface);
    }

    public override IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers, Civ2Interface civ2Interface)
    {
        if (result.SelectedButton == Labels.Cancel)
        {
            return civDialogHandlers[SelectGender.Title].Show(civ2Interface);
        }
        
        Initialization.ConfigObject.PlayerCiv.CityStyle = result.SelectedIndex;

        Initialization.CompleteConfig();

        if (Initialization.ConfigObject.SelectComputerOpponents && Initialization.ConfigObject.Civilizations.Count <
            Initialization.ConfigObject.NumberOfCivs)
        {
            return civDialogHandlers[SelectOpponent.Title].Show(civ2Interface);
        }

        return civDialogHandlers[Init.Title].Show(civ2Interface);
    }
}