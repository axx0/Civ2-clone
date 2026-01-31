using Model.Input;
using Civ2engine;
using Model.Controls;
using JetBrains.Annotations;

namespace RaylibUI.RunGame.Commands;

[UsedImplicitly]
public class Quit(GameScreen gameScreen)
    : AlwaysOnCommand(gameScreen, CommandIds.QuitGame, [new Shortcut(Key.Q, ctrl: true)])
{
    public override void Action()
    {
        // ReSharper disable once StringLiteralTypo
        GameScreen.ShowPopup("REALLYQUIT", DialogClick,
            dialogImage: new([GameScreen.Main.ActiveInterface.PicSources["backgroundImageSmall2"][0]]));
    }

    private void DialogClick(string button, int option, IList<bool>? _, IDictionary<string, string>? _2)
    {
        if (button == Labels.Ok && option == 1)
        {
            GameScreen.Main.ReloadMain();
        }
    }
}