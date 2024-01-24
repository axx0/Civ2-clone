using System.Diagnostics;
using System.Linq;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.Units;
using Model;
using Model.Menu;
using Raylib_cs;

namespace RaylibUI.RunGame.GameModes.Orders;

public class UnloadOrder : Order
{

    public UnloadOrder(GameScreen gameScreen) : 
        base(gameScreen, new Shortcut(KeyboardKey.KEY_U), CommandIds.UnloadOrder)
    {
    }

    public override void Update()
    {
        var activeUnit = _gameScreen.Player.ActiveUnit;
        if (activeUnit == null)
        {
            SetCommandState(CommandStatus.Invalid);
        }
        else
        {
            SetCommandState(activeUnit.CarriedUnits.Count > 0 && activeUnit.CarriedUnits.Any(u => u.MovePoints > 0)
                ? CommandStatus.Normal
                : CommandStatus.Disabled);
        }
    }

    public override void Action()
    {
        var player = _gameScreen.Player;
        Debug.Assert(player.ActiveUnit != null, "player.ActiveUnit != null");
        player.ActiveUnit.CarriedUnits.ForEach(u =>
        {
            u.Order = OrderType.NoOrders;
            u.InShip = null;
        });
        var next = player.ActiveUnit.CarriedUnits.FirstOrDefault(u=>u.AwaitingOrders);
        player.ActiveUnit.CarriedUnits.Clear();
        player.ActiveUnit = next;
    }
}