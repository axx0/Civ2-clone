using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PoskusCiv2.Enums;
using PoskusCiv2.Units;
using PoskusCiv2.Terrains;
using PoskusCiv2.Improvements;
using PoskusCiv2.Forms;
using PoskusCiv2.Imagery;
using ExtensionMethods;
using System.Drawing;

namespace PoskusCiv2
{
    public partial class Game
    {
        public static List<IUnit> Units = new List<IUnit>();
        public static List<IUnit> DeadUnits = new List<IUnit>();
        public static List<City> Cities = new List<City>();
        public static List<Civilization> Civs = new List<Civilization>();        
        public static ITerrain[,] Terrain;
        public static Options Options;
        public static Data Data;
        public static Bitmap Map;

        //load sound for moving piece
        //System.Media.SoundPlayer moveSound = new System.Media.SoundPlayer(@"C:\DOS\CIV 2\Civ2\Sound\MOVPIECE.WAV");
        //System.Media.SoundPlayer fightSound = new System.Media.SoundPlayer(@"C:\DOS\CIV 2\Civ2\Sound\SWORDFGT.WAV");

        public static void StartGame()
        {
            //Console.WriteLine("Active units:");
            //foreach (IUnit unit in Game.Units)  //List active units
            //{
            //    Console.WriteLine("{0} real=({1},{2}), civ2=({3},{4})", unit.Name, unit.X, unit.Y, unit.X2, unit.Y2);
            //}
            //Console.WriteLine("Dead units:");
            //foreach (IUnit unit in Game.DeadUnits)  //List dead units
            //{
            //    Console.WriteLine("{0} real=({1},{2}), civ2=({3},{4})", unit.Name, unit.X, unit.Y, unit.X2, unit.Y2);
            //}

            //Draw game map
            //Draw Draw = new Draw();
            Map = Draw.DrawMap(); //prepare whole game map

            //Set active unit at game start
            Game.Instance.ActiveUnit = Game.Units[Data.UnitSelectedAtGameStart];
        }

        private int _activeUnit;
        public IUnit ActiveUnit
        {
            get { return Units[_activeUnit]; }
            set { _activeUnit = Units.IndexOf(value); }
        }

        public static void SetOptions(int version, bool bloodlust, bool simplifiedCombat, bool flatEarth, bool dontRestartIfEliminated, bool moveUnitsWithoutMouse, bool enterClosestCityScreen, bool grid, bool soundEffects, bool music, bool cheatMenu, bool alwaysWaitAtEndOfTurn, bool autosaveEachTurn, bool showEnemyMoves, bool noPauseAfterEnemyMoves, bool fastPieceSlide, bool instantAdvice, bool tutorialHelp, bool animatedHeralds, bool highCouncil, bool civilopediaForAdvances, bool throneRoomGraphics, bool diplomacyScreenGraphics, bool wonderMovies, bool cheatPenaltyWarning, bool announceWeLoveKingDay, bool warnWhenFoodDangerouslyLow, bool announceCitiesInDisorder, bool announceOrderRestored, bool showNonCombatUnitsBuilt, bool showInvalidBuildInstructions, bool warnWhenCityGrowthHalted, bool showCityImprovementsBuilt, bool zoomToCityNotDefaultAction, bool warnWhenPollutionOccurs, bool warnWhenChangingProductionWillCostShields)
        {
            Options opt = new Options
            {
                Version = version,
                Bloodlust = bloodlust,
                SimplifiedCombat = simplifiedCombat,
                FlatEarth = flatEarth,
                DontRestartIfEliminated = dontRestartIfEliminated,
                MoveUnitsWithoutMouse = moveUnitsWithoutMouse,
                EnterClosestCityScreen = enterClosestCityScreen,
                Grid = grid,
                SoundEffects = soundEffects,
                Music = music,
                CheatMenu = cheatMenu,
                AlwaysWaitAtEndOfTurn = alwaysWaitAtEndOfTurn,
                AutosaveEachTurn = autosaveEachTurn,
                ShowEnemyMoves = showEnemyMoves,
                NoPauseAfterEnemyMoves = noPauseAfterEnemyMoves,
                FastPieceSlide = fastPieceSlide,
                InstantAdvice = instantAdvice,
                TutorialHelp = tutorialHelp,
                AnimatedHeralds = animatedHeralds,
                HighCouncil = highCouncil,
                CivilopediaForAdvances = civilopediaForAdvances,
                ThroneRoomGraphics = throneRoomGraphics,
                DiplomacyScreenGraphics = diplomacyScreenGraphics,
                WonderMovies = wonderMovies,
                CheatPenaltyWarning = cheatPenaltyWarning,
                AnnounceWeLoveKingDay = announceWeLoveKingDay,
                WarnWhenFoodDangerouslyLow = warnWhenFoodDangerouslyLow,
                AnnounceCitiesInDisorder = announceCitiesInDisorder,
                AnnounceOrderRestored = announceOrderRestored,
                ShowNonCombatUnitsBuilt = showNonCombatUnitsBuilt,
                ShowInvalidBuildInstructions = showInvalidBuildInstructions,
                WarnWhenCityGrowthHalted = warnWhenCityGrowthHalted,
                ShowCityImprovementsBuilt = showCityImprovementsBuilt,
                ZoomToCityNotDefaultAction = zoomToCityNotDefaultAction,
                WarnWhenPollutionOccurs = warnWhenPollutionOccurs,
                WarnWhenChangingProductionWillCostShields = warnWhenChangingProductionWillCostShields
            };

            Options = opt;
        }


