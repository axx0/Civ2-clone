using Civ2engine;
using Civ2engine.MapObjects;
using Civ2engine.Units;
using Model;
using Model.Menu;
using Raylib_cs;

namespace RaylibUI.RunGame.GameModes.Orders;
 
public class SleepOrder : Order
{
    public SleepOrder(GameScreen gameScreen) : 
        base(gameScreen, new Shortcut(KeyboardKey.KEY_S), CommandIds.SleepOrder)
    {
    }

    public override void Update()
    {
        SetCommandState(_gameScreen.Player.ActiveUnit != null ? CommandStatus.Normal : CommandStatus.Invalid);
    }

    public override void Action()
    {
        var game = _gameScreen.Game;
        _gameScreen.Player.ActiveUnit?.Sleep();
        game.ChooseNextUnit();
    }
}