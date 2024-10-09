using System.Diagnostics;
using Civ2engine;
using Civ2engine.IO;
using Civ2engine.MapObjects;
using Civ2engine.Units;
using Model;
using Model.Menu;
using Raylib_CSharp.Interact;

namespace RaylibUI.RunGame.GameModes.Orders;

public class WaitOrder : Order
{

    public WaitOrder(GameScreen gameScreen): 
        base(gameScreen, new Shortcut(KeyboardKey.W), CommandIds.WaitOrder)
    {
    }

    public override bool Update()
    {
        return SetCommandState(GameScreen.Player.ActiveUnit != null ? CommandStatus.Normal : CommandStatus.Invalid);
    }

    public override void Action()
    {
        Debug.Assert(GameScreen.Player.ActiveUnit != null, "_gameScreen.Player.ActiveUnit != null");
        GameScreen.Player.WaitingList.Add(GameScreen.Player.ActiveUnit);
        GameScreen.Game.ChooseNextUnit();
    }
}