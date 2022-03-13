using System.Linq;
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
            if (_imp.Effects.ContainsKey(ImprovementEffect.Unique))
            {
                foreach (var previousCity in city.Owner.Cities.Where(c=> c.ImprovementExists(_imp.Type)))
                {
                    previousCity.SellImprovement(_imp);
                }
            }
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