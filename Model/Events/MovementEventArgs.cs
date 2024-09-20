using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.Units;

namespace Civ2engine.Events
{
    public class MovementEventArgs : UnitEventArgs
    {
        public MovementEventArgs(Unit unit, Tile tileFrom, Tile tileTo) : base(UnitEventType.MoveCommand, new [] { tileFrom, tileTo })
        {
            Unit = unit;
        }

        public Unit Unit { get; }  
    }
}