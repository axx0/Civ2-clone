using System.Linq;
using Model;
using Model.Constants;
using Model.Images;
using Model.Interface;

namespace Civ2engine.Production
{
    public class BuildingProductionOrder : ProductionOrder
    {
        private readonly int _firstWonderIndex;

        public BuildingProductionOrder(Improvement imp, int i, int firstWonderIndex) : base(imp.Cost, ItemType.Building, i+1, imp.Prerequisite)
        {
            _firstWonderIndex = firstWonderIndex;
            Improvement = imp;
        }

        public Improvement Improvement { get; }

        public override string Title => Improvement.Name;

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

        public override IImageSource? GetIcon(IUserInterface activeInterface)
        {
            return Improvement.Icon ?? activeInterface.GetImprovementImage(Improvement, _firstWonderIndex);
        }

        public override bool IsValidBuild(City city)
        {
            if (!city.ImprovementExists(Improvement.Type))
            {
                //Ocean improvements can't be built inland
                if (!city.IsNextToOcean() && Improvement.Effects.ContainsKey(Effects.OceanRequired))
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public override string GetDescription()
        {
            return Improvement.Name;
        }

        public override ListBoxEntry GetBuildListEntry(IUserInterface active)
        {
            return new ListBoxEntry { LeftText = Improvement.Name, Icon = GetIcon(active) };
        }
    }
}