using Model.Input;
using Civ2engine.IO;
using Model.Core;
using Model.Controls;

namespace RaylibUI.RunGame.Commands.Cheat;

public class SetGameYear(GameScreen gameScreen) : AlwaysOnCommand(gameScreen, CommandIds.CheatSetGameYear, [new Shortcut(Key.F4, shift: true)])
{
    private CivDialog? _placeholderDialog;

    public override void Action()
    {
        _placeholderDialog = new CivDialog(GameScreen.Main, new DialogElements(new PopupBox
        {
            Title = GetType().Name,
            Text = [$"This is a placeholder for {Command?.GameCommand?.Id}, which is not yet implemented."],
            LineStyles = [TextStyles.Left],
            Button = [Labels.Ok]
        }), PlaceholderHandler);

        GameScreen.ShowDialog(_placeholderDialog);
    }

    private void PlaceholderHandler(string button, int selection, IList<bool>? arg3, IDictionary<string, string>? arg4)
    {
        GameScreen.CloseDialog(_placeholderDialog);
    }
}