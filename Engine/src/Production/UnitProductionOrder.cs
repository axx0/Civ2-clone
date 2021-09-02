using Civ2engine.Units;

namespace Civ2engine.Production
{
    public class UnitProductionOrder : ProductionOrder
    {
        private readonly UnitDefinition _unitDefinition;
        public UnitProductionOrder(UnitDefinition unitDefinition, int index) : base(unitDefinition.Cost, ItemType.Unit, index)
        {
            _unitDefinition = unitDefinition;
        }
    }
}