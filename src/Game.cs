using System.Collections.Generic;
using System.Drawing;
using civ2.Enums;
using civ2.Units;
using civ2.Terrains;
using civ2.Improvements;
using civ2.Sounds;
using civ2.Bitmaps;

namespace civ2
{
    public partial class Game
    {
        public static List<IUnit> Units = new List<IUnit>();
        public static List<IUnit> DeadUnits = new List<IUnit>();
        public static List<City> Cities = new List<City>();
        public static List<Civilization> Civs = new List<Civilization>();
        public static ITerrain[,] TerrainTile;
        public static Bitmap[] CivsMap;

        #region Loads stuff when civ2 starts
        public static void Preloading(string civ2path)
        {
            //Images.LoadDLLimages();
            Sound.LoadSounds(civ2path);
        }
        #endregion  

        public static void StartNewGame()
        {
        }

        public static void LoadGame(string SAVpath)
        {
            ReadFiles.ReadRULES(Settings.Civ2Path + "RULES.TXT");
            ImportSAV(SAVpath);
            Images.CreateLoadGameGraphics();
            Game.Instance.ActiveUnit = Data.SelectedUnitIndex == -1 ? null : Game.Units.Find(unit => unit.Id == Data.SelectedUnitIndex);    //null means all units have ended turn
            Game.Instance.ActiveCiv = Civs[Data.HumanPlayer];

            //FOR HELP!!!
            //foreach (IUnit unit in Game.Units)
            //{
            //    if (unit.Civ == 5 && unit.HomeCity != 255)
            //        Console.WriteLine($"Unit, Id={unit.Id}, {Civs[unit.Civ].TribeName}, {unit.Type}, ({unit.X},{unit.Y}), Lastmove={unit.LastMove}, Firstmove={unit.FirstMove}," +
            //            $"order={unit.Order}, {Cities[unit.HomeCity].Name}");
            //}
            //for (int i = 0; i < 8; i++)
            //{
            //    Console.WriteLine("Civ{0}, AnyUnitsAwaitingOrders={1}", i, Actions.AnyUnitsAwaitingOrders(i));
            //}

        }

        private IUnit _activeUnit;
        public IUnit ActiveUnit
        {
            get { return _activeUnit; }
            set { _activeUnit = value; }
        }

        private Civilization _activeCiv;
        public Civilization ActiveCiv
        {
            get { return _activeCiv; }
            set { _activeCiv = value; }
        }

        public static void CreateTerrain(int x, int y, TerrainType type, int specialtype, bool resource, bool river, int island, bool unit_present, bool city_present, bool irrigation, bool mining, bool road, bool railroad, bool fortress, bool pollution, bool farmland, bool airbase, bool[] visibility, string hexvalue)
        {
            ITerrain tile;
            SpecialType? stype = null;
            switch (type)
            {
                case TerrainType.Desert:
                    {
                        if (specialtype == 1) stype = SpecialType.Oasis;
                        if (specialtype == 2) stype = SpecialType.DesertOil;
                        break;
                    }
                case TerrainType.Plains:
                    {
                        if (specialtype == 1) stype = SpecialType.Buffalo;
                        if (specialtype == 2) stype = SpecialType.Wheat;
                        break;
                    }
                case TerrainType.Grassland:
                    {
                        if (specialtype == 1) stype = SpecialType.GrasslandShield;
                        if (specialtype == 2) stype = SpecialType.Grassland;
                        break;
                    }
                case TerrainType.Forest:
                    {
                        if (specialtype == 1) stype = SpecialType.Pheasant;
                        if (specialtype == 2) stype = SpecialType.Silk;
                        break;
                    }
                case TerrainType.Hills:
                    {
                        if (specialtype == 1) stype = SpecialType.Coal;
                        if (specialtype == 2) stype = SpecialType.Wine;
                        break;
                    }
                case TerrainType.Mountains:
                    {
                        if (specialtype == 1) stype = SpecialType.Gold;
                        if (specialtype == 2) stype = SpecialType.Iron;
                        break;
                    }
                case TerrainType.Tundra:
                    {
                        if (specialtype == 1) stype = SpecialType.Game;
                        if (specialtype == 2) stype = SpecialType.Furs;
                        break;
                    }
                case TerrainType.Glacier:
                    {
                        if (specialtype == 1) stype = SpecialType.Ivory;
                        if (specialtype == 2) stype = SpecialType.GlacierOil;
                        break;
                    }
                case TerrainType.Swamp:
                    {
                        if (specialtype == 1) stype = SpecialType.Peat;
                        if (specialtype == 2) stype = SpecialType.Spice;
                        break;
                    }
                case TerrainType.Jungle:
                    {
                        if (specialtype == 1) stype = SpecialType.Gems;
                        if (specialtype == 2) stype = SpecialType.Fruit;
                        break;
                    }
                case TerrainType.Ocean:
                    {
                        if (specialtype == 1) stype = SpecialType.Fish;
                        if (specialtype == 2) stype = SpecialType.Whales;
                        break;
                    }
                default: return ;
            }
            tile = new Terrain(type, stype)
            {
                Type = type,
                SpecType = stype,
                Resource = resource,
                River = river,
                Island = island,
                UnitPresent = unit_present,
                CityPresent = city_present,
                Irrigation = irrigation,
                Mining = mining,
                Road = road,
                Railroad = railroad,
                Fortress = fortress,
                Pollution = pollution,
                Farmland = farmland,
                Airbase = airbase,
                Visibility = visibility,
                Hexvalue = hexvalue
            };
            TerrainTile[x, y] = tile;
        }

