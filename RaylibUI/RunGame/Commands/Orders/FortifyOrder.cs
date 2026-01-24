using System.Diagnostics;
using Civ2engine.Enums;
using Civ2engine.UnitActions;
using JetBrains.Annotations;
using Model;
using Model.Constants;
using Model.Core;
using Model.Input;
using Model.Menu;

namespace RaylibUI.RunGame.Commands.Orders;

[UsedImplicitly]
public class FortifyOrder(GameScreen gameScreen) : Order(gameScreen, new Shortcut(Key.F), CommandIds.FortifyOrder)
{
    private readonly IGame _game = gameScreen.Game;
    private readonly LocalPlayer _player = gameScreen.Player;

    public override bool Update()
    {
        var activeUnit = _player.ActiveUnit;
        if (activeUnit == null || activeUnit.AiRole == AiRoleType.Settle)
        {
            return SetCommandState(CommandStatus.Invalid);
        }

        var canFortifyHere = UnitFunctions.CanFortifyHere(activeUnit, _player.ActiveTile);
        return SetCommandState(canFortifyHere ? CommandStatus.Normal : CommandStatus.Disabled);
    }

    public override void Action()
    {
        Debug.Assert(_player.ActiveUnit != null);
        _player.ActiveUnit.Order = (int)OrderType.Fortify;
        _player.ActiveUnit.MovePointsLost = _player.ActiveUnit.MaxMovePoints;
        _game.ChooseNextUnit();
    }
}