using Civ2engine.Improvements;

namespace Civ2engine.Production
{
    public class BuildingProductionOrder : ProductionOrder
    {
        private readonly Improvement _imp;

        public BuildingProductionOrder(Improvement imp, int i) : base(imp.Cost, ItemType.Building, i+1, imp.Prerequisite)
        {
            _imp = imp;
        }

        public override void CompleteProduction(City city, Rules rules)
        {
            city.AddImprovement(_imp);
        }

        public override bool IsValidBuild(City city)
        {
            if (!city.ImprovementExists(_imp.Type))
            {
                //TODO: Ocean improvements
                
                return true;
            }
            return false;
        }
    }
}