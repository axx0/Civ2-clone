﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTciv2.Enums;
using RTciv2.Units;
using RTciv2.Terrains;
using System.IO;

namespace RTciv2
{
    public partial class Game
    {
        public static void ImportSAV(string SAVpath)
        {
            FileStream fs = new FileStream(SAVpath, FileMode.Open, FileAccess.Read);        //Enter filename
            int[] dataArray = new int[fs.Length];
            string bin;
            int intVal1, intVal2, intVal3, intVal4;

            //Read every byte
            for (int i = 0; i < fs.Length; i++) dataArray[i] = fs.ReadByte();

            #region Start of saved game file
            //=========================
            //START OF SAVED GAME FILE
            //=========================
            //Determine version        
            int version;
            if (dataArray[10] == 39)        version = 1;    //Conflicts (27 hex)
            else if (dataArray[10] == 40)   version = 2;    //FW (28 hex)
            else if (dataArray[10] == 44)   version = 3;    //MGE (2C hex)
            else if (dataArray[10] == 49)   version = 4;    //ToT1.0 (31 hex)
            else if (dataArray[10] == 50)   version = 5;    //ToT1.1 (32 hex)
            else                            version = 1;    //lower than Conflicts

            bin = Convert.ToString(dataArray[12], 2).PadLeft(8, '0');   //you have to pad zeros to the left because ToString doesn't write first zeros
            Options.Bloodlust = (bin[0] == '1') ? true : false;         //Bloodlust on/off            
            Options.SimplifiedCombat = (bin[3] == '1') ? true : false;  //Simplified combat on/off
                        
            bin = Convert.ToString(dataArray[13], 2).PadLeft(8, '0');
            Options.FlatEarth = (bin[0] == '1') ? true : false;                 //Flat/round earth            
            Options.DontRestartIfEliminated = (bin[7] == '1') ? true : false;   //Don't restart if eliminated
                        
            bin = Convert.ToString(dataArray[14], 2).PadLeft(8, '0');
            Options.MoveUnitsWithoutMouse = (bin[0] == '1') ? true : false;     //Move units without mouse            
            Options.EnterClosestCityScreen = (bin[1] == '1') ? true : false;    //Enter closes city screen            
            Options.Grid = (bin[2] == '1') ? true : false;                      //Grid on/off            
            Options.SoundEffects = (bin[3] == '1') ? true : false;              //Sound effects on/off            
            Options.Music = (bin[4] == '1') ? true : false;                     //Music on/off
            
            bin = Convert.ToString(dataArray[15], 2).PadLeft(8, '0');
            Options.CheatMenu = (bin[0] == '1') ? true : false;             //Cheat menu on/off            
            Options.AlwaysWaitAtEndOfTurn = (bin[1] == '1') ? true : false; //Always wait at end of turn on/off           
            Options.AutosaveEachTurn = (bin[2] == '1') ? true : false;      //Autosave each turn on/off            
            Options.ShowEnemyMoves = (bin[3] == '1') ? true : false;        //Show enemy moves on/off            
            Options.NoPauseAfterEnemyMoves = (bin[4] == '1') ? true : false;//No pause after enemy moves on/off            
            Options.FastPieceSlide = (bin[5] == '1') ? true : false;        //Fast piece slide on/off            
            Options.InstantAdvice = (bin[6] == '1') ? true : false;         //Instant advice on/off            
            Options.TutorialHelp = (bin[7] == '1') ? true : false;          //Tutorial help on/off
            
            bin = Convert.ToString(dataArray[16], 2).PadLeft(8, '0');
            Options.AnimatedHeralds = (bin[2] == '1') ? true : false;           //Animated heralds on/off            
            Options.HighCouncil = (bin[3] == '1') ? true : false;               //High council on/off            
            Options.CivilopediaForAdvances = (bin[4] == '1') ? true : false;    //Civilopedia for advances on/off            
            Options.ThroneRoomGraphics = (bin[5] == '1') ? true : false;        //Throne room graphics on/off            
            Options.DiplomacyScreenGraphics = (bin[6] == '1') ? true : false;   //Diplomacy screen graphics on/off            
            Options.WonderMovies = (bin[7] == '1') ? true : false;              //Wonder movies on/off
            
            bin = Convert.ToString(dataArray[20], 2).PadLeft(8, '0');
            Options.CheatPenaltyWarning = (bin[3] == '1') ? true : false;   //Cheat penalty/warning on/off
                        
            bin = Convert.ToString(dataArray[22], 2).PadLeft(8, '0');
            Options.AnnounceWeLoveKingDay = (bin[0] == '1') ? true : false;         //Announce we love king day on/off            
            Options.WarnWhenFoodDangerouslyLow = (bin[1] == '1') ? true : false;    //Warn when food dangerously low on/off            
            Options.AnnounceCitiesInDisorder = (bin[2] == '1') ? true : false;      //Announce cities in disorder on/off            
            Options.AnnounceOrderRestored = (bin[3] == '1') ? true : false;         //Announce order restored in cities on/off            
            Options.ShowNonCombatUnitsBuilt = (bin[4] == '1') ? true : false;       //Show non combat units build on/off           
            Options.ShowInvalidBuildInstructions = (bin[5] == '1') ? true : false;  //Show invalid build instructions on/off            
            Options.WarnWhenCityGrowthHalted = (bin[6] == '1') ? true : false;      //Warn when city growth halted on/off            
            Options.ShowCityImprovementsBuilt = (bin[7] == '1') ? true : false;     //Show city improvements built on/off
            
            bin = Convert.ToString(dataArray[23], 2).PadLeft(8, '0');
            Options.ZoomToCityNotDefaultAction = (bin[5] == '1') ? true : false;                //Zoom to city not default action on/off            
            Options.WarnWhenPollutionOccurs = (bin[6] == '1') ? true : false;                   //Warn when pollution occurs on/off           
            Options.WarnWhenChangingProductionWillCostShields = (bin[7] == '1') ? true : false; //Warn when changing production will cost shileds on/off

            //Number of turns passed
            intVal1 = dataArray[28];
            intVal2 = dataArray[29];
            Data.TurnNumber = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);    //convert hex value 2 & 1 (in that order) together to int

