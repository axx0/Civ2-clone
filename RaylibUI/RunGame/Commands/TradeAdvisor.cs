using JetBrains.Annotations;
using Model.Controls;
using Model.Input;
using RaylibUI.RunGame.GameControls;

namespace RaylibUI.RunGame.Commands;

[UsedImplicitly]
public class TradeAdvisor(GameScreen gameScreen)
    : AlwaysOnCommand(gameScreen, CommandIds.TradeAdvisor, [new Shortcut(Key.F5)])
{
    public override void Action()
    {
        // ReSharper disable once StringLiteralTypo
        GameScreen.ShowDialog(new TradeAdvisorWindow(gameScreen));
    }
}