        public static IUnit CreateUnit(UnitType type, int x, int y, bool dead, bool firstMove, bool greyStarShield, bool veteran, int civId, int movePointsLost, int hitpointsLost, int lastMove, int caravanCommodity, OrderType orders, int homeCity, int goToX, int goToY, int linkOtherUnitsOnTop, int linkOtherUnitsUnder)
        {
            IUnit unit;
            unit = new Unit(type)
            {
                Id = Game.Units.Count + Game.DeadUnits.Count,
                Type = type,
                X = x,
                Y = y,
                FirstMove = firstMove,
                GreyStarShield = greyStarShield,
                Veteran = veteran,
                CivId = civId,
                MaxMovePoints = 3 * ReadFiles.UnitMove[(int)type],
                MovePoints = 3 * ReadFiles.UnitMove[(int)type] - movePointsLost,
                MaxHitPoints = 10 * ReadFiles.UnitHitp[(int)type],
                HitPoints = 10 * ReadFiles.UnitHitp[(int)type] - hitpointsLost,
                LastMove = lastMove,
                CaravanCommodity = caravanCommodity,
                Order = orders,
                HomeCity = homeCity,
                GoToX = goToX,
                GoToY = goToY,
                LinkOtherUnitsOnTop = linkOtherUnitsOnTop,
                LinkOtherUnitsUnder = linkOtherUnitsUnder
            };

            if (dead) DeadUnits.Add(unit);
            else Units.Add(unit);

            return unit;
        }
        
        public static City CreateCity(int x, int y, bool canBuildCoastal, bool autobuildMilitaryRule, bool stolenTech, bool improvementSold, bool weLoveKingDay, bool civilDisorder, 
                                      bool canBuildShips, bool objectivex3, bool objectivex1, int owner, int size, int whoBuiltIt, int foodInStorage, int shieldsProgress, int netTrade, 
                                      string name, int[] distributionWorkers, int noOfSpecialistsx4, bool[] improvements, int itemInProduction, int activeTradeRoutes, int[] commoditySupplied,
                                      int[] commodityDemanded, int[] commodityInRoute, int[] tradeRoutePartnerCity, int science, int tax, int noOfTradeIcons, int foodProduction, 
                                      int shieldProduction, int happyCitizens, int unhappyCitizens, bool[] wonders)
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
                Owner = owner,
                Size = size,
                WhoBuiltIt = whoBuiltIt,
                FoodInStorage = foodInStorage,
                ShieldsProgress = shieldsProgress,
                NetTrade = netTrade,
                Name = name,
                DistributionWorkers = distributionWorkers,
                NoOfSpecialistsx4 = noOfSpecialistsx4,
                ItemInProduction = itemInProduction,
                ActiveTradeRoutes = activeTradeRoutes,
                CommoditySupplied = new CommodityType[] { (CommodityType)commoditySupplied[0], (CommodityType)commoditySupplied[1], (CommodityType)commoditySupplied[2] },
                CommodityDemanded = new CommodityType[] { (CommodityType)commodityDemanded[0], (CommodityType)commodityDemanded[1], (CommodityType)commodityDemanded[2] },
                CommodityInRoute = new CommodityType[] { (CommodityType)commodityInRoute[0], (CommodityType)commodityInRoute[1], (CommodityType)commodityInRoute[2] },
                TradeRoutePartnerCity = new int[] { tradeRoutePartnerCity[0], tradeRoutePartnerCity[1], tradeRoutePartnerCity[2] },
                //Science = science,    //what does this mean???
                //Tax = tax,
                //NoOfTradeIcons = noOfTradeIcons,
                FoodProduction = foodProduction,
                ShieldProduction = shieldProduction,
                HappyCitizens = happyCitizens,
                UnhappyCitizens = unhappyCitizens
            };

