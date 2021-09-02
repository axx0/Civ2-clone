using Civ2engine.Improvements;

namespace Civ2engine.Production
{
    public class BuildingProductionOrder : ProductionOrder
    {
        private readonly Improvement _imp;

        public BuildingProductionOrder(Improvement imp, int i) : base(imp.Cost, ItemType.Building, i+1)
        {
            _imp = imp;
        }
    }
}