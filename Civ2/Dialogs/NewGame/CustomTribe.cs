using Civ2.Rules;
using Civ2engine;
using Model.Dialog;
using Model.Interface;
using Model.InterfaceActions;

namespace Civ2.Dialogs.NewGame;

public class CustomTribe : BaseDialogHandler
{
    public const string Title = "CUSTOMTRIBE";

    public CustomTribe() : base(Title, 0, -0.03)
    {
    }

    public override IInterfaceAction Show(Civ2Interface activeInterface)
    {
        if (Dialog.TextBoxes == null || Dialog.TextBoxes.Count == 0)
        {
            Dialog.TextBoxes = new List<TextBoxDefinition>
            {
                new()
                {
                    Index = 0, Name = "Leader", CharLimit = 23,
                    InitialValue = Initialization.ConfigObject.PlayerCiv.LeaderName, Width = 300
                },
                new()
                {
                    Index = 1, Name = "Tribe", CharLimit = 23,
                    InitialValue = Initialization.ConfigObject.PlayerCiv.TribeName, Width = 300
                },
                new()
                {
                    Index = 2, Name = "Adjective", CharLimit = 23,
                    InitialValue = Initialization.ConfigObject.PlayerCiv.Adjective, Width = 300
                }
            };
             
            if (Dialog.Dialog.Options is not null)
            {
                Dialog.TextBoxes[0].Description = Dialog.Dialog.Options[0];
                Dialog.TextBoxes[1].Description = Dialog.Dialog.Options[1];
                Dialog.TextBoxes[2].Description = Dialog.Dialog.Options[2];
                Dialog.Dialog.Options = null;
            }
        }
        else
        {
            Dialog.TextBoxes[0].InitialValue = Initialization.ConfigObject.PlayerCiv.LeaderName;
            Dialog.TextBoxes[1].InitialValue = Initialization.ConfigObject.PlayerCiv.TribeName;
            Dialog.TextBoxes[2].InitialValue = Initialization.ConfigObject.PlayerCiv.Adjective;
        }

        return base.Show(activeInterface);
    }

    public override IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers, Civ2Interface civ2Interface)
    {
        if (result.SelectedButton == Labels.Cancel)
        {
            return civDialogHandlers[SelectGender.Title].Show(civ2Interface);
        }

        if (result.TextValues != null)
        {
            Initialization.ConfigObject.PlayerCiv.LeaderName = result.TextValues["Leader"];
            Initialization.ConfigObject.PlayerCiv.TribeName = result.TextValues["Tribe"];
            Initialization.ConfigObject.PlayerCiv.Adjective = result.TextValues["Adjective"];
        }

        if (result.SelectedButton == "Titles")
        {
            return civDialogHandlers[CustomTribe2.Title].Show(civ2Interface);
        }

        return civDialogHandlers[SelectCityStyle.Title].Show(civ2Interface);
    }
}