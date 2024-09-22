using System.Collections.Generic;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.Units;

namespace Civ2engine.Events
{
    public class UnitInfo : IUnit
    {
        public  Tile CurrentLocation { get; }

        public readonly List<int> Hitpoints;

        public UnitInfo(Unit unit, List<int> hitpoints)
        {
            CurrentLocation = unit.CurrentLocation;
            HitpointsBase = unit.HitpointsBase;
            RemainingHitpoints = unit.RemainingHitpoints;
            Type = unit.Type;
            Order = unit.Order;
            Owner = unit.Owner;
            IsInStack = unit.IsInStack;
            
            Hitpoints = hitpoints;
        }

        public int HitpointsBase { get; }
        public int RemainingHitpoints { get; }
        public int Type { get; set; }
        public int Order { get; set; }
        public Civilization Owner { get; set; }
        public bool IsInStack { get; }
    }
}