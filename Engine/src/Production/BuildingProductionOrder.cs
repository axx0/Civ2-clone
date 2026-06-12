using System;
using Model;
using Model.Constants;
using Model.Controls;
using Model.Images;
using System.Linq;
using Model.Core.Cities;
using Model.Core.GameRules;
using Model.Core.Production;

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

        public override ListboxGroup GetBuildListEntry(IUserInterface active, City city)
        {
            var turns = Math.Max(1, (int)Math.Ceiling(Math.Max(0, 10 * Improvement.Cost - city.ShieldsProgress) /
                                                      (decimal)Math.Max(1, city.Production)));
            return new ListboxGroup
            {
                Elements = [ new() { Icon = GetIcon(active), Width = 70, ScaleIcon = 0.85f },
                             new() { Text = Improvement.Name, Width = 260, TextSizeOverride = 18, VerticalAlignment = VerticalAlignment.Center },
                             new() { Text = $"({turns} Turns)", TextSizeOverride = 16,
                                 HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center } ],
                Height = 38,
            };
        }
    }
}
