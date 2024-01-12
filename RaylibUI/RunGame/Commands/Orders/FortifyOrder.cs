using System.Diagnostics;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.UnitActions;
using Civ2engine.Units;
using Model;
using Model.Menu;
using Raylib_cs;

namespace RaylibUI.RunGame.GameModes.Orders;

public class FortifyOrder : Order
{
    private readonly Game _game;
    private readonly LocalPlayer _player;

    public FortifyOrder(GameScreen gameScreen) : 
        base(gameScreen, new Shortcut( KeyboardKey.KEY_F), CommandIds.FortifyOrder)
    {
        _game = gameScreen.Game;
        _player = gameScreen.Player;
    }

    public override void Update()
    {
        var activeUnit = _player.ActiveUnit;
        if (activeUnit == null)
        {
            SetCommandState(CommandStatus.Invalid);
        }
        else if (activeUnit.AIrole == AIroleType.Settle)
        {
            SetCommandState(CommandStatus.Invalid);
        }
        else
        {
            var canFortifyHere = UnitFunctions.CanFortifyHere(activeUnit, _player.ActiveTile);
            SetCommandState(canFortifyHere.Enabled ? CommandStatus.Normal : CommandStatus.Disabled);
        }
    }

    public override void Action()
    {
        Debug.Assert(_player.ActiveUnit != null, "_player.ActiveUnit != null");
        _player.ActiveUnit.Order = OrderType.Fortify;
        _player.ActiveUnit.MovePointsLost = _player.ActiveUnit.MaxMovePoints;
        _game.ChooseNextUnit();
    }
}