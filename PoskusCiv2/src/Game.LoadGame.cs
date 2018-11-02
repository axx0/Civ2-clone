using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;
using PoskusCiv2.Units;
using PoskusCiv2.Terrains;
using System.IO;

namespace PoskusCiv2
{
    public partial class Game
    {        
        public static void LoadGame(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);        //Enter filename
            int[] dataArray = new int[fs.Length];
            string bin;

            //Read every byte
            for (int i = 0; i < fs.Length; i++)
            {
                dataArray[i] = fs.ReadByte();
            }

            //=========================
            //START OF SAVED GAME FILE
            //=========================
            //Determine version        
            int version;
            if (dataArray[10] == 39) { version = 1; }    //Conflicts (27 hex)
            else if (dataArray[10] == 40) { version = 2; } //FW (28 hex)
            else if (dataArray[10] == 44) { version = 3; }    //MGE (2C hex)
            else if (dataArray[10] == 49) { version = 4; }    //ToT1.0 (31 hex)
            else if (dataArray[10] == 50) { version = 5; }    //ToT1.1 (32 hex)
            else { version = 1; }   //lower than Conflicts
            Console.WriteLine("Game Version = {0}", version);
                        
            //Bloodlust on/off
            bool bloodlust;
            bin = Convert.ToString(dataArray[12], 2).PadLeft(8, '0');    //you have to pad zeros to the left because ToString doesn't write first zeros
            if (bin[0] == '1') { bloodlust = true; }
            else { bloodlust = false; }
                        
            //Simplified combat on/off
            bool simplifiedCombat;
            if (bin[3] == '1') { simplifiedCombat = true; }
            else { simplifiedCombat = false; }
                        
            //Flat/round earth
            bool flatEarth;
            bin = Convert.ToString(dataArray[13], 2).PadLeft(8, '0');
            if (bin[0] == '1') { flatEarth = true; }
            else { flatEarth = false; }

            //Don't restart if eliminated
            bool dontRestartIfEliminated;
            if (bin[7] == '1') { dontRestartIfEliminated = true; }
            else { dontRestartIfEliminated = false; }

            //Move units without mouse
            bool moveUnitsWithoutMouse;
            bin = Convert.ToString(dataArray[14], 2).PadLeft(8, '0');
            if (bin[0] == '1') { moveUnitsWithoutMouse = true; }
            else { moveUnitsWithoutMouse = false; }

            //Enter closes city screen
            bool enterClosestCityScreen;
            if (bin[1] == '1') { enterClosestCityScreen = true; }
            else { enterClosestCityScreen = false; }
                        
            //Grid on/off
            bool grid;
            if (bin[2] == '1') { grid = true; }
            else { grid = false; }

            //Sound effects on/off
            bool soundEffects;
            if (bin[3] == '1') { soundEffects = true; }
            else { soundEffects = false; }

            //Music on/off
            bool music;
            if (bin[4] == '1') { music = true; }
            else { music = false; }

            //Cheat menu on/off
            bool cheatMenu;
            bin = Convert.ToString(dataArray[15], 2).PadLeft(8, '0');
            if (bin[0] == '1') { cheatMenu = true; }
            else { cheatMenu = false; }

            //Always wait at end of turn on/off
            bool alwaysWaitAtEndOfTurn;
            if (bin[1] == '1') { alwaysWaitAtEndOfTurn = true; }
            else { alwaysWaitAtEndOfTurn = false; }

            //Autosave each turn on/off
            bool autosaveEachTurn;
            if (bin[2] == '1') { autosaveEachTurn = true; }
            else { autosaveEachTurn = false; }

            //Show enemy moves on/off
            bool showEnemyMoves;
            if (bin[3] == '1') { showEnemyMoves = true; }
            else { showEnemyMoves = false; }

            //No pause after enemy moves on/off
            bool noPauseAfterEnemyMoves;
            if (bin[4] == '1') { noPauseAfterEnemyMoves = true; }
            else { noPauseAfterEnemyMoves = false; }

            //Fast piece slide on/off
            bool fastPieceSlide;
            if (bin[5] == '1') { fastPieceSlide = true; }
            else { fastPieceSlide = false; }

            //Instant advice on/off
            bool instantAdvice;
            if (bin[6] == '1') { instantAdvice = true; }
            else { instantAdvice = false; }

