using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.Units;
using Civ2engine.Improvements;
using Civ2engine.Terrains;

namespace Civ2engine
{
    public partial class Game : BaseInstance
    {
        private readonly Options _options;
        private readonly Rules _rules;
        private readonly GameVersionType _gameVersion;
        private readonly DifficultyType _difficultyLevel;
        private readonly BarbarianActivityType _barbarianActivity;
        
        public List<Unit> AllUnits { get; } = new();

        public List<Unit> GetCasualties => AllUnits.Where(u => u.Dead).ToList();
        public List<Unit> ActiveUnits => AllUnits.Where(u => !u.Dead).ToList();
        public List<City> GetCities { get; } = new();

        public List<Civilization> GetCivs { get; } = new();

        public List<Civilization> GetActiveCivs => GetCivs.Where(c => c.Alive).ToList();
        public Options Options => _options;
        public Rules Rules => _rules;
        public GameVersionType GameVersion => _gameVersion;
        
        private int _turnNumber;
        public int TurnNumber => _turnNumber;
        private int _gameYear;
        public int GetGameYear
        {
            get
            {
                _gameYear = _turnNumber switch
                {
                    < 250 => -4000 + (_turnNumber - 1) * 20,
                    >= 250 and < 300 => 1000 + (_turnNumber - 1 - 250) * 10,
                    >= 300 and < 350 => 1500 + (_turnNumber - 1 - 300) * 5,
                    >= 350 and < 400 => 1750 + (_turnNumber - 1 - 350) * 2,
                    _ => 1850 + (_turnNumber - 1 - 400)
                };
                return _gameYear;
            }
        }
        public string GetGameYearString => GetGameYear < 0 ? string.Join(" ", new string[] { Math.Abs(GetGameYear).ToString(), "B.C." }) : string.Join(" ", new string[] { GetGameYear.ToString(), "A.D." });
        public int TurnNumberForGameYear { get; set; }
        public DifficultyType DifficultyLevel => _difficultyLevel;
        public BarbarianActivityType BarbarianActivity => _barbarianActivity;
        public int PollutionAmount { get; set; }
        public int GlobalTempRiseOccured { get; set; }
        public int NoOfTurnsOfPeace { get; set; }
        public int NumberOfUnits { get; set; }
        public int NumberOfCities { get; set; }

        private Unit _activeUnit;
        public Unit ActiveUnit
        {
            get
            {
                return _activeUnit;
            }
            set
            {
                if (!value.TurnEnded)
                {
                    _activeUnit = value;
                }
                else
                {
#if DEBUG
                    throw new NotSupportedException("Tried to set ended unit to active");
#endif
                }
            }
        }

        private Civilization _playerCiv;
        public Civilization GetPlayerCiv => _playerCiv;

        private Civilization _activeCiv;    // ActiveCiv can be AI. PlayerCiv is human. They are equal except during enemy turns.
        public Civilization GetActiveCiv => _activeCiv;

        // Helper functions
        public City CityHere(int x, int y) => GetCities.Find(city => city.X == x && city.Y == y);
        public List<Unit> UnitsHere(int x, int y) => AllUnits.FindAll(unit => unit.X == x && unit.Y == y).AsEnumerable().Reverse().ToList();
        public bool AnyUnitsPresentHere(int x, int y) => AllUnits.Any(unit => unit.X == x && unit.Y == y);
        public bool EnemyUnitsPresentHere(int x, int y) => AllUnits.Any(unit => unit.X == x && unit.Y == y && unit.Owner != _activeUnit.Owner);
        public bool AnyCitiesPresentHere(int x, int y) => GetCities.Any(city => city.X == x && city.Y == y);

        /// <summary>
        /// Determine all units inside a ship.
        /// </summary>
        /// <param name="ship">Ship.</param>
        /// <returns>A list of all units in a ship.</returns>
        public List<Unit> UnitsOnShip(IUnit ship)
        {
            var unitsHere = Game.UnitsHere(ship.XY[0], ship.XY[1]);
            unitsHere.RemoveAll(u => u.Domain != UnitGAS.Ground);  // Remove all naval/air units
            var unitsInShip = unitsHere.Take(ship.ShipHold).ToList();

            return unitsInShip;
        }

