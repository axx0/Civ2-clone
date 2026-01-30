using JetBrains.Annotations;
using Model;
using Model.Controls;
using Model.Input;

namespace RaylibUI.RunGame.Commands.Orders;

[UsedImplicitly]
public class SkipOrder(GameScreen gameScreen) : Order(gameScreen, new Shortcut(Key.Space), CommandIds.SkipOrder)
{
    public override bool Update()
    {
        return SetCommandState(GameScreen.Player.ActiveUnit != null ? CommandStatus.Normal : CommandStatus.Invalid);
    }

    public override void Action()
    {
        GameScreen.Game.ActivePlayer.ActiveUnit?.SkipTurn();
        GameScreen.Game.ChooseNextUnit();
    }
}