        public static void SetGameData(int turnNumber, int turnNumberForGameYear, int unitSelectedAtGameStart, int whichHumanPlayerIsUsed, int[] civsInPlay, int playersMapUsed, int playersCivilizationNumberUsed, bool mapRevealed, int difficultyLevel, int barbarianActivity, int pollutionAmount, int globalTempRiseOccured, int noOfTurnsOfPeace, int numberOfUnits, int numberOfCities, int mapxdim, int mapydim, int mapArea, int mapSeed, int locatorX, int locatorY)
        {
            Data data = new Data
            {
                TurnNumber = turnNumber,
                TurnNumberForGameYear = turnNumberForGameYear,
                UnitSelectedAtGameStart = unitSelectedAtGameStart,
                HumanPlayerUsed = whichHumanPlayerIsUsed,
                CivsInPlay = civsInPlay,
                PlayersMapUsed = playersMapUsed,
                PlayersCivilizationNumberUsed = playersCivilizationNumberUsed,
                MapRevealed = mapRevealed,
                DifficultyLevel = difficultyLevel,
                BarbarianActivity = barbarianActivity,
                PollutionAmount = pollutionAmount,
                GlobalTempRiseOccured = globalTempRiseOccured,
                NoOfTurnsOfPeace = noOfTurnsOfPeace,
                NumberOfUnits = numberOfUnits,
                NumberOfCities = numberOfCities,
                MapXdim = mapxdim,
                MapYdim = mapydim,
                MapArea = mapArea,
                MapSeed = mapSeed,
                MapLocatorXdim = locatorX,
                MapLocatorYdim = locatorY
            };

            Data = data;
        }

        public static void CreateTerrain(int x, int y, TerrainType type, int specialtype, bool resource, bool river, int island, bool unit_present, bool city_present, bool irrigation, bool mining, bool road, bool railroad, bool fortress, bool pollution, bool farmland, bool airbase, string hexvalue)
        {
            ITerrain terrain;
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
            terrain = new Terrain(type, stype)
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
                Hexvalue = hexvalue
            };
            Terrain[x, y] = terrain;
        }