        //public static void CreateTerrain (int x, int y, TerrainType type, int specialtype, bool resource, bool river, int island, bool unit_present, bool city_present, bool irrigation, 
        //                                  bool mining, bool road, bool railroad, bool fortress, bool pollution, bool farmland, bool airbase, bool[] visibility, string hexvalue)
        //{
        //    ITerrain tile;
        //    SpecialType? stype = null;
        //    switch (type)
        //    {
        //        case TerrainType.Desert:
        //            {
        //                if (specialtype == 1) stype = SpecialType.Oasis;
        //                if (specialtype == 2) stype = SpecialType.DesertOil;
        //                break;
        //            }
        //        case TerrainType.Plains:
        //            {
        //                if (specialtype == 1) stype = SpecialType.Buffalo;
        //                if (specialtype == 2) stype = SpecialType.Wheat;
        //                break;
        //            }
        //        case TerrainType.Grassland:
        //            {
        //                if (specialtype == 1) stype = SpecialType.GrasslandShield;
        //                if (specialtype == 2) stype = SpecialType.Grassland;
        //                break;
        //            }
        //        case TerrainType.Forest:
        //            {
        //                if (specialtype == 1) stype = SpecialType.Pheasant;
        //                if (specialtype == 2) stype = SpecialType.Silk;
        //                break;
        //            }
        //        case TerrainType.Hills:
        //            {
        //                if (specialtype == 1) stype = SpecialType.Coal;
        //                if (specialtype == 2) stype = SpecialType.Wine;
        //                break;
        //            }
        //        case TerrainType.Mountains:
        //            {
        //                if (specialtype == 1) stype = SpecialType.Gold;
        //                if (specialtype == 2) stype = SpecialType.Iron;
        //                break;
        //            }
        //        case TerrainType.Tundra:
        //            {
        //                if (specialtype == 1) stype = SpecialType.Game;
        //                if (specialtype == 2) stype = SpecialType.Furs;
        //                break;
        //            }
        //        case TerrainType.Glacier:
        //            {
        //                if (specialtype == 1) stype = SpecialType.Ivory;
        //                if (specialtype == 2) stype = SpecialType.GlacierOil;
        //                break;
        //            }
        //        case TerrainType.Swamp:
        //            {
        //                if (specialtype == 1) stype = SpecialType.Peat;
        //                if (specialtype == 2) stype = SpecialType.Spice;
        //                break;
        //            }
        //        case TerrainType.Jungle:
        //            {
        //                if (specialtype == 1) stype = SpecialType.Gems;
        //                if (specialtype == 2) stype = SpecialType.Fruit;
        //                break;
        //            }
        //        case TerrainType.Ocean:
        //            {
        //                if (specialtype == 1) stype = SpecialType.Fish;
        //                if (specialtype == 2) stype = SpecialType.Whales;
        //                break;
        //            }
        //        default: return ;
        //    }
        //    tile = new Terrain(type, stype)
        //    {
        //        Type = type,
        //        SpecType = stype,
        //        Resource = resource,
        //        River = river,
        //        Island = island,
        //        UnitPresent = unit_present,
        //        CityPresent = city_present,
        //        Irrigation = irrigation,
        //        Mining = mining,
        //        Road = road,
        //        Railroad = railroad,
        //        Fortress = fortress,
        //        Pollution = pollution,
        //        Farmland = farmland,
        //        Airbase = airbase,
        //        Visibility = visibility,
        //        Hexvalue = hexvalue
        //    };
        //    TerrainTile[x, y] = tile;
        //}

        public void CreateUnit (UnitType type, int x, int y, bool dead, bool firstMove, bool greyStarShield, bool veteran, int civId,
                                    int movePointsLost, int hitPointsLost, int prevX, int prevY, CommodityType caravanCommodity, OrderType orders,
                                    int homeCity, int goToX, int goToY, int linkOtherUnitsOnTop, int linkOtherUnitsUnder)
        {
            Unit unit = new Unit
            {
                Id = AllUnits.Count,
                TypeDefinition = Rules.UnitTypes[(int)type],
                Dead = dead || y < 0 || x < 0,
                Type = type,
                X = x,
                Y = y,
                MovePointsLost = movePointsLost,
                HitPointsLost = hitPointsLost,
                FirstMove = firstMove,
                GreyStarShield = greyStarShield,
                Veteran = veteran,
                Owner = GetCivs[civId],
                PrevXY = new int[] { prevX, prevY },
                CaravanCommodity = caravanCommodity,
                Order = orders,
                HomeCity = homeCity == 255 ? null : GetCities[homeCity],
                GoToX = goToX,
                GoToY = goToY,
                LinkOtherUnitsOnTop = linkOtherUnitsOnTop,
                LinkOtherUnitsUnder = linkOtherUnitsUnder
            };

            AllUnits.Add(unit);
        }

