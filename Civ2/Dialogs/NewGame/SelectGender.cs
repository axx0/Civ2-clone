using Civ2.Rules;
using Civ2engine;
using Model;
using Model.InterfaceActions;

namespace Civ2.Dialogs.NewGame;

public class SelectGender : BaseDialogHandler
{
    public const string Title = "GENDER";

    public SelectGender() : base(Title, 0, -0.03)
    {
    }

    public override IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers, Civ2Interface civ2Interface)
    {
        if (result.SelectedButton == Labels.Cancel)
        {
            return civDialogHandlers[Difficulty.Title].Show(civ2Interface);
        }

        Initialization.ConfigObject.Gender = result.SelectedIndex;

        return civDialogHandlers[SelectTribe.Title].Show(civ2Interface);
    }
}