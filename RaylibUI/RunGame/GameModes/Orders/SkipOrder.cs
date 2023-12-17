using Civ2engine;
using Civ2engine.MapObjects;
using Civ2engine.Units;
using Raylib_cs;

namespace RaylibUI.RunGame.GameModes.Orders;

public class SkipOrder : Order
{
    private readonly Game _game;

    public SkipOrder(GameScreen gameScreen, string defaultLabel, Game game) : 
        base(gameScreen, KeyboardKey.KEY_SPACE, defaultLabel, 6)
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
        _gameScreen.Game.ActiveUnit?.SkipTurn();
        //player.ActiveUnit.MovePointsLost += player.ActiveUnit.MovePoints;
        _game.ChooseNextUnit();
    }
}