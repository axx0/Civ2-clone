using System;
using System.Collections.Generic;
using Civ2engine.Enums;
using Civ2engine.Terrains;
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
        public ActivationEventArgs(Unit unit) : base(UnitEventType.NewUnitActivated, new[] { unit.CurrentLocation })
        {
            
        }
    }
}
