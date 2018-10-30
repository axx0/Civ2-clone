using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace PoskusCiv2
{
    public class ImportSavegame
    //internal class ImportSavegame : Game
    {
        public int GameVersion { get { return version; } }
        public bool Bloodlust { get { return bloodlust; } }
        public bool SimplifiedCombat { get { return simplifiedCombat; } }
        public bool FlatEarth { get { return flatEarth; } }
        public bool DontRestartIfEliminated { get { return dontRestartIfEliminated; } }
        public bool MoveUnitsWithoutMouse { get { return moveUnitsWithoutMouse; } }
        public bool EnterClosestCityScreen { get { return enterClosestCityScreen; } }
        public bool Grid { get { return grid; } }
        public bool SoundEffects { get { return soundEffects; } }
        public bool Music { get { return music; } }
        public bool CheatMenu { get { return cheatMenu; } }
        public bool AlwaysWaitAtEndOfTurn { get { return alwaysWaitAtEndOfTurn; } }
        public bool AutosaveEachTurn { get { return autosaveEachTurn; } }
        public bool ShowEnemyMoves { get { return showEnemyMoves; } }
        public bool NoPauseAfterEnemyMoves { get { return noPauseAfterEnemyMoves; } }
        public bool FastPieceSlide { get { return fastPieceSlide; } }
        public bool InstantAdvice { get { return instantAdvice; } }
        public bool TutorialHelp { get { return tutorialHelp; } }
        public bool AnimatedHeralds { get { return animatedHeralds; } }
        public bool HighCouncil { get { return highCouncil; } }
        public bool CivilopediaForAdvances { get { return civilopediaForAdvances; } }
        public bool ThroneRoomGraphics { get { return throneRoomGraphics; } }
        public bool DiplomacyScreenGraphics { get { return diplomacyScreenGraphics; } }
        public bool WonderMovies { get { return wonderMovies; } }
        public bool CheatPenaltyWarning { get { return cheatPenaltyWarning; } }
        public bool AnnounceWeLoveKingDay { get { return announceWeLoveKingDay; } }
        public bool WarnWhenFoodDangerouslyLow { get { return warnWhenFoodDangerouslyLow; } }
        public bool AnnounceCitiesInDisorder { get { return announceCitiesInDisorder; } }
        public bool AnnounceOrderRestored { get { return announceOrderRestored; } }
        public bool ShowNonCombatUnitsBuilt { get { return showNonCombatUnitsBuilt; } }
        public bool ShowInvalidBuildInstructions { get { return showInvalidBuildInstructions; } }
        public bool WarnWhenCityGrowthHalted { get { return warnWhenCityGrowthHalted; } }
        public bool ShowCityImprovementsBuilt { get { return showCityImprovementsBuilt; } }
        public bool ZoomToCityNotDefaultAction { get { return zoomToCityNotDefaultAction; } }
        public bool WarnWhenPollutionOccurs { get { return warnWhenPollutionOccurs; } }
        public bool WarnWhenChangingProductionWillCostShields { get { return warnWhenChangingProductionWillCostShields; } }
        public int TurnNumber { get { return turnNumber; } }
        public int TurnNumberForGameYear { get { return turnNumberForGameYear; } }
        public int UnitSelectedAtGameStart { get { return unitSelectedAtGameStart; } }
        public int WhichHumanPlayerIsUsed { get { return whichHumanPlayerIsUsed; } }
        public int PlayersMapUsed { get { return playersMapUsed; } }
        public int PlayersCivilizationNumberUsed { get { return playersCivilizationNumberUsed; } }
        public bool MapRevealed { get { return mapRevealed; } }
        public int DifficultyLevel { get { return difficultyLevel; } }
        public int BarbarianActivity { get { return barbarianActivity; } }
        public int PollutionAmount { get { return pollutionAmount; } }
        public int GlobalTempRiseOccured { get { return globalTempRiseOccured; } }
        public int NoOfTurnsOfPeace { get { return noOfTurnsOfPeace; } }
        public int NumberOfUnits { get { return numberOfUnits; } }
        public int NumberOfCities { get { return numberOfCities; } }

        public int MapXdimension { get { return mapXdimension; } }
        public int MapYdimension { get { return mapYdimension; } }
        public int MapArea { get { return mapArea; } }
        public int MapSeed { get { return mapSeed; } }
        public int LocatorMapXDimension { get { return locatorMapXDimension; } }
        public int LocatorMapYDimension { get { return locatorMapYDimension; } }
        public int[] Terrain { get { return terrain; } }
        public int[] TerrainIsland { get { return terrain_island; } }

        public int[] UnitXlocation { get { return unitXlocation; } }
        public int[] UnitYlocation { get { return unitYlocation; } }
        public bool[] UnitGreyStarShield { get { return unitGreyStarShield; } }
        public bool[] UnitVeteranStatus { get { return unitVeteranStatus; } }
        public int[] UnitType { get { return unitType; } }
        public int[] UnitCiv { get { return unitCiv; } }
        public int[] UnitHitpointsLost { get { return unitHitpointsLost; } }
        public int[] UnitCaravanCommodity { get { return unitCaravanCommodity; } }
        public int[] UnitOrders { get { return unitOrders; } }
        public int[] UnitHomeCity { get { return unitHomeCity; } }
        public int[] UnitGoToX { get { return unitGoToX; } }
        public int[] UnitGoToY { get { return unitGoToY; } }
        public int[] UnitLinkOtherUnitsOnTop { get { return unitLinkOtherUnitsOnTop; } }
        public int[] UnitLinkOtherUnitsUnder { get { return unitLinkOtherUnitsUnder; } }

        private int version, turnNumber, turnNumberForGameYear, unitSelectedAtGameStart, whichHumanPlayerIsUsed, playersMapUsed, playersCivilizationNumberUsed, difficultyLevel, barbarianActivity, pollutionAmount, globalTempRiseOccured, noOfTurnsOfPeace, mapXdimension, mapYdimension, mapArea, locatorMapXDimension, locatorMapYDimension, mapSeed, numberOfUnits, numberOfCities;
        private bool bloodlust, simplifiedCombat, flatEarth, dontRestartIfEliminated, moveUnitsWithoutMouse, enterClosestCityScreen, grid, soundEffects, music, cheatMenu, alwaysWaitAtEndOfTurn, autosaveEachTurn, showEnemyMoves, noPauseAfterEnemyMoves, fastPieceSlide, instantAdvice, tutorialHelp, animatedHeralds, highCouncil, civilopediaForAdvances, throneRoomGraphics, diplomacyScreenGraphics, wonderMovies, cheatPenaltyWarning, announceWeLoveKingDay, warnWhenFoodDangerouslyLow, announceCitiesInDisorder, announceOrderRestored, showNonCombatUnitsBuilt, showInvalidBuildInstructions, warnWhenCityGrowthHalted, showCityImprovementsBuilt, zoomToCityNotDefaultAction, warnWhenPollutionOccurs, warnWhenChangingProductionWillCostShields, mapRevealed;
        private int[] terrain, terrain_island, dataArray, unitXlocation, unitYlocation, unitType, unitCiv, unitHitpointsLost, unitCaravanCommodity, unitOrders, unitHomeCity, unitGoToX, unitGoToY, unitLinkOtherUnitsOnTop, unitLinkOtherUnitsUnder;
        private bool[] unitGreyStarShield, unitVeteranStatus;

        public ImportSavegame()
        {
            //FileStream fs = new FileStream("C:/DOS/CIV 2/Civ2/Ab_Auto2.SAV", FileMode.Open, FileAccess.Read);        //Enter filename
            FileStream fs = new FileStream("C:/DOS/CIV 2/Civ2/Rome01.SAV", FileMode.Open, FileAccess.Read);        //Enter filename
            dataArray = new int[fs.Length];
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
            if (dataArray[10] == 39) { version = 1; }    //Conflicts (27 in hex)
            else if (dataArray[10] == 40) { version = 2; } //FW (28 in hex)
            else if (dataArray[10] == 44) { version = 3; }    //MGE (2C in hex)
            else if (dataArray[10] == 49) { version = 4; }    //ToT (31 in hex)
            else { version = 1; }   //lower than Conflicts

            //Bloodlust on/off
            bin = Convert.ToString(dataArray[12], 2).PadLeft(8, '0');    //you have to pad zeros to the left because ToString doesn't write first zeros
            if (bin[0] == '1') { bloodlust = true; }
            else { bloodlust = false; }

            //Simplified combat on/off
            if (bin[3] == '1') { simplifiedCombat = true; }
            else { simplifiedCombat = false; }

            //Flat/round earth
            bin = Convert.ToString(dataArray[13], 2).PadLeft(8, '0');
            if (bin[0] == '1') { flatEarth = true; }
            else { flatEarth = false; }

            //Don't restart if eliminated
            if (bin[7] == '1') { dontRestartIfEliminated = true; }
            else { dontRestartIfEliminated = false; }

            //Move units without mouse
            bin = Convert.ToString(dataArray[14], 2).PadLeft(8, '0');
            if (bin[0] == '1') { moveUnitsWithoutMouse = true; }
            else { moveUnitsWithoutMouse = false; }

            //Enter closes city screen
            if (bin[1] == '1') { enterClosestCityScreen = true; }
            else { enterClosestCityScreen = false; }

            //Grid on/off
            if (bin[2] == '1') { grid = true; }
            else { grid = false; }

            //Sound effects on/off
            if (bin[3] == '1') { soundEffects = true; }
            else { soundEffects = false; }

            //Music on/off
            if (bin[4] == '1') { music = true; }
            else { music = false; }

            //Cheat menu on/off
            bin = Convert.ToString(dataArray[15], 2).PadLeft(8, '0');
            if (bin[0] == '1') { cheatMenu = true; }
            else { cheatMenu = false; }

            //Always wait at end of turn on/off
            if (bin[1] == '1') { alwaysWaitAtEndOfTurn = true; }
            else { alwaysWaitAtEndOfTurn = false; }

            //Autosave each turn on/off
            if (bin[2] == '1') { autosaveEachTurn = true; }
            else { autosaveEachTurn = false; }

            //Show enemy moves on/off
            if (bin[3] == '1') { showEnemyMoves = true; }
            else { showEnemyMoves = false; }

            //No pause after enemy moves on/off
            if (bin[4] == '1') { noPauseAfterEnemyMoves = true; }
            else { noPauseAfterEnemyMoves = false; }

            //Fast piece slide on/off
            if (bin[5] == '1') { fastPieceSlide = true; }
            else { fastPieceSlide = false; }

            //Instant advice on/off
            if (bin[6] == '1') { instantAdvice = true; }
            else { instantAdvice = false; }

            //Tutorial help on/off
            if (bin[7] == '1') { tutorialHelp = true; }
            else { tutorialHelp = false; }

            //Animated heralds on/off
            bin = Convert.ToString(dataArray[16], 2).PadLeft(8, '0');
            if (bin[2] == '1') { animatedHeralds = true; }
            else { animatedHeralds = false; }

            //High council on/off
            if (bin[3] == '1') { highCouncil = true; }
            else { highCouncil = false; }

            //Civilopedia for advances on/off
            if (bin[4] == '1') { civilopediaForAdvances = true; }
            else { civilopediaForAdvances = false; }

            //Throne room graphics on/off
            if (bin[5] == '1') { throneRoomGraphics = true; }
            else { throneRoomGraphics = false; }

            //Diplomacy screen graphics on/off
            if (bin[6] == '1') { diplomacyScreenGraphics = true; }
            else { diplomacyScreenGraphics = false; }

            //Wonder movies on/off
            if (bin[7] == '1') { wonderMovies = true; }
            else { wonderMovies = false; }

            //Cheat penalty/warning on/off
            bin = Convert.ToString(dataArray[20], 2).PadLeft(8, '0');
            if (bin[3] == '1') { cheatPenaltyWarning = true; }
            else { cheatPenaltyWarning = false; }

            //Announce we love king day on/off
            bin = Convert.ToString(dataArray[22], 2).PadLeft(8, '0');
            if (bin[0] == '1') { announceWeLoveKingDay = true; }
            else { announceWeLoveKingDay = false; }

            //Warn when food dangerously low on/off
            if (bin[1] == '1') { warnWhenFoodDangerouslyLow = true; }
            else { warnWhenFoodDangerouslyLow = false; }

            //Announce cities in disorder on/off
            if (bin[2] == '1') { announceCitiesInDisorder = true; }
            else { announceCitiesInDisorder = false; }

            //Announce order restored in cities on/off
            if (bin[3] == '1') { announceOrderRestored = true; }
            else { announceOrderRestored = false; }

            //Show non combat units build on/off
            if (bin[4] == '1') { showNonCombatUnitsBuilt = true; }
            else { showNonCombatUnitsBuilt = false; }

            //Show invalid build instructions on/off
            if (bin[5] == '1') { showInvalidBuildInstructions = true; }
            else { showInvalidBuildInstructions = false; }

            //Warn when city growth halted on/off
            if (bin[6] == '1') { warnWhenCityGrowthHalted = true; }
            else { warnWhenCityGrowthHalted = false; }

            //Show city improvements built on/off
            if (bin[7] == '1') { showCityImprovementsBuilt = true; }
            else { showCityImprovementsBuilt = false; }

            //Zoom to city not default action on/off
            bin = Convert.ToString(dataArray[23], 2).PadLeft(8, '0');
            if (bin[5] == '1') { zoomToCityNotDefaultAction = true; }
            else { zoomToCityNotDefaultAction = false; }

            //Warn when pollution occurs on/off
            if (bin[6] == '1') { warnWhenPollutionOccurs = true; }
            else { warnWhenPollutionOccurs = false; }

            //Warn when changing production will cost shileds on/off
            if (bin[7] == '1') { warnWhenChangingProductionWillCostShields = true; }
            else { warnWhenChangingProductionWillCostShields = false; }

            //Number of turns passed
            int intVal1 = dataArray[28];
            int intVal2 = dataArray[29];
            turnNumber = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);    //convert hex value 2 & 1 (in that order) together to int

            //Number of turns passed for game year calculation
            intVal1 = dataArray[30];
            intVal2 = dataArray[31];
            turnNumberForGameYear = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

            //Which unit is selected at start of game
            intVal1 = dataArray[34];
            intVal2 = dataArray[35];
            unitSelectedAtGameStart = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

            //Which human player is used
            whichHumanPlayerIsUsed = dataArray[39];

            //Players map which is used
            playersMapUsed = dataArray[40];

            //Players map which is used
            playersCivilizationNumberUsed = dataArray[41];

            //Map revealed
            if (dataArray[43] == 1) { mapRevealed = true; }
            else { mapRevealed = false; }

            //Difficulty level
            difficultyLevel = dataArray[44];

            //Barbarian activity
            barbarianActivity = dataArray[45];

            //Amount of pollution
            pollutionAmount = dataArray[50];

            //Global temp rising times occured
            globalTempRiseOccured = dataArray[51];

            //Number of turns of peace
            noOfTurnsOfPeace = dataArray[56];

            //Number of units
            intVal1 = dataArray[58];
            intVal2 = dataArray[59];
            numberOfUnits = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

            //Number of cities
            intVal1 = dataArray[60];
            intVal2 = dataArray[61];
            numberOfCities = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

            //=========================
            //WONDERS
            //=========================


            //=========================
            //TRIBES
            //=========================


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
            mapXdimension = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);
            mapXdimension = mapXdimension  / 2; //map 150x120 is really 75x120

            //Map Y dimension
            intVal1 = dataArray[ofset + 2];
            intVal2 = dataArray[ofset + 3];
            mapYdimension = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

            //Map area:
            intVal1 = dataArray[ofset + 4];
            intVal2 = dataArray[ofset + 5];
            mapArea = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

            ////Flat Earth flag (info already given before!!)
            //intVal1 = dataArray[ofset + 6];
            //intVal2 = dataArray[ofset + 7];
            //flatEarth = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

            //Map seed
            intVal1 = dataArray[ofset + 8];
            intVal2 = dataArray[ofset + 9];
            mapSeed = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

            //Locator map X dimension
            intVal1 = dataArray[ofset + 10];
            intVal2 = dataArray[ofset + 11];
            locatorMapXDimension = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

            //Locator map Y dimension
            int intValue11 = dataArray[ofset + 12];
            int intValue12 = dataArray[ofset + 13];
            locatorMapYDimension = int.Parse(string.Concat(intValue12.ToString("X"), intValue11.ToString("X")), System.Globalization.NumberStyles.HexNumber);

            //block 1 - terrain improvements
            int ofsetB1 = ofset + 14; //offset for block 2 values
            //...........
            //block 2 - terrain type
            int ofsetB2 = ofsetB1 + 7 * mapArea; //offset for block 2 values
            terrain = new int[mapArea];
            terrain_island = new int[mapArea];    //island no. in which the terrain is located
            for (int i = 0; i < mapArea; i++)
            {
                // Reading terrain tiles:
                // 0dec,0hex=desert, 1dec,1hex=plains, 2dec,2hex=grassland, 3dec,3hex=forest, 4dec,4hex=hills, 5dec,5hex=mountains, 6dec,6hex=tundra, 7dec,7hex=glacier, 8dec,8hex=swamp, 9dec,9hex=jungle, 10dec,Ahex=ocean
                // 74dec,4Ahex=ocean
                // 128dec,80hex=desert+river, 129dec,81hex=plains+river, 130dec,82hex=grassland+river, 131dec,83hex=forest+river, 132dec,84hex=hills+river, 133dec,85hex=mountains+river, 134dec,86hex=tundra+river, 135dec,87hex=glacier+river, 136dec,88hex=swamp+river, 137dec,89hex=jungle+river
                terrain[i] = dataArray[ofsetB2 + i * 6 + 0];

                int intValueB22 = dataArray[ofsetB2 + i * 6 + 1];   //?
                int intValueB23 = dataArray[ofsetB2 + i * 6 + 2];   //?
                terrain_island[i] = dataArray[ofsetB2 + i * 6 + 3];
                int intValueB25 = dataArray[ofsetB2 + i * 6 + 4];   //?
                int intValueB26 = dataArray[ofsetB2 + i * 6 + 5];   //?
            }
            //block 3 - locator map
            int ofsetB3 = ofsetB2 + 6 * mapArea; //offset for block 2 values
            //...............

            //=========================
            //UNIT INFO
            //=========================
            int ofsetU = ofsetB3 + 2 * locatorMapXDimension * locatorMapYDimension + 1024;

            unitXlocation = new int[numberOfUnits];
            unitYlocation = new int[numberOfUnits];
            unitGreyStarShield = new bool[numberOfUnits];
            unitVeteranStatus = new bool[numberOfUnits];
            unitType = new int[numberOfUnits];
            unitCiv = new int[numberOfUnits];
            unitHitpointsLost = new int[numberOfUnits];
            unitCaravanCommodity = new int[numberOfUnits];
            unitOrders = new int[numberOfUnits];
            unitHomeCity = new int[numberOfUnits];
            unitGoToX = new int[numberOfUnits];
            unitGoToY = new int[numberOfUnits];
            unitLinkOtherUnitsOnTop = new int[numberOfUnits];
            unitLinkOtherUnitsUnder = new int[numberOfUnits];
            for (int i = 0; i < numberOfUnits; i++)
            {
                //Unit X locatioin
                intVal1 = dataArray[ofsetU + 26 * i + 0];
                intVal2 = dataArray[ofsetU + 26 * i + 1];
                unitXlocation[i] = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                //Unit Y locatioin
                intVal1 = dataArray[ofsetU + 26 * i + 2];
                intVal2 = dataArray[ofsetU + 26 * i + 3];
                unitYlocation[i] = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                //Grey star to the shield
                bin = Convert.ToString(dataArray[ofsetU + 26 * i + 5], 2).PadLeft(8, '0');
                if (bin[0] == '1') { unitGreyStarShield[i] = true; }
                else { unitGreyStarShield[i] = false; }

                //Veteran status
                if (bin[2] == '1') { unitVeteranStatus[i] = true; }
                else { unitVeteranStatus[i] = false; }

                //Unit type
                unitType[i] = dataArray[ofsetU + 26 * i + 6];

                //Unit civ
                unitCiv[i] = dataArray[ofsetU + 26 * i + 7];

                //Unit hitpoints lost
                unitHitpointsLost[i] = dataArray[ofsetU + 26 * i + 11];

                //Unit caravan commodity
                unitCaravanCommodity[i] = dataArray[ofsetU + 26 * i + 13];

                //Unit orders
                unitOrders[i] = dataArray[ofsetU + 26 * i + 15];

                //Unit home city
                unitHomeCity[i] = dataArray[ofsetU + 26 * i + 16];

                //Unit go-to X
                intVal1 = dataArray[ofsetU + 26 * i + 18];
                intVal2 = dataArray[ofsetU + 26 * i + 19];
                unitGoToX[i] = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                //Unit go-to Y
                intVal1 = dataArray[ofsetU + 26 * i + 20];
                intVal2 = dataArray[ofsetU + 26 * i + 21];
                unitGoToY[i] = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                //Unit link to other units on top of it
                intVal1 = dataArray[ofsetU + 26 * i + 22];
                intVal2 = dataArray[ofsetU + 26 * i + 23];
                unitLinkOtherUnitsOnTop[i] = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                //Unit link to other units under it
                intVal1 = dataArray[ofsetU + 26 * i + 24];
                intVal2 = dataArray[ofsetU + 26 * i + 25];
                unitLinkOtherUnitsUnder[i] = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);
            }



            //=========================
            //CITIES
            //=========================


            //=========================
            //OTHER
            //=========================

        }
    }
}