﻿using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.Enums;
using Civ2engine.Improvements;
using Civ2engine.Units;
using Civ2engine.Terrains;
using System.Diagnostics;

namespace Civ2engine
{
    public class City : BaseInstance
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
        public int OwnerId => Game.GetCivs.IndexOf(Owner);
        public int Size { get; set; }
        public Civilization WhoBuiltIt { get; set; }
        public int FoodInStorage { get; set; }
        public int MaxFoodInStorage => 10 * (Size + 1);
        public int NetTrade { get; set; }
        public string Name { get; set; }
        public bool[] DistributionWorkers { get; set; }
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
        public List<IUnit> UnitsInCity => Game.AllUnits.Where(unit => unit.X == X && unit.Y == Y).ToList();
        public List<IUnit> SupportedUnits => Game.AllUnits.Where(unit => unit.HomeCity == this).ToList();
        public bool AnyUnitsPresent() => Game.AllUnits.Any(unit => unit.X == this.X && unit.Y == this.Y);

        // Determine which units, supported by this city, cost shields
        public bool[] SupportedUnitsWhichCostShields()
        {
            List<IUnit> supportedUnits = SupportedUnits;
            bool[] costShields = new bool[SupportedUnits.Count];
            // First determine how many units have 0 costs due to different goverernment types
            int noCost = 0;
            switch (Game.GetCivs[OwnerId].Government)
            {
                case GovernmentType.Anarchy:
                case GovernmentType.Despotism:
                    noCost = Math.Min(supportedUnits.Count, Size); // Only units above city size cost 1 shield
                    break;
                case GovernmentType.Communism:
                case GovernmentType.Monarchy:
                    noCost = Math.Min(supportedUnits.Count, 3);    // First 3 units have no shield cost
                    break;
                case GovernmentType.Fundamentalism:
                    noCost = Math.Min(supportedUnits.Count, 10);   // First 10 units have no shield cost
                    break;
                case GovernmentType.Republic:
                case GovernmentType.Democracy:
                    noCost = 0;    // Each unit costs 1 shield per turn
                    break;
            }
            // Now determine bools of units that require upkeep
            for (int i = 0; i < supportedUnits.Count; i++)
            {
                if (supportedUnits[i].Type == UnitType.Diplomat || supportedUnits[i].Type == UnitType.Caravan || supportedUnits[i].Type == UnitType.Fanatics || supportedUnits[i].Type == UnitType.Spy || supportedUnits[i].Type == UnitType.Freight)   // Some units never require upkeep
                {
                    costShields[i] = false;
                }
                else if (noCost != 0)
                {
                    costShields[i] = false;
                    noCost--;   // Reduce counter
                }
                else
                {
                    costShields[i] = true;
                }
            }

            return costShields;
        }

        private int[] _foodDistribution;
        public int[] FoodDistribution
        {
            get
            {
                _foodDistribution = new int[21];    // 21 squares around city
                int[,] offsets = new int[21, 2] { { 0, 0 }, { -1, -3 }, { -3, -1 }, { -3, 1 }, { -1, 3 }, { 1, 3 }, { 3, 1 }, { 3, -1 }, { 1, -3 }, { -2, -2 }, { -2, 2 }, { 2, 2 }, { 2, -2 }, { 0, -2 }, { -1, -1 }, { -2, 0 }, { -1, 1 }, { 0, 2 }, { 1, 1 }, { 2, 0 }, { 1, -1 } };    // Offset of squares from city square (0,0)
                for (int i = 0; i < 21; i++)
                {
                    if(!Map.IsValidTileC2(X + offsets[i, 0], Y + offsets[i, 1])) continue;
                    _foodDistribution[i] = Map.TileC2(X + offsets[i, 0], Y + offsets[i, 1]).Food;
                    if (Map.TileC2(X + offsets[i, 0], Y + offsets[i, 1]).Farmland && ImprovementExists(ImprovementType.Supermarket)) _foodDistribution[i] += 1;    // Farmland has effect only if city has supermarket
                    if (offsets[i, 0] == 0 && offsets[i, 1] == 0) _foodDistribution[i] += 1;    // City square has +1 food
                    if (offsets[i, 0] == 0 && offsets[i, 1] == 0 && ImprovementExists(ImprovementType.Supermarket)) _foodDistribution[i] += 1;    // +1 food if city has supermarket
                }
                return _foodDistribution;
            }
        }

