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
            for (int i = 0; i < fs.Length; i++) dataArray[i] = fs.ReadByte();

            //=========================
            //START OF SAVED GAME FILE
            //=========================
            //Determine version        
            int version;
            if (dataArray[10] == 39)        version = 1;    //Conflicts (27 hex)
            else if (dataArray[10] == 40)   version = 2; //FW (28 hex)
            else if (dataArray[10] == 44)   version = 3;    //MGE (2C hex)
            else if (dataArray[10] == 49)   version = 4;    //ToT1.0 (31 hex)
            else if (dataArray[10] == 50)   version = 5;    //ToT1.1 (32 hex)
            else                            version = 1;   //lower than Conflicts
                        
            //Bloodlust on/off
            bool bloodlust = false;
            bin = Convert.ToString(dataArray[12], 2).PadLeft(8, '0');    //you have to pad zeros to the left because ToString doesn't write first zeros
            if (bin[0] == '1') bloodlust = true;
                        
            //Simplified combat on/off
            bool simplifiedCombat = false;
            if (bin[3] == '1') simplifiedCombat = true;
                        
            //Flat/round earth
            bool flatEarth = false;
            bin = Convert.ToString(dataArray[13], 2).PadLeft(8, '0');
            if (bin[0] == '1') flatEarth = true;

            //Don't restart if eliminated
            bool dontRestartIfEliminated = false;
            if (bin[7] == '1') dontRestartIfEliminated = true;

            //Move units without mouse
            bool moveUnitsWithoutMouse = false;
            bin = Convert.ToString(dataArray[14], 2).PadLeft(8, '0');
            if (bin[0] == '1') moveUnitsWithoutMouse = true;

            //Enter closes city screen
            bool enterClosestCityScreen = false;
            if (bin[1] == '1') enterClosestCityScreen = true;
                        
            //Grid on/off
            bool grid = false;
            if (bin[2] == '1') grid = true;

            //Sound effects on/off
            bool soundEffects = false;
            if (bin[3] == '1') soundEffects = true;

            //Music on/off
            bool music = false;
            if (bin[4] == '1') music = true;

            //Cheat menu on/off
            bool cheatMenu = false;
            bin = Convert.ToString(dataArray[15], 2).PadLeft(8, '0');
            if (bin[0] == '1') cheatMenu = true;

            //Always wait at end of turn on/off
            bool alwaysWaitAtEndOfTurn = false;
            if (bin[1] == '1') alwaysWaitAtEndOfTurn = true;

            //Autosave each turn on/off
            bool autosaveEachTurn = false;
            if (bin[2] == '1') autosaveEachTurn = true;

            //Show enemy moves on/off
            bool showEnemyMoves = false;
            if (bin[3] == '1') showEnemyMoves = true;

            //No pause after enemy moves on/off
            bool noPauseAfterEnemyMoves = false;
            if (bin[4] == '1') noPauseAfterEnemyMoves = true;

            //Fast piece slide on/off
            bool fastPieceSlide = false;
            if (bin[5] == '1') fastPieceSlide = true;

            //Instant advice on/off
            bool instantAdvice = false;
            if (bin[6] == '1') instantAdvice = true;

            //Tutorial help on/off
            bool tutorialHelp = false;
            if (bin[7] == '1') tutorialHelp = true;

            //Animated heralds on/off
            bool animatedHeralds = false;
            bin = Convert.ToString(dataArray[16], 2).PadLeft(8, '0');
            if (bin[2] == '1') animatedHeralds = true;

            //High council on/off
            bool highCouncil = false;
            if (bin[3] == '1') highCouncil = true;

            //Civilopedia for advances on/off
            bool civilopediaForAdvances = false;
            if (bin[4] == '1') civilopediaForAdvances = true;

            //Throne room graphics on/off
            bool throneRoomGraphics = false;
            if (bin[5] == '1') throneRoomGraphics = true;
            
            //Diplomacy screen graphics on/off
            bool diplomacyScreenGraphics = false;
            if (bin[6] == '1') diplomacyScreenGraphics = true;

            //Wonder movies on/off
            bool wonderMovies = false;
            if (bin[7] == '1') wonderMovies = true;

            //Cheat penalty/warning on/off
            bool cheatPenaltyWarning = false;
            bin = Convert.ToString(dataArray[20], 2).PadLeft(8, '0');
            if (bin[3] == '1') cheatPenaltyWarning = true;

            //Announce we love king day on/off
            bool announceWeLoveKingDay = false;
            bin = Convert.ToString(dataArray[22], 2).PadLeft(8, '0');
            if (bin[0] == '1') announceWeLoveKingDay = true;

            //Warn when food dangerously low on/off
            bool warnWhenFoodDangerouslyLow = false;
            if (bin[1] == '1') warnWhenFoodDangerouslyLow = true;

            //Announce cities in disorder on/off
            bool announceCitiesInDisorder = false;
            if (bin[2] == '1') announceCitiesInDisorder = true;

            //Announce order restored in cities on/off
            bool announceOrderRestored = false;
            if (bin[3] == '1') announceOrderRestored = true;

            //Show non combat units build on/off
            bool showNonCombatUnitsBuilt = false;
            if (bin[4] == '1') showNonCombatUnitsBuilt = true;

            //Show invalid build instructions on/off
            bool showInvalidBuildInstructions = false;
            if (bin[5] == '1') showInvalidBuildInstructions = true;

            //Warn when city growth halted on/off
            bool warnWhenCityGrowthHalted = false;
            if (bin[6] == '1') warnWhenCityGrowthHalted = true;

            //Show city improvements built on/off
            bool showCityImprovementsBuilt = false;
            if (bin[7] == '1') showCityImprovementsBuilt = true;

            //Zoom to city not default action on/off
            bool zoomToCityNotDefaultAction = false;
            bin = Convert.ToString(dataArray[23], 2).PadLeft(8, '0');
            if (bin[5] == '1') zoomToCityNotDefaultAction = true;

            //Warn when pollution occurs on/off
            bool warnWhenPollutionOccurs = false;
            if (bin[6] == '1') warnWhenPollutionOccurs = true;

            //Warn when changing production will cost shileds on/off
            bool warnWhenChangingProductionWillCostShields = false;
            if (bin[7] == '1') warnWhenChangingProductionWillCostShields = true;

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
            bool mapRevealed = false;
            if (dataArray[43] == 1) mapRevealed = true;

            //Difficulty level
            int difficultyLevel = dataArray[44];

            //Barbarian activity
            int barbarianActivity = dataArray[45];

            //Civs in play
            int[] civsInPlay = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
            string conv = Convert.ToString(dataArray[46], 2).PadLeft(8, '0');
            for (int i = 0; i < 8; i++) if (conv[i] == '1') civsInPlay[i] = 1;

            //Civs with human player playing (multiplayer)
            string humanPlayerPlayed = Convert.ToString(dataArray[47], 2).PadLeft(8, '0');

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
            int[] wonderCity = new int[28]; //city which has wonder
            bool[] wonderBuilt = new bool[28];  //has the wonder been built
            bool[] wonderDestroyed = new bool[28];  //has the wonder been destroyed
            for (int i = 0; i < 28; i++)
            {
                //City number with the wonder
                intVal1 = dataArray[266 + 2 * i];
                intVal2 = dataArray[266 + 2 * i + 1];
                wonderCity[i] = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                //determine if wonder is built/destroyed
                if (wonderCity[i] == 65535) wonderBuilt[i] = false; //FFFF(hex)
                else if (wonderCity[i] == 65279) wonderDestroyed[i] = true; //FEFF(hex)
                else { wonderBuilt[i] = true; wonderDestroyed[i] = false; }
            }

            //=========================
            //CIVS
            //=========================
            char[] asciich = new char[23];
            int[] civCityStyle = new int[8];
            string[] civLeaderName = new string[8];
            string[] civTribeName = new string[8];
            string[] civAdjective = new string[8];
            for (int i = 0; i < 7; i++) //for 7 civs, but NOT for barbarians (barbarians have i=0, so begin count at 1)
            {
                //City style
                civCityStyle[i + 1] = dataArray[584 + 242 * i];

                //Leader names (if empty, get the name from RULES.TXT)
                for (int j = 0; j < 23; j++)
                {
                    asciich[j] = Convert.ToChar(dataArray[584 + 2 + 242 * i + j]);
                }
                civLeaderName[i + 1] = new string(asciich);
                civLeaderName[i + 1] = civLeaderName[i + 1].Replace("\0", string.Empty);  //remove null characters
                //Console.WriteLine(civLeaderName);

                //Tribe name (if empty, get the name from RULES.TXT)
                for (int j = 0; j < 23; j++)
                {
                    asciich[j] = Convert.ToChar(dataArray[584 + 2 + 23 + 242 * i + j]);
                }
                civTribeName[i + 1] = new string(asciich);
                civTribeName[i + 1] = civTribeName[i + 1].Replace("\0", string.Empty);
                //Console.WriteLine(civTribeName[i + 1]);

                //Adjective (if empty, get the name from RULES.TXT)
                for (int j = 0; j < 23; j++)
                {
                    asciich[j] = Convert.ToChar(dataArray[584 + 2 + 23 + 23 + 242 * i + j]);
                }
                civAdjective[i + 1] = new string(asciich);
                civAdjective[i + 1] = civAdjective[i + 1].Replace("\0", string.Empty);
                //Console.WriteLine(civAdjective[i + 1]);

                //Leader titles (Anarchy, Despotism, ...)
                // .... TO-DO ....

            }

            //Manually add data for barbarians
            civCityStyle[0] = 0;
            civLeaderName[0] = "NULL";
            civTribeName[0] = "Barbarians";
            civAdjective[0] = "Barbarian";

            //=========================
            //TECH & MONEY
            //=========================
            int[] rulerGender = new int[8];
            int[] civMoney = new int[8];
            int[] tribeNumber = new int[8];
            int[] civResearchProgress = new int[8];
            int[] civResearchingTech = new int[8];
            int[] civTaxRate = new int[8];
            int[] civGovernment = new int[8];
            int[] civReputation = new int[8];
            int[] civTechs = new int[89];
            //starting offset = 8E6(hex) = 2278(10), each block has 1427(10) bytes
            for (int i = 0; i < 8; i++) //for each civ
            {
                //Gender (0=male, 2=female)
                rulerGender[i] = dataArray[2278 + 1428 * i + 1]; //2nd byte in tribe block

                //Money
                intVal1 = dataArray[2278 + 1428 * i + 2];    //3rd byte in tribe block
                intVal2 = dataArray[2278 + 1428 * i + 3];    //4th byte in tribe block
                civMoney[i] = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                //Tribe number as per @Leaders table in RULES.TXT
                tribeNumber[i] = dataArray[2278 + 1428 * i + 6];    //7th byte in tribe block

                //Research progress
                intVal1 = dataArray[2278 + 1428 * i + 8];    //9th byte in tribe block
                intVal2 = dataArray[2278 + 1428 * i + 9];    //10th byte in tribe block
                civResearchProgress[i] = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                //Tech currently being researched
                civResearchingTech[i] = dataArray[2278 + 1428 * i + 10]; //11th byte in tribe block (FF(hex) = no goal)

                //Tax/science percentages
                civTaxRate[i] = dataArray[2278 + 1428 * i + 20]; //21st byte in tribe block

                //Government
                civGovernment[i] = dataArray[2278 + 1428 * i + 21]; //22nd byte in tribe block (0=anarchy, ...)

                //Reputation
                civReputation[i] = dataArray[2278 + 1428 * i + 30]; //31st byte in tribe block

                //Treaties
                // ..... TO-DO .....

                //Attitudes
                // ..... TO-DO .....

                //Technologies
                string civTechs1 = Convert.ToString(dataArray[2278 + 1428 * i + 88], 2).PadLeft(8, '0');    //89th byte
                string civTechs2 = Convert.ToString(dataArray[2278 + 1428 * i + 89], 2).PadLeft(8, '0');
                string civTechs3 = Convert.ToString(dataArray[2278 + 1428 * i + 90], 2).PadLeft(8, '0');
                string civTechs4 = Convert.ToString(dataArray[2278 + 1428 * i + 91], 2).PadLeft(8, '0');
                string civTechs5 = Convert.ToString(dataArray[2278 + 1428 * i + 92], 2).PadLeft(8, '0');
                string civTechs6 = Convert.ToString(dataArray[2278 + 1428 * i + 93], 2).PadLeft(8, '0');
                string civTechs7 = Convert.ToString(dataArray[2278 + 1428 * i + 94], 2).PadLeft(8, '0');
                string civTechs8 = Convert.ToString(dataArray[2278 + 1428 * i + 95], 2).PadLeft(8, '0');
                string civTechs9 = Convert.ToString(dataArray[2278 + 1428 * i + 96], 2).PadLeft(8, '0');
                string civTechs10 = Convert.ToString(dataArray[2278 + 1428 * i + 97], 2).PadLeft(8, '0');
                string civTechs11 = Convert.ToString(dataArray[2278 + 1428 * i + 98], 2).PadLeft(8, '0');
                string civTechs12 = Convert.ToString(dataArray[2278 + 1428 * i + 99], 2).PadLeft(8, '0');
                string civTechs13 = Convert.ToString(dataArray[2278 + 1428 * i + 100], 2).PadLeft(8, '0');   //101st byte
                civTechs13 = civTechs13.Remove(civTechs13.Length - 4); //remove last 4 bits, they are not important
                //Put all techs into one large string, where bit0=1st tech, bit1=2nd tech, ..., bit99=100th tech
                //First reverse bit order in all strings
                civTechs1 = Reverse(civTechs1);
                civTechs2 = Reverse(civTechs2);
                civTechs3 = Reverse(civTechs3);
                civTechs4 = Reverse(civTechs4);
                civTechs5 = Reverse(civTechs5);
                civTechs6 = Reverse(civTechs6);
                civTechs7 = Reverse(civTechs7);
                civTechs8 = Reverse(civTechs8);
                civTechs9 = Reverse(civTechs9);
                civTechs10 = Reverse(civTechs10);
                civTechs11 = Reverse(civTechs11);
                civTechs12 = Reverse(civTechs12);
                civTechs13 = Reverse(civTechs13);
                //Merge all strings into a large string
                string civTechs_ = String.Concat(civTechs1, civTechs2, civTechs3, civTechs4, civTechs5, civTechs6, civTechs7, civTechs8, civTechs9, civTechs10, civTechs11, civTechs12, civTechs13);
                //True = tech researched, false = not researched
                for (int no = 0; no < 89; no++)
                {
                    if (civTechs_[no] == '1') civTechs[no] = 1;
                    else civTechs[no] = 0;
                }

                Civilization civ = CreateCiv(i, whichHumanPlayerIsUsed, civCityStyle[i], civLeaderName[i], civTribeName[i], civAdjective[i], rulerGender[i], civMoney[i], tribeNumber[i], civResearchProgress[i], civResearchingTech[i], civTaxRate[i], civGovernment[i], civReputation[i], civTechs);
            }



            //=========================
            //MAPS
            //=========================
            //Map header ofset
            int ofset;
            if (version > 1) ofset = 13702;  //FW and later (offset=3586hex)
            else ofset = 13432; //Conflicts (offset=3478hex)

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

            SetGameData(turnNumber, turnNumberForGameYear, unitSelectedAtGameStart, whichHumanPlayerIsUsed, civsInPlay, playersMapUsed, playersCivilizationNumberUsed, mapRevealed, difficultyLevel, barbarianActivity, pollutionAmount, globalTempRiseOccured, noOfTurnsOfPeace, numberOfUnits, numberOfCities, mapXdimension, mapYdimension, mapArea, mapSeed, locatorMapXDimension, locatorMapYDimension);
            
            //Initialize Terrain array now that you know its size
            Terrain = new ITerrain[mapXdimension, mapYdimension];

            //block 1 - terrain improvements (for indivudual civs)
            int ofsetB1 = ofset + 14; //offset for block 2 values
            //...........
            //block 2 - terrain type
            int ofsetB2 = ofsetB1 + 7 * mapArea; //offset for block 2 values
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
                if (bin[7] == '1') unit_present = true;
                if (bin[6] == '1') city_present = true;
                if (bin[5] == '1') irrigation = true;
                if (bin[4] == '1') mining = true;
                if (bin[3] == '1') road = true;
                if (bin[2] == '1' && bin[3] == '1') railroad = true;
                if (bin[1] == '1') fortress = true;
                if (bin[0] == '1') pollution = true;
                if (bin[4] == '1' && bin[5] == '1') farmland = true;
                if (bin[1] == '1' && bin[6] == '1') airbase = true;

                //City radius (TO-DO)
                int intValueB23 = dataArray[ofsetB2 + i * 6 + 2];
                
                //Island counter
                int terrain_island = dataArray[ofsetB2 + i * 6 + 3];

                //Visibility (TO-DO)
                int intValueB25 = dataArray[ofsetB2 + i * 6 + 4];

                int intValueB26 = dataArray[ofsetB2 + i * 6 + 5];   //?

                //string hexValue = intValueB26.ToString("X");

                //SAV file doesn't tell where special resources are, so you have to determine this yourself
                int specialtype = ReturnSpecial(x, y, type, mapXdimension, mapYdimension);

                CreateTerrain(x, y, type, specialtype, resource, river, terrain_island, unit_present, city_present, irrigation, mining, road, railroad, fortress, pollution, farmland, airbase, bin);
                
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
            if (version <= 2) multipl = 26; //FW or CiC
            else if (version == 3) multipl = 32;    //MGE
            else multipl = 40;  //ToT

            for (int i = 0; i < numberOfUnits; i++)
            {
                //Unit X location (civ2-style)
                intVal1 = dataArray[ofsetU + multipl * i + 0];
                intVal2 = dataArray[ofsetU + multipl * i + 1];
                int unitXlocation = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                //Unit is inactive (dead) if the value of X-Y is negative (1st bit = 1)
                bool unit_dead = false;
                bin = Convert.ToString(intVal2, 2).PadLeft(8, '0');
                if (bin[0] == '1') unit_dead = true;

                //Unit Y location (civ2-style)
                intVal1 = dataArray[ofsetU + multipl * i + 2];
                intVal2 = dataArray[ofsetU + multipl * i + 3];
                int unitYlocation = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                //Transform x-units from civ2-style to real coordiantes
                unitXlocation = (unitXlocation - (unitYlocation % 2)) / 2;

                //If this is the unit's first move
                bin = Convert.ToString(dataArray[ofsetU + multipl * i + 4], 2).PadLeft(8, '0');
                bool unitFirstMove = false;
                if (bin[1] == '1') unitFirstMove = true;

                //Grey star to the shield
                bin = Convert.ToString(dataArray[ofsetU + multipl * i + 5], 2).PadLeft(8, '0');
                bool unitGreyStarShield = false;
                if (bin[0] == '1') unitGreyStarShield = true;

                //Veteran status
                bool unitVeteranStatus = false;
                if (bin[2] == '1') unitVeteranStatus = true;

                //Unit type
                int unitType = dataArray[ofsetU + multipl * i + 6];

                //Unit civ
                int unitCiv = dataArray[ofsetU + multipl * i + 7];  //00 = barbarians
               
                //Unit move points expended
                int unitMovePointsLost = dataArray[ofsetU + multipl * i + 8];

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
                
                IUnit unit = CreateUnit((UnitType)unitType, unitXlocation, unitYlocation, unit_dead, unitFirstMove, unitGreyStarShield, unitVeteranStatus, unitCiv, unitMovePointsLost, unitHitpointsLost, unitLastMove, unitCaravanCommodity, (OrderType)unitOrders, unitHomeCity, unitGoToX, unitGoToY, unitLinkOtherUnitsOnTop, unitLinkOtherUnitsUnder);
            }


            //=========================
            //CITIES
            //=========================
            int ofsetC = ofsetU + multipl * numberOfUnits;
            //Console.WriteLine("offset Cities=" + ofsetC.ToString());

            if (version <= 2) multipl = 84; //FW or CiC
            else if (version == 3) multipl = 88;    //MGE
            else multipl = 92;  //ToT
                        
            char[] asciichar = new char[15];            
            for (int i = 0; i < numberOfCities; i++)
            {
                //City X location (civ2-style)
                intVal1 = dataArray[ofsetC + multipl * i + 0];
                intVal2 = dataArray[ofsetC + multipl * i + 1];
                int cityXlocation = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                //City Y location (civ2-style)
                intVal1 = dataArray[ofsetC + multipl * i + 2];
                intVal2 = dataArray[ofsetC + multipl * i + 3];
                int cityYlocation = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                //Transform x city location from civ2-style to real coordiantes
                cityXlocation = (cityXlocation - (cityYlocation % 2)) / 2;

                //Can build coastal improvements
                bin = Convert.ToString(dataArray[ofsetC + multipl * i + 4], 2).PadLeft(8, '0');
                bool cityCanBuildCoastal = false;
                if (bin[0] == '1') cityCanBuildCoastal = true;

                //Auto build under military rule
                bool cityAutobuildMilitaryRule = false;
                if (bin[3] == '1') cityAutobuildMilitaryRule = true;

                //Stolen tech
                bool cityStolenTech = false;
                if (bin[4] == '1') cityStolenTech = true;

                //Improvement sold
                bool cityImprovementSold = false;
                if (bin[5] == '1') cityImprovementSold = true;

                //We love king day
                bool cityWeLoveKingDay = false;
                if (bin[6] == '1') cityWeLoveKingDay = true;

                //Civil disorder
                bool cityCivilDisorder = false;
                if (bin[7] == '1') cityCivilDisorder = true;

                //Can build ships
                bin = Convert.ToString(dataArray[ofsetC + multipl * i + 6], 2).PadLeft(8, '0');
                bool cityCanBuildShips = false;
                if (bin[2] == '1') cityCanBuildShips = true;

                //Objective x3
                bin = Convert.ToString(dataArray[ofsetC + multipl * i + 7], 2).PadLeft(8, '0');
                bool cityObjectivex3 = false;
                if (bin[3] == '1') cityObjectivex3 = true;

                //Objective x1
                bin = Convert.ToString(dataArray[ofsetC + multipl * i + 7], 2).PadLeft(8, '0');
                bool cityObjectivex1 = false;
                if (bin[5] == '1') cityObjectivex1 = true;

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

                //Food in storage
                intVal1 = dataArray[ofsetC + multipl * i + 26];
                intVal2 = dataArray[ofsetC + multipl * i + 27];
                int cityFoodInStorage = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                //Shield progress
                intVal1 = dataArray[ofsetC + multipl * i + 28];
                intVal2 = dataArray[ofsetC + multipl * i + 29];
                int cityShieldsProgress = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

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

                //Workers in inner circle
                int cityWorkersInnerCircle = dataArray[ofsetC + multipl * i + 48];

                //Workers on 8 of the outer circle
                int cityWorkersOn8 = dataArray[ofsetC + multipl * i + 49];
                
                //Workers on 4 of the outer circle
                int cityWorkersOn4 = dataArray[ofsetC + multipl * i + 50];

                //Number of specialists x4
                int cityNoOfSpecialistsx4 = dataArray[ofsetC + multipl * i + 51];

                //Improvements
                string cityImprovements1 = Convert.ToString(dataArray[ofsetC + multipl * i + 52], 2).PadLeft(8, '0');   //bit6=palace (1st improvement), bit7=not important
                cityImprovements1 = cityImprovements1.Remove(cityImprovements1.Length - 1); //remove last bit, it is not important
                string cityImprovements2 = Convert.ToString(dataArray[ofsetC + multipl * i + 53], 2).PadLeft(8, '0');
                string cityImprovements3 = Convert.ToString(dataArray[ofsetC + multipl * i + 54], 2).PadLeft(8, '0');
                string cityImprovements4 = Convert.ToString(dataArray[ofsetC + multipl * i + 55], 2).PadLeft(8, '0');
                string cityImprovements5 = Convert.ToString(dataArray[ofsetC + multipl * i + 56], 2).PadLeft(8, '0');   //bit0-bit4=not important, bit5=port facility (last improvement)
                //Put all improvements into one large string, where bit0=palace, bit1=barracks, ..., bit33=port facility
                //First reverse bit order in all strings
                cityImprovements1 = Reverse(cityImprovements1);
                cityImprovements2 = Reverse(cityImprovements2);
                cityImprovements3 = Reverse(cityImprovements3);
                cityImprovements4 = Reverse(cityImprovements4);
                cityImprovements5 = Reverse(cityImprovements5);
                cityImprovements5 = cityImprovements5.Remove(cityImprovements5.Length - 5); //remove last 5 bits, they are not important
                //Merge all strings into a large string
                string cityImprovements = string.Format("{0}{1}{2}{3}{4}", cityImprovements1, cityImprovements2, cityImprovements3, cityImprovements4, cityImprovements5);

                //Item in production
                //0(dec)/0(hex) ... 61(dec)/3D(hex) are units, improvements are inversed (FF(hex)=1st, FE(hex)=2nd, ...)
                //convert this notation of improvements, so that 62(dec) is 1st improvement, 63(dec) is 2nd, ...
                int cityItemInProduction = dataArray[ofsetC + multipl * i + 57];
                if (cityItemInProduction > 70)  //if it is improvement
                {
                    cityItemInProduction = 255 - cityItemInProduction + 62; //62 because 0...61 are units
                }

                //No of active trade routes
                int cityActiveTradeRoutes = dataArray[ofsetC + multipl * i + 58];

                //1st, 2nd, 3rd trade commodities available
                //...

                //1st, 2nd, 3rd trade commodities demanded
                //...

                //1st, 2nd, 3rd trade commodities in route
                //...

                //1st, 2nd, 3rd trade route partner city number
                //...

                //Science
                intVal1 = dataArray[ofsetC + multipl * i + 74];
                intVal2 = dataArray[ofsetC + multipl * i + 75];
                int cityScience = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                //Tax
                intVal1 = dataArray[ofsetC + multipl * i + 76];
                intVal2 = dataArray[ofsetC + multipl * i + 77];
                int cityTax = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                //No of trade icons
                intVal1 = dataArray[ofsetC + multipl * i + 78];
                intVal2 = dataArray[ofsetC + multipl * i + 79];
                int cityNoOfTradeIcons = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                //Total food production
                int cityFoodProduction = dataArray[ofsetC + multipl * i + 80];

                //Total shield production
                int cityShieldProduction = dataArray[ofsetC + multipl * i + 81];

                //No of happy citizens
                int cityHappyCitizens = dataArray[ofsetC + multipl * i + 82];

                //No of unhappy citizens
                int cityUnhappyCitizens = dataArray[ofsetC + multipl * i + 83];

                //Sequence number of the city
                //...

                //Check if wonder is in city (28 possible wonders)
                int[] cityWonders = new int[28];
                for (int wndr = 0; wndr < 28; wndr++)
                {
                    if (wonderCity[wndr] == i) cityWonders[wndr] = 1;
                    else cityWonders[wndr] = 0;
                }

                City city = CreateCity(cityXlocation, cityYlocation, cityCanBuildCoastal, cityAutobuildMilitaryRule, cityStolenTech, cityImprovementSold, cityWeLoveKingDay, cityCivilDisorder, cityCanBuildShips, cityObjectivex3, cityObjectivex1, cityOwner, citySize, cityWhoBuiltIt, cityFoodInStorage, cityShieldsProgress, cityNetTrade, cityName, cityWorkersInnerCircle, cityWorkersOn8, cityWorkersOn4, cityNoOfSpecialistsx4, cityImprovements, cityItemInProduction, cityActiveTradeRoutes, cityScience, cityTax, cityNoOfTradeIcons, cityFoodProduction, cityShieldProduction, cityHappyCitizens, cityUnhappyCitizens, cityWonders);
            }


            //=========================
            //OTHER
            //=========================



        }
        
        //Reverse a string
        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        static int ReturnSpecial(int col, int row, TerrainType type, int mapXdim, int mapYdim)
        {
            int special = 0;

            //Special Resources (only grassland)
            //Grassland shield is present in pattern 1100110011... in 1st line, in 3rd line shifted by 1 to right (01100110011...), in 5th line shifted by 1 to right (001100110011...) etc.
            //In 2nd line 00110011..., in 4th line shifted right by 1 (1001100...), in 6th line shifted by 1 to right (11001100...) etc.
            //For grassland special = 0 (no shield), special = 1 (shield).
            if (type == TerrainType.Grassland)
            {
                if (row % 2 == 0) //odd lines
                {
                    if ((col + 4 - (row % 8) / 2) % 4 == 0 || (col + 4 - (row % 8) / 2) % 4 == 1) special = 1;
                    else special = 0;
                }
                else    //even lines
                {
                    if ((col + 4 - (row % 8) / 2) % 4 == 2 || (col + 4 - (row % 8) / 2) % 4 == 3) special = 1;
                    else special = 0;
                }

                //if (Game.Terrain[col, row].Special == 1) { graphics.DrawImage(Images.Shield, 0, 0); }
            }

            //Special Resources (not grassland)
            //(not yet 100% sure how this works)
            //No matter which terrain tile it is (except grassland). 2 special resources R1 & R2 (e.g. palm & oil for desert). R1 is (in x-direction) always followed by R2, then R1, R2, R1, ... First 2 (j=1,3) are special as they do not belong to other blocks described below. Next block has 7 y-coordinates (j=8,10,...20), next block has 6 (j=25,27,...35), next block 7 (j=40,42,...52), next block 6 (j=57,59,...67), ... Blocks are always 5 tiles appart in y-direction. In x-direction for j=1 the resources are 3/5/3/5 etc. tiles appart. For j=3 they are 8 tiles appart in x-direction. For the next block they are 8-8-8-(3/5/3/5)-8-8-8 tiles appart in x-direction. For the next block they are 8-(3/5/3/5)-8-8-(3/5/3/5)-8 tiles appart. Then these 4 blocks start repeating again. Starting points: For j=1 it is (0,1), for j=3 it is (6,3). The starting (x) points for the next block are x=3,6,4,2,5,3,6. For next block they are x=2,0,3,1,4,2. For the next block they are x=7,2,0,3,1,7,2. For next block they are x=6,1,7,5,3,6. These 4 patterns then start repeating again. So the next block has again pattern 3,6,4,2,5,3,6, the next block has x=2,0,3,1,4,2, etc.
            //For these tiles special=0 (no special, e.g. only desert), special=1 (special #1, e.g. oasis for desert), special=2 (special #2, e.g. oil for desert)
            int[] startx_B1 = new int[] { 3, 6, 4, 2, 5, 3, 6 };  //starting x-points for 4 blocks
            int[] startx_B2 = new int[] { 2, 0, 3, 1, 4, 2 };
            int[] startx_B3 = new int[] { 7, 2, 0, 3, 1, 7, 2 };
            int[] startx_B4 = new int[] { 6, 1, 7, 5, 3, 6 };
            if (type != TerrainType.Grassland)
            {
                special = 0;    //for start we presume this 
                bool found = false;

                if (row == 1) //prva posebna tocka
                {
                    int novi_i = 0; //zacetna tocka pri j=1 (0,1)
                    while (novi_i < mapXdim)  //keep jumping in x-direction till map end
                    {
                        if (novi_i < mapXdim && col == novi_i) special = 2; break;   //tocke (3,1), (11,1), (19,1), ...
                        novi_i += 3;
                        if (novi_i < mapXdim && col == novi_i) special = 1; break;   //tocke (8,1), (16,1), (24,1), ...
                        novi_i += 5;
                    }

                }
                else if (row == 3)    //druga posebna tocka
                {
                    int novi_i = 6; //zacetna tocka pri j=3 je (6,3)
                    while (novi_i < mapXdim)
                    {
                        if (novi_i < mapXdim && col == novi_i) { special = 1; break; }
                        novi_i += 8;
                        if (novi_i < mapXdim && col == novi_i) { special = 2; break; }
                        novi_i += 8;
                    }

                }
                else
                {
                    int novi_j = 3;
                    while (novi_j < mapYdim)  //skakanje za 4 bloke naprej
                    {
                        if (found) break;

                        //BLOCK 1
                        int counter = 0;
                        novi_j += 5;   //jump to block beginning
                        while (novi_j < mapYdim && counter < 7)  //7 jumps in y-direction
                        {
                            if (found) break;

                            if (row == novi_j)    //correct y-loc found, now start looking for x
                            {
                                int novi_i = startx_B1[counter];
                                //set which resources will be and jumps
                                int res1, res2;
                                int skok_x1, skok_x2;
                                if (counter == 3)
                                {
                                    skok_x1 = 5;
                                    skok_x2 = 3;
                                    res1 = 2;
                                    res2 = 1;
                                }
                                else if (counter == 0 || counter == 1 || counter == 4)
                                {
                                    skok_x1 = 8;
                                    skok_x2 = 8;
                                    res1 = 2;
                                    res2 = 2;
                                }
                                else
                                {
                                    skok_x1 = 8;
                                    skok_x2 = 8;
                                    res1 = 1;
                                    res2 = 1;
                                }

                                while (novi_i < mapXdim)
                                {
                                    if (novi_i < mapXdim && col == novi_i) { special = res1; found = true; break; }
                                    novi_i += skok_x1;
                                    if (novi_i < mapXdim && col == novi_i) { special = res2; found = true; break; }
                                    novi_i += skok_x2;

                                    if (found) break;
                                }
                                break;   //terminate search
                            }
                            novi_j += 2;
                            counter += 1;
                        }
                        if (found) break;

                        //BLOCK 2
                        counter = 0;
                        novi_j += 5;   //jump to block beginning
                        while (novi_j < mapYdim && counter < 6)  //6 jumps in y-direction
                        {
                            if (found) break;

                            if (row == novi_j)    //correct y-loc found, now start looking for x
                            {
                                int novi_i = startx_B2[counter];
                                //set which resources will be and jumps
                                int res1, res2;
                                int skok_x1, skok_x2;
                                if (counter == 1)   //1st jump
                                {
                                    skok_x1 = 5;
                                    skok_x2 = 3;
                                    res1 = 1;
                                    res2 = 2;
                                }
                                else if (counter == 4)  //4th jump
                                {
                                    skok_x1 = 3;
                                    skok_x2 = 5;
                                    res1 = 2;
                                    res2 = 1;
                                }
                                else if (counter == 0 || counter == 3)
                                {
                                    skok_x1 = 8;
                                    skok_x2 = 8;
                                    res1 = 2;
                                    res2 = 2;
                                }
                                else
                                {
                                    skok_x1 = 8;
                                    skok_x2 = 8;
                                    res1 = 1;
                                    res2 = 1;
                                }

                                while (novi_i < mapXdim)
                                {
                                    if (novi_i < mapXdim && col == novi_i) { special = res1; found = true; break; }
                                    novi_i += skok_x1;
                                    if (novi_i < mapXdim && col == novi_i) { special = res2; found = true; break; }
                                    novi_i += skok_x2;

                                    if (found) break;
                                }
                                break;   //terminate search
                            }
                            novi_j += 2;
                            counter += 1;
                        }
                        if (found) break;

                        //BLOCK 3
                        counter = 0;
                        novi_j += 5;   //jump to block beginning
                        while (novi_j < mapYdim && counter < 7)  //7 jumps in y-direction
                        {
                            if (found) break;

                            if (row == novi_j)    //correct y-loc found, now start looking for x
                            {
                                int novi_i = startx_B3[counter];
                                //set which resources will be and jumps
                                int res1, res2;
                                int skok_x1, skok_x2;
                                if (counter == 3)   //3rd jump
                                {
                                    skok_x1 = 3;
                                    skok_x2 = 5;
                                    res1 = 1;
                                    res2 = 2;
                                }
                                else if (counter == 0 || counter == 1 || counter == 4)
                                {
                                    skok_x1 = 8;
                                    skok_x2 = 8;
                                    res1 = 2;
                                    res2 = 2;
                                }
                                else
                                {
                                    skok_x1 = 8;
                                    skok_x2 = 8;
                                    res1 = 1;
                                    res2 = 1;
                                }

                                while (novi_i < mapXdim)
                                {
                                    if (novi_i < mapXdim && col == novi_i) { special = res1; found = true; break; }
                                    novi_i += skok_x1;
                                    if (novi_i < mapXdim && col == novi_i) { special = res2; found = true; break; }
                                    novi_i += skok_x2;

                                    if (found) break;
                                }
                                break;   //terminate search
                            }
                            novi_j += 2;
                            counter += 1;
                        }
                        if (found) break;

                        //BLOCK 4
                        counter = 0;
                        novi_j += 5;   //jump to block beginning
                        while (novi_j < mapYdim && counter < 6)  //6 jumps in y-direction
                        {
                            if (found) break;

                            if (row == novi_j)    //correct y-loc found, now start looking for x
                            {
                                int novi_i = startx_B4[counter];
                                //set which resources will be and jumps
                                int res1, res2;
                                int skok_x1, skok_x2;
                                if (counter == 1 || counter == 4)   //1st & 3rd jump
                                {
                                    skok_x1 = 3;
                                    skok_x2 = 5;
                                    res1 = 2;
                                    res2 = 1;
                                }
                                else if (counter == 0 || counter == 3)
                                {
                                    skok_x1 = 8;
                                    skok_x2 = 8;
                                    res1 = 2;
                                    res2 = 2;
                                }
                                else
                                {
                                    skok_x1 = 8;
                                    skok_x2 = 8;
                                    res1 = 1;
                                    res2 = 1;
                                }

                                while (novi_i < mapXdim)
                                {
                                    if (novi_i < mapXdim && col == novi_i) { special = res1; found = true; break; }
                                    novi_i += skok_x1;
                                    if (novi_i < mapXdim && col == novi_i) { special = res2; found = true; break; }
                                    novi_i += skok_x2;

                                    if (found) break;
                                }
                                break;   //terminate search
                            }
                            novi_j += 2;
                            counter += 1;
                        }
                        if (found) break;

                    }
                }

            }

            return special;
        }

    }
}
