using System.Diagnostics;
using Civ2engine;
using Civ2engine.IO;
using Civ2engine.MapObjects;
using Civ2engine.Units;
using Model;
using Model.Menu;
using Raylib_cs;

namespace RaylibUI.RunGame.GameModes.Orders;

public class WaitOrder : Order
{

    public WaitOrder(GameScreen gameScreen): 
        base(gameScreen, new Shortcut(KeyboardKey.W), CommandIds.WaitOrder)
    {
    }

    public override void Update()
    {
        SetCommandState(_gameScreen.Player.ActiveUnit != null ? CommandStatus.Normal : CommandStatus.Invalid);
    }

    public override void Action()
    {
        Debug.Assert(_gameScreen.Player.ActiveUnit != null, "_gameScreen.Player.ActiveUnit != null");
        _gameScreen.Player.WaitingList.Add(_gameScreen.Player.ActiveUnit);
        _gameScreen.Game.ChooseNextUnit();
    }
}