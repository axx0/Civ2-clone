using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.Enums;
using Civ2engine.Improvements;
using Civ2engine.Units;
using Civ2engine.MapObjects;
using Civ2engine.Production;

namespace Civ2engine
{
    public class City : IMapItem
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool CanBuildCoastal { get; set; }
        public bool AutobuildMilitaryRule { get; set; }
        public bool StolenTech { get; set; }
        public bool ImprovementSold { get; set; }
        public bool WeLoveKingDay { get; set; }
        public bool CivilDisorder { get; set; }
        public bool CanBuildHydro { get; set; }
        public bool CanBuildShips { get; set; }
        public bool Objectivex3 { get; set; }
        public bool Objectivex1 { get; set; }
        public bool AutobuildDomesticAdvisor { get; set; }
        public bool AutobuildMilitaryAdvisor { get; set; }
        public Civilization Owner { get; set; }
        public int OwnerId => Owner.Id;
        public int Size { get; set; }
        public Civilization WhoBuiltIt { get; set; }
        public int FoodInStorage { get; set; }
        public int NetTrade { get; set; }
        public string Name { get; set; }
        public int NoOfSpecialistsx4 { get; set; }
        public ProductionOrder ItemInProduction { get; set; }
        public int ActiveTradeRoutes { get; set; }
        public CommodityType[] CommoditySupplied { get; set; }
        public CommodityType[] CommodityDemanded { get; set; }
        public CommodityType[] CommodityInRoute { get; set; }
        public int[] TradeRoutePartnerCity { get; set; }
        public int NoOfTradeIcons { get; set; }

        public int HappyCitizens { get; set; }
        public int UnhappyCitizens { get; set; }

        private int _population;

        public int Population
        {
            get
            {
                _population = 0;
                for (int i = 1; i <= Size; i++)
                    _population += i * 10000;
                return _population;
            }
        }

        internal readonly SortedList<ImprovementType, Improvement> _improvements = new();
        public IReadOnlyList<Improvement> Improvements => _improvements.Values.ToArray();
        public List<Unit> UnitsInCity => Location.UnitsHere;
        public List<Unit> SupportedUnits => Owner.Units.Where(unit => unit.HomeCity == this).ToList();
        public bool AnyUnitsPresent() => Location.UnitsHere.Count > 0;

        public int FoodProduction { get; set; }

        public int Food => Math.Min(FoodProduction, FoodConsumption);

        public int FoodConsumption { get; set; }

        public int SurplusHunger { get; set; }

        public int OrganizationLevel
        {
            get
            {
                var level = Owner.Government switch
                {
                    GovernmentType.Anarchy => 0,
                    GovernmentType.Despotism => 0,
                    GovernmentType.Monarchy => 1,
                    GovernmentType.Communism => 1,
                    GovernmentType.Fundamentalism => 1,
                    GovernmentType.Republic => 2,
                    GovernmentType.Democracy => 2,
                    _ => throw new ArgumentOutOfRangeException()
                };
                return WeLoveKingDay ? level + 1 : level;
            }
        }

        public int Trade { get; set; }
        public int Corruption { get; set; }

        /// <summary>
        /// Adjusted formula as it should always round excess into tax
        /// </summary>
        public int Tax => (int)((Trade - BaseScience - BaseLux) * GetMultiplier(Effects.TaxMultiplier));

        private int BaseLux => Trade * Owner.LuxRate / 100;

        public int Lux =>
            (int)(BaseLux * GetMultiplier(Effects.LuxMultiplier));

        public int BaseScience => Trade * Owner.ScienceRate / 100;

        public int Science => (int)(BaseScience * GetMultiplier(Effects.ScienceMultiplier));
        private decimal GetMultiplier(Effects effect)
        {
            return (100 + Improvements
                .Where(i => i.Effects.ContainsKey(effect))
                .Select(b => b.Effects[effect]).Sum()) / 100m;
        }


        // PRODUCTION
        public int TotalProduction { get; set; }
        public int Production { get; set; }
        public int ShieldsProgress { get; set; }
        
        public int Support { get; set; }
        public int Waste { get; set; }
        

        private PeopleType[] _people;
        public PeopleType[] People
        {
            get
            {
                _people = new PeopleType[Size];
                // Unhappy
                int additUnhappy = Size - 6;    // Without units & improvements present, 6 people are content
                additUnhappy -= Math.Min(Location.UnitsHere.Count, 3);  // Each new unit in city -> 1 less unhappy (up to 3 max)
                if (Improvements.Any(impr => impr.Type == ImprovementType.Temple)) additUnhappy -= 2;
                if (Improvements.Any(impr => impr.Type == ImprovementType.Colosseum)) additUnhappy -= 3;
                if (Improvements.Any(impr => impr.Type == ImprovementType.Cathedral)) additUnhappy -= 3;
                // Aristocrats
                int additArist = 0;
                switch (Size + 1 - WorkedTiles.Count)   // Populating aristocrats based on workers removed
                {
                    case 1: additArist += 1; break;
                    case 2:
                    case 3: additArist += 3; break;
                    case 4:
                    case 5:
                    case 6: additArist += 4; break;
                    case 7: additArist += 5; break;
                    case 8:
                    case 9: additArist += 6; break;
                    case 10: additArist += 7; break;
                    case 11: additArist += 8; break;
                    default: break;
                }
                // Elvis
                int additElvis = Size + 1 - WorkedTiles.Count;  // No of elvis = no of workers removed
                // Populate
                for (int i = 0; i < Size; i++) _people[i] = PeopleType.Worker;
                for (int i = 0; i < additUnhappy; i++) _people[Size - 1 - i] = PeopleType.Unhappy;
                for (int i = 0; i < additArist; i++) _people[i] = PeopleType.Aristocrat;
                for (int i = 0; i < additElvis; i++) _people[Size - 1 - i] = PeopleType.Elvis;
                return _people;
            }
            set
            {
                _people = value;
            }
        }

        public bool IsNextToRiver => Location.River || Location.Neighbours().Any(t => t.River);

        public bool IsNextToOcean =>  Location.Neighbours().Any(t => t.Type == TerrainType.Ocean);
        
        public Tile Location { get; init; }
        public List<Tile> WorkedTiles { get; } = new();
        public int Pollution { get; set; }

        public ResourceValues GetConsumableResourceValues(string resourceName)
        {
            switch (resourceName)
            {
                case "Food":
                    return SurplusHunger > 0
                        ? new ResourceValues(consumption: Food, surplus: SurplusHunger, loss: 0)
                        : new ResourceValues(consumption: Food, surplus: 0, loss: -SurplusHunger);
                case "Shields":
                    return new ResourceValues(consumption: Support, surplus: Production, loss: Waste);
                case "Trade":
                    return new ResourceValues(consumption: Trade, surplus: 0, loss: Corruption);
                default:
                    throw new NotImplementedException();
            }
        }

        public int GetResourceValues(string name)
        {
            return name switch
            {
                "Science" => Science,
                "Lux" => Lux,
                "Tax" => Tax,
                "Shields" => Production,
                "Food" => SurplusHunger,
                _ => throw new NotSupportedException()
            };
        }
    }
}
