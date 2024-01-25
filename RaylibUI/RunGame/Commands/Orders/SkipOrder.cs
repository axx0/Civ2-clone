using Civ2engine;
using Civ2engine.MapObjects;
using Civ2engine.Units;
using Model;
using Model.Menu;
using Raylib_cs;

namespace RaylibUI.RunGame.GameModes.Orders;

public class SkipOrder : Order
{

    public SkipOrder(GameScreen gameScreen) : 
        base(gameScreen, new Shortcut(KeyboardKey.KEY_SPACE), CommandIds.SkipOrder)
    {
    }

    public override void Update()
    {
        SetCommandState(_gameScreen.Player.ActiveUnit != null ? CommandStatus.Normal : CommandStatus.Invalid);
    }

    public override void Action()
    {
        _gameScreen.Game.ActiveUnit?.SkipTurn();
        _gameScreen.Game.ChooseNextUnit();
    }
}