            //Tutorial help on/off
            bool tutorialHelp;
            if (bin[7] == '1') { tutorialHelp = true; }
            else { tutorialHelp = false; }

            //Animated heralds on/off
            bool animatedHeralds;
            bin = Convert.ToString(dataArray[16], 2).PadLeft(8, '0');
            if (bin[2] == '1') { animatedHeralds = true; }
            else { animatedHeralds = false; }

            //High council on/off
            bool highCouncil;
            if (bin[3] == '1') { highCouncil = true; }
            else { highCouncil = false; }

            //Civilopedia for advances on/off
            bool civilopediaForAdvances;
            if (bin[4] == '1') { civilopediaForAdvances = true; }
            else { civilopediaForAdvances = false; }

            //Throne room graphics on/off
            bool throneRoomGraphics;
            if (bin[5] == '1') { throneRoomGraphics = true; }
            else { throneRoomGraphics = false; }

            //Diplomacy screen graphics on/off
            bool diplomacyScreenGraphics;
            if (bin[6] == '1') { diplomacyScreenGraphics = true; }
            else { diplomacyScreenGraphics = false; }

            //Wonder movies on/off
            bool wonderMovies;
            if (bin[7] == '1') { wonderMovies = true; }
            else { wonderMovies = false; }

            //Cheat penalty/warning on/off
            bool cheatPenaltyWarning;
            bin = Convert.ToString(dataArray[20], 2).PadLeft(8, '0');
            if (bin[3] == '1') { cheatPenaltyWarning = true; }
            else { cheatPenaltyWarning = false; }

            //Announce we love king day on/off
            bool announceWeLoveKingDay;
            bin = Convert.ToString(dataArray[22], 2).PadLeft(8, '0');
            if (bin[0] == '1') { announceWeLoveKingDay = true; }
            else { announceWeLoveKingDay = false; }

            //Warn when food dangerously low on/off
            bool warnWhenFoodDangerouslyLow;
            if (bin[1] == '1') { warnWhenFoodDangerouslyLow = true; }
            else { warnWhenFoodDangerouslyLow = false; }

            //Announce cities in disorder on/off
            bool announceCitiesInDisorder;
            if (bin[2] == '1') { announceCitiesInDisorder = true; }
            else { announceCitiesInDisorder = false; }

            //Announce order restored in cities on/off
            bool announceOrderRestored;
            if (bin[3] == '1') { announceOrderRestored = true; }
            else { announceOrderRestored = false; }

            //Show non combat units build on/off
            bool showNonCombatUnitsBuilt;
            if (bin[4] == '1') { showNonCombatUnitsBuilt = true; }
            else { showNonCombatUnitsBuilt = false; }

            //Show invalid build instructions on/off
            bool showInvalidBuildInstructions;
            if (bin[5] == '1') { showInvalidBuildInstructions = true; }
            else { showInvalidBuildInstructions = false; }

            //Warn when city growth halted on/off
            bool warnWhenCityGrowthHalted;
            if (bin[6] == '1') { warnWhenCityGrowthHalted = true; }
            else { warnWhenCityGrowthHalted = false; }

            //Show city improvements built on/off
            bool showCityImprovementsBuilt;
            if (bin[7] == '1') { showCityImprovementsBuilt = true; }
            else { showCityImprovementsBuilt = false; }

            //Zoom to city not default action on/off
            bool zoomToCityNotDefaultAction;
            bin = Convert.ToString(dataArray[23], 2).PadLeft(8, '0');
            if (bin[5] == '1') { zoomToCityNotDefaultAction = true; }
            else { zoomToCityNotDefaultAction = false; }

            //Warn when pollution occurs on/off
            bool warnWhenPollutionOccurs;
            if (bin[6] == '1') { warnWhenPollutionOccurs = true; }
            else { warnWhenPollutionOccurs = false; }

            //Warn when changing production will cost shileds on/off
            bool warnWhenChangingProductionWillCostShields;
            if (bin[7] == '1') { warnWhenChangingProductionWillCostShields = true; }
            else { warnWhenChangingProductionWillCostShields = false; }

            //Number of turns passed
            int intVal1 = dataArray[28];
            int intVal2 = dataArray[29];
            int turnNumber = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);    //convert hex value 2 & 1 (in that order) together to int

            //Number of turns passed for game year calculation
            intVal1 = dataArray[30];
            intVal2 = dataArray[31];
            int turnNumberForGameYear = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

