using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.Enums;
using Civ2engine.Improvements;
using Civ2engine.Units;
using Civ2engine.Terrains;
using System.Diagnostics;

namespace Civ2engine
{
    public class City : BaseInstance, IMapItem
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool CanBuildCoastal { get; set; }
        public bool AutobuildMilitaryRule { get; set; }
        public bool StolenTech { get; set; }
        public bool ImprovementSold { get; set; }
        public bool WeLoveKingDay { get; set; }
        public bool CivilDisorder { get; set; }
        public bool CanBuildShips { get; set; }
        public bool Objectivex3 { get; set; }
        public bool Objectivex1 { get; set; }
        public Civilization Owner { get; set; }
        public int OwnerId => Game.AllCivilizations.IndexOf(Owner);
        public int Size { get; set; }
        public Civilization WhoBuiltIt { get; set; }
        public int FoodInStorage { get; set; }
        public int MaxFoodInStorage => 10 * (Size + 1);
        public int NetTrade { get; set; }
        public string Name { get; set; }
        public int NoOfSpecialistsx4 { get; set; }
        public int ItemInProduction { get; set; }
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

        private readonly List<Improvement> _improvements = new List<Improvement>();
        public Improvement[] Improvements => _improvements.OrderBy(i => i.Id).ToArray();
        public void AddImprovement(Improvement improvement) => _improvements.Add(improvement);
        public bool ImprovementExists(ImprovementType improvement) => _improvements.Exists(i => i.Type == improvement);
        public List<Unit> UnitsInCity => Location.UnitsHere;
        public List<Unit> SupportedUnits => Owner.Units.Where(unit => unit.HomeCity == this).ToList();
        public bool AnyUnitsPresent() => Location.UnitsHere.Count > 0;

        // Determine which units, supported by this city, cost shields
        public bool[] SupportedUnitsWhichCostShields()
        {
            List<Unit> supportedUnits = SupportedUnits;
            bool[] costShields = new bool[SupportedUnits.Count];
            // First determine how many units have 0 costs due to different goverernment types
            var noCost = Game.AllCivilizations[OwnerId].Government switch
            {
                GovernmentType.Anarchy =>
                    Math.Min(supportedUnits.Count, Size) // Only units above city size cost 1 shield
                ,
                GovernmentType.Despotism =>
                    Math.Min(supportedUnits.Count, Size) // Only units above city size cost 1 shield
                ,
                GovernmentType.Communism => Math.Min(supportedUnits.Count, Game.Rules.Cosmic.CommunismPaysSupport) // First 3 units have no shield cost
                ,
                GovernmentType.Monarchy => Math.Min(supportedUnits.Count, Game.Rules.Cosmic.MonarchyPaysSupport) // First 3 units have no shield cost
                ,
                GovernmentType.Fundamentalism =>
                    Math.Min(supportedUnits.Count, Game.Rules.Cosmic.FundamentalismPaysSupport) // First 10 units have no shield cost
                ,
                GovernmentType.Republic => 0 // Each unit costs 1 shield per turn
                ,
                GovernmentType.Democracy => 0 // Each unit costs 1 shield per turn
                ,
                _ => 0
            };
            // Now determine bools of units that require upkeep
            for (int i = 0; i < supportedUnits.Count; i++)
            {
                if (supportedUnits[i].Type is UnitType.Diplomat or UnitType.Caravan or UnitType.Fanatics or UnitType.Spy or UnitType.Freight) // Some units never require upkeep
                {
                    costShields[i] = false;
                }
                else
                {
                    costShields[i] = noCost <= 0;
                    noCost--;
                }
            }

            return costShields;
        }

        public int FoodProduction
        {
            get
            {
                var hasSupermarket = ImprovementExists(ImprovementType.Supermarket);
                return WorkedTiles.Select(t => t.GetFood(OrganizationLevel == 0, hasSupermarket)).Sum();
                
            }
        }

        public int Food => Math.Min(FoodProduction, FoodConsumption);

        private int FoodConsumption =>
            Size * Game.Rules.Cosmic.FoodEatenPerTurn +
            SupportedUnits.Count(u => u.TypeDefinition.IsSettler || u.TypeDefinition.IsEngineer) *
            (Owner.Government <= GovernmentType.Monarchy
                ? Game.Rules.Cosmic.SettlersEatTillMonarchy
                : Game.Rules.Cosmic.SettlersEatFromCommunism); 

        public int SurplusHunger => FoodProduction - FoodConsumption;

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

        private int _trade;
        public int Trade
        {
            get
            {
                var hasSuperhighways = ImprovementExists(ImprovementType.Superhighways);
                return WorkedTiles.Select(t => t.GetTrade(OrganizationLevel, hasSuperhighways)).Sum();
            }
        }

        private int _corruption;
        public int Corruption
        {
            get
            {
                _corruption = 0;
                if (!Improvements.Any(impr => impr.Type == ImprovementType.Palace)) _corruption++;  // If not capital
                return _corruption;
            }
        }

        public int Tax => Trade * Owner.TaxRate / 100;
        public int Lux => Trade - Science - Tax;
        public int Science => Trade * Owner.ScienceRate / 100;

        // PRODUCTION
        public int TotalShieldProduction
        {
            get
            {
                var lowOrganisation = Owner.Government <= GovernmentType.Despotism;
                return WorkedTiles.Select(t => t.GetShields(lowOrganisation)).Sum();
            }
        }

        public int Production => TotalShieldProduction - Support;
        public int Support => SupportedUnitsWhichCostShields().Count(c => c);   // Count true occurances
        public int ShieldsProgress { get; set; }

        private int _waste;
        public int Waste
        {
            get
            {
                _waste = 1;
                return _waste;
            }
        }

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

        public bool IsNextToRiver => Map.Neighbours(Location).Any(t => t.River);

        public bool IsNextToOcean => Map.Neighbours(Location).Any(t => t.Type == TerrainType.Ocean);
        
        public Tile Location { get; init; }
        public List<Tile> WorkedTiles { get; } = new();
    }
}
