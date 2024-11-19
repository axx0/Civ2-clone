using Civ2engine;
using Model.Images;
using Model.Menu;
using Raylib_CSharp.Interact;

namespace RaylibUI.RunGame.Commands;

public class Quit : AlwaysOnCommand
{
    public Quit(GameScreen gameScreen) : base(gameScreen, CommandIds.QuitGame, [new Shortcut(KeyboardKey.Q, ctrl: true)])
    {
    }

    public override void Action()
    {
        // ReSharper disable once StringLiteralTypo
        GameScreen.ShowPopup("REALLYQUIT", DialogClick, dialogImage: new(new[] { GameScreen.Main.ActiveInterface.PicSources["backgroundImageSmall2"][0] }));
    }

    private void DialogClick(string button, int option, IList<bool>? _, IDictionary<string, string>? _2)
    {
        if (button == Labels.Ok && option == 1)
        {
            GameScreen.Main.ReloadMain();
        }
    }
}