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

        public override bool CompleteProduction(City city, Rules rules)
        {
            if (!IsValidBuild(city)) return false;
            if (Improvement.Effects.ContainsKey(Effects.Unique))
            {
                foreach (var previousCity in city.Owner.Cities.Where(c => c.ImprovementExists(Improvement.Type)))
                {
                    previousCity.SellImprovement(Improvement);
                }
            }

            city.AddImprovement(Improvement);
            return true;

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