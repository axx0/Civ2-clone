using System;
using System.Collections.Generic;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.Units;

namespace Civ2engine.Events
{
    public abstract class UnitEventArgs : EventArgs
    {
        public IList<Tile> Location { get; }
        public UnitEventType EventType { get; }
        
        protected UnitEventArgs(UnitEventType eventType, IList<Tile> locations)
        {
            Location = locations;
            EventType = eventType;
        }
    }

    public class ActivationEventArgs : UnitEventArgs
    {
        public bool UserInitiated { get; }
        public bool Reactivation { get; }

        public ActivationEventArgs(Unit unit, bool userInitiated, bool reactivation) : base(UnitEventType.NewUnitActivated, new[] { unit.CurrentLocation })
        {
            UserInitiated = userInitiated;
            Reactivation = reactivation;
            Unit = unit;
        }

        public Unit Unit { get; }  
    }
}