        private int[] _tradeDistribution;
        public int[] TradeDistribution
        {
            get
            {
                _tradeDistribution = new int[21];    // 21 squares around city
                int[,] offsets = new int[21, 2] { { 0, 0 }, { -1, -3 }, { -3, -1 }, { -3, 1 }, { -1, 3 }, { 1, 3 }, { 3, 1 }, { 3, -1 }, { 1, -3 }, { -2, -2 }, { -2, 2 }, { 2, 2 }, { 2, -2 }, { 0, -2 }, { -1, -1 }, { -2, 0 }, { -1, 1 }, { 0, 2 }, { 1, 1 }, { 2, 0 }, { 1, -1 } };    // Offset of squares from city square (0,0)
                for (int i = 0; i < 21; i++)
                {
                    if(!Map.IsValidTileC2(X + offsets[i, 0], Y + offsets[i, 1])) continue;
                    ITerrain map = Map.TileC2(X + offsets[i, 0], Y + offsets[i, 1]);
                    _tradeDistribution[i] = map.Trade;
                    if (map.Road && (map.Type == TerrainType.Desert || map.Type == TerrainType.Grassland || map.Type == TerrainType.Plains)) _tradeDistribution[i]++;
                }
                return _tradeDistribution;
            }
        }

        private int _food;
        public int Food
        {
            get
            {
                int maxFood = 2 * Size;
                foreach (IUnit unit in Game.AllUnits.Where(u => (u.Type == UnitType.Settlers || u.Type == UnitType.Engineers) && u.HomeCity == this)) maxFood++;  // Increase max food for settlers & engineers with this home city
                int _foodFromWorkers = 0;
                for (int i = 0; i < 21; i++) _foodFromWorkers += FoodDistribution[i] * (DistributionWorkers[i] ? 1 : 0);
                _food = Math.Min(_foodFromWorkers, maxFood);
                return _food;
            }
        }

        private int _surplusHunger;
        public int SurplusHunger
        {
            get
            {
                int maxFood = 2 * Size;
                foreach (IUnit unit in Game.AllUnits.Where(u => (u.Type == UnitType.Settlers || u.Type == UnitType.Engineers) && u.HomeCity == this)) maxFood++;  // Increase max food for settlers & enineers with this home city
                int _foodFromWorkers = 0;
                for (int i = 0; i < 21; i++) _foodFromWorkers += FoodDistribution[i] * (DistributionWorkers[i] ? 1 : 0);
                _surplusHunger = _foodFromWorkers - maxFood;
                return _surplusHunger;
            }
        }

