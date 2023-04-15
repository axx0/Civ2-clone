using Civ2.Rules;
using Civ2engine;
using Model;
using Model.Interface;

namespace Civ2.Dialogs.NewGame;

public class SelectCityStyle : BaseDialogHandler
{
    public const string Title = "CUSTOMCITY";

    public SelectCityStyle() : base(Title, -0.085, -0.03)
    {
    }

    public override ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox> popups)
    {
        var res = base.UpdatePopupData(popups);

        if (!res.Dialog.Dialog.Button.Contains(Labels.Cancel))
        {
            res.Dialog.Dialog.Button.Add(Labels.Cancel);
        }

        res.Dialog.Dialog.Options ??= Labels.Items[247..251];

        //var citiesDialog = new Civ2dialog(mainForm, citiesPopup,
//                 icons: new[]
//                 {
//                     MapImages.Cities[7].Bitmap,
//                     MapImages.Cities[15].Bitmap,
//                     MapImages.Cities[23].Bitmap,
//                     MapImages.Cities[31].Bitmap
//                 }) { SelectedIndex = (int)config.PlayerCiv.CityStyle };
        return res;
    }

    public override IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers)
    {
        if (result.SelectedButton == Labels.Cancel)
        {
            return civDialogHandlers[SelectGender.Title].Show();
        }

        return civDialogHandlers[MainMenu.Title].Show();
    }
}