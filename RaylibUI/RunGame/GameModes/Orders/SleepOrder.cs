using Civ2engine;
using Civ2engine.MapObjects;
using Civ2engine.Units;
using Raylib_cs;

namespace RaylibUI.RunGame.GameModes.Orders;
 
public class SleepOrder : Order
{
    private readonly Game _game;

    public SleepOrder(GameScreen gameScreen, string defaultLabel, Game game) : base(gameScreen, KeyboardKey.KEY_S, defaultLabel, 5)
    {
        _game = game;
    }

    public override Order Update(Tile activeTile, Unit activeUnit)
    {
        SetCommandState(activeUnit != null ? OrderStatus.Active : OrderStatus.Illegal);
        return this;
    }

    protected override void Execute(LocalPlayer player)
    {
        _game.ActiveUnit?.Sleep();
        _game.ChooseNextUnit();
    }
}