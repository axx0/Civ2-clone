using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PoskusCiv2.Enums;
using PoskusCiv2.Units;
using PoskusCiv2.Terrains;
using PoskusCiv2.Imagery;
using ExtensionMethods;

namespace PoskusCiv2
{
    public partial class Game
    {
        public static List<IUnit> Units = new List<IUnit>();
        public static List<City> Cities = new List<City>();
        public static List<Civilization> Civs = new List<Civilization>();        
        public static ITerrain[,] Terrain;// = new ITerrain[5, 5];
        public static Options Options;
        public static Data Data;

        private int _activeUnit;

        public static void StartGame()
        {
            Game.Instance.ActiveUnit = Units[Data.UnitSelectedAtGameStart];

        }

        public static int gameTurn = 0;
        public static int gameYear = -4000;
        public static int people = 20000;
        public static int gold = 400;
        public static int unitNo = 2;
        public static int unitInLine = 0;  //which unit's turn it is

        //load sound for moving piece
        System.Media.SoundPlayer moveSound = new System.Media.SoundPlayer(@"C:\DOS\CIV 2\Civ2\Sound\MOVPIECE.WAV");
        System.Media.SoundPlayer fightSound = new System.Media.SoundPlayer(@"C:\DOS\CIV 2\Civ2\Sound\SWORDFGT.WAV");

        public static void defineStartGameParameters()
        {
            gameTurn = 0;
            gameYear = -4000;
            people = 20000;
            gold = 400;
            unitNo = 2;
        }

        //Define map coordinates
        public int[,] ConstructMap(int height, int width)
        {
            int i, j;
            int[,] grid = new int[height + 1, width + 1];
            for (i = 0; i <= height; i++)
            {
                for (j = 0; j <= width; j++)
                {
                    grid[i, j] = 1;
                }
            }
            return grid;
        }

        public static void NewTurn()
        {
            gameTurn += 1;
            gameYear += 20;

            //update game year text in Status form
            if (gameYear < 0)
            {
                Application.OpenForms.OfType<Forms.StatusForm>().First().UpdateGameYearLabel(Math.Abs(gameYear).ToString() + " B.C. (Turn " + gameTurn.ToString() + ")");
            }
            else
            {
                Application.OpenForms.OfType<Forms.StatusForm>().First().UpdateGameYearLabel(gameYear.ToString() + " A.D. (Turn " + gameTurn.ToString() + ")");
            }

        }

        public static void UserInput(char pressedKey)
        {
            switch (pressedKey)
            {
                case (char)Keys.Enter: MoveUnit(pressedKey); break;
                case (char)Keys.D6: MoveUnit(pressedKey); break;
                default: break;
            }
        }

        public static void MoveUnit(char pressedKey)
        {
            //if (Game.Terrain[XY[0], XY[1] - 1].Type != TerrainType.Ocean)
            //{
                Game.Instance.ActiveUnit.X += 2;
            //} 
            int[] XY = new int[] { Game.Instance.ActiveUnit.X, Game.Instance.ActiveUnit.Y }.Civ2xy();
            Console.WriteLine(Game.Terrain[XY[0] + 1, XY[1]].Type);
        }

        //User pressed a key
        public static void _UserInput(char pressedKey)
        {

            //if it is not end of turn, give orders for unit movement, otherwise wait for enter
            if (unitInLine != Units_.unitNumber)
            {
                //determine what happens to the moved unit
                Game.DetermineUnitMovementOutcome(pressedKey);                

                //TO-DO:
                //If there are multiple units on top another, determine which ones are to be drawn
                //Currently active unit always has to be on top
                //Also draw only units in visual range of screen

                Application.OpenForms.OfType<Forms.MapForm>().First().Invalidate();   //redraw MapForm because of movement
            }
            else    
            {
                //last unit has moved. Wait for enter and then reset movement counter.
                //currently: unitInLine = Units_.unitNumber
                if (pressedKey == (char)Keys.Enter)
                {
                    unitInLine = 0; //start over
                    Application.OpenForms.OfType<Forms.StatusForm>().First().StopTimerInStatusForm(); //stop animating text in status form
                    Game.NewTurn();
                }
            }

            Application.OpenForms.OfType<Forms.StatusForm>().First().UpdateUnitLabels(unitInLine);    //Update StatusForm with new unit info

        }

