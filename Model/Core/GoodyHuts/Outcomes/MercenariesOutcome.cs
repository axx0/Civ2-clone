using Civ2engine.Enums;
using Model.Core.Units;

namespace Model.Core.GoodyHuts.Outcomes
{
    public class MercenariesOutcome : GoodyHutOutcome
    {
        private readonly UnitDefinition? _unitDefinition;

        public MercenariesOutcome(UnitDefinition? unitDefinition = null)
        {
            _unitDefinition = unitDefinition;
        }

        public string Description => "You have discovered a friendly tribe of skilled mercenaries.";
        public override GoodyHutOutcomeResult ApplyOutcome(Unit unit)
        {
            var createdUnit = UnitFromHut(unit, _unitDefinition ?? unit.TypeDefinition, veteran: true);
            return new GoodyHutOutcomeResult(Description, true, "Mercenaries")
            {
                CreatedUnit = createdUnit
            };
        }

        internal static Unit UnitFromHut(Unit unit, UnitDefinition unitDefinition, bool veteran = false)
        {
            var createdUnit = new Unit
            {
                Counter = 0,
                Dead = false,
                Id = unit.Owner.Units.Count != 0 ? unit.Owner.Units.Max(u => u.Id) + 1 : 0,
                Order = (int)OrderType.NoOrders,
                Owner = unit.Owner,
                Veteran = veteran,
                X = unit.CurrentLocation.X,
                Y = unit.CurrentLocation.Y,
                MapIndex = unit.CurrentLocation.Z,
                CurrentLocation = unit.CurrentLocation,
                TypeDefinition = unitDefinition,
                NeedsSupport = false
            };

            unit.Owner.Units.Add(createdUnit);
            return createdUnit;
        }
    }
}