        public void CreateCity (int x, int y, bool canBuildCoastal, bool autobuildMilitaryRule, bool stolenTech, bool improvementSold, bool weLoveKingDay, bool civilDisorder, bool canBuildShips, bool objectivex3, bool objectivex1, int owner, int size, int whoBuiltIt, int foodInStorage, int shieldsProgress, int netTrade, string name, bool[] distributionWorkers, int noOfSpecialistsx4, bool[] improvements, int itemInProduction, int activeTradeRoutes, CommodityType[] commoditySupplied, CommodityType[] commodityDemanded, CommodityType[] commodityInRoute, int[] tradeRoutePartnerCity, int science, int tax, int noOfTradeIcons, int totalFoodProduction, int totalShieldProduction, int happyCitizens, int unhappyCitizens)
        {
            City city = new City
            {
                X = x,
                Y = y,
                CanBuildCoastal = canBuildCoastal,
                AutobuildMilitaryRule = autobuildMilitaryRule,
                StolenTech = stolenTech,
                ImprovementSold = improvementSold,
                WeLoveKingDay = weLoveKingDay,
                CivilDisorder = civilDisorder,
                CanBuildShips = canBuildShips,
                Objectivex3 = objectivex3,
                Objectivex1 = objectivex1,
                Owner = GetCivs[owner],
                Size = size,
                WhoBuiltIt = GetCivs[whoBuiltIt],
                FoodInStorage = foodInStorage,
                ShieldsProgress = shieldsProgress,
                NetTrade = netTrade,
                Name = name,
                DistributionWorkers = distributionWorkers,
                NoOfSpecialistsx4 = noOfSpecialistsx4,
                ItemInProduction = itemInProduction,
                ActiveTradeRoutes = activeTradeRoutes,
                CommoditySupplied = commoditySupplied,
                CommodityDemanded = commodityDemanded,
                CommodityInRoute = commodityInRoute,
                TradeRoutePartnerCity = tradeRoutePartnerCity,
                //Science = science,    //what does this mean???
                //Tax = tax,
                //NoOfTradeIcons = noOfTradeIcons,
                //TotalFoodProduction = totalFoodProduction,    // No need to import this, it's calculated
                //TotalShieldProduction = totalShieldProduction,    // No need to import this, it's calculated
                HappyCitizens = happyCitizens,
                UnhappyCitizens = unhappyCitizens
            };

            for (int improvNo = 0; improvNo < 34; improvNo++)
                if (improvements[improvNo]) city.AddImprovement(Rules.Improvements[improvNo+1]);

            // TODO: add wonders to city at city import
            //if (wonders[0]) city.AddImprovement(new Improvement(ImprovementType.Pyramids));
            //if (wonders[1]) city.AddImprovement(new Improvement(ImprovementType.HangingGardens));
            //if (wonders[2]) city.AddImprovement(new Improvement(ImprovementType.Colossus));
            //if (wonders[3]) city.AddImprovement(new Improvement(ImprovementType.Lighthouse));
            //if (wonders[4]) city.AddImprovement(new Improvement(ImprovementType.GreatLibrary));
            //if (wonders[5]) city.AddImprovement(new Improvement(ImprovementType.Oracle));
            //if (wonders[6]) city.AddImprovement(new Improvement(ImprovementType.GreatWall));
            //if (wonders[7]) city.AddImprovement(new Improvement(ImprovementType.WarAcademy));
            //if (wonders[8]) city.AddImprovement(new Improvement(ImprovementType.KR_Crusade));
            //if (wonders[9]) city.AddImprovement(new Improvement(ImprovementType.MP_Embassy));
            //if (wonders[10]) city.AddImprovement(new Improvement(ImprovementType.MichChapel));
            //if (wonders[11]) city.AddImprovement(new Improvement(ImprovementType.CoperObserv));
            //if (wonders[12]) city.AddImprovement(new Improvement(ImprovementType.MagellExped));
            //if (wonders[13]) city.AddImprovement(new Improvement(ImprovementType.ShakespTheat));
            //if (wonders[14]) city.AddImprovement(new Improvement(ImprovementType.DV_Workshop));
            //if (wonders[15]) city.AddImprovement(new Improvement(ImprovementType.JSB_Cathedral));
            //if (wonders[16]) city.AddImprovement(new Improvement(ImprovementType.IN_College));
            //if (wonders[17]) city.AddImprovement(new Improvement(ImprovementType.TradingCompany));
            //if (wonders[18]) city.AddImprovement(new Improvement(ImprovementType.DarwinVoyage));
            //if (wonders[19]) city.AddImprovement(new Improvement(ImprovementType.StatueLiberty));
            //if (wonders[20]) city.AddImprovement(new Improvement(ImprovementType.EiffelTower));
            //if (wonders[21]) city.AddImprovement(new Improvement(ImprovementType.HooverDam));
            //if (wonders[22]) city.AddImprovement(new Improvement(ImprovementType.WomenSuffrage));
            //if (wonders[23]) city.AddImprovement(new Improvement(ImprovementType.ManhattanProj));
            //if (wonders[24]) city.AddImprovement(new Improvement(ImprovementType.UnitedNations));
            //if (wonders[25]) city.AddImprovement(new Improvement(ImprovementType.ApolloProgr));
            //if (wonders[26]) city.AddImprovement(new Improvement(ImprovementType.SETIProgr));
            //if (wonders[27]) city.AddImprovement(new Improvement(ImprovementType.CureCancer));

            GetCities.Add(city);
        }

