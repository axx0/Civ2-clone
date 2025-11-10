using System.Linq;
using Civ2engine.MapObjects;
using Civ2engine.UnitActions;
using Model.Core.Units;

namespace Civ2engine.Scripting.UnitActions;

public class InvasionAction(Unit baseUnit, Tile possibleMove, Game game) : TileAction(baseUnit, possibleMove, "Invasion", game)
{
    public override void Execute()
    {
        var unitToMove = BaseUnit.CarriedUnits.FirstOrDefault(u=>u.CanMakeAmphibiousAssaults);
        if(unitToMove != null)
        {
            MovementFunctions.AttackAtTile(unitToMove, game, Tile);
        }
    }
}