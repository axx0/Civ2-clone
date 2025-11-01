using System.Linq;
using Civ2engine.MapObjects;
using Model.Core.Units;

namespace Civ2engine.UnitActions;

public class InvasionAction(Unit unit, Tile possibleMove, Game game) : TileAction(unit, possibleMove, "Invasion")
{
    public override void Execute()
    {
        var unitToMove = Unit.CarriedUnits.FirstOrDefault(u=>u.CanMakeAmphibiousAssaults);
        if(unitToMove != null)
        {
            MovementFunctions.AttackAtTile(unitToMove, game, Tile);
        }
    }
}