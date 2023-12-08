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

    public override IInterfaceAction Show(Civ2Interface activeInterface)
    {
        if (Dialog.TextBoxes == null || Dialog.TextBoxes.Count == 0)
        {
            Dialog.TextBoxes = new List<TextBoxDefinition>
            {
                new()
                {
                    index = 0,
                    Name = "Name",
                    InitialValue = Initialization.ConfigObject.PlayerCiv.LeaderName,
                    Width = 400,
                    Description = Dialog.Dialog.Options?[0] ?? string.Empty
                },
            };
            Dialog.Dialog.Options = null;
        }
        else
        {
            Dialog.TextBoxes[0].InitialValue = Initialization.ConfigObject.PlayerCiv.LeaderName;
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
            Initialization.ConfigObject.PlayerCiv.LeaderName = result.TextValues["Name"];
        }

        return civDialogHandlers[SelectCityStyle.Title].Show(civ2Interface);
    }
}