using Civ2engine;
using Civ2engine.MapObjects;
using Civ2engine.Units;
using Model;
using Model.Menu;
using Raylib_CSharp.Interact;

namespace RaylibUI.RunGame.GameModes.Orders;
 
public class SleepOrder : Order
{
    public SleepOrder(GameScreen gameScreen) : 
        base(gameScreen, new Shortcut(KeyboardKey.S), CommandIds.SleepOrder)
    {
    }

    public override bool Update()
    {
        return SetCommandState(GameScreen.Player.ActiveUnit != null ? CommandStatus.Normal : CommandStatus.Invalid);
    }

    public override void Action()
    {
        var game = GameScreen.Game;
        GameScreen.Player.ActiveUnit?.Sleep();
        game.ChooseNextUnit();
    }
}