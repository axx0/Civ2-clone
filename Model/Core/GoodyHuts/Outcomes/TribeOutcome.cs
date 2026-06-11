using Model.Core.Mapping;
using Model.Core.Units;

namespace Model.Core.GoodyHuts.Outcomes
{
    public class TribeOutcome : GoodyHutOutcome
    {
        private readonly UnitDefinition? _settlerDefinition;

        public TribeOutcome(UnitDefinition? settlerDefinition = null)
        {
            _settlerDefinition = settlerDefinition;
        }

        public override GoodyHutOutcomeResult ApplyOutcome(Unit unit)
        {
            if (unit.CurrentLocation.Terrain.Type is TerrainType.Grassland or TerrainType.Plains)
            {
                return new GoodyHutOutcomeResult("You have discovered an advanced tribe.", true, "AdvancedTribe");
            }

            var createdUnit = MercenariesOutcome.UnitFromHut(unit, _settlerDefinition ?? unit.TypeDefinition);
            return new GoodyHutOutcomeResult("You discover a band of wandering nomads. They agree to join your tribe.", true, "Nomads")
            {
                CreatedUnit = createdUnit
            };
        }
    }
}
