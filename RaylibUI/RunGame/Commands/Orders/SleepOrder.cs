using JetBrains.Annotations;
using Model;
using Model.Controls;
using Model.Input;

namespace RaylibUI.RunGame.Commands.Orders;
 
[UsedImplicitly]
public class SleepOrder(GameScreen gameScreen) : Order(gameScreen, new Shortcut(Key.S), CommandIds.SleepOrder)
{
    public override bool Update()
    {
        return SetCommandState(GameScreen.Player.ActiveUnit != null ? CommandStatus.Normal : CommandStatus.Invalid);
    }

    public override void Action()
    {
        var game = GameScreen.Game;
        GameScreen.Player.ActiveUnit?.Sleep();
        game.ChooseNextUnit();
    }
}