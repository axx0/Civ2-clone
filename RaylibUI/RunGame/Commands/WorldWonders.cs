using JetBrains.Annotations;
using Model.Controls;
using Model.Input;
using RaylibUI.RunGame.GameControls;

namespace RaylibUI.RunGame.Commands;

[UsedImplicitly]
public class WorldWonders(GameScreen gameScreen)
    : AlwaysOnCommand(gameScreen, CommandIds.WorldWonders, [new Shortcut(Key.F7)])
{
    public override void Action()
    {
        // ReSharper disable once StringLiteralTypo
        GameScreen.ShowDialog(new WondersWindow(gameScreen));
    }
}