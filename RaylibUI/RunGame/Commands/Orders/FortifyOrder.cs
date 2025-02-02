using System.Diagnostics;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.UnitActions;
using Civ2engine.Units;
using Model;
using Model.Constants;
using Model.Core;
using Model.Menu;
using Raylib_CSharp.Interact;

namespace RaylibUI.RunGame.GameModes.Orders;

public class FortifyOrder : Order
{
    private readonly IGame _game;
    private readonly LocalPlayer _player;

    public FortifyOrder(GameScreen gameScreen) : 
        base(gameScreen, new Shortcut(KeyboardKey.F), CommandIds.FortifyOrder)
    {
        _game = gameScreen.Game;
        _player = gameScreen.Player;
    }

    public override bool Update()
    {
        var activeUnit = _player.ActiveUnit;
        if (activeUnit == null)
        {
            return SetCommandState(CommandStatus.Invalid);
        }
        
        if (activeUnit.AiRole == AiRoleType.Settle)
        {
            return SetCommandState(CommandStatus.Invalid);
        }
        
        var canFortifyHere = UnitFunctions.CanFortifyHere(activeUnit, _player.ActiveTile);
        return SetCommandState(canFortifyHere ? CommandStatus.Normal : CommandStatus.Disabled);
    }

    public override void Action()
    {
        Debug.Assert(_player.ActiveUnit != null, "_player.ActiveUnit != null");
        _player.ActiveUnit.Order = (int)OrderType.Fortify;
        _player.ActiveUnit.MovePointsLost = _player.ActiveUnit.MaxMovePoints;
        _game.ChooseNextUnit();
    }
}