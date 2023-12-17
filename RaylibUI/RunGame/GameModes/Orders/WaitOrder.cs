using Civ2engine;
using Civ2engine.IO;
using Civ2engine.MapObjects;
using Civ2engine.Units;
using Raylib_cs;

namespace RaylibUI.RunGame.GameModes.Orders;

public class WaitOrder : Order
{
    private readonly Game _game;

    public WaitOrder(GameScreen gameScreen, string defaultLabel, Game instance): 
        base(gameScreen, KeyboardKey.KEY_W, defaultLabel, 5)
    {
        _game = instance;
    }

    public override Order Update(Tile activeTile, Unit activeUnit)
    {
        SetCommandState(activeUnit != null ? OrderStatus.Active : OrderStatus.Illegal);
        return this;
    }

    protected override void Execute(LocalPlayer player)
    {
        player.WaitingList.Add(player.ActiveUnit);
        _game.ChooseNextUnit();
    }
}