        public static IUnit CreateUnit(UnitType type, int x, int y, bool dead, bool firstMove, bool greyStarShield, bool veteran, int civ, int movePointsLost, int hitpointsLost, int lastMove, int caravanCommodity, OrderType orders, int homeCity, int goToX, int goToY, int linkOtherUnitsOnTop, int linkOtherUnitsUnder)
        {
            IUnit unit;
            unit = new Unit(type)
            {
                Type = type,
                X = x,
                Y = y,
                FirstMove = firstMove,
                GreyStarShield = greyStarShield,
                Veteran = veteran,
                Civ = civ,
                MovePointsLost = movePointsLost,
                HitpointsLost = hitpointsLost,
                LastMove = lastMove,
                CaravanCommodity = caravanCommodity,
                Action = orders,
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
        
        public static City CreateCity(int x, int y, bool canBuildCoastal, bool autobuildMilitaryRule, bool stolenTech, bool improvementSold, bool weLoveKingDay, bool civilDisorder, bool canBuildShips, bool objectivex3, bool objectivex1, int owner, int size, int whoBuiltIt, int foodInStorage, int shieldsProgress, int netTrade, string name, int workersInnerCircle, int workersOn8, int workersOn4, int noOfSpecialistsx4, string improvements, int itemInProduction, int activeTradeRoutes, int science, int tax, int noOfTradeIcons, int foodProduction, int shieldProduction, int happyCitizens, int unhappyCitizens, int[] wonders)
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
                WorkersInnerCircle = workersInnerCircle,
                WorkersOn8 = workersOn8,
                WorkersOn4 = workersOn4,
                NoOfSpecialistsx4 = noOfSpecialistsx4,
                ItemInProduction = itemInProduction,
                ActiveTradeRoutes = activeTradeRoutes,
                Science = science,
                Tax = tax,
                NoOfTradeIcons = noOfTradeIcons,
                FoodProduction = foodProduction,
                ShieldProduction = shieldProduction,
                HappyCitizens = happyCitizens,
                UnhappyCitizens = unhappyCitizens
            };

            if (improvements[0] == '1') city.AddImprovement(new Improvement(ImprovementType.Palace));
            if (improvements[1] == '1') city.AddImprovement(new Improvement(ImprovementType.Barracks));
            if (improvements[2] == '1') city.AddImprovement(new Improvement(ImprovementType.Granary));
            if (improvements[3] == '1') city.AddImprovement(new Improvement(ImprovementType.Temple));
            if (improvements[4] == '1') city.AddImprovement(new Improvement(ImprovementType.Marketplace));
            if (improvements[5] == '1') city.AddImprovement(new Improvement(ImprovementType.Library));
            if (improvements[6] == '1') city.AddImprovement(new Improvement(ImprovementType.Courthouse));
            if (improvements[7] == '1') city.AddImprovement(new Improvement(ImprovementType.CityWalls));
            if (improvements[8] == '1') city.AddImprovement(new Improvement(ImprovementType.Aqueduct));
            if (improvements[9] == '1') city.AddImprovement(new Improvement(ImprovementType.Bank));
            if (improvements[10] == '1') city.AddImprovement(new Improvement(ImprovementType.Cathedral));
            if (improvements[11] == '1') city.AddImprovement(new Improvement(ImprovementType.University));
            if (improvements[12] == '1') city.AddImprovement(new Improvement(ImprovementType.MassTransit));
            if (improvements[13] == '1') city.AddImprovement(new Improvement(ImprovementType.Colosseum));
            if (improvements[14] == '1') city.AddImprovement(new Improvement(ImprovementType.Factory));
            if (improvements[15] == '1') city.AddImprovement(new Improvement(ImprovementType.MfgPlant));
            if (improvements[16] == '1') city.AddImprovement(new Improvement(ImprovementType.SDIdefense));
            if (improvements[17] == '1') city.AddImprovement(new Improvement(ImprovementType.RecyclCentre));
            if (improvements[18] == '1') city.AddImprovement(new Improvement(ImprovementType.PowerPlant));
            if (improvements[19] == '1') city.AddImprovement(new Improvement(ImprovementType.HydroPlant));
            if (improvements[20] == '1') city.AddImprovement(new Improvement(ImprovementType.NuclearPlant));
            if (improvements[21] == '1') city.AddImprovement(new Improvement(ImprovementType.StockExch));
            if (improvements[22] == '1') city.AddImprovement(new Improvement(ImprovementType.SewerSystem));
            if (improvements[23] == '1') city.AddImprovement(new Improvement(ImprovementType.Supermarket));
            if (improvements[24] == '1') city.AddImprovement(new Improvement(ImprovementType.Superhighways));
            if (improvements[25] == '1') city.AddImprovement(new Improvement(ImprovementType.ResearchLab));
            if (improvements[26] == '1') city.AddImprovement(new Improvement(ImprovementType.SAMbattery));
            if (improvements[27] == '1') city.AddImprovement(new Improvement(ImprovementType.CoastalFort));
            if (improvements[28] == '1') city.AddImprovement(new Improvement(ImprovementType.SolarPlant));
            if (improvements[29] == '1') city.AddImprovement(new Improvement(ImprovementType.Harbour));
            if (improvements[30] == '1') city.AddImprovement(new Improvement(ImprovementType.OffshorePlat));
            if (improvements[31] == '1') city.AddImprovement(new Improvement(ImprovementType.Airport));
            if (improvements[32] == '1') city.AddImprovement(new Improvement(ImprovementType.PoliceStat));
            if (improvements[33] == '1') city.AddImprovement(new Improvement(ImprovementType.PortFacil));

            if (wonders[0] == 1) city.AddImprovement(new Improvement(ImprovementType.Pyramids));
            if (wonders[1] == 1) city.AddImprovement(new Improvement(ImprovementType.HangingGardens));
            if (wonders[2] == 1) city.AddImprovement(new Improvement(ImprovementType.Colossus));
            if (wonders[3] == 1) city.AddImprovement(new Improvement(ImprovementType.Lighthouse));
            if (wonders[4] == 1) city.AddImprovement(new Improvement(ImprovementType.GreatLibrary));
            if (wonders[5] == 1) city.AddImprovement(new Improvement(ImprovementType.Oracle));
            if (wonders[6] == 1) city.AddImprovement(new Improvement(ImprovementType.GreatWall));
            if (wonders[7] == 1) city.AddImprovement(new Improvement(ImprovementType.WarAcademy));
            if (wonders[8] == 1) city.AddImprovement(new Improvement(ImprovementType.KR_Crusade));
            if (wonders[9] == 1) city.AddImprovement(new Improvement(ImprovementType.MP_Embassy));
            if (wonders[10] == 1) city.AddImprovement(new Improvement(ImprovementType.MichChapel));
            if (wonders[11] == 1) city.AddImprovement(new Improvement(ImprovementType.CoperObserv));
            if (wonders[12] == 1) city.AddImprovement(new Improvement(ImprovementType.MagellExped));
            if (wonders[13] == 1) city.AddImprovement(new Improvement(ImprovementType.ShakespTheat));
            if (wonders[14] == 1) city.AddImprovement(new Improvement(ImprovementType.DV_Workshop));
            if (wonders[15] == 1) city.AddImprovement(new Improvement(ImprovementType.JSB_Cathedral));
            if (wonders[16] == 1) city.AddImprovement(new Improvement(ImprovementType.IN_College));
            if (wonders[17] == 1) city.AddImprovement(new Improvement(ImprovementType.TradingCompany));
            if (wonders[18] == 1) city.AddImprovement(new Improvement(ImprovementType.DarwinVoyage));
            if (wonders[19] == 1) city.AddImprovement(new Improvement(ImprovementType.StatueLiberty));
            if (wonders[20] == 1) city.AddImprovement(new Improvement(ImprovementType.EiffelTower));
            if (wonders[21] == 1) city.AddImprovement(new Improvement(ImprovementType.HooverDam));
            if (wonders[22] == 1) city.AddImprovement(new Improvement(ImprovementType.WomenSuffrage));
            if (wonders[23] == 1) city.AddImprovement(new Improvement(ImprovementType.ManhattanProj));
            if (wonders[24] == 1) city.AddImprovement(new Improvement(ImprovementType.UnitedNations));
            if (wonders[25] == 1) city.AddImprovement(new Improvement(ImprovementType.ApolloProgr));
            if (wonders[26] == 1) city.AddImprovement(new Improvement(ImprovementType.SETIProgr));
            if (wonders[27] == 1) city.AddImprovement(new Improvement(ImprovementType.CureCancer));

            Cities.Add(city);
            return city;
        }

        public static Civilization CreateCiv(int id, int whichHumanPlayerIsUsed, int style, string leaderName, string tribeName, string adjective, int gender, int money, int tribeNumber, int researchProgress, int researchingTech, int taxRate, int government, int reputation, int[] techs)
        {
            //if leader name string is empty (no manual input), find the name in RULES.TXT (don't search for barbarians)
            if (id != 0 && leaderName == "")
            {
                if (gender == 0) leaderName = ReadFiles.LeaderNameHIS[tribeNumber];
                else leaderName = ReadFiles.LeaderNameHER[tribeNumber];
            }

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
                Techs = techs
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
