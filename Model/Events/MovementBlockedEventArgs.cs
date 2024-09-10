using Civ2engine.Enums;
using Civ2engine.Units;

namespace Civ2engine.Events
{
    public class MovementBlockedEventArgs : UnitEventArgs
    {
        public BlockedReason Reason { get; set; }
        
        public MovementBlockedEventArgs(UnitEventType eventType, IUnit subjectUnit, BlockedReason reason) : base(eventType, new []{  subjectUnit.CurrentLocation})
        {
            Reason = reason;
        }
    }
}