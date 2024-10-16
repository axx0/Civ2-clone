using Civ2engine;
using Civ2engine.MapObjects;
using Civ2engine.Units;
using Model;
using Model.Menu;
using Raylib_CSharp.Interact;

namespace RaylibUI.RunGame.GameModes.Orders;

public class SkipOrder : Order
{

    public SkipOrder(GameScreen gameScreen) : 
        base(gameScreen, new Shortcut(KeyboardKey.Space), CommandIds.SkipOrder)
    {
    }

    public override bool Update()
    {
        return SetCommandState(GameScreen.Player.ActiveUnit != null ? CommandStatus.Normal : CommandStatus.Invalid);
    }

    public override void Action()
    {
        GameScreen.Game.ActivePlayer.ActiveUnit?.SkipTurn();
        GameScreen.Game.ChooseNextUnit();
    }
}