using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.MapObjects;
using Civ2engine.Units;
using Model.Core.Cities;

namespace Civ2engine
{
    public class City : IMapItem
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int MapIndex { get; set; } = 0;
        public bool CanBuildCoastal { get; set; }
        public bool AutobuildMilitaryRule { get; set; }
        public bool StolenTech { get; set; }
        public bool ImprovementSold { get; set; }
        public bool WeLoveKingDay { get; set; }
        public bool CivilDisorder { get; set; }
        public bool CanBuildHydro { get; set; }
        public bool CanBuildShips { get; set; }
        public int Objective { get; set; }
        public bool AutobuildDomesticAdvisor { get; set; }
        public bool AutobuildMilitaryAdvisor { get; set; }
        public Civilization Owner { get; set; }
        public int OwnerId => Owner.Id;
        public int Size { get; set; }
        public Civilization WhoBuiltIt { get; set; }
        public bool[] WhoKnowsAboutIt { get; set; }
        public int[] LastSizeRevealedToCivs { get; set; }
        public int FoodInStorage { get; set; }
        public int NetTrade { get; set; }
        public string Name { get; set; }
        public int NoOfSpecialistsx4 { get; set; }
        public IProductionOrder ItemInProduction { get; set; }
        public int ActiveTradeRoutes { get; set; }
        public Commodity[]? CommoditySupplied { get; set; }
        public Commodity[]? CommodityDemanded { get; set; }
        public Commodity[]? CommodityInRoute { get; set; }
        public int[]? TradeRoutePartnerCity { get; set; }
        
        public TradeRoute[] TradeRoutes { get; set; }
        
        public int NoOfTradeIcons { get; set; }

        public int HappyCitizens { get; set; }
        public int UnhappyCitizens { get; set; }

        public readonly SortedList<int, Improvement> OrderedImprovements = new();
        public IReadOnlyList<Improvement> Improvements => OrderedImprovements.Values.ToArray();
        public List<Unit> UnitsInCity => Location.UnitsHere;
        public List<Unit> SupportedUnits => Owner.Units.Where(unit => unit.HomeCity == this).ToList();
        public bool AnyUnitsPresent() => Location.UnitsHere.Count > 0;

        public int FoodProduction { get; set; }

        public int Food => Math.Min(FoodProduction, FoodConsumption);

        public int FoodConsumption { get; set; }

        public int SurplusHunger { get; set; }

        public int Trade { get; set; }
        public int Corruption { get; set; }
        
        // PRODUCTION
        public int TotalProduction { get; set; }
        public int Production { get; set; }
        public int ShieldsProgress { get; set; }
        
        public int Support { get; set; }
        public int Waste { get; set; }
        
        public Tile Location { get; set; }
        public List<Tile> WorkedTiles { get; } = new();
        public int Pollution { get; set; }
        
    }
}