        //Determine what happens to the unit when a key is pressed
        public static void DetermineUnitMovementOutcome(char pressedKey)
        {
            int intendedX = 0, intendedY = 0, indexAttackedEnemy = 0, indeks = 0;
            bool wrongKeyIsPressed = false, enemyIsThere = false, movementAllowed = true;

            //1) calculate intended x-y locations
            if (pressedKey == '4') { intendedX = -2; intendedY = 0; }
            else if (pressedKey == '6') { intendedX = 2; intendedY = 0; }
            else if (pressedKey == '8') { intendedX = 0; intendedY = -2; }
            else if (pressedKey == '2') { intendedX = 0; intendedY = 2; }
            else if (pressedKey == '9') { intendedX = 1; intendedY = -1; }
            else if (pressedKey == '7') { intendedX = -1; intendedY = -1; }
            else if (pressedKey == '1') { intendedX = -1; intendedY = 1; }
            else if (pressedKey == '3') { intendedX = 1; intendedY = 1; }
            else if (pressedKey == (char)Keys.Space) { intendedX = 0; intendedY = 0; }
            else { wrongKeyIsPressed = true; }

            //2) determine what happens to the units 
            if (pressedKey == (char)Keys.Space)   //SPACE is pressed
            {
                Units_.unitTurnsLeft[unitInLine] = 0;
            }
            else if (wrongKeyIsPressed == true) //wrong key is pressed? Then do nothing.
            {

            }
            else  //Movement key is pressed. Determine if enemy civ is in square (then attack it) or not (then just move)
            {
                //Determine if enemy is on the intended moved square
                for (int i = 0; i < Units_.unitNumber; i++)
                {
                    if (i != unitInLine)    //skip yourself
                    {
                        if ((Units_.locationX[unitInLine] + intendedX == Units_.locationX[i]) && (Units_.locationY[unitInLine] + intendedY == Units_.locationY[i]) && (Units_.unitCiv[unitInLine] != Units_.unitCiv[i])) { enemyIsThere = true; indexAttackedEnemy = i; }  //if X and Y locations are the same and unit is not of same Civ
                    }                    
                }

                //If enemy is in the new square, delete it, else just move
                if (enemyIsThere)
                {
                    Units_.locationX[unitInLine] += 0;
                    Units_.locationY[unitInLine] += 0;
                    Units_.unitTurnsLeft[unitInLine] -= 1;
                    // attack! Delete the unit which lost (attacker or defender).
                    Game.AttackUnit(unitInLine, indexAttackedEnemy);
                    //Game.fightSound.Play();
                    if (unitInLine == Units_.unitNumber) { unitInLine = 0; } //unit destroyed was last unit. Restart counter.
                }
                else    // just movement
                {
                    //determine if movement is forbidden
                    indeks = (Units_.locationY[unitInLine] + intendedY) * Game.Data.MapXdim + (int)Math.Floor((double)Units_.locationX[unitInLine] + (double)intendedX) / 2; //index of map terrain of intended movement

                    //Land unit cannot move into ocean:
                    //if ((Units_.landseaairUnit[Units_.unitType[unitInLine]] == 1) && ((Game.Terrain[indeks] == 10) || (DrawMap.Map.Terrain[indeks] == 74)))
                    //{
                    //    movementAllowed = false;
                    //}
                    //Sea unit cannot move into land:
                    //if ((Units_.landseaairUnit[Units_.unitType[unitInLine]] == 2) && ((DrawMap.Map.Terrain[indeks] != 10) && (DrawMap.Map.Terrain[indeks] != 74)))
                    //{
                    //    movementAllowed = false;
                    //}

                    //move if allowed, else do nothing
                    if (movementAllowed == true)
                    {
                        //Game.moveSound.Play();
                        Units_.locationX[unitInLine] += intendedX;
                        Units_.locationY[unitInLine] += intendedY;
                        Units_.unitTurnsLeft[unitInLine] -= 1;
                    }
                }
            }

            //3) If no moves are left --> move to next unit and reset no of turns of this unit
            if (Units_.unitTurnsLeft[unitInLine] == 0)
            {
                Units_.unitTurnsLeft[unitInLine] = Units_.unitTurns[Units_.unitType[unitInLine]];  //reset turns counter
                unitInLine = unitInLine + 1;
            }

        }

        //Unit in line is attacking another one
        public static void AttackUnit(int attackerIndex, int defenderIndex)
        {
            bool attackerWon;
            int indeks = 0;

            //1) Determine outcome of attack
            attackerWon = true;

            //2) Defeated unit should be removed from list
            if (attackerWon)
            {
                indeks = defenderIndex; //attacker won, remove defender
            }
            else
            {
                indeks = attackerIndex; //defender won, remove attacker
            }
            Units_.unitNumber -= 1;
            Units_.unitType = Units_.unitType.RemoveAt(indeks);  //remove unit elements from arrays
            Units_.unitTurnsLeft = Units_.unitTurnsLeft.RemoveAt(indeks);
            Units_.veteranStatus = Units_.veteranStatus.RemoveAt(indeks);
            Units_.locationX = Units_.locationX.RemoveAt(indeks);
            Units_.locationY = Units_.locationY.RemoveAt(indeks);
            Units_.unitCity = Units_.unitCity.RemoveAt(indeks);
            Units_.unitCiv = Units_.unitCiv.RemoveAt(indeks);
            
            //3) Update index of current unit in line
            if (unitInLine == indeks) { }   //Attacker lost. No need to change index.
            else if (unitInLine < indeks) { }   //Defender (who lost) has higher index. Do nothing.
            else { unitInLine -= 1; }   //Defender (who lost) has lower index. Reduce current unit index by 1.
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

        public static IUnit CreateUnit(UnitType type, int x, int y, bool firstMove, bool greyStarShield, bool veteran, int civ, int movesMade, int hitpointsLost, int caravanCommodity, int orders, int homeCity, int goToX, int goToY, int linkOtherUnitsOnTop, int linkOtherUnitsUnder)
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
            unit.CaravanCommodity = caravanCommodity;
            unit.Orders = orders;
            unit.HomeCity = homeCity;
            unit.GoToX = goToX;
            unit.GoToY = goToY;
            unit.LinkOtherUnitsOnTop = linkOtherUnitsOnTop;
            unit.LinkOtherUnitsUnder = linkOtherUnitsUnder;

            Units.Add(unit);
            return unit;
        }


        public static City CreateCity(int x, int y, bool canBuildCoastal, bool autobuildMilitaryRule, bool stolenTech, bool improvementSold, bool weLoveKingDay, bool civilDisorder, bool canBuildShips, bool objectivex3, bool objectivex1, int owner, int size, int whoBuiltIt, int foodBox, int shieldBox, int netTrade, string name, int workersInnerCircle, int workersOn8, int workersOn4, int noOfSpecialistsx4)
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

            Cities.Add(city);
            return city;
        }

        public static Civilization CreateCiv(int style, string leaderName, string tribeName, string adjective)
        {
            Civilization civ = new Civilization
            {
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
