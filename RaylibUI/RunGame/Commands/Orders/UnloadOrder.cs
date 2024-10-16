using System.Diagnostics;
using System.Linq;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.Units;
using Model;
using Model.Menu;
using Raylib_CSharp.Interact;

namespace RaylibUI.RunGame.GameModes.Orders;

public class UnloadOrder : Order
{

    public UnloadOrder(GameScreen gameScreen) : 
        base(gameScreen, new Shortcut(KeyboardKey.U), CommandIds.UnloadOrder)
    {
    }

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
        Debug.Assert(player.ActiveUnit != null, "player.ActiveUnit != null");
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