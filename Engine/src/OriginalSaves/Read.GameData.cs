using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Civ2engine.Enums;

namespace Civ2engine
{
    // Read game data from SAV and RULES.txt
    public class Read
    {
        // READ SAV GAME
        public static GameData ReadSAVFile(string savPath, string savName)
        {
            var data = new GameData();

            // Read every byte from SAV
            byte[] bytes = File.ReadAllBytes(savPath + Path.DirectorySeparatorChar + savName);

            #region Start of saved game file
            //=========================
            //START OF SAVED GAME FILE
            //=========================
            // Determine game version
            if (bytes[10] == 39)        data.GameVersion = GameVersionType.CiC;      // Conflicts (27 hex)
            else if (bytes[10] == 40)   data.GameVersion = GameVersionType.FW;       // FW (28 hex)
            else if (bytes[10] == 44)   data.GameVersion = GameVersionType.MGE;      // MGE (2C hex)
            else if (bytes[10] == 49)   data.GameVersion = GameVersionType.ToT10;    // ToT1.0 (31 hex)
            else if (bytes[10] == 50)   data.GameVersion = GameVersionType.ToT11;    // ToT1.1 (32 hex)
            else                        data.GameVersion = GameVersionType.CiC;      // lower than Conflicts

            // Options
            // TODO: determine if randomizing villages/resources, randomizing player starting locations, select comp. opponents, accelerated sturtup options are selected from SAV file
            data.OptionsArray = new bool[35];
            data.OptionsArray[0] = GetBit(bytes[12], 4);     // Simplified combat on/off
            data.OptionsArray[1] = GetBit(bytes[12], 7);     // Bloodlust on/off            
            data.OptionsArray[2] = GetBit(bytes[13], 0);     // Don't restart if eliminated
            data.OptionsArray[3] = GetBit(bytes[13], 7);     // Flat earth
            data.OptionsArray[4] = GetBit(bytes[14], 3);     // Music on/off
            data.OptionsArray[5] = GetBit(bytes[14], 4);     // Sound effects on/off
            data.OptionsArray[6] = GetBit(bytes[14], 5);     // Grid on/off
            data.OptionsArray[7] = GetBit(bytes[14], 6);     // Enter closes city screen     
            data.OptionsArray[8] = GetBit(bytes[14], 7);     // Move units without mouse
            data.OptionsArray[9] = GetBit(bytes[15], 0);     // Tutorial help on/off
            data.OptionsArray[10] = GetBit(bytes[15], 1);    // Instant advice on/off
            data.OptionsArray[11] = GetBit(bytes[15], 2);    // Fast piece slide on/off
            data.OptionsArray[12] = GetBit(bytes[15], 3);    // No pause after enemy moves on/off
            data.OptionsArray[13] = GetBit(bytes[15], 4);    // Show enemy moves on/off
            data.OptionsArray[14] = GetBit(bytes[15], 5);    // Autosave each turn on/off
            data.OptionsArray[15] = GetBit(bytes[15], 6);    // Always wait at end of turn on/off
            data.OptionsArray[16] = GetBit(bytes[15], 7);    // Cheat menu on/off
            data.OptionsArray[17] = GetBit(bytes[16], 0);    // Wonder movies on/off
            data.OptionsArray[18] = GetBit(bytes[16], 1);    // Diplomacy screen graphics on/off
            data.OptionsArray[19] = GetBit(bytes[16], 2);    // Throne room graphics on/off
            data.OptionsArray[20] = GetBit(bytes[16], 3);    // Civilopedia for advances on/off
            data.OptionsArray[21] = GetBit(bytes[16], 4);    // High council on/off
            data.OptionsArray[22] = GetBit(bytes[16], 5);    // Animated heralds on/off
            data.OptionsArray[23] = GetBit(bytes[20], 4);    // Cheat penalty/warning on/off
            data.OptionsArray[24] = GetBit(bytes[22], 0);    // Show city improvements built on/off
            data.OptionsArray[25] = GetBit(bytes[22], 1);    // Warn when city growth halted on/off
            data.OptionsArray[26] = GetBit(bytes[22], 2);    // Show invalid build instructions on/off
            data.OptionsArray[27] = GetBit(bytes[22], 3);    // Show non combat units build on/off
            data.OptionsArray[28] = GetBit(bytes[22], 4);    // Announce order restored in cities on/off
            data.OptionsArray[29] = GetBit(bytes[22], 5);    // Announce cities in disorder on/off
            data.OptionsArray[30] = GetBit(bytes[22], 6);    // Warn when food dangerously low on/off
            data.OptionsArray[31] = GetBit(bytes[22], 7);    // Announce we love king day on/off
            data.OptionsArray[32] = GetBit(bytes[23], 0);    // Warn when changing production will cost shileds on/off
            data.OptionsArray[33] = GetBit(bytes[23], 1);    // Warn when pollution occurs on/off
            data.OptionsArray[34] = GetBit(bytes[23], 2);    // Zoom to city not default action on/off
            
            data.TurnNumber = BitConverter.ToInt16(bytes, 28);  // Number of turns passed
            data.TurnNumberForGameYear = BitConverter.ToInt16(bytes, 30);   // Number of turns passed for game year calculation
            data.SelectedUnitIndex = BitConverter.ToInt16(bytes, 34);   // Which unit is selected at start of game (-1 if no unit)
            data.PlayersCivIndex = bytes[39];   // Human player TODO: how is this different from bytes[41]???
            data.WhichCivsMapShown = bytes[40]; // Players map currently shown
            data.PlayersCivilizationNumberUsed = bytes[41]; // Players civ number used
            data.MapRevealed = bytes[43] == 1;  // Map revealed
            data.DifficultyLevel = (DifficultyType)bytes[44];   // Difficulty level
            data.BarbarianActivity = (BarbarianActivityType)bytes[45];  // Barbarian activity

            // Civs in play
            data.CivsInPlay = new bool[8];
            for (int i = 0; i < 8; i++)
                data.CivsInPlay[i] = GetBit(bytes[46], i);

            // Civs with human player playing (multiplayer)
            //string humanPlayerPlayed = Convert.ToString(bytes[47], 2).PadLeft(8, '0');

            data.PollutionAmount = bytes[50];   // Amount of pollution
            data.GlobalTempRiseOccured = bytes[51]; // Global temp rising times occured
            data.NoOfTurnsOfPeace = bytes[56];  //Number of turns of peace
            data.NumberOfUnits = BitConverter.ToInt16(bytes, 58);   // Number of units
            data.NumberOfCities = BitConverter.ToInt16(bytes, 60);  // Number of cities

            #endregion
            #region Wonders
            //=========================
            //WONDERS
            //=========================
            data.WonderCity = new short[28];      // city with wonder
            data.WonderBuilt = new bool[28];      // has the wonder been built?
            data.WonderDestroyed = new bool[28];  // has the wonder been destroyed?
            for (int i = 0; i < 28; i++)
            {
                data.WonderCity[i] = BitConverter.ToInt16(bytes, 266 + 2 * i); // City number with the wonder

                // Determine if wonder is built/destroyed
                if (data.WonderCity[i] == -1)   // 0xFFFF
                {
                    data.WonderBuilt[i] = false;
                }
                else if (data.WonderCity[i] == -2)    // 0xFEFF
                {
                    data.WonderDestroyed[i] = true;
                }
                else
                {
                    data.WonderBuilt[i] = true;
                    data.WonderDestroyed[i] = false;
                }
            }
            #endregion
            #region Civ variables 1
            //=========================
            //Civ variables 1 (without barbarians)
            //=========================
            char[] asciich = new char[24];
            data.CivCityStyle = new byte[8];
            data.CivLeaderName = new string[8];
            data.CivTribeName = new string[8];
            data.CivAdjective = new string[8];
            // Manually add data for barbarians
            data.CivCityStyle[0] = 0;
            data.CivLeaderName[0] = "NULL";
            data.CivTribeName[0] = "Barbarians";
            data.CivAdjective[0] = "Barbarian";
            // Add data for other 7 civs
            for (int i = 0; i < 7; i++)
            {
                data.CivCityStyle[i + 1] = bytes[584 + 242 * i];    // City style

                // Leader names (if empty, get the name from RULES.TXT)
                for (int j = 0; j < 24; j++) 
                {
                    asciich[j] = Convert.ToChar(bytes[584 + 2 + 242 * i + j]);
                    if (asciich[j] == 0x0) break;
                }
                data.CivLeaderName[i + 1] = new string(asciich);
                data.CivLeaderName[i + 1] = data.CivLeaderName[i + 1].Replace("\0", string.Empty);  // remove null characters

                // Tribe name (if empty, get the name from RULES.TXT)
                asciich = new char[24];
                for (int j = 0; j < 24; j++)
                {
                    asciich[j] = Convert.ToChar(bytes[584 + 2 + 24 + 242 * i + j]);
                    if (asciich[j] == 0x0) break;
                }
                data.CivTribeName[i + 1] = new string(asciich);
                data.CivTribeName[i + 1] = data.CivTribeName[i + 1].Replace("\0", string.Empty);

                // Adjective (if empty, get the name from RULES.TXT)
                asciich = new char[24];
                for (int j = 0; j < 24; j++) 
                { 
                    asciich[j] = Convert.ToChar(bytes[584 + 2 + 24 + 24 + 242 * i + j]);
                    if (asciich[j] == 0x0) break;
                }
                data.CivAdjective[i + 1] = new string(asciich);
                data.CivAdjective[i + 1] = data.CivAdjective[i + 1].Replace("\0", string.Empty);

                //Leader titles (Anarchy, Despotism, ...)
                // .... TO-DO ....
            }
            #endregion
            #region Civ variables 2
            //=========================
            //Civ variables 2 (with barbarians)
            //=========================
            data.RulerGender = new byte[8];
            data.CivMoney = new short[8];
            data.CivNumber = new byte[8];
            data.CivResearchProgress = new short[8];
            data.CivResearchingAdvance = new byte[8];
            data.CivSciRate = new byte[8];
            data.CivTaxRate = new byte[8];
            data.CivGovernment = new byte[8];
            data.CivReputation = new byte[8];
            data.CivAdvances = new bool[8][];
            data.CivActiveUnitsPerUnitType = new byte[8][];
            data.CivCasualtiesPerUnitType = new byte[8][];
            // starting offset = 8E6(hex) = 2278(10), each block has 1427(10) bytes
            for (int civId = 0; civId < 8; civId++) // for each civ
            {
                data.RulerGender[civId] = bytes[2278 + 1428 * civId + 1]; // Gender (0=male, 2=female)
                data.CivMoney[civId] = BitConverter.ToInt16(bytes, 2278 + 1428 * civId + 2); // Money
                data.CivNumber[civId] = bytes[2278 + 1428 * civId + 6]; // Tribe number as per @Leaders table in RULES.TXT
                data.CivResearchProgress[civId] = BitConverter.ToInt16(bytes, 2278 + 1428 * civId + 8); // Research progress
                data.CivResearchingAdvance[civId] = bytes[2278 + 1428 * civId + 10]; // Advance currently being researched (FF(hex) = no goal)
                data.CivSciRate[civId] = bytes[2278 + 1428 * civId + 19]; // Science rate (%/10)
                data.CivTaxRate[civId] = bytes[2278 + 1428 * civId + 20]; // Tax rate (%/10)
                data.CivGovernment[civId] = bytes[2278 + 1428 * civId + 21]; // Government (0=anarchy, ...)
                data.CivReputation[civId] = bytes[2278 + 1428 * civId + 30]; // Reputation

                // No. of casualties per unit type
                data.CivActiveUnitsPerUnitType[civId] = new byte[62];
                data.CivCasualtiesPerUnitType[civId] = new byte[62];
                for (int type = 0; type < 62; type++)
                {
                    data.CivActiveUnitsPerUnitType[civId][type] = bytes[2278 + 1428 * civId + 216 + type];
                    data.CivCasualtiesPerUnitType[civId][type] = bytes[2278 + 1428 * civId + 278 + type];
                }

                // Treaties
                // ..... TO-DO .....

                // Attitudes
                // ..... TO-DO .....

                // Technologies
                data.CivAdvances[civId] = new bool[100];
                for (int block = 0; block < 13; block++)
                {
                    for (int bit = 0; bit < 8; bit++)
                    {
                        data.CivAdvances[civId][block * 8 + bit] = GetBit(bytes[2278 + 1428 * civId + 88 + block], bit);

                        if (block == 12 && bit == 3) 
                            break;
                    }
                }
            }
            #endregion
            #region Map
            //=========================
            //MAP
            //=========================
            // Map header offset
            // FW and later (offset=3586hex)
            // Conflicts (offset=3478hex)
            var mapDataOffset = bytes[10] > 39 ? 13702 : 13432;

            data.MapXdim = BitConverter.ToInt16(bytes, mapDataOffset + 0);  // Map X dimension
            data.MapYdim = BitConverter.ToInt16(bytes, mapDataOffset + 2);  // Map Y dimension
            data.MapArea = BitConverter.ToInt16(bytes, mapDataOffset + 4);  // Map area
            //flatEarth = BitConverter.ToInt16(bytes, mapDataOffset + 6);   // Flat Earth flag (info already given before!!)
            data.MapResourceSeed = BitConverter.ToInt16(bytes, mapDataOffset + 8);  // Map resource seed
            data.MapLocatorXdim = BitConverter.ToInt16(bytes, mapDataOffset + 10);  // Locator map X dimension
            data.MapLocatorYdim = BitConverter.ToInt16(bytes, mapDataOffset + 12);  // Locator map Y dimension

            // Initialize Terrain array now that you know its size
            //TerrainTile = new ITerrain[Data.MapXdim, Data.MapYdim];   //TODO: where to put this?

            // block 1 - terrain improvements (for individual civs)
            int ofsetB1 = mapDataOffset + 14; //offset for block 2 values
            //...........
            // block 2 - terrain type
            int ofsetB2 = ofsetB1 + 7 * data.MapArea; //offset for block 2 values
            data.MapTerrainType = new TerrainType[data.MapXdim / 2, data.MapYdim];
            data.MapVisibilityCivs = new bool[data.MapXdim / 2, data.MapYdim][];
            data.MapRiverPresent = new bool[data.MapXdim / 2, data.MapYdim];
            data.MapResourcePresent = new bool[data.MapXdim / 2, data.MapYdim];
            data.MapUnitPresent = new bool[data.MapXdim / 2, data.MapYdim];
            data.MapCityPresent = new bool[data.MapXdim / 2, data.MapYdim];
            data.MapIrrigationPresent = new bool[data.MapXdim / 2, data.MapYdim];
            data.MapMiningPresent = new bool[data.MapXdim / 2, data.MapYdim];
            data.MapRoadPresent = new bool[data.MapXdim / 2, data.MapYdim];
            data.MapRailroadPresent = new bool[data.MapXdim / 2, data.MapYdim];
            data.MapFortressPresent = new bool[data.MapXdim / 2, data.MapYdim];
            data.MapPollutionPresent = new bool[data.MapXdim / 2, data.MapYdim];
            data.MapFarmlandPresent = new bool[data.MapXdim / 2, data.MapYdim];
            data.MapAirbasePresent = new bool[data.MapXdim / 2, data.MapYdim];
            data.MapIslandNo = new byte[data.MapXdim / 2, data.MapYdim];
            data.MapSpecialType = new SpecialType[data.MapXdim / 2, data.MapYdim];
            for (int i = 0; i < data.MapArea; i++)
            {
                int x = i % (data.MapXdim / 2);
                int y = i / (data.MapXdim / 2);

                // Terrain type
                int terrB = ofsetB2 + i * 6 + 0;
                data.MapTerrainType[x,y] = (TerrainType)(bytes[terrB] & 0xF);
                data.MapRiverPresent[x, y] = GetBit(bytes[terrB], 7);  // river (1xxx xxxx)

                // Determine if resources are present
                data.MapResourcePresent[x, y] = false;
                //!!! NOT WORKING PROPERLY !!!
                //bin = Convert.ToString(dataArray[ofsetB2 + i * 6 + 0], 2).PadLeft(8, '0');
                //if (bin[1] == '1') { resource = true; }

                // Tile improvements (for all civs! In block 1 it's for indivudual civs)
                terrB = ofsetB2 + i * 6 + 1;
                data.MapUnitPresent[x, y] = GetBit(bytes[terrB], 0);
                data.MapCityPresent[x, y] = GetBit(bytes[terrB], 1);
                data.MapIrrigationPresent[x, y] = GetBit(bytes[terrB], 2);
                data.MapMiningPresent[x, y] = GetBit(bytes[terrB], 3);
                data.MapRoadPresent[x, y] = GetBit(bytes[terrB], 4);
                data.MapRailroadPresent[x, y] = GetBit(bytes[terrB], 4) && GetBit(bytes[terrB], 5);
                data.MapFortressPresent[x, y] = GetBit(bytes[terrB], 6);
                data.MapPollutionPresent[x, y] = GetBit(bytes[terrB], 7);
                data.MapFarmlandPresent[x, y] = GetBit(bytes[terrB], 2) && GetBit(bytes[terrB], 3);
                data.MapAirbasePresent[x, y] = GetBit(bytes[terrB], 3) && GetBit(bytes[terrB], 4);

                int intValueB23 = bytes[ofsetB2 + i * 6 + 2];       // TODO: city radius

                data.MapIslandNo[x, y] = bytes[ofsetB2 + i * 6 + 3];    // Island counter

                // Visibility of squares for all civs (0...red (barbarian), 1...white, 2...green, etc.)
                data.MapVisibilityCivs[x, y] = new bool[8];
                for (int civ = 0; civ < 8; civ++)
                    data.MapVisibilityCivs[x, y][civ] = GetBit(bytes[ofsetB2 + i * 6 + 4], civ);

                int intValueB26 = bytes[ofsetB2 + i * 6 + 5];       //?

                //string hexValue = intValueB26.ToString("X");

                // SAV file doesn't tell where special resources are, so you have to set this yourself

                //data.MapSpecialType[x, y] = ReturnSpecial(x, y, data.MapTerrainType[x, y], data.MapXdim, data.MapYdim);
            }
            // block 3 - locator map
            int ofsetB3 = ofsetB2 + 6 * data.MapArea; //offset for block 2 values
                                                      //...............
            #endregion
            #region Units
            //=========================
            //UNIT INFO
            //=========================
            int ofsetU = ofsetB3 + 2 * data.MapLocatorXdim * data.MapLocatorYdim + 1024;

            // Determine byte length of units
            int multipl;
            if (bytes[10] <= 40) multipl = 26;   // FW or CiC
            else if (bytes[10] == 44) multipl = 32;   // MGE
            else multipl = 40;   // ToT

            data.UnitXloc = new short[data.NumberOfUnits];
            data.UnitYloc = new short[data.NumberOfUnits];
            data.UnitDead = new bool[data.NumberOfUnits];
            data.UnitFirstMove = new bool[data.NumberOfUnits];
            data.UnitImmobile = new bool[data.NumberOfUnits];
            data.UnitGreyStarShield = new bool[data.NumberOfUnits];
            data.UnitVeteran = new bool[data.NumberOfUnits];
            data.UnitType = new UnitType[data.NumberOfUnits];
            data.UnitCiv = new byte[data.NumberOfUnits];
            data.UnitMovePointsLost = new byte[data.NumberOfUnits];
            data.UnitHitPointsLost = new byte[data.NumberOfUnits];
            data.UnitPrevXloc = new short[data.NumberOfUnits];
            data.UnitPrevYloc = new short[data.NumberOfUnits];
            data.UnitCaravanCommodity = new CommodityType[data.NumberOfUnits];
            data.UnitOrders = new OrderType[data.NumberOfUnits];
            data.UnitHomeCity = new byte[data.NumberOfUnits];
            data.UnitGotoX = new short[data.NumberOfUnits];
            data.UnitGotoY = new short[data.NumberOfUnits];
            data.UnitLinkOtherUnitsOnTop = new short[data.NumberOfUnits];
            data.UnitLinkOtherUnitsUnder = new short[data.NumberOfUnits];
            for (int i = 0; i < data.NumberOfUnits; i++)
            {
                data.UnitXloc[i] = BitConverter.ToInt16(bytes, ofsetU + multipl * i);       // Unit X location
                data.UnitYloc[i] = BitConverter.ToInt16(bytes, ofsetU + multipl * i + 2);   // Unit Y location
                data.UnitDead[i] = data.UnitXloc[i] < 0;                                    // Unit is inactive (dead) if the value of X-Y is negative
                data.UnitFirstMove[i] = GetBit(bytes[ofsetU + multipl * i + 4], 1);         // If this is the unit's first move
                data.UnitImmobile[i] = GetBit(bytes[ofsetU + multipl * i + 4], 6);          // Immobile
                data.UnitGreyStarShield[i] = GetBit(bytes[ofsetU + multipl * i + 5], 0);    // Grey star to the shield
                data.UnitVeteran[i] = GetBit(bytes[ofsetU + multipl * i + 5], 2);           // Veteran status
                data.UnitType[i] = (UnitType)bytes[ofsetU + multipl * i + 6];               // Unit type
                data.UnitCiv[i] = bytes[ofsetU + multipl * i + 7];                          // Unit civ, 00 = barbarians                
                data.UnitMovePointsLost[i] = bytes[ofsetU + multipl * i + 8];               // Unit move points expended                
                data.UnitHitPointsLost[i] = bytes[ofsetU + multipl * i + 10];               // Unit hitpoints lost
                switch (bytes[ofsetU + multipl * i + 11])                                   // Unit previous move (00=up-right, 01=right, ..., 07=up, FF=no movement)   
                {
                    case 0:
                        data.UnitPrevXloc[i] = (short)(data.UnitXloc[i] - 1);
                        data.UnitPrevYloc[i] = (short)(data.UnitYloc[i] + 1);
                        break;
                    case 1:
                        data.UnitPrevXloc[i] = (short)(data.UnitXloc[i] - 2);
                        data.UnitPrevYloc[i] = (short)(data.UnitYloc[i]);
                        break;
                    case 2:
                        data.UnitPrevXloc[i] = (short)(data.UnitXloc[i] - 1);
                        data.UnitPrevYloc[i] = (short)(data.UnitYloc[i] - 1);
                        break;
                    case 3:
                        data.UnitPrevXloc[i] = (short)(data.UnitXloc[i]);
                        data.UnitPrevYloc[i] = (short)(data.UnitYloc[i] - 2);
                        break;
                    case 4:
                        data.UnitPrevXloc[i] = (short)(data.UnitXloc[i] + 1);
                        data.UnitPrevYloc[i] = (short)(data.UnitYloc[i] - 1);
                        break;
                    case 5:
                        data.UnitPrevXloc[i] = (short)(data.UnitXloc[i] + 2);
                        data.UnitPrevYloc[i] = (short)(data.UnitYloc[i]);
                        break;
                    case 6:
                        data.UnitPrevXloc[i] = (short)(data.UnitXloc[i] + 1);
                        data.UnitPrevYloc[i] = (short)(data.UnitYloc[i] + 1);
                        break;
                    case 7:
                        data.UnitPrevXloc[i] = (short)(data.UnitXloc[i]);
                        data.UnitPrevYloc[i] = (short)(data.UnitYloc[i] + 2);
                        break;
                    case 255:   // No movement
                        data.UnitPrevXloc[i] = data.UnitXloc[i];
                        data.UnitPrevYloc[i] = data.UnitYloc[i];
                        break;
                }
                data.UnitCaravanCommodity[i] = (CommodityType)bytes[ofsetU + multipl * i + 13]; // Unit caravan commodity                
                data.UnitOrders[i] = (OrderType)bytes[ofsetU + multipl * i + 15];           // Unit orders
                if (bytes[ofsetU + multipl * i + 15] == 27) data.UnitOrders[i] = OrderType.NoOrders;    // TODO: (this is temp) find out what 0x1B means in unit orders
                data.UnitHomeCity[i] = bytes[ofsetU + multipl * i + 16];                    // Unit home city
                data.UnitGotoX[i] = BitConverter.ToInt16(bytes, ofsetU + multipl * i + 18); // Unit go-to X
                data.UnitGotoY[i] = BitConverter.ToInt16(bytes, ofsetU + multipl * i + 20); // Unit go-to Y
                data.UnitLinkOtherUnitsOnTop[i] = BitConverter.ToInt16(bytes, ofsetU + multipl * i + 22); // Unit link to other units on top of it
                data.UnitLinkOtherUnitsUnder[i] = BitConverter.ToInt16(bytes, ofsetU + multipl * i + 24); // Unit link to other units under it
            }
            #endregion
            #region Cities
            //=========================
            //CITIES
            //=========================
            int ofsetC = ofsetU + multipl * data.NumberOfUnits;

            if (bytes[10] <= 40) multipl = 84;   // FW or CiC
            else if (bytes[10] == 44) multipl = 88;   // MGE
            else multipl = 92;   // ToT

            char[] asciichar = new char[16];
            data.CityXloc = new short[data.NumberOfCities];
            data.CityYloc = new short[data.NumberOfCities];
            data.CityCanBuildCoastal = new bool[data.NumberOfCities];
            data.CityAutobuildMilitaryRule = new bool[data.NumberOfCities];
            data.CityStolenAdvance = new bool[data.NumberOfCities];
            data.CityImprovementSold = new bool[data.NumberOfCities];
            data.CityWeLoveKingDay = new bool[data.NumberOfCities];
            data.CityCivilDisorder = new bool[data.NumberOfCities];
            data.CityCanBuildShips = new bool[data.NumberOfCities];
            data.CityObjectivex3 = new bool[data.NumberOfCities];
            data.CityObjectivex1 = new bool[data.NumberOfCities];
            data.CityOwner = new byte[data.NumberOfCities];
            data.CitySize = new byte[data.NumberOfCities];
            data.CityWhoBuiltIt = new byte[data.NumberOfCities];
            data.CityFoodInStorage = new short[data.NumberOfCities];
            data.CityShieldsProgress = new short[data.NumberOfCities];
            data.CityNetTrade = new short[data.NumberOfCities];
            data.CityName = new string[data.NumberOfCities];
            data.CityDistributionWorkers = new bool[data.NumberOfCities][];
            data.CityNoOfSpecialistsx4 = new byte[data.NumberOfCities];
            data.CityImprovements = new bool[data.NumberOfCities][];
            data.CityItemInProduction = new byte[data.NumberOfCities];
            data.CityActiveTradeRoutes = new int[data.NumberOfCities];
            data.CityCommoditySupplied = new CommodityType[data.NumberOfCities][];
            data.CityCommodityDemanded = new CommodityType[data.NumberOfCities][];
            data.CityCommodityInRoute = new CommodityType[data.NumberOfCities][];
            data.CityTradeRoutePartnerCity = new int[data.NumberOfCities][];
            data.CityScience = new short[data.NumberOfCities];
            data.CityTax = new short[data.NumberOfCities];
            data.CityNoOfTradeIcons = new short[data.NumberOfCities];
            data.CityTotalFoodProduction = new byte[data.NumberOfCities];
            data.CityTotalShieldProduction = new byte[data.NumberOfCities];
            data.CityHappyCitizens = new byte[data.NumberOfCities];
            data.CityUnhappyCitizens = new byte[data.NumberOfCities];
            for (int i = 0; i < data.NumberOfCities; i++)
            {
                data.CityXloc[i] = BitConverter.ToInt16(bytes, ofsetC + multipl * i + 0);   // City X location
                data.CityYloc[i] = BitConverter.ToInt16(bytes, ofsetC + multipl * i + 2);   // City Y location
                data.CityCanBuildCoastal[i] = GetBit(bytes[ofsetC + multipl * i + 4], 0);    // Can build coastal improvements
                data.CityAutobuildMilitaryRule[i] = GetBit(bytes[ofsetC + multipl * i + 4], 3);    // Auto build under military rule
                data.CityStolenAdvance[i] = GetBit(bytes[ofsetC + multipl * i + 4], 4);         // Stolen advance
                data.CityImprovementSold[i] = GetBit(bytes[ofsetC + multipl * i + 4], 5);    // Improvement sold
                data.CityWeLoveKingDay[i] = GetBit(bytes[ofsetC + multipl * i + 4], 6);    // We love king day
                data.CityCivilDisorder[i] = GetBit(bytes[ofsetC + multipl * i + 4], 7);    // Civil disorder
                data.CityCanBuildShips[i] = GetBit(bytes[ofsetC + multipl * i + 6], 2);    // Can build ships
                data.CityObjectivex3[i] = GetBit(bytes[ofsetC + multipl * i + 7], 3);    // Objective x3
                data.CityObjectivex1[i] = GetBit(bytes[ofsetC + multipl * i + 7], 5);    // Objective x1
                data.CityOwner[i] = bytes[ofsetC + multipl * i + 8];        // Owner
                data.CitySize[i] = bytes[ofsetC + multipl * i + 9];         // Size
                data.CityWhoBuiltIt[i] = bytes[ofsetC + multipl * i + 10];  // Who built it

                // Production squares
                //???????????????????

                // Specialists
                //??????????????????
                
                data.CityFoodInStorage[i] = BitConverter.ToInt16(bytes, ofsetC + multipl * i + 26); // Food in storage
                data.CityShieldsProgress[i] = BitConverter.ToInt16(bytes, ofsetC + multipl * i + 28); // Shield progress
                data.CityNetTrade[i] = BitConverter.ToInt16(bytes, ofsetC + multipl * i + 30); // Net trade

                // Name
                for (int j = 0; j < 16; j++) asciichar[j] = Convert.ToChar(bytes[ofsetC + multipl * i + j + 32]);
                var cityName = new string(asciichar);
                data.CityName[i] = cityName[..cityName.IndexOf('\0')];

                // Distribution of workers on map in city view
                // 1st byte - inner circle (starting from N, going in counter-clokwise direction)
                // 2nd byte - 8 on outer circle
                // 3nd byte - 4 on outer circle + city square
                data.CityDistributionWorkers[i] = new bool[21];
                for (int bit = 0; bit < 8; bit++)
                {
                    data.CityDistributionWorkers[i][0 * 8 + bit] = GetBit(bytes[ofsetC + multipl * i + 48 + 0], 7 - bit);
                    data.CityDistributionWorkers[i][1 * 8 + bit] = GetBit(bytes[ofsetC + multipl * i + 48 + 1], 7 - bit);
                    if (bit > 2)
                    {
                        data.CityDistributionWorkers[i][2 * 8 + bit - 3] = GetBit(bytes[ofsetC + multipl * i + 48 + 2], 7 - bit);
                    }
                }

                data.CityNoOfSpecialistsx4[i] = bytes[ofsetC + multipl * i + 51];   // Number of specialists x4

                // Improvements
                data.CityImprovements[i] = new bool[34];
                int count = 0;
                for (int block = 0; block < 5; block++)
                {
                    for (int bit = 0; bit < 8; bit++)
                    {
                        if ((block == 0 && bit > 0) || (block == 4 && bit < 3) || (1 <= block && block <= 3))
                        {
                            data.CityImprovements[i][count] = GetBit(bytes[ofsetC + multipl * i + 52 + block], bit);
                            count++;
                        }
                    }
                }
                
                // Item in production
                // 0(dec)/0(hex) ... 61(dec)/3D(hex) are units, improvements are inversed (FF(hex)=1st, FE(hex)=2nd, ...)
                // convert this notation of improvements, so that 62(dec) is 1st improvement, 63(dec) is 2nd, ...
                data.CityItemInProduction[i] = bytes[ofsetC + multipl * i + 57];
                if (data.CityItemInProduction[i] > 70)  //if it is improvement
                    data.CityItemInProduction[i] = (byte)(255 - data.CityItemInProduction[i] + 62); // 62 because 0...61 are units

                data.CityActiveTradeRoutes[i] = bytes[ofsetC + multipl * i + 58];   // No of active trade routes

                // 1st, 2nd, 3rd trade commodities supplied
                data.CityCommoditySupplied[i] = new CommodityType[] { (CommodityType)bytes[ofsetC + multipl * i + 59], (CommodityType)bytes[ofsetC + multipl * i + 60], (CommodityType)bytes[ofsetC + multipl * i + 61] };

                // 1st, 2nd, 3rd trade commodities demanded
                data.CityCommodityDemanded[i] = new CommodityType[] { (CommodityType)bytes[ofsetC + multipl * i + 62], (CommodityType)bytes[ofsetC + multipl * i + 63], (CommodityType)bytes[ofsetC + multipl * i + 64] };

                // 1st, 2nd, 3rd trade commodities in route
                data.CityCommodityInRoute[i] = new CommodityType[] { (CommodityType)bytes[ofsetC + multipl * i + 65], (CommodityType)bytes[ofsetC + multipl * i + 66], (CommodityType)bytes[ofsetC + multipl * i + 67] };

                // 1st, 2nd, 3rd trade route partner city number
                data.CityTradeRoutePartnerCity[i] = new int[] { bytes[ofsetC + multipl * i + 68], bytes[ofsetC + multipl * i + 69], bytes[ofsetC + multipl * i + 70] };

                data.CityScience[i] = BitConverter.ToInt16(bytes, ofsetC + multipl * i + 74);   // Science
                data.CityTax[i] = BitConverter.ToInt16(bytes, ofsetC + multipl * i + 76);   // Tax
                data.CityNoOfTradeIcons[i] = BitConverter.ToInt16(bytes, ofsetC + multipl * i + 78);   // No of trade icons
                data.CityTotalFoodProduction[i] = bytes[ofsetC + multipl * i + 80];  // Total food production
                data.CityTotalShieldProduction[i] = bytes[ofsetC + multipl * i + 81];    // Total shield production
                data.CityHappyCitizens[i] = bytes[ofsetC + multipl * i + 82];   // No of happy citizens
                data.CityUnhappyCitizens[i] = bytes[ofsetC + multipl * i + 83]; // No of unhappy citizens

                // Sequence number of the city
                //...

                //// Check if wonder is in city (28 possible wonders)
                //bool[] cityWonders = new bool[28];
                //for (int wndr = 0; wndr < 28; wndr++)
                //    cityWonders[wndr] = (wonderCity[wndr] == i) ? true : false;

            }
            #endregion
            #region Other
            //=========================
            //OTHER
            //=========================
            int ofsetO = ofsetC + multipl * data.NumberOfCities;

            // Active cursor XY position
            data.ActiveCursorXY = new short[] { BitConverter.ToInt16(bytes, ofsetO + 63), BitConverter.ToInt16(bytes, ofsetO + 65) };

            // Clicked tile with your mouse XY position (does not count if you clicked on a city)
            data.ClickedXY = new int[] { BitConverter.ToInt16(bytes, ofsetO + 1425), BitConverter.ToInt16(bytes, ofsetO + 1427) };

            // Zoom (=-7(min)...+8(max), 0=std.)
            data.Zoom = BitConverter.ToInt16(bytes, ofsetO + 1429);
            #endregion
            #region Scenario
            //=========================
            //SCENARIO PARAMS
            //=========================
            int ofsetS = ofsetO + 1501;

            data.TotalWar = GetBit(bytes[ofsetS + 0], 0);
            data.ObjectiveVictory = GetBit(bytes[ofsetS + 0], 1);
            data.CountWondersAsObjectives = GetBit(bytes[ofsetS + 0], 2);
            data.ForbidGovernmentSwitching = GetBit(bytes[ofsetS + 0], 4);
            data.ForbidTechFromConquests = GetBit(bytes[ofsetS + 0], 5);
            data.ElliminatePollution = GetBit(bytes[ofsetS + 0], 6);
            data.SpecialWWIIonlyAI = GetBit(bytes[ofsetS + 1], 7);

            // Scenario name (read till first null character)
            int step = 0;
            List<char> chars = new();
            while (bytes[ofsetS + 2 + step] != 0x0)
            {
                chars.Add((char)bytes[ofsetS + 2 + step]);
                step++;
            }
            data.ScenarioName = String.Concat(chars);

            data.TechParadigm = BitConverter.ToInt16(bytes, ofsetS + 82);
            data.TurnYearIncrement = BitConverter.ToInt16(bytes, ofsetS + 84);
            data.StartingYear = BitConverter.ToInt16(bytes, ofsetS + 86);
            data.MaxTurns = BitConverter.ToInt16(bytes, ofsetS + 88);
            data.ObjectiveProtagonist = BitConverter.ToInt16(bytes, ofsetS + 91);
            data.NoObjectivesDecisiveVictory = BitConverter.ToInt16(bytes, ofsetS + 93);
            data.NoObjectivesMarginalVictory = BitConverter.ToInt16(bytes, ofsetS + 95);
            data.NoObjectivesMarginalDefeat = BitConverter.ToInt16(bytes, ofsetS + 97);
            data.NoObjectivesDecisiveDefeat = BitConverter.ToInt16(bytes, ofsetS + 99);

            #endregion
            #region Events
            //=========================
            //EVENTS
            //=========================
            var offsetE = IndexofStringInByteArray(bytes, "EVNT");
            if (offsetE != -1)
            {
                offsetE += 4;
                data.NumberOfEvents = BitConverter.ToInt16(bytes, offsetE);

                data.EventTriggerIds = new int[data.NumberOfEvents];
                data.EventActionIds = new int[data.NumberOfEvents][];
                data.EventTriggerParam = new int[data.NumberOfEvents][];
                data.EventActionParam = new int[data.NumberOfEvents][];

                offsetE += 4;
                multipl = 444;  // no of bytes for each event

                // Read strings
                data.EventStrings = ReadEventStrings(bytes, offsetE + multipl * data.NumberOfEvents);

                for (int i = 0; i < data.NumberOfEvents; i++)
                {
                    data.EventTriggerIds[i] = FindPositionOfBits(BitConverter.ToInt32(bytes, offsetE + multipl * i))[0];
                    data.EventActionIds[i] = FindPositionOfBits(BitConverter.ToInt32(bytes, offsetE + 4 + multipl * i)).ToArray();

                    // Trigger parameters
                    var _row = new int[12];
                    for (int j = 0; j < 12; j++)
                    {
                        _row[j] = BitConverter.ToInt32(bytes, offsetE + multipl * i + 8 + 4 * j);
                    }
                    data.EventTriggerParam[i] = _row;

                    // Action parameters
                    _row = new int[97];
                    for (int j = 0; j < 97; j++)
                    {
                        _row[j] = BitConverter.ToInt32(bytes, offsetE + multipl * i + 8 + 4 * 12 + 4 * j);
                    }
                    data.EventActionParam[i] = _row;
                }
            }
            #endregion

            return data;
        }

