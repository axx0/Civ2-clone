using Civ2.Rules;
using Civ2engine;
using Model;
using Model.Interface;

namespace Civ2.Dialogs.NewGame;

public class CustomTribe2 : BaseDialogHandler
{
    public const string Title = "CUSTOMTRIBE2";

    public CustomTribe2() : base(Title, 0, -0.03)
    {
    }

    public override ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox> popups)
    {
        var res = base.UpdatePopupData(popups);

        res.Dialog.Dialog.Text = new[] { "Anarchy", "Despotism", "Monarchy", "Communism", "Fundamentalism", "Republic", "Democracy" };
        var titles = new string[] { "Mr.", "Emperor", "King", "Comrade", "High Priest", "Consul", "President" };
        res.Dialog.TextBoxes = new List<TextBoxDefinition>();
        for (int i = 0; i < 7; i++)
        {
            res.Dialog.TextBoxes.Add(
               new()
               {
                   index = i,
                   Name = res.Dialog.Dialog.Text[i],
                   CharLimit = 23,
                   InitialValue = titles[i],
                   Width = 300,
               });
        }
        return res;
    }

    public override IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers)
    {
        if (result.SelectedButton == Labels.Cancel)
        {
            return civDialogHandlers[CustomTribe.Title].Show();
        }

        // TODO: update data

        return civDialogHandlers[CustomTribe.Title].Show();
    }
}