            //Which unit is selected at start of game
            intVal1 = dataArray[34];
            intVal2 = dataArray[35];
            int unitSelectedAtGameStart = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

            //Which human player is used
            int whichHumanPlayerIsUsed = dataArray[39];

            //Players map which is used
            int playersMapUsed = dataArray[40];

            //Players map which is used
            int playersCivilizationNumberUsed = dataArray[41];

            //Map revealed
            bool mapRevealed;
            if (dataArray[43] == 1) { mapRevealed = true; }
            else { mapRevealed = false; }

            //Difficulty level
            int difficultyLevel = dataArray[44];

            //Barbarian activity
            int barbarianActivity = dataArray[45];

            //Amount of pollution
            int pollutionAmount = dataArray[50];

            //Global temp rising times occured
            int globalTempRiseOccured = dataArray[51];

            //Number of turns of peace
            int noOfTurnsOfPeace = dataArray[56];

            //Number of units
            intVal1 = dataArray[58];
            intVal2 = dataArray[59];
            int numberOfUnits = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

            //Number of cities
            intVal1 = dataArray[60];
            intVal2 = dataArray[61];
            int numberOfCities = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

            SetOptions(version, bloodlust, simplifiedCombat, flatEarth, dontRestartIfEliminated, moveUnitsWithoutMouse, enterClosestCityScreen, grid, soundEffects, music, cheatMenu, alwaysWaitAtEndOfTurn, autosaveEachTurn, showEnemyMoves, noPauseAfterEnemyMoves, fastPieceSlide, instantAdvice, tutorialHelp, animatedHeralds, highCouncil, civilopediaForAdvances, throneRoomGraphics, diplomacyScreenGraphics, wonderMovies, cheatPenaltyWarning, announceWeLoveKingDay, warnWhenFoodDangerouslyLow, announceCitiesInDisorder, announceOrderRestored, showNonCombatUnitsBuilt, showInvalidBuildInstructions, warnWhenCityGrowthHalted, showCityImprovementsBuilt, zoomToCityNotDefaultAction, warnWhenPollutionOccurs, warnWhenChangingProductionWillCostShields);

