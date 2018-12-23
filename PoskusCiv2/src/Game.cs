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

        public static void CreateTerrain(int x, int y, TerrainType type, int specialtype, bool resource, bool river, int island, bool unit_present, bool city_present, bool irrigation, bool mining, bool road, bool railroad, bool fortress, bool pollution, bool farmland, bool airbase, string hexvalue)
        {
            ITerrain terrain;
            SpecialType special = SpecialType.NoSpecial;
            switch (type)
            {
                case TerrainType.Desert:
                    {
                        if (specialtype == 1) { special = SpecialType.Oasis; }
                        if (specialtype == 2) { special = SpecialType.DesertOil; }
                        terrain = new Desert(special);
                        break;
                    }
                case TerrainType.Plains:
                    {
                        if (specialtype == 1) { special = SpecialType.Buffalo; }
                        if (specialtype == 2) { special = SpecialType.Wheat; }
                        terrain = new Plains(special);
                        break;
                    }
                case TerrainType.Grassland:
                    {
                        if (specialtype == 1) { special = SpecialType.GrasslandShield; }
                        terrain = new Grassland(special);
                        break;
                    }
                case TerrainType.Forest:
                    {
                        if (specialtype == 1) { special = SpecialType.Pheasant; }
                        if (specialtype == 2) { special = SpecialType.Silk; }
                        terrain = new Forest(special);
                        break;
                    }
                case TerrainType.Hills:
                    {
                        if (specialtype == 1) { special = SpecialType.Coal; }
                        if (specialtype == 2) { special = SpecialType.Wine; }
                        terrain = new Hills(special);
                        break;
                    }
                case TerrainType.Mountains:
                    {
                        if (specialtype == 1) { special = SpecialType.Gold; }
                        if (specialtype == 2) { special = SpecialType.Iron; }
                        terrain = new Mountains(special);
                        break;
                    }
                case TerrainType.Tundra:
                    {
                        if (specialtype == 1) { special = SpecialType.Game; }
                        if (specialtype == 2) { special = SpecialType.Furs; }
                        terrain = new Tundra(special);
                        break;
                    }
                case TerrainType.Glacier:
                    {
                        if (specialtype == 1) { special = SpecialType.Ivory; }
                        if (specialtype == 2) { special = SpecialType.GlacierOil; }
                        terrain = new Glacier(special);
                        break;
                    }
                case TerrainType.Swamp:
                    {
                        if (specialtype == 1) { special = SpecialType.Peat; }
                        if (specialtype == 2) { special = SpecialType.Spice; }
                        terrain = new Swamp(special);
                        break;
                    }
                case TerrainType.Jungle:
                    {
                        if (specialtype == 1) { special = SpecialType.Gems; }
                        if (specialtype == 2) { special = SpecialType.Fruit; }
                        terrain = new Jungle(special);
                        break;
                    }
                case TerrainType.Ocean:
                    {
                        if (specialtype == 1) { special = SpecialType.Fish; }
                        if (specialtype == 2) { special = SpecialType.Whales; }
                        terrain = new Ocean(special);
                        break;
                    }
                default: return ;
            }
            terrain.Resource = resource;
            terrain.River = river;
            terrain.Island = island;
            terrain.UnitPresent = unit_present;
            terrain.CityPresent = city_present;
            terrain.Irrigation = irrigation;
            terrain.Mining = mining;
            terrain.Road = road;
            terrain.Railroad = railroad;
            terrain.Fortress = fortress;
            terrain.Pollution = pollution;
            terrain.Farmland = farmland;
            terrain.Airbase = airbase;
            terrain.Hexvalue = hexvalue;
            Terrain[x, y] = terrain;
        }

        public static IUnit CreateUnit(UnitType type, int x, int y, bool dead, bool firstMove, bool greyStarShield, bool veteran, int civ, int movePointsLost, int hitpointsLost, int lastMove, int caravanCommodity, UnitAction orders, int homeCity, int goToX, int goToY, int linkOtherUnitsOnTop, int linkOtherUnitsUnder)
        {
            IUnit unit;
            switch (type)
            {
                case UnitType.Settlers: unit = new Settlers(); break;
                case UnitType.Engineers: unit = new Engineers(); break;
                case UnitType.Warriors: unit = new Warriors(); break;
                case UnitType.Phalanx: unit = new Phalanx(); break;
                case UnitType.Archers: unit = new Archers(); break;
                case UnitType.Legions: unit = new Legions(); break;
                case UnitType.Pikemen: unit = new Pikemen(); break;
                case UnitType.Musketeers: unit = new Musketeers(); break;
                case UnitType.Fanatics: unit = new Fanatics(); break;
                case UnitType.Partisans: unit = new Partisans(); break;
                case UnitType.AlpineTroops: unit = new AlpineTroops(); break;
                case UnitType.Riflemen: unit = new Riflemen(); break;
                case UnitType.Marines: unit = new Marines(); break;
                case UnitType.Paratroopers: unit = new Paratroopers(); break;
                case UnitType.MechInf: unit = new MechInf(); break;
                case UnitType.Horsemen: unit = new Horsemen(); break;
                case UnitType.Chariot: unit = new Chariot(); break;
                case UnitType.Elephant: unit = new Elephant(); break;
                case UnitType.Crusaders: unit = new Crusaders(); break;
                case UnitType.Knights: unit = new Knights(); break;
                case UnitType.Dragoons: unit = new Dragoons(); break;
                case UnitType.Cavalry: unit = new Cavalry(); break;
                case UnitType.Armor: unit = new Armor(); break;
                case UnitType.Catapult: unit = new Catapult(); break;
                case UnitType.Cannon: unit = new Cannon(); break;
                case UnitType.Artillery: unit = new Artillery(); break;
                case UnitType.Howitzer: unit = new Howitzer(); break;
                case UnitType.Fighter: unit = new Fighter(); break;
                case UnitType.Bomber: unit = new Bomber(); break;
                case UnitType.Helicopter: unit = new Helicopter(); break;
                case UnitType.StlthFtr: unit = new StlthFtr(); break;
                case UnitType.StlthBmbr: unit = new StlthBmbr(); break;
                case UnitType.Trireme: unit = new Trireme(); break;
                case UnitType.Caravel: unit = new Caravel(); break;
                case UnitType.Galleon: unit = new Galleon(); break;
                case UnitType.Frigate: unit = new Frigate(); break;
                case UnitType.Ironclad: unit = new Ironclad(); break;
                case UnitType.Destroyer: unit = new Destroyer(); break;
                case UnitType.Cruiser: unit = new Cruiser(); break;
                case UnitType.AEGISCruiser: unit = new AEGISCruiser(); break;
                case UnitType.Battleship: unit = new Battleship(); break;
                case UnitType.Submarine: unit = new Submarine(); break;
                case UnitType.Carrier: unit = new Carrier(); break;
                case UnitType.Transport: unit = new Transport(); break;
                case UnitType.CruiseMsl: unit = new CruiseMsl(); break;
                case UnitType.NuclearMsl: unit = new NuclearMsl(); break;
                case UnitType.Diplomat: unit = new Diplomat(); break;
                case UnitType.Spy: unit = new Spy(); break;
                case UnitType.Caravan: unit = new Caravan(); break;
                case UnitType.Freight: unit = new Freight(); break;
                case UnitType.Explorer: unit = new Explorer(); break;
                default: return null;
            }
            unit.X = x;
            unit.Y = y;
            unit.FirstMove = firstMove;
            unit.GreyStarShield = greyStarShield;
            unit.Veteran = veteran;
            unit.Civ = civ;
            unit.MovePointsLost = movePointsLost;
            unit.HitpointsLost = hitpointsLost;
            unit.LastMove = lastMove;
            unit.CaravanCommodity = caravanCommodity;
            unit.Action = orders;
            unit.HomeCity = homeCity;
            unit.GoToX = goToX;
            unit.GoToY = goToY;
            unit.LinkOtherUnitsOnTop = linkOtherUnitsOnTop;
            unit.LinkOtherUnitsUnder = linkOtherUnitsUnder;

            if (dead) { DeadUnits.Add(unit); }
            else { Units.Add(unit); }            
            return unit;
        }

        public static City CreateCity(int x, int y, bool canBuildCoastal, bool autobuildMilitaryRule, bool stolenTech, bool improvementSold, bool weLoveKingDay, bool civilDisorder, bool canBuildShips, bool objectivex3, bool objectivex1, int owner, int size, int whoBuiltIt, int foodInStorage, int shieldsInProduction, int netTrade, string name, int workersInnerCircle, int workersOn8, int workersOn4, int noOfSpecialistsx4, string improvements, int[] wonders)
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
                ShieldsInProduction = shieldsInProduction,
                NetTrade = netTrade,
                Name = name,
                WorkersInnerCircle = workersInnerCircle,
                WorkersOn8 = workersOn8,
                WorkersOn4 = workersOn4,
                NoOfSpecialistsx4 = noOfSpecialistsx4                
            };

            if (improvements[0] == '1') { city.AddImprovement(new Palace()); }
            if (improvements[1] == '1') { city.AddImprovement(new Barracks()); }
            if (improvements[2] == '1') { city.AddImprovement(new Granary()); }
            if (improvements[3] == '1') { city.AddImprovement(new Temple()); }
            if (improvements[4] == '1') { city.AddImprovement(new Marketplace()); }
            if (improvements[5] == '1') { city.AddImprovement(new Library()); }
            if (improvements[6] == '1') { city.AddImprovement(new Courthouse()); }
            if (improvements[7] == '1') { city.AddImprovement(new CityWalls()); }
            if (improvements[8] == '1') { city.AddImprovement(new Aqueduct()); }
            if (improvements[9] == '1') { city.AddImprovement(new Bank()); }
            if (improvements[10] == '1') { city.AddImprovement(new Cathedral()); }
            if (improvements[11] == '1') { city.AddImprovement(new University()); }
            if (improvements[12] == '1') { city.AddImprovement(new MassTransit()); }
            if (improvements[13] == '1') { city.AddImprovement(new Colosseum()); }
            if (improvements[14] == '1') { city.AddImprovement(new Factory()); }
            if (improvements[15] == '1') { city.AddImprovement(new MfgPlant()); }
            if (improvements[16] == '1') { city.AddImprovement(new SDIdefense()); }
            if (improvements[17] == '1') { city.AddImprovement(new RecyclCentre()); }
            if (improvements[18] == '1') { city.AddImprovement(new PowerPlant()); }
            if (improvements[19] == '1') { city.AddImprovement(new HydroPlant()); }
            if (improvements[20] == '1') { city.AddImprovement(new NuclearPlant()); }
            if (improvements[21] == '1') { city.AddImprovement(new StockExch()); }
            if (improvements[22] == '1') { city.AddImprovement(new SewerSystem()); }
            if (improvements[23] == '1') { city.AddImprovement(new Supermarket()); }
            if (improvements[24] == '1') { city.AddImprovement(new Superhighways()); }
            if (improvements[25] == '1') { city.AddImprovement(new ResearchLab()); }
            if (improvements[26] == '1') { city.AddImprovement(new SAMbattery()); }
            if (improvements[27] == '1') { city.AddImprovement(new CoastalFort()); }
            if (improvements[28] == '1') { city.AddImprovement(new SolarPlant()); }
            if (improvements[29] == '1') { city.AddImprovement(new Harbour()); }
            if (improvements[30] == '1') { city.AddImprovement(new OffsehorePlat()); }
            if (improvements[31] == '1') { city.AddImprovement(new Airport()); }
            if (improvements[32] == '1') { city.AddImprovement(new PoliceStat()); }
            if (improvements[33] == '1') { city.AddImprovement(new PortFacil()); }

            if (wonders[0] == 1) { city.AddWonder(new Pyramids()); }
            if (wonders[1] == 1) { city.AddWonder(new HangingGardens()); }
            if (wonders[2] == 1) { city.AddWonder(new Colossus()); }
            if (wonders[3] == 1) { city.AddWonder(new Lighthouse()); }
            if (wonders[4] == 1) { city.AddWonder(new GreatLibrary()); }
            if (wonders[5] == 1) { city.AddWonder(new Oracle()); }
            if (wonders[6] == 1) { city.AddWonder(new GreatWall()); }
            if (wonders[7] == 1) { city.AddWonder(new WarAcademy()); }
            if (wonders[8] == 1) { city.AddWonder(new KR_Crusade()); }
            if (wonders[9] == 1) { city.AddWonder(new MP_Embassy()); }
            if (wonders[10] == 1) { city.AddWonder(new MichChapel()); }
            if (wonders[11] == 1) { city.AddWonder(new CoperObserv()); }
            if (wonders[12] == 1) { city.AddWonder(new MagellExped()); }
            if (wonders[13] == 1) { city.AddWonder(new ShakespTheat()); }
            if (wonders[14] == 1) { city.AddWonder(new DV_Workshop()); }
            if (wonders[15] == 1) { city.AddWonder(new JSB_Cathedral()); }
            if (wonders[16] == 1) { city.AddWonder(new IN_College()); }
            if (wonders[17] == 1) { city.AddWonder(new TradingCompany()); }
            if (wonders[18] == 1) { city.AddWonder(new DarwinVoyage()); }
            if (wonders[19] == 1) { city.AddWonder(new StatueLiberty()); }
            if (wonders[20] == 1) { city.AddWonder(new EiffelTower()); }
            if (wonders[21] == 1) { city.AddWonder(new HooverDam()); }
            if (wonders[22] == 1) { city.AddWonder(new WomenSuffrage()); }
            if (wonders[23] == 1) { city.AddWonder(new ManhattanProj()); }
            if (wonders[24] == 1) { city.AddWonder(new UnitedNations()); }
            if (wonders[25] == 1) { city.AddWonder(new ApolloProgr()); }
            if (wonders[26] == 1) { city.AddWonder(new SETIProgr()); }
            if (wonders[27] == 1) { city.AddWonder(new CureCancer()); }

            Cities.Add(city);
            return city;
        }

        public static Civilization CreateCiv(int id, int style, string leaderName, string tribeName, string adjective, int gender, int money, int researchProgress, int researchingTech, int taxRate, int government, int reputation, string techs)
        {
            Civilization civ = new Civilization
            {
                Id = id,
                CityStyle = style,
                LeaderName = leaderName,
                TribeName = tribeName,
                Adjective = adjective,
                Money = money
            };

            Civs.Add(civ);
            return civ;
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

        public static void SetGameData(int turnNumber, int turnNumberForGameYear, int unitSelectedAtGameStart, int whichHumanPlayerIsUsed, int playersMapUsed, int playersCivilizationNumberUsed, bool mapRevealed, int difficultyLevel, int barbarianActivity, int pollutionAmount, int globalTempRiseOccured, int noOfTurnsOfPeace, int numberOfUnits, int numberOfCities, int mapxdim, int mapydim, int mapArea, int mapSeed, int locatorX, int locatorY)
        {
            Data data = new Data
            {
                TurnNumber = turnNumber,
                TurnNumberForGameYear = turnNumberForGameYear,
                UnitSelectedAtGameStart = unitSelectedAtGameStart,
                HumanPlayerUsed = whichHumanPlayerIsUsed,
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

        private static Game instance;  //Singleton

        public static Game Instance //Singleton
        {
            get
            {
                if (instance == null) { instance = new Game(); }
                return instance;
            }
        }

    }
}
