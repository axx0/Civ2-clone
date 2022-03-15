using System.Linq;
using Civ2engine.Improvements;

namespace Civ2engine.Production
{
    public class BuildingProductionOrder : ProductionOrder
    {
        public BuildingProductionOrder(Improvement imp, int i) : base(imp.Cost, ItemType.Building, i+1, imp.Prerequisite)
        {
            Improvement = imp;
        }

        public Improvement Improvement { get; }

        public override void CompleteProduction(City city, Rules rules)
        {
            if (Improvement.Effects.ContainsKey(ImprovementEffect.Unique))
            {
                foreach (var previousCity in city.Owner.Cities.Where(c=> c.ImprovementExists(Improvement.Type)))
                {
                    previousCity.SellImprovement(Improvement);
                }
            }
            city.AddImprovement(Improvement);
        }

        public override bool IsValidBuild(City city)
        {
            if (!city.ImprovementExists(Improvement.Type))
            {
                //TODO: Ocean improvements
                
                return true;
            }
            return false;
        }

        public override string GetDescription()
        {
            return Improvement.Name;
        }
    }
}