        public void CreateCiv(int id, int whichHumanPlayerIsUsed, bool alive, int style, string leaderName, string tribeName, string adjective,
                            int gender, int money, int tribeNumber, int researchProgress, int researchingTech, int sciRate, int taxRate,
                            int government, int reputation, bool[] techs)
        {
            var tribe = Rules.Leaders[tribeNumber];
            // If leader name string is empty (no manual input), find the name in RULES.TXT (don't search for barbarians)
            if (id != 0 && leaderName.Length == 0) leaderName = (gender == 0) ? tribe.NameMale : tribe.NameFemale;

            // If tribe name string is empty (no manual input), find the name in RULES.TXT (don't search for barbarians)
            if (id != 0 && tribeName.Length == 0) tribeName = tribe.Plural;

            // If adjective string is empty (no manual input), find adjective in RULES.TXT (don't search for barbarians)
            if (id != 0 && adjective.Length == 0) adjective = tribe.Adjective;

            // Set citystyle from input only for player civ. Other civs (AI) have set citystyle from RULES.TXT
            if (id != 0 && id != whichHumanPlayerIsUsed) style = tribe.CityStyle;

            var gov = Rules.Governments[government];
            Civilization civ = new Civilization
            {
                Id = id,
                Alive = alive,
                CityStyle = (CityStyleType)style,
                LeaderName = leaderName,
                LeaderGender = gender,
                LeaderTitle = (gender == 0) ? gov.TitleMale : gov.TitleFemale,
                TribeName = tribeName,
                Adjective = adjective,
                Money = money,
                ReseachingTech = researchingTech,
                Techs = techs,
                ScienceRate = sciRate * 10,
                TaxRate = taxRate * 10,
                Government = (GovernmentType)government
            };

            GetCivs.Add(civ);
        }

        // Singleton instance of a game
        private static Game _instance;
        private readonly Map[] _maps;

        public static Game Instance
        {
            get
            {
                if (_instance == null)
                {
                    Console.WriteLine("Game instance does not exist!");
                }
                return _instance;
            }
        }

        private int _currentMap = 0;

        public Map CurrentMap => _maps[_currentMap];

        public void TriggerUnitEvent(UnitEventType eventType, Unit movedUnit, BlockedReason blockedReason = BlockedReason.NotBlocked)
        {
            OnUnitEvent?.Invoke(this,new UnitEventArgs(eventType, movedUnit, blockedReason));
        }

        public void TriggerUnitEvent(UnitEventArgs args)
        {
            OnUnitEvent?.Invoke(this,args);
        }
        public void TriggerMapEvent(MapEventType eventType, List<Tile> neighbours)
        {
            OnMapEvent?.Invoke(this,new MapEventArgs(eventType));
        }

    }
}