            //=========================
            //WONDERS
            //=========================
            int[] wonderCity = new int[28]; //28 wonders
            for (int i = 0; i < 28; i++)
            {
                //City number with the wonder
                intVal1 = dataArray[266 + 2 * i];
                intVal2 = dataArray[266 + 2 * i + 1];
                wonderCity[i] = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);
            }

            //=========================
            //CIVS
            //=========================
            char[] asciich = new char[23];            
            for (int i = 0; i < 7; i++) //7 civs
            {
                //City style
                int civCityStyle = dataArray[584 + 242 * i];

                //Leader names
                for (int j = 0; j < 23; j++)
                {
                    asciich[j] = Convert.ToChar(dataArray[584 + 2 + 242 * i + j]);
                }
                string civLeaderName = new string(asciich);
                civLeaderName = civLeaderName.Replace("\0", string.Empty);  //remove null characters
                //Console.WriteLine(civLeaderName);

                //Tribe name
                for (int j = 0; j < 23; j++)
                {
                    asciich[j] = Convert.ToChar(dataArray[584 + 2 + 23 + 242 * i + j]);
                }
                string civTribeName = new string(asciich);
                civTribeName = civTribeName.Replace("\0", string.Empty);
                //Console.WriteLine(civTribeName);

                //Adjective
                for (int j = 0; j < 23; j++)
                {
                    asciich[j] = Convert.ToChar(dataArray[584 + 2 + 23 + 23 + 242 * i + j]);
                }
                string civAdjective = new string(asciich);
                civAdjective = civAdjective.Replace("\0", string.Empty);
                //Console.WriteLine(civAdjective);

                //Before adding civs, add a barbarian civ in the beginning (Civ2 gives units civ=1 for 1st civ (and not civ=0), so civ=0 is obviously reserved for barbarians)
                if (i == 0) { Civilization civ1 = CreateCiv(0, "NULL", "Barbarian", "Barbarians"); }

                Civilization civ = CreateCiv(civCityStyle, civLeaderName, civTribeName, civAdjective);                
            }



            //=========================
            //TECH & MONEY
            //=========================


            //=========================
            //MAPS
            //=========================
            //Map header ofset
            int ofset;
            if (version > 1) { ofset = 13702; }  //FW and later (offset=3586hex)
            else { ofset = 13432; } //Conflicts (offset=3478hex)

            //Map X dimension
            intVal1 = dataArray[ofset + 0];
            intVal2 = dataArray[ofset + 1];
            int mapXdimension = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);
            mapXdimension = mapXdimension / 2; //map 150x120 is really 75x120

            //Map Y dimension
            intVal1 = dataArray[ofset + 2];
            intVal2 = dataArray[ofset + 3];
            int mapYdimension = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

            //Map area:
            intVal1 = dataArray[ofset + 4];
            intVal2 = dataArray[ofset + 5];
            int mapArea = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

            ////Flat Earth flag (info already given before!!)
            //intVal1 = dataArray[ofset + 6];
            //intVal2 = dataArray[ofset + 7];
            //flatEarth = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

            //Map seed
            intVal1 = dataArray[ofset + 8];
            intVal2 = dataArray[ofset + 9];
            int mapSeed = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

            //Locator map X dimension
            intVal1 = dataArray[ofset + 10];
            intVal2 = dataArray[ofset + 11];
            int locatorMapXDimension = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

            //Locator map Y dimension
            int intValue11 = dataArray[ofset + 12];
            int intValue12 = dataArray[ofset + 13];
            int locatorMapYDimension = int.Parse(string.Concat(intValue12.ToString("X"), intValue11.ToString("X")), System.Globalization.NumberStyles.HexNumber);

            SetGameData(turnNumber, turnNumberForGameYear, unitSelectedAtGameStart, whichHumanPlayerIsUsed, playersMapUsed, playersCivilizationNumberUsed, mapRevealed, difficultyLevel, barbarianActivity, pollutionAmount, globalTempRiseOccured, noOfTurnsOfPeace, numberOfUnits, numberOfCities, mapXdimension, mapYdimension, mapArea, mapSeed, locatorMapXDimension, locatorMapYDimension);
            //Console.WriteLine("MAP Xdim=" + mapXdimension.ToString() + " Ydim=" + mapYdimension.ToString());

            //Initialize Terrain array now that you know its size
            Terrain = new ITerrain[mapXdimension, mapYdimension];

            //block 1 - terrain improvements (for indivudual civs)
            int ofsetB1 = ofset + 14; //offset for block 2 values
            //...........
            //block 2 - terrain type
            int ofsetB2 = ofsetB1 + 7 * mapArea; //offset for block 2 values
            Console.WriteLine("MapArea=" + mapArea.ToString() + " X=" + mapXdimension.ToString() + " Y=" + mapYdimension);
            for (int i = 0; i < mapArea; i++)
            {
                int x = i % mapXdimension;
                int y = i / mapXdimension;

                // Terrain type
                TerrainType type = TerrainType.Desert;  //only initial
                bool river = false;
                int terrain_type = dataArray[ofsetB2 + i * 6 + 0];
                if (terrain_type == 0)   { type = TerrainType.Desert; river = false; }   //0dec=0hex
                if (terrain_type == 128) { type = TerrainType.Desert; river = true; }   //128dec=80hex
                if (terrain_type == 1)   { type = TerrainType.Plains; river = false; }   //1dec=1hex
                if (terrain_type == 129) { type = TerrainType.Plains; river = true; }   //129dec=81hex
                if (terrain_type == 2)   { type = TerrainType.Grassland; river = false; }   //2dec=2hex
                if (terrain_type == 130) { type = TerrainType.Grassland; river = true; }   //130dec=82hex
                if (terrain_type == 3)   { type = TerrainType.Forest; river = false; }   //3dec=3hex
                if (terrain_type == 131) { type = TerrainType.Forest; river = true; }   //131dec=83hex
                if (terrain_type == 4)   { type = TerrainType.Hills; river = false; }   //4dec=4hex
                if (terrain_type == 132) { type = TerrainType.Hills; river = true; }   //132dec=84hex
                if (terrain_type == 5)   { type = TerrainType.Mountains; river = false; }   //5dec=5hex
                if (terrain_type == 133) { type = TerrainType.Mountains; river = true; }   //133dec=85hex
                if (terrain_type == 6)   { type = TerrainType.Tundra; river = false; }   //6dec=6hex
                if (terrain_type == 134) { type = TerrainType.Tundra; river = true; }   //134dec=86hex
                if (terrain_type == 7)   { type = TerrainType.Glacier; river = false; }   //7dec=7hex
                if (terrain_type == 135) { type = TerrainType.Glacier; river = true; }   //135dec=87hex
                if (terrain_type == 8)   { type = TerrainType.Swamp; river = false; }   //8dec=8hex
                if (terrain_type == 136) { type = TerrainType.Swamp; river = true; }   //136dec=88hex
                if (terrain_type == 9)   { type = TerrainType.Jungle; river = false; }   //9dec=9hex
                if (terrain_type == 137) { type = TerrainType.Jungle; river = true; }   //137dec=89hex
                if (terrain_type == 10)  { type = TerrainType.Ocean; river = false; }   //10dec=Ahex
                if (terrain_type == 74)  { type = TerrainType.Ocean; river = false; }   //74dec=4Ahex
                //determine if resources are present
                bool resource = false;
                //!!! NOT WORKING PROPERLY !!!
                //bin = Convert.ToString(dataArray[ofsetB2 + i * 6 + 0], 2).PadLeft(8, '0');
                //if (bin[1] == '1') { resource = true; }

                // Tile improvements (for all civs! In block 1 it's for indivudual civs)
                int tile_improv = dataArray[ofsetB2 + i * 6 + 1];
                bool unit_present = false, city_present = false, irrigation = false, mining = false, road = false, railroad = false, fortress = false, pollution = false, farmland = false, airbase = false;
                bin = Convert.ToString(tile_improv, 2).PadLeft(8, '0');
                if (bin[7] == '1') { unit_present = true; }
                if (bin[6] == '1') { city_present = true; }
                if (bin[5] == '1') { irrigation = true; }
                if (bin[4] == '1') { mining = true; }
                if (bin[3] == '1') { road = true; }
                if (bin[2] == '1' && bin[3] == '1') { railroad = true; }
                if (bin[1] == '1') { fortress = true; }
                if (bin[0] == '1') { pollution = true; }
                if (bin[4] == '1' && bin[5] == '1') { farmland = true; }
                if (bin[1] == '1' && bin[6] == '1') { airbase = true; }

                //City radius (TO-DO)
                int intValueB23 = dataArray[ofsetB2 + i * 6 + 2];
                
                //Island counter
                int terrain_island = dataArray[ofsetB2 + i * 6 + 3];

                //Visibility (TO-DO)
                int intValueB25 = dataArray[ofsetB2 + i * 6 + 4];

                int intValueB26 = dataArray[ofsetB2 + i * 6 + 5];   //?

                //string hexValue = intValueB26.ToString("X");

                CreateTerrain(x, y, type, resource, river, terrain_island, unit_present, city_present, irrigation, mining, road, railroad, fortress, pollution, farmland, airbase, bin);
                
            }
            //block 3 - locator map
            int ofsetB3 = ofsetB2 + 6 * mapArea; //offset for block 2 values
            //...............

            //=========================
            //UNIT INFO
            //=========================
            int ofsetU = ofsetB3 + 2 * locatorMapXDimension * locatorMapYDimension + 1024;
            //Console.WriteLine("offset Unit=" + ofsetU.ToString());

            //determine byte length of units
            int multipl;
            if (version <= 2) { multipl = 26; } //FW or CiC
            else if (version == 3) { multipl = 32; }    //MGE
            else { multipl = 40; }  //ToT

            for (int i = 0; i < numberOfUnits; i++)
            {
                //Unit X locatioin
                intVal1 = dataArray[ofsetU + multipl * i + 0];
                intVal2 = dataArray[ofsetU + multipl * i + 1];
                int unitXlocation = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                //Unit Y locatioin
                intVal1 = dataArray[ofsetU + multipl * i + 2];
                intVal2 = dataArray[ofsetU + multipl * i + 3];
                int unitYlocation = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                //Transform x-units from civ2-style to real coordiantes
                unitXlocation = (unitXlocation - (unitYlocation % 2)) / 2;

                //If this is the unit's first move
                bin = Convert.ToString(dataArray[ofsetU + multipl * i + 4], 2).PadLeft(8, '0');
                bool unitFirstMove;
                if (bin[1] == '1') { unitFirstMove = true; }
                else { unitFirstMove = false; }

                //Grey star to the shield
                bin = Convert.ToString(dataArray[ofsetU + multipl * i + 5], 2).PadLeft(8, '0');
                bool unitGreyStarShield;
                if (bin[0] == '1') { unitGreyStarShield = true; }
                else { unitGreyStarShield = false; }

                //Veteran status
                bool unitVeteranStatus;
                if (bin[2] == '1') { unitVeteranStatus = true; }
                else { unitVeteranStatus = false; }

                //Unit type
                int unitType = dataArray[ofsetU + multipl * i + 6];

                //Unit civ
                int unitCiv = dataArray[ofsetU + multipl * i + 7];  //00 = barbarians
               
                //Unit moves made
                int unitMovesMade = dataArray[ofsetU + multipl * i + 8];

                //Unit hitpoints lost
                int unitHitpointsLost = dataArray[ofsetU + multipl * i + 10];

                //Unit previous move
                int unitLastMove = dataArray[ofsetU + multipl * i + 11];    //06=right, 02=down, ...

                //Unit caravan commodity
                int unitCaravanCommodity = dataArray[ofsetU + multipl * i + 13];

                //Unit orders
                int unitOrders = dataArray[ofsetU + multipl * i + 15];

                //Unit home city
                int unitHomeCity = dataArray[ofsetU + multipl * i + 16];

                //Unit go-to X
                intVal1 = dataArray[ofsetU + multipl * i + 18];
                intVal2 = dataArray[ofsetU + multipl * i + 19];
                int unitGoToX = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                //Unit go-to Y
                intVal1 = dataArray[ofsetU + multipl * i + 20];
                intVal2 = dataArray[ofsetU + multipl * i + 21];
                int unitGoToY = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                //Transform x-goto units from civ2-style to real coordiantes
                unitGoToX = (unitGoToX - (unitGoToY % 2)) / 2;

                //Unit link to other units on top of it
                intVal1 = dataArray[ofsetU + multipl * i + 22];
                intVal2 = dataArray[ofsetU + multipl * i + 23];
                int unitLinkOtherUnitsOnTop = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                //Unit link to other units under it
                intVal1 = dataArray[ofsetU + multipl * i + 24];
                intVal2 = dataArray[ofsetU + multipl * i + 25];
                int unitLinkOtherUnitsUnder = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                IUnit unit = CreateUnit((UnitType)unitType, unitXlocation, unitYlocation, unitFirstMove, unitGreyStarShield, unitVeteranStatus, unitCiv, unitMovesMade, unitHitpointsLost, unitLastMove, unitCaravanCommodity, unitOrders, unitHomeCity, unitGoToX, unitGoToY, unitLinkOtherUnitsOnTop, unitLinkOtherUnitsUnder);
            }


            //=========================
            //CITIES
            //=========================
            int ofsetC = ofsetU + multipl * numberOfUnits;
            Console.WriteLine("offset Cities=" + ofsetC.ToString());

            if (version <= 2) { multipl = 84; } //FW or CiC
            else if (version == 3) { multipl = 88; }    //MGE
            else { multipl = 92; }  //ToT
                        
            char[] asciichar = new char[15];            
            for (int i = 0; i < numberOfCities; i++)
            {
                //City X location
                intVal1 = dataArray[ofsetC + multipl * i + 0];
                intVal2 = dataArray[ofsetC + multipl * i + 1];
                int cityXlocation = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                //City Y location
                intVal1 = dataArray[ofsetC + multipl * i + 2];
                intVal2 = dataArray[ofsetC + multipl * i + 3];
                int cityYlocation = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                //Transform x city location from civ2-style to real coordiantes
                cityXlocation = (cityXlocation - (cityYlocation % 2)) / 2;

                //Can build coastal improvements
                bin = Convert.ToString(dataArray[ofsetC + multipl * i + 4], 2).PadLeft(8, '0');
                bool cityCanBuildCoastal;
                if (bin[0] == '1') { cityCanBuildCoastal = true; }
                else { cityCanBuildCoastal = false; }

                //Auto build under military rule
                bool cityAutobuildMilitaryRule;
                if (bin[3] == '1') { cityAutobuildMilitaryRule = true; }
                else { cityAutobuildMilitaryRule = false; }

                //Stolen tech
                bool cityStolenTech;
                if (bin[4] == '1') { cityStolenTech = true; }
                else { cityStolenTech = false; }

                //Improvement sold
                bool cityImprovementSold;
                if (bin[5] == '1') { cityImprovementSold = true; }
                else { cityImprovementSold = false; }

                //We love king day
                bool cityWeLoveKingDay;
                if (bin[6] == '1') { cityWeLoveKingDay = true; }
                else { cityWeLoveKingDay = false; }

                //Civil disorder
                bool cityCivilDisorder;
                if (bin[7] == '1') { cityCivilDisorder = true; }
                else { cityCivilDisorder = false; }

                //Can build ships
                bin = Convert.ToString(dataArray[ofsetC + multipl * i + 6], 2).PadLeft(8, '0');
                bool cityCanBuildShips;
                if (bin[2] == '1') { cityCanBuildShips = true; }
                else { cityCanBuildShips = false; }

                //Objective x3
                bin = Convert.ToString(dataArray[ofsetC + multipl * i + 7], 2).PadLeft(8, '0');
                bool cityObjectivex3;
                if (bin[3] == '1') { cityObjectivex3 = true; }
                else { cityObjectivex3 = false; }

                //Objective x1
                bin = Convert.ToString(dataArray[ofsetC + multipl * i + 7], 2).PadLeft(8, '0');
                bool cityObjectivex1;
                if (bin[5] == '1') { cityObjectivex1 = true; }
                else { cityObjectivex1 = false; }

                //Owner
                int cityOwner = dataArray[ofsetC + multipl * i + 8];

                //Size
                int citySize = dataArray[ofsetC + multipl * i + 9];

                //Who built it
                int cityWhoBuiltIt = dataArray[ofsetC + multipl * i + 10];

                //Production squares
                //???????????????????

                //Specialists
                //??????????????????

                //Food in food box
                intVal1 = dataArray[ofsetC + multipl * i + 26];
                intVal2 = dataArray[ofsetC + multipl * i + 27];
                int cityFoodBox = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                //Shield in shield box
                intVal1 = dataArray[ofsetC + multipl * i + 28];
                intVal2 = dataArray[ofsetC + multipl * i + 29];
                int cityShieldBox = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                //Net trade
                intVal1 = dataArray[ofsetC + multipl * i + 30];
                intVal2 = dataArray[ofsetC + multipl * i + 31];
                int cityNetTrade = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                //Name        
                for (int j = 0; j < 15; j++)
                {
                    asciichar[j] = Convert.ToChar(dataArray[ofsetC + multipl * i + j + 32]);
                }
                string cityName = new string(asciichar);
                cityName = cityName.Replace("\0", string.Empty);
                //Console.WriteLine(cityName[i]);

                //Workers in inner circle
                int cityWorkersInnerCircle = dataArray[ofsetC + multipl * i + 48];

                //Workers on 8 of the outer circle
                int cityWorkersOn8 = dataArray[ofsetC + multipl * i + 49];
                
                //Workers on 4 of the outer circle
                int cityWorkersOn4 = dataArray[ofsetC + multipl * i + 50];

                //Number of specialists x4
                int cityNoOfSpecialistsx4 = dataArray[ofsetC + multipl * i + 51];

                //Improvements
                //...

                //Item in production
                //...

                //No of active trade routes
                //...

                //1st, 2nd, 3rd trade commodities available
                //...

                //1st, 2nd, 3rd trade commodities demanded
                //...

                //1st, 2nd, 3rd trade commodities in route
                //...

                //1st, 2nd, 3rd trade route partner city number
                //...

                //Science
                //...

                //Tax
                //...

                //No of trade icons
                //...

                //Total food production
                //...

                //Total shield production
                //...

                //No of happy citizens
                //...

                //No of unhappy citizens
                //...

                //Sequence number of the city
                //...

                City city = CreateCity(cityXlocation, cityYlocation, cityCanBuildCoastal, cityAutobuildMilitaryRule, cityStolenTech, cityImprovementSold, cityWeLoveKingDay, cityCivilDisorder, cityCanBuildShips, cityObjectivex3, cityObjectivex1, cityOwner, citySize, cityWhoBuiltIt, cityFoodBox, cityShieldBox, cityNetTrade, cityName, cityWorkersInnerCircle, cityWorkersOn8, cityWorkersOn4, cityNoOfSpecialistsx4);
            }


            //=========================
            //OTHER
            //=========================

        }
    }
}
