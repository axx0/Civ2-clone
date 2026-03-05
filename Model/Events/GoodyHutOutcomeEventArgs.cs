using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.MapObjects;
using Model.Core.GoodyHuts.Outcomes;
using Model.Core.Units;

namespace Model.Events
{
    public class GoodyHutOutcomeEventArgs : UnitEventArgs
    {
        public Unit Unit { get; }
        public GoodyHutOutcomeResult Outcome { get; }

        public GoodyHutOutcomeEventArgs(Unit unit, GoodyHutOutcomeResult outcome)
            : base(UnitEventType.GoodyHutOutcome, new List<Tile> { unit.CurrentLocation })
        {
            Unit = unit;
            Outcome = outcome;
        }
    }
}
