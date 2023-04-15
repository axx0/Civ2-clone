using Civ2.Rules;
using Civ2engine;
using Model;

namespace Civ2.Dialogs.NewGame;

public class SelectGender : BaseDialogHandler
{
    public const string Title = "GENDER";

    public SelectGender() : base(Title, 0, -0.03)
    {
    }

    public override IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers)
    {
        if (result.SelectedButton == Labels.Cancel)
        {
            return civDialogHandlers[Difficulty.Title].Show();
        }

        Initialization.ConfigObject.Gender = result.SelectedIndex;

        return civDialogHandlers[SelectTribe.Title].Show();
    }
}