        private int _trade;
        public int Trade
        {
            get
            {
                _trade = 0;
                for (int i = 0; i < 21; i++) _trade += TradeDistribution[i] * (DistributionWorkers[i] ? 1 : 0);
                return _trade;
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

        public int Tax => Trade * Game.GetCivs[OwnerId].TaxRate / 100;
        public int Lux => Trade - Science - Tax;
        public int Science => Trade * Game.GetCivs[OwnerId].ScienceRate / 100;

        // PRODUCTION
        private int[] _shieldDistribution;
        public int[] ShieldDistribution
        {
            get
            {
                _shieldDistribution = new int[21];    // 21 squares around city
                int[,] offsets = new int[21, 2] { { 0, 0 }, { -1, -3 }, { -3, -1 }, { -3, 1 }, { -1, 3 }, { 1, 3 }, { 3, 1 }, { 3, -1 }, { 1, -3 }, { -2, -2 }, { -2, 2 }, { 2, 2 }, { 2, -2 }, { 0, -2 }, { -1, -1 }, { -2, 0 }, { -1, 1 }, { 0, 2 }, { 1, 1 }, { 2, 0 }, { 1, -1 } };    // Offset of squares from city square (0,0)
                for (int i = 0; i < 21; i++)
                {
                    if(!Map.IsValidTileC2(X + offsets[i, 0], Y + offsets[i, 1])) continue;
                    _shieldDistribution[i] = Map.TileC2(X + offsets[i, 0], Y + offsets[i, 1]).Shields;
                    if (Map.TileC2(X + offsets[i, 0], Y + offsets[i, 1]).Mining) _shieldDistribution[i] += Map.TileC2(X + offsets[i, 0], Y + offsets[i, 1]).MiningBonus;
                }
                return _shieldDistribution;
            }
        }

        private int _totalShieldProduction;
        public int TotalShieldProduction
        {
            get
            {
                _totalShieldProduction = 0;
                for (int i = 0; i < 21; i++) _totalShieldProduction += ShieldDistribution[i] * (DistributionWorkers[i] ? 1 : 0);
                return _totalShieldProduction;
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
                additUnhappy -= Math.Min(Game.AllUnits.Count(unit => unit.X == X && unit.Y == Y), 3);  // Each new unit in city -> 1 less unhappy (up to 3 max)
                if (Improvements.Any(impr => impr.Type == ImprovementType.Temple)) additUnhappy -= 2;
                if (Improvements.Any(impr => impr.Type == ImprovementType.Colosseum)) additUnhappy -= 3;
                if (Improvements.Any(impr => impr.Type == ImprovementType.Cathedral)) additUnhappy -= 3;
                // Aristocrats
                int additArist = 0;
                switch (Size + 1 - DistributionWorkers.Count(c => c))   // Populating aristocrats based on workers removed
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
                int additElvis = Size + 1 - DistributionWorkers.Count(c => c);  // No of elvis = no of workers removed
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

        private bool _isNextToRiver;
        public bool IsNextToRiver
        {
            get
            {
                _isNextToRiver = false;
                var surroundingTiles = new List<int[]>
                {
                    new int[] {X + 0, Y + 0},
                    new int[] {X - 1, Y - 1},
                    new int[] {X + 0, Y - 2},
                    new int[] {X + 1, Y - 1},
                    new int[] {X + 2, Y + 0},
                    new int[] {X + 1, Y + 1},
                    new int[] {X + 0, Y + 2},
                    new int[] {X - 1, Y + 1},
                    new int[] {X - 2, Y + 0}
                };
                foreach (int[] surroundingTile in surroundingTiles)
                {
                    //Debug.WriteLine($"X={X}, Y={Y}, is valid={Map.IsValidTileC2(surroundingTile[0], surroundingTile[1])}");
                    //Debug.WriteLine($"X={X}, Y={Y}, river={Map.TileC2(surroundingTile[0], surroundingTile[1]).River}");
                    if (Map.IsValidTileC2(surroundingTile[0], surroundingTile[1]) && Map.TileC2(surroundingTile[0], surroundingTile[1]).River)
                        _isNextToRiver = true;
                }
                    
                return _isNextToRiver;
            }
        }

        private bool _isNextToOcean;
        public bool IsNextToOcean
        {
            get
            {
                _isNextToOcean = false;
                var surroundingTiles = new List<int[]>
                {
                    new int[] {X - 1, Y - 1},
                    new int[] {X + 0, Y - 2},
                    new int[] {X + 1, Y - 1},
                    new int[] {X + 2, Y + 0},
                    new int[] {X + 1, Y + 1},
                    new int[] {X + 0, Y + 2},
                    new int[] {X - 1, Y + 1},
                    new int[] {X - 2, Y + 0}
                };
                foreach (int[] surroundingTile in surroundingTiles)
                    if (Map.IsValidTileC2(surroundingTile[0], surroundingTile[1]) && Map.TileC2(surroundingTile[0], surroundingTile[1]).Type == TerrainType.Ocean)
                        _isNextToOcean = true;
                return _isNextToOcean;
            }
        }
    }
}