            if (improvements[0]) city.AddImprovement(new Improvement(ImprovementType.Palace));
            if (improvements[1]) city.AddImprovement(new Improvement(ImprovementType.Barracks));
            if (improvements[2]) city.AddImprovement(new Improvement(ImprovementType.Granary));
            if (improvements[3]) city.AddImprovement(new Improvement(ImprovementType.Temple));
            if (improvements[4]) city.AddImprovement(new Improvement(ImprovementType.Marketplace));
            if (improvements[5]) city.AddImprovement(new Improvement(ImprovementType.Library));
            if (improvements[6]) city.AddImprovement(new Improvement(ImprovementType.Courthouse));
            if (improvements[7]) city.AddImprovement(new Improvement(ImprovementType.CityWalls));
            if (improvements[8]) city.AddImprovement(new Improvement(ImprovementType.Aqueduct));
            if (improvements[9]) city.AddImprovement(new Improvement(ImprovementType.Bank));
            if (improvements[10]) city.AddImprovement(new Improvement(ImprovementType.Cathedral));
            if (improvements[11]) city.AddImprovement(new Improvement(ImprovementType.University));
            if (improvements[12]) city.AddImprovement(new Improvement(ImprovementType.MassTransit));
            if (improvements[13]) city.AddImprovement(new Improvement(ImprovementType.Colosseum));
            if (improvements[14]) city.AddImprovement(new Improvement(ImprovementType.Factory));
            if (improvements[15]) city.AddImprovement(new Improvement(ImprovementType.MfgPlant));
            if (improvements[16]) city.AddImprovement(new Improvement(ImprovementType.SDIdefense));
            if (improvements[17]) city.AddImprovement(new Improvement(ImprovementType.RecyclCentre));
            if (improvements[18]) city.AddImprovement(new Improvement(ImprovementType.PowerPlant));
            if (improvements[19]) city.AddImprovement(new Improvement(ImprovementType.HydroPlant));
            if (improvements[20]) city.AddImprovement(new Improvement(ImprovementType.NuclearPlant));
            if (improvements[21]) city.AddImprovement(new Improvement(ImprovementType.StockExch));
            if (improvements[22]) city.AddImprovement(new Improvement(ImprovementType.SewerSystem));
            if (improvements[23]) city.AddImprovement(new Improvement(ImprovementType.Supermarket));
            if (improvements[24]) city.AddImprovement(new Improvement(ImprovementType.Superhighways));
            if (improvements[25]) city.AddImprovement(new Improvement(ImprovementType.ResearchLab));
            if (improvements[26]) city.AddImprovement(new Improvement(ImprovementType.SAMbattery));
            if (improvements[27]) city.AddImprovement(new Improvement(ImprovementType.CoastalFort));
            if (improvements[28]) city.AddImprovement(new Improvement(ImprovementType.SolarPlant));
            if (improvements[29]) city.AddImprovement(new Improvement(ImprovementType.Harbour));
            if (improvements[30]) city.AddImprovement(new Improvement(ImprovementType.OffshorePlat));
            if (improvements[31]) city.AddImprovement(new Improvement(ImprovementType.Airport));
            if (improvements[32]) city.AddImprovement(new Improvement(ImprovementType.PoliceStat));
            if (improvements[33]) city.AddImprovement(new Improvement(ImprovementType.PortFacil));