        // Helper function
        private static bool GetBit(byte b, int bitNumber)
        {
            return (b & (1 << bitNumber)) != 0;
        }

        // Find offset of string in byte array
        private static int IndexofStringInByteArray(byte[] byteArray, string text)
        {
            byte[] stringBytes = Encoding.ASCII.GetBytes(text);

            bool found;
            for (int i = 0; i < byteArray.Length - stringBytes.Length; i++)
            {
                found = true;
                for (int j = 0; j < stringBytes.Length; j++)
                {
                    if (byteArray[i + j] != stringBytes[j])
                        found = false;
                }

                if (found)
                    return i;
            }

            return -1;
        }

        // Read event strings at end of .sav
        private static List<string> ReadEventStrings(byte[] byteArray, int begin)
        {
            List<string> strings = new();

            if (byteArray.Length == begin) return strings;

            int offset = 0;
            List<char> chars = new();
            do
            {
                if ((char)byteArray[begin + offset] != '\0')
                {
                    chars.Add((char)byteArray[begin + offset]);
                }
                else
                {
                    strings.Add(string.Concat(chars));
                    chars.Clear();
                }
                offset++;
            } while (begin + offset < byteArray.Length);

            return strings;
        }

        // Find position of bits
        private static List<int> FindPositionOfBits(int n)
        {
            var positions = new List<int>();
            for (int BitToTest = 0; BitToTest < 32; BitToTest++)
            {
                if ((n & (1 << BitToTest)) != 0)
                {
                    positions.Add(BitToTest);
                }
            }

            return positions;
        }
    }
}