            //Number of turns passed for game year calculation
            intVal1 = dataArray[30];
            intVal2 = dataArray[31];
            Data.TurnNumberForGameYear = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

            //Which unit is selected at start of game (return -1 if no unit is selected (FFFFhex=65535dec))
            intVal1 = dataArray[34];
            intVal2 = dataArray[35];
            int _selectedIndex = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);
            Data.SelectedUnitIndex = (_selectedIndex == 65535) ? -1 : _selectedIndex;
                        
            Data.HumanPlayer = dataArray[39];//Which human player is used
            
            Data.PlayersMapUsed = dataArray[40];//Players map which is used
            
            Data.PlayersCivilizationNumberUsed = dataArray[41];//Players civ number used
            
            Data.MapRevealed = (dataArray[43] == 1) ? true : false;//Map revealed
            
            Data.DifficultyLevel = dataArray[44];//Difficulty level
            
            Data.BarbarianActivity = dataArray[45];//Barbarian activity

            //Civs in play
            Data.CivsInPlay = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
            string conv = Convert.ToString(dataArray[46], 2).PadLeft(8, '0');
            for (int i = 0; i < 8; i++)
                if (conv[i] == '1')
                    Data.CivsInPlay[i] = 1;
            
            string humanPlayerPlayed = Convert.ToString(dataArray[47], 2).PadLeft(8, '0');//Civs with human player playing (multiplayer)
                        
            Data.PollutionAmount = dataArray[50];//Amount of pollution
                        
            Data.GlobalTempRiseOccured = dataArray[51];//Global temp rising times occured
                       
            Data.NoOfTurnsOfPeace = dataArray[56]; //Number of turns of peace

            //Number of units
            intVal1 = dataArray[58];
            intVal2 = dataArray[59];
            Data.NumberOfUnits = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

            //Number of cities
            intVal1 = dataArray[60];
            intVal2 = dataArray[61];
            Data.NumberOfCities = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

            #endregion
            #region Wonders
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
            #endregion
            #region Civs
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
                for (int j = 0; j < 23; j++) asciich[j] = Convert.ToChar(dataArray[584 + 2 + 242 * i + j]);
                civLeaderName[i + 1] = new string(asciich);
                civLeaderName[i + 1] = civLeaderName[i + 1].Replace("\0", string.Empty);  //remove null characters

