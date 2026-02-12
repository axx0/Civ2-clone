using JetBrains.Annotations;
using Model.Controls;
using Model.Input;
using RaylibUI.RunGame.GameControls;

namespace RaylibUI.RunGame.Commands;

[UsedImplicitly]
public class CityStatus(GameScreen gameScreen)
    : AlwaysOnCommand(gameScreen, CommandIds.CityStatus, [new Shortcut(Key.F1)])
{
    public override void Action()
    {
        // ReSharper disable once StringLiteralTypo
        GameScreen.ShowDialog(new CityStatusWindow(gameScreen));
    }
}