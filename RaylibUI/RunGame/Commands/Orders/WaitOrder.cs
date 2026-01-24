using System.Diagnostics;
using JetBrains.Annotations;
using Model;
using Model.Input;
using Model.Menu;

namespace RaylibUI.RunGame.Commands.Orders;

[UsedImplicitly]
public class WaitOrder(GameScreen gameScreen) : Order(gameScreen, new Shortcut(Key.W), CommandIds.WaitOrder)
{
    public override bool Update()
    {
        return SetCommandState(GameScreen.Player.ActiveUnit != null ? CommandStatus.Normal : CommandStatus.Invalid);
    }

    public override void Action()
    {
        Debug.Assert(GameScreen.Player.ActiveUnit != null, "_gameScreen.Player.ActiveUnit != null");
        GameScreen.Player.WaitingList.Add(GameScreen.Player.ActiveUnit);
        GameScreen.Game.ChooseNextUnit();
    }
}