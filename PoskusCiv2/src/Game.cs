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

namespace PoskusCiv2
{
    public partial class Game
    {
        public static List<IUnit> Units = new List<IUnit>();
        public static List<City> Cities = new List<City>();
        public static List<Civilization> Civs = new List<Civilization>();        
        public static ITerrain[,] Terrain;
        public static Options Options;
        public static Data Data;

        //load sound for moving piece
        //System.Media.SoundPlayer moveSound = new System.Media.SoundPlayer(@"C:\DOS\CIV 2\Civ2\Sound\MOVPIECE.WAV");
        //System.Media.SoundPlayer fightSound = new System.Media.SoundPlayer(@"C:\DOS\CIV 2\Civ2\Sound\SWORDFGT.WAV");

        private int _activeUnit;

        public static void StartGame()
        {
            //int stej = 0;
            //foreach (IUnit unit in Units.Where(n => n.Civ == Game.Data.WhichHumanPlayerIsUsed))
            //{
            //    Console.WriteLine("Unit{0}: {1}, {2}", stej++, unit.Name, Game.Civs[unit.Civ].TribeName);
            //    Game.Instance.ActiveUnit = unit;
            //}

            Game.Instance.ActiveUnit = Units[Data.UnitSelectedAtGameStart];
        }

        
        public IUnit ActiveUnit
        {
            get { return Units[_activeUnit]; }
            set { _activeUnit = Units.IndexOf(value); }
        }

        public static void CreateTerrain(int x, int y, TerrainType type, bool resource, bool river, int island, bool unit_present, bool city_present, bool irrigation, bool mining, bool road, bool railroad, bool fortress, bool pollution, bool farmland, bool airbase, string hexvalue)
        {
            ITerrain terrain;
            switch (type)
            {
                case TerrainType.Desert: terrain = new Desert(); break;
                case TerrainType.Plains: terrain = new Plains(); break;
                case TerrainType.Grassland: terrain = new Grassland(); break;
                case TerrainType.Forest: terrain = new Forest(); break;
                case TerrainType.Hills: terrain = new Hills(); break;
                case TerrainType.Mountains: terrain = new Mountains(); break;
                case TerrainType.Tundra: terrain = new Tundra(); break;
                case TerrainType.Glacier: terrain = new Glacier(); break;
                case TerrainType.Swamp: terrain = new Swamp(); break;
                case TerrainType.Jungle: terrain = new Jungle(); break;
                case TerrainType.Ocean: terrain = new Ocean(); break;
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

        public static IUnit CreateUnit(UnitType type, int x, int y, bool firstMove, bool greyStarShield, bool veteran, int civ, int movesMade, int hitpointsLost, int lastMove, int caravanCommodity, UnitAction orders, int homeCity, int goToX, int goToY, int linkOtherUnitsOnTop, int linkOtherUnitsUnder)
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
            unit.MovesMade = movesMade;
            unit.HitpointsLost = hitpointsLost;
            unit.LastMove = lastMove;
            unit.CaravanCommodity = caravanCommodity;
            unit.Action = orders;
            unit.HomeCity = homeCity;
            unit.GoToX = goToX;
            unit.GoToY = goToY;
            unit.LinkOtherUnitsOnTop = linkOtherUnitsOnTop;
            unit.LinkOtherUnitsUnder = linkOtherUnitsUnder;

            Units.Add(unit);
            return unit;
        }


        public static City CreateCity(int x, int y, bool canBuildCoastal, bool autobuildMilitaryRule, bool stolenTech, bool improvementSold, bool weLoveKingDay, bool civilDisorder, bool canBuildShips, bool objectivex3, bool objectivex1, int owner, int size, int whoBuiltIt, int foodBox, int shieldBox, int netTrade, string name, int workersInnerCircle, int workersOn8, int workersOn4, int noOfSpecialistsx4, string improvements)
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
                FoodBox = foodBox,
                ShieldBox = shieldBox,
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

            Cities.Add(city);
            return city;
        }

        public static Civilization CreateCiv(int id, int style, string leaderName, string tribeName, string adjective)
        {
            Civilization civ = new Civilization
            {
                Id = id,
                CityStyle = style,
                LeaderName = leaderName,
                TribeName = tribeName,
                Adjective = adjective
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
                WhichHumanPlayerIsUsed = whichHumanPlayerIsUsed,
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