            if (wonders[0]) city.AddImprovement(new Improvement(ImprovementType.Pyramids));
            if (wonders[1]) city.AddImprovement(new Improvement(ImprovementType.HangingGardens));
            if (wonders[2]) city.AddImprovement(new Improvement(ImprovementType.Colossus));
            if (wonders[3]) city.AddImprovement(new Improvement(ImprovementType.Lighthouse));
            if (wonders[4]) city.AddImprovement(new Improvement(ImprovementType.GreatLibrary));
            if (wonders[5]) city.AddImprovement(new Improvement(ImprovementType.Oracle));
            if (wonders[6]) city.AddImprovement(new Improvement(ImprovementType.GreatWall));
            if (wonders[7]) city.AddImprovement(new Improvement(ImprovementType.WarAcademy));
            if (wonders[8]) city.AddImprovement(new Improvement(ImprovementType.KR_Crusade));
            if (wonders[9]) city.AddImprovement(new Improvement(ImprovementType.MP_Embassy));
            if (wonders[10]) city.AddImprovement(new Improvement(ImprovementType.MichChapel));
            if (wonders[11]) city.AddImprovement(new Improvement(ImprovementType.CoperObserv));
            if (wonders[12]) city.AddImprovement(new Improvement(ImprovementType.MagellExped));
            if (wonders[13]) city.AddImprovement(new Improvement(ImprovementType.ShakespTheat));
            if (wonders[14]) city.AddImprovement(new Improvement(ImprovementType.DV_Workshop));
            if (wonders[15]) city.AddImprovement(new Improvement(ImprovementType.JSB_Cathedral));
            if (wonders[16]) city.AddImprovement(new Improvement(ImprovementType.IN_College));
            if (wonders[17]) city.AddImprovement(new Improvement(ImprovementType.TradingCompany));
            if (wonders[18]) city.AddImprovement(new Improvement(ImprovementType.DarwinVoyage));
            if (wonders[19]) city.AddImprovement(new Improvement(ImprovementType.StatueLiberty));
            if (wonders[20]) city.AddImprovement(new Improvement(ImprovementType.EiffelTower));
            if (wonders[21]) city.AddImprovement(new Improvement(ImprovementType.HooverDam));
            if (wonders[22]) city.AddImprovement(new Improvement(ImprovementType.WomenSuffrage));
            if (wonders[23]) city.AddImprovement(new Improvement(ImprovementType.ManhattanProj));
            if (wonders[24]) city.AddImprovement(new Improvement(ImprovementType.UnitedNations));
            if (wonders[25]) city.AddImprovement(new Improvement(ImprovementType.ApolloProgr));
            if (wonders[26]) city.AddImprovement(new Improvement(ImprovementType.SETIProgr));
            if (wonders[27]) city.AddImprovement(new Improvement(ImprovementType.CureCancer));

            Cities.Add(city);
            return city;
        }

        public static Civilization CreateCiv(int id, int whichHumanPlayerIsUsed, int style, string leaderName, string tribeName, string adjective, int gender, int money, int tribeNumber, 
                                             int researchProgress, int researchingTech, int sciRate, int taxRate, int government, int reputation, int[] techs)
        {
            //if leader name string is empty (no manual input), find the name in RULES.TXT (don't search for barbarians)
            if (id != 0 && leaderName == "") leaderName = (gender == 0) ? ReadFiles.LeaderNameHIS[tribeNumber] : ReadFiles.LeaderNameHER[tribeNumber];

            //if tribe name string is empty (no manual input), find the name in RULES.TXT (don't search for barbarians)
            if (id != 0 && tribeName == "") tribeName = ReadFiles.LeaderPlural[tribeNumber];

            //if adjective string is empty (no manual input), find adjective in RULES.TXT (don't search for barbarians)
            if (id != 0 && adjective == "") adjective = ReadFiles.LeaderAdjective[tribeNumber];

            //Set citystyle from input only for player civ. Other civs (AI) have set citystyle from RULES.TXT
            if (id != 0 && id != whichHumanPlayerIsUsed) style = ReadFiles.LeaderCityStyle[tribeNumber];

            Civilization civ = new Civilization
            {
                Id = id,
                CityStyle = style,
                LeaderName = leaderName,
                TribeName = tribeName,
                Adjective = adjective,
                Money = money,
                ReseachingTech = researchingTech,
                Techs = techs,
                ScienceRate = sciRate * 10,
                TaxRate = taxRate * 10,
                Government = (GovernmentType)government
            };

            Civs.Add(civ);
            return civ;
        }

        
        private static Game instance;  //Singleton
        public static Game Instance //Singleton
        {
            get
            {
                if (instance == null) instance = new Game();
                return instance;
            }
        }

    }
}
