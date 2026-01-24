using System.Diagnostics;
using Civ2engine.Enums;
using JetBrains.Annotations;
using Model;
using Model.Input;
using Model.Menu;

namespace RaylibUI.RunGame.Commands.Orders;

[UsedImplicitly]
public class UnloadOrder(GameScreen gameScreen) : Order(gameScreen, new Shortcut(Key.U), CommandIds.UnloadOrder)
{
    public override bool Update()
    {
        var activeUnit = GameScreen.Player.ActiveUnit;
        if (activeUnit == null)
        {
            return SetCommandState(CommandStatus.Invalid);
        }

        return SetCommandState(activeUnit.CarriedUnits.Count > 0 && activeUnit.CarriedUnits.Any(u => u.MovePoints > 0)
            ? CommandStatus.Normal
            : CommandStatus.Disabled);

    }

    public override void Action()
    {
        var player = GameScreen.Player;
        Debug.Assert(player.ActiveUnit != null);
        player.ActiveUnit.CarriedUnits.ForEach(u =>
        {
            u.Order = (int)OrderType.NoOrders;
            u.InShip = null;
        });
        var next = player.ActiveUnit.CarriedUnits.FirstOrDefault(u=>u.AwaitingOrders);
        player.ActiveUnit.CarriedUnits.Clear();
        player.ActiveUnit = next;
    }
}