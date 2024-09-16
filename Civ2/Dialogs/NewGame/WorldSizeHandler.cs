using Civ2.Dialogs.NewGame.CustomWorldDialogs;
using Civ2.Rules;
using Civ2engine;
using Model.Dialog;
using Model.InterfaceActions;

namespace Civ2.Dialogs.NewGame;

public class WorldSizeHandler : BaseDialogHandler
{
    public const string Title = "SIZEOFMAP";

    public WorldSizeHandler() : base(Title, 0, -0.03)
    {
    }

    public override IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers, Civ2Interface civ2Interface)
    {
        if (result.SelectedButton == Labels.Cancel)
        {
            return civDialogHandlers[MainMenu.Title].Show(civ2Interface);
        }

        Initialization.ConfigObject.WorldSize = result.SelectedIndex switch
        {
            1 => new[] { 50, 80 },
            2 => new[] { 75, 120 },
            _ => new[] { 40, 50 }
        };

        if (result.SelectedButton == "Custom")
        {
            return civDialogHandlers[CustomWorldSize.Title].Show(civ2Interface);
        }

        return civDialogHandlers[
            Initialization.ConfigObject.CustomizeWorld ? CustomisePercentageLand.Title : DifficultyHandler.Title].Show(civ2Interface);
    }
}