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

        res.Dialog.TextBoxes = new List<TextBoxDefinition>
        {
            new()
            {
                index = 0, Name = "Anarchy", CharLimit = 23, Description = "Anarchy",
                InitialValue = "Mr.", Width = 300
            },
            new()
            {
                index = 1, Name = "Despotism", CharLimit = 23, Description = "Despotism",
                InitialValue = "Emperor", Width = 300
            },
            new()
            {
                index = 2, Name = "Monarchy", CharLimit = 23, Description = "Monarchy",
                InitialValue = "King", Width = 300
            },
            new()
            {
                index = 3, Name = "Communism", CharLimit = 23, Description = "Communism",
                InitialValue = "Comrade", Width = 300
            },
            new()
            {
                index = 3, Name = "Fundamentalism", CharLimit = 23, Description = "Fundamentalism",
                InitialValue = "High Priest", Width = 300
            },
            new()
            {
                index = 3, Name = "Republic", CharLimit = 23, Description = "Republic",
                InitialValue = "Consul", Width = 300
            },
            new()
            {
                index = 3, Name = "Democracy", CharLimit = 23, Description = "Democracy",
                InitialValue = "President", Width = 300
            }
        };
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