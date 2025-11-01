using Civ2engine.Enums;
using Civ2engine.Units;
using Model.Core.Units;

namespace Civ2engine.Events
{
    public class MovementBlockedEventArgs(IUnit subjectUnit, BlockedReason reason)
        : UnitEventArgs(UnitEventType.MovementBlocked, [subjectUnit.CurrentLocation])
    {
        public BlockedReason Reason { get; set; } = reason;
    }
}