                //Tribe name (if empty, get the name from RULES.TXT)
                for (int j = 0; j < 23; j++) asciich[j] = Convert.ToChar(dataArray[584 + 2 + 23 + 242 * i + j]);
                civTribeName[i + 1] = new string(asciich);
                civTribeName[i + 1] = civTribeName[i + 1].Replace("\0", string.Empty);

                //Adjective (if empty, get the name from RULES.TXT)
                for (int j = 0; j < 23; j++) asciich[j] = Convert.ToChar(dataArray[584 + 2 + 23 + 23 + 242 * i + j]);
                civAdjective[i + 1] = new string(asciich);
                civAdjective[i + 1] = civAdjective[i + 1].Replace("\0", string.Empty);

                //Leader titles (Anarchy, Despotism, ...)
                // .... TO-DO ....

            }
            //Manually add data for barbarians
            civCityStyle[0] = 0;
            civLeaderName[0] = "NULL";
            civTribeName[0] = "Barbarians";
            civAdjective[0] = "Barbarian";
            #endregion
            #region Tech & money
            //=========================
            //TECH & MONEY
            //=========================
            int[] rulerGender = new int[8];
            int[] civMoney = new int[8];
            int[] tribeNumber = new int[8];
            int[] civResearchProgress = new int[8];
            int[] civResearchingTech = new int[8];
            int[] civSciRate = new int[8];
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

                //Science rate (%/10)
                civSciRate[i] = dataArray[2278 + 1428 * i + 19]; //20th byte in tribe block

                //Tax rate (%/10)
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
                    civTechs[no] = (civTechs_[no] == '1') ? 1 : 0;

                Civilization civ = CreateCiv(i, Data.HumanPlayer, civCityStyle[i], civLeaderName[i], civTribeName[i], civAdjective[i], rulerGender[i], civMoney[i], tribeNumber[i], 
                    civResearchProgress[i], civResearchingTech[i], civSciRate[i], civTaxRate[i], civGovernment[i], civReputation[i], civTechs);
            }
            #endregion
            #region Map
            //=========================
            //MAP
            //=========================
            //Map header ofset
            int ofset;
            if (version > 1) ofset = 13702;  //FW and later (offset=3586hex)
            else ofset = 13432; //Conflicts (offset=3478hex)

            //Map X dimension
            intVal1 = dataArray[ofset + 0];
            intVal2 = dataArray[ofset + 1];
            Data.MapXdim = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber) / 2; //map 150x120 is really 75x120

            //Map Y dimension
            intVal1 = dataArray[ofset + 2];
            intVal2 = dataArray[ofset + 3];
            Data.MapYdim = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

            //Map area:
            intVal1 = dataArray[ofset + 4];
            intVal2 = dataArray[ofset + 5];
            Data.MapArea = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

            ////Flat Earth flag (info already given before!!)
            //intVal1 = dataArray[ofset + 6];
            //intVal2 = dataArray[ofset + 7];
            //flatEarth = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

            //Map seed
            intVal1 = dataArray[ofset + 8];
            intVal2 = dataArray[ofset + 9];
            Data.MapSeed = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

            //Locator map X dimension
            intVal1 = dataArray[ofset + 10];
            intVal2 = dataArray[ofset + 11];
            Data.MapLocatorXdim = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);  //TODO: what does this do?

            //Locator map Y dimension
            int intValue11 = dataArray[ofset + 12];
            int intValue12 = dataArray[ofset + 13];
            Data.MapLocatorYdim = int.Parse(string.Concat(intValue12.ToString("X"), intValue11.ToString("X")), System.Globalization.NumberStyles.HexNumber);

            //Initialize Terrain array now that you know its size
            Map = new ITerrain[Data.MapXdim, Data.MapYdim];   //TODO: where to put this?

            //block 1 - terrain improvements (for individual civs)
            int ofsetB1 = ofset + 14; //offset for block 2 values
            //...........
            //block 2 - terrain type
            int ofsetB2 = ofsetB1 + 7 * Data.MapArea; //offset for block 2 values
            for (int i = 0; i < Data.MapArea; i++)
            {
                int x = i % Data.MapXdim;
                int y = i / Data.MapXdim;

                // Terrain type
                TerrainType type = TerrainType.Desert;  //only initial
                bool river = false;
                int terrain_type = dataArray[ofsetB2 + i * 6 + 0];
                if (terrain_type == 0) { type = TerrainType.Desert; river = false; }   //0dec=0hex
                if (terrain_type == 128) { type = TerrainType.Desert; river = true; }   //128dec=80hex
                if (terrain_type == 1) { type = TerrainType.Plains; river = false; }   //1dec=1hex
                if (terrain_type == 129) { type = TerrainType.Plains; river = true; }   //129dec=81hex
                if (terrain_type == 2) { type = TerrainType.Grassland; river = false; }   //2dec=2hex
                if (terrain_type == 130) { type = TerrainType.Grassland; river = true; }   //130dec=82hex
                if (terrain_type == 3) { type = TerrainType.Forest; river = false; }   //3dec=3hex
                if (terrain_type == 131) { type = TerrainType.Forest; river = true; }   //131dec=83hex
                if (terrain_type == 4) { type = TerrainType.Hills; river = false; }   //4dec=4hex
                if (terrain_type == 132) { type = TerrainType.Hills; river = true; }   //132dec=84hex
                if (terrain_type == 5) { type = TerrainType.Mountains; river = false; }   //5dec=5hex
                if (terrain_type == 133) { type = TerrainType.Mountains; river = true; }   //133dec=85hex
                if (terrain_type == 6) { type = TerrainType.Tundra; river = false; }   //6dec=6hex
                if (terrain_type == 134) { type = TerrainType.Tundra; river = true; }   //134dec=86hex
                if (terrain_type == 7) { type = TerrainType.Glacier; river = false; }   //7dec=7hex
                if (terrain_type == 135) { type = TerrainType.Glacier; river = true; }   //135dec=87hex
                if (terrain_type == 8) { type = TerrainType.Swamp; river = false; }   //8dec=8hex
                if (terrain_type == 136) { type = TerrainType.Swamp; river = true; }   //136dec=88hex
                if (terrain_type == 9) { type = TerrainType.Jungle; river = false; }   //9dec=9hex
                if (terrain_type == 137) { type = TerrainType.Jungle; river = true; }   //137dec=89hex
                if (terrain_type == 10) { type = TerrainType.Ocean; river = false; }   //10dec=Ahex
                if (terrain_type == 74) { type = TerrainType.Ocean; river = false; }   //74dec=4Ahex
                //determine if resources are present
                bool resource = false;
                //!!! NOT WORKING PROPERLY !!!
                //bin = Convert.ToString(dataArray[ofsetB2 + i * 6 + 0], 2).PadLeft(8, '0');
                //if (bin[1] == '1') { resource = true; }

                // Tile improvements (for all civs! In block 1 it's for indivudual civs)
                int tile_improv = dataArray[ofsetB2 + i * 6 + 1];
                bool unit_present = false, city_present = false, irrigation = false, mining = false, road = false, railroad = false, fortress = false, pollution = false, farmland = false, airbase = false;
                bin = Convert.ToString(tile_improv, 2).PadLeft(8, '0');
                if (bin[7] == '1')                  unit_present = true;
                if (bin[6] == '1')                  city_present = true;
                if (bin[5] == '1')                  irrigation = true;
                if (bin[4] == '1')                  mining = true;
                if (bin[3] == '1')                  road = true;
                if (bin[2] == '1' && bin[3] == '1') railroad = true;
                if (bin[1] == '1')                  fortress = true;
                if (bin[0] == '1')                  pollution = true;
                if (bin[4] == '1' && bin[5] == '1') farmland = true;
                if (bin[1] == '1' && bin[6] == '1') airbase = true;
                
                int intValueB23 = dataArray[ofsetB2 + i * 6 + 2];       //City radius (TO-DO)
                
                int terrain_island = dataArray[ofsetB2 + i * 6 + 3];    //Island counter
                
                int intValueB25 = dataArray[ofsetB2 + i * 6 + 4];       //Visibility (TO-DO)

                int intValueB26 = dataArray[ofsetB2 + i * 6 + 5];       //?

                //string hexValue = intValueB26.ToString("X");

                //SAV file doesn't tell where special resources are, so you have to determine this yourself
                int specialtype = ReturnSpecial(x, y, type, Data.MapXdim, Data.MapYdim);

                CreateTerrain(x, y, type, specialtype, resource, river, terrain_island, unit_present, city_present, irrigation, mining, road, railroad, fortress, pollution, farmland, airbase, bin);

            }
            //block 3 - locator map
            int ofsetB3 = ofsetB2 + 6 * Data.MapArea; //offset for block 2 values
                                                      //...............
            #endregion
            #region Units
            //=========================
            //UNIT INFO
            //=========================
            int ofsetU = ofsetB3 + 2 * Data.MapLocatorXdim * Data.MapLocatorYdim + 1024;

            //determine byte length of units
            int multipl;
            if (version <= 2)       multipl = 26;   //FW or CiC
            else if (version == 3)  multipl = 32;   //MGE
            else                    multipl = 40;   //ToT

            for (int i = 0; i < Data.NumberOfUnits; i++)
            {
                //Unit X location
                intVal1 = dataArray[ofsetU + multipl * i + 0];
                intVal2 = dataArray[ofsetU + multipl * i + 1];
                int unitXlocation = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                //Unit is inactive (dead) if the value of X-Y is negative (1st bit = 1)
                bin = Convert.ToString(intVal2, 2).PadLeft(8, '0');
                bool unit_dead = (bin[0] == '1') ? true : false;

                //Unit Y location
                intVal1 = dataArray[ofsetU + multipl * i + 2];
                intVal2 = dataArray[ofsetU + multipl * i + 3];
                int unitYlocation = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);
                                
                bin = Convert.ToString(dataArray[ofsetU + multipl * i + 4], 2).PadLeft(8, '0');
                bool unitFirstMove = (bin[1] == '1') ? true : false;            //If this is the unit's first move
                                
                bin = Convert.ToString(dataArray[ofsetU + multipl * i + 5], 2).PadLeft(8, '0');
                bool unitGreyStarShield = (bin[0] == '1') ? true : false;       //Grey star to the shield                
                bool unitVeteranStatus = (bin[2] == '1') ? true : false;        //Veteran status                
                int unitType = dataArray[ofsetU + multipl * i + 6];             //Unit type
                int unitCiv = dataArray[ofsetU + multipl * i + 7];              //Unit civ, 00 = barbarians                
                int unitMovePointsLost = dataArray[ofsetU + multipl * i + 8];   //Unit move points expended                
                int unitHitpointsLost = dataArray[ofsetU + multipl * i + 10];   //Unit hitpoints lost
                int unitLastMove = dataArray[ofsetU + multipl * i + 11];        //Unit previous move (00=up-right, 01=right, ..., 07=up, FF=no movement)                
                int unitCaravanCommodity = dataArray[ofsetU + multipl * i + 13];//Unit caravan commodity                
                int unitOrders = dataArray[ofsetU + multipl * i + 15];          //Unit orders                
                int unitHomeCity = dataArray[ofsetU + multipl * i + 16];        //Unit home city

                //Unit go-to X
                intVal1 = dataArray[ofsetU + multipl * i + 18];
                intVal2 = dataArray[ofsetU + multipl * i + 19];
                int unitGoToX = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                //Unit go-to Y
                intVal1 = dataArray[ofsetU + multipl * i + 20];
                intVal2 = dataArray[ofsetU + multipl * i + 21];
                int unitGoToY = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

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
            #endregion
            #region Cities
            //=========================
            //CITIES
            //=========================
            int ofsetC = ofsetU + multipl * Data.NumberOfUnits;

            if (version <= 2)       multipl = 84;   //FW or CiC
            else if (version == 3)  multipl = 88;   //MGE
            else                    multipl = 92;   //ToT

            char[] asciichar = new char[15];
            for (int i = 0; i < Data.NumberOfCities; i++)
            {
                //City X location
                intVal1 = dataArray[ofsetC + multipl * i + 0];
                intVal2 = dataArray[ofsetC + multipl * i + 1];
                int cityXlocation = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                //City Y location
                intVal1 = dataArray[ofsetC + multipl * i + 2];
                intVal2 = dataArray[ofsetC + multipl * i + 3];
                int cityYlocation = int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                bin = Convert.ToString(dataArray[ofsetC + multipl * i + 4], 2).PadLeft(8, '0');
                bool cityCanBuildCoastal = (bin[0] == '1') ? true : false;          //Can build coastal improvements
                bool cityAutobuildMilitaryRule = (bin[3] == '1') ? true : false;    //Auto build under military rule
                bool cityStolenTech = (bin[4] == '1') ? true : false;               //Stolen tech
                bool cityImprovementSold = (bin[5] == '1') ? true : false;          //Improvement sold
                bool cityWeLoveKingDay = (bin[6] == '1') ? true : false;            //We love king day
                bool cityCivilDisorder = (bin[7] == '1') ? true : false;            //Civil disorder

                bin = Convert.ToString(dataArray[ofsetC + multipl * i + 6], 2).PadLeft(8, '0');
                bool cityCanBuildShips = (bin[2] == '1') ? true : false;    //Can build ships

                bin = Convert.ToString(dataArray[ofsetC + multipl * i + 7], 2).PadLeft(8, '0');
                bool cityObjectivex3 = (bin[3] == '1') ? true : false;  //Objective x3

                bin = Convert.ToString(dataArray[ofsetC + multipl * i + 7], 2).PadLeft(8, '0');
                bool cityObjectivex1 = (bin[5] == '1') ? true : false;  //Objective x1
                                
                int cityOwner = dataArray[ofsetC + multipl * i + 8];    //Owner
                
                int citySize = dataArray[ofsetC + multipl * i + 9];     //Size
                                
                int cityWhoBuiltIt = dataArray[ofsetC + multipl * i + 10];  //Who built it

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
                for (int j = 0; j < 15; j++) asciichar[j] = Convert.ToChar(dataArray[ofsetC + multipl * i + j + 32]);
                string cityName = new string(asciichar);
                cityName = cityName.Replace("\0", string.Empty);

                //Distribution of workers on map in city view
                string cityWorkDistr1 = Convert.ToString(dataArray[ofsetC + multipl * i + 48], 2).PadLeft(8, '0');  //inner circle (starting from N, going in counter-clokwise direction)                
                string cityWorkDistr2 = Convert.ToString(dataArray[ofsetC + multipl * i + 49], 2).PadLeft(8, '0');  //on 8 of the outer circle    
                string cityWorkDistr3 = Convert.ToString(dataArray[ofsetC + multipl * i + 50], 2).PadLeft(5, '0');  //on 4 of the outer circle
                string _cityDistributionWorkers = string.Format("{0}{1}{2}", cityWorkDistr3, cityWorkDistr2, cityWorkDistr1);
                int[] cityDistributionWorkers = new int[21];
                for (int distNo = 0; distNo < 21; distNo++)
                    cityDistributionWorkers[distNo] = (_cityDistributionWorkers[distNo] == '1') ? 1 : 0;

                int cityNoOfSpecialistsx4 = dataArray[ofsetC + multipl * i + 51];   //Number of specialists x4

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
                string cityImprovements_ = string.Format("{0}{1}{2}{3}{4}", cityImprovements1, cityImprovements2, cityImprovements3, cityImprovements4, cityImprovements5);
                //convert string array to bool array
                bool[] cityImprovements = new bool[34];
                for (int impNo = 0; impNo < 34; impNo++)
                    cityImprovements[impNo] = (cityImprovements_[impNo] == '1') ? true : false;

                //Item in production
                //0(dec)/0(hex) ... 61(dec)/3D(hex) are units, improvements are inversed (FF(hex)=1st, FE(hex)=2nd, ...)
                //convert this notation of improvements, so that 62(dec) is 1st improvement, 63(dec) is 2nd, ...
                int cityItemInProduction = dataArray[ofsetC + multipl * i + 57];
                if (cityItemInProduction > 70)  //if it is improvement
                    cityItemInProduction = 255 - cityItemInProduction + 62; //62 because 0...61 are units
                                
                int cityActiveTradeRoutes = dataArray[ofsetC + multipl * i + 58];   //No of active trade routes

                //1st, 2nd, 3rd trade commodities supplied
                int[] cityCommoditySupplied = new int[] { dataArray[ofsetC + multipl * i + 59], dataArray[ofsetC + multipl * i + 60], dataArray[ofsetC + multipl * i + 61] };

                //1st, 2nd, 3rd trade commodities demanded
                int[] cityCommodityDemanded = new int[] { dataArray[ofsetC + multipl * i + 62], dataArray[ofsetC + multipl * i + 63], dataArray[ofsetC + multipl * i + 64] };

                //1st, 2nd, 3rd trade commodities in route
                int[] cityCommodityInRoute = new int[] { dataArray[ofsetC + multipl * i + 65], dataArray[ofsetC + multipl * i + 66], dataArray[ofsetC + multipl * i + 67] };

                //1st, 2nd, 3rd trade route partner city number
                int[] cityTradeRoutePartnerCity = new int[] { dataArray[ofsetC + multipl * i + 68], dataArray[ofsetC + multipl * i + 69], dataArray[ofsetC + multipl * i + 70] };

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
                                
                int cityFoodProduction = dataArray[ofsetC + multipl * i + 80];  //Total food production
                                
                int cityShieldProduction = dataArray[ofsetC + multipl * i + 81];    //Total shield production
                                
                int cityHappyCitizens = dataArray[ofsetC + multipl * i + 82];   //No of happy citizens
                                
                int cityUnhappyCitizens = dataArray[ofsetC + multipl * i + 83]; //No of unhappy citizens

                //Sequence number of the city
                //...

                //Check if wonder is in city (28 possible wonders)
                bool[] cityWonders = new bool[28];
                for (int wndr = 0; wndr < 28; wndr++)
                    cityWonders[wndr] = (wonderCity[wndr] == i) ? true : false;

                CreateCity(cityXlocation, cityYlocation, cityCanBuildCoastal, cityAutobuildMilitaryRule, cityStolenTech, cityImprovementSold, cityWeLoveKingDay, cityCivilDisorder, 
                           cityCanBuildShips, cityObjectivex3, cityObjectivex1, cityOwner, citySize, cityWhoBuiltIt, cityFoodInStorage, cityShieldsProgress, cityNetTrade, cityName,
                           cityDistributionWorkers, cityNoOfSpecialistsx4, cityImprovements, cityItemInProduction, cityActiveTradeRoutes, cityCommoditySupplied, cityCommodityDemanded,
                           cityCommodityInRoute, cityTradeRoutePartnerCity, cityScience, cityTax, cityNoOfTradeIcons, cityFoodProduction, cityShieldProduction, cityHappyCitizens, 
                           cityUnhappyCitizens, cityWonders);
            }
            #endregion
            #region Other
            //=========================
            //OTHER
            //=========================
            int ofsetO = ofsetC + multipl * Data.NumberOfCities;

            //active cursor XY position
            intVal1 = dataArray[ofsetO + 63];
            intVal2 = dataArray[ofsetO + 64];
            intVal3 = dataArray[ofsetO + 65];
            intVal4 = dataArray[ofsetO + 66];
            Data.ActiveXY = new int[] { int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber), int.Parse(string.Concat(intVal4.ToString("X"), intVal3.ToString("X")), System.Globalization.NumberStyles.HexNumber) };

            //clicked tile X position
            intVal1 = dataArray[ofsetO + 1425];
            intVal2 = dataArray[ofsetO + 1426];
            intVal3 = dataArray[ofsetO + 1427];
            intVal4 = dataArray[ofsetO + 1428];
            Data.ClickedXY = new int[] { int.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber), int.Parse(string.Concat(intVal4.ToString("X"), intVal3.ToString("X")), System.Globalization.NumberStyles.HexNumber) };

            Console.WriteLine($"Pos: cursor=({Data.ActiveXY[0]},{Data.ActiveXY[1]}), view=({Data.ClickedXY[0]},{Data.ClickedXY[1]})");
            #endregion

            fs.Dispose();
        }
                
        public static string Reverse(string s)   //Reverse a string
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
                    special = ((col + 4 - (row % 8) / 2) % 4 == 0 || (col + 4 - (row % 8) / 2) % 4 == 1) ? 1 : 0;
                else    //even lines
                    special = ((col + 4 - (row % 8) / 2) % 4 == 2 || (col + 4 - (row % 8) / 2) % 4 == 3) ? 1 : 0;

                //if (Game.Map[col, row].Special == 1) { graphics.DrawImage(Images.Shield, 0, 0); }
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
                        if (novi_i < mapXdim && col == novi_i) { special = 2; break; }   //tocke (3,1), (11,1), (19,1), ...
                        novi_i += 3;
                        if (novi_i < mapXdim && col == novi_i) { special = 1; break; }   //tocke (8,1), (16,1), (24,1), ...
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
