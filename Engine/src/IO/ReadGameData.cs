using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Civ2engine.Enums;

namespace Civ2engine
{
    // Read game data from SAV and RULES.txt
    public class ReadGameData
    {
        public GameData data;

        public GameData Read_SAV_and_RULES(string savPath, string savName)
        {
            data = new GameData();
            ReadRules(savPath);
            ReadSAV(savPath, savName);
            return data;
        }

        // Read RULES.TXT
        public void ReadRules(string path)
        {
            // Read in SAV directory path. If it doesn't exist there, read from root civ2 directory.
            string rulesPath1 = path + Path.DirectorySeparatorChar + "RULES.TXT";
            string rulesPath2 = Settings.Civ2Path + "RULES.TXT";
            string filePath = null;
            if (File.Exists(rulesPath1))
            {
                filePath = rulesPath1;
            }
            else if (File.Exists(rulesPath2))
            {
                filePath = rulesPath2;
            }
            else
            {
                Console.WriteLine("RULES.TXT not found!");
            }

            data.Rules = new List<string[]>();

            // Read the file and display it line by line.  
            using StreamReader file = new StreamReader(filePath);
            string line;
            while ((line = file.ReadLine()) != null)
            {
                List<string> text;

                // Read COSMIC PRINCIPLES
                if (line == "@COSMIC")
                {
                    string[] cosmicRules = new string[22];
                    for (int i = 0; i < 22; i++)
                    {
                        cosmicRules[i] = file.ReadLine().Split(';').ToList()[0].Trim();
                    }
                    data.Rules.Add(cosmicRules);
                }

                // Read TECH RULES
                if (line == "@CIVILIZE")
                {
                    string[] techName = new string[100];
                    string[] techAIvalue = new string[100];
                    string[] techModifier = new string[100];
                    string[] techPrereq1 = new string[100];
                    string[] techPrereq2 = new string[100];
                    string[] techEpoch = new string[100];
                    string[] techCategory = new string[100];
                    string[] techShortName = new string[100];
                    for (int row = 0; row < 100; row++)
                    {
                        line = file.ReadLine();
                        text = line.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        techName[row] = text[0];
                        techAIvalue[row] = text[1].Trim();
                        techModifier[row] = text[2].Trim();
                        techPrereq1[row] = text[3].Trim();
                        techPrereq2[row] = text[4].Trim();
                        techEpoch[row] = text[5].Trim();
                        techCategory[row] = text[6].Trim();
                        techShortName[row] = text[7].Trim();
                    }
                    data.Rules.Add(techName);
                    data.Rules.Add(techAIvalue);
                    data.Rules.Add(techModifier);
                    data.Rules.Add(techPrereq1);
                    data.Rules.Add(techPrereq2);
                    data.Rules.Add(techEpoch);
                    data.Rules.Add(techCategory);
                    data.Rules.Add(techShortName);
                }

                // Read IMPROVEMENTS
                if (line == "@IMPROVE")
                {
                    string[] improvementName = new string[67];
                    string[] improvementCost = new string[67];
                    string[] improvementUpkeep = new string[67];
                    string[] improvementPrereq = new string[67];
                    for (int row = 0; row < 67; row++)
                    {
                        line = file.ReadLine();
                        text = line.Split(',').ToList();
                        improvementName[row] = text[0];
                        improvementCost[row] = text[1].Trim();
                        improvementUpkeep[row] = text[2].Trim();
                        improvementPrereq[row] = text[3].Trim();
                    }
                    data.Rules.Add(improvementName);
                    data.Rules.Add(improvementCost);
                    data.Rules.Add(improvementUpkeep);
                    data.Rules.Add(improvementPrereq);
                }

                // Read EXPIRATION OF ADVANCES
                if (line == "@ENDWONDER")
                {
                    // First 39 are city improvements, they have no expiration
                    string[] improvementAdvanceExpiration = new string[67];
                    for (int row = 0; row < 39; row++)
                    {
                        improvementAdvanceExpiration[row] = "";
                    }

                    // Next 28 are advances
                    for (int row = 0; row < 28; row++)
                    {
                        line = file.ReadLine();
                        improvementAdvanceExpiration[row + 39] = line.Split(',').ToList()[0];
                    }
                    data.Rules.Add(improvementAdvanceExpiration);
                }

                // Read UNIT RULES
                if (line == "@UNITS")
                {
                    string[] unitName = new string[62];
                    string[] unitUntil = new string[62];
                    string[] unitDomain = new string[62];
                    string[] unitMove = new string[62];
                    string[] unitRange = new string[62];
                    string[] unitAttack = new string[62];
                    string[] unitDefense = new string[62];
                    string[] unitHitp = new string[62];
                    string[] unitFirepwr = new string[62];
                    string[] unitCost = new string[62];
                    string[] unitHold = new string[62];
                    string[] unitAIrole = new string[62];
                    string[] unitPrereq = new string[62];
                    string[] unitFlags = new string[62];
                    for (int row = 0; row < 62; row++)
                    {
                        line = file.ReadLine();
                        text = line.Split(',').ToList();
                        unitName[row] = text[0];
                        unitUntil[row] = text[1];
                        unitDomain[row] = text[2].Trim();
                        unitMove[row] = text[3].Trim().Replace(".", string.Empty);
                        unitRange[row] = text[4].Trim();
                        unitAttack[row] = text[5].Trim().Replace("a", string.Empty);
                        unitDefense[row] = text[6].Trim().Replace("d", string.Empty);
                        unitHitp[row] = text[7].Trim().Replace("h", string.Empty);
                        unitFirepwr[row] = text[8].Trim().Replace("f", string.Empty);
                        unitCost[row] = text[9].Trim();
                        unitHold[row] = text[10].Trim();
                        unitAIrole[row] = text[11].Trim();
                        unitPrereq[row] = text[12];
                        unitFlags[row] = text[13];
                    }
                    data.Rules.Add(unitName);
                    data.Rules.Add(unitUntil);
                    data.Rules.Add(unitDomain);
                    data.Rules.Add(unitMove);
                    data.Rules.Add(unitRange);
                    data.Rules.Add(unitAttack);
                    data.Rules.Add(unitDefense);
                    data.Rules.Add(unitHitp);
                    data.Rules.Add(unitFirepwr);
                    data.Rules.Add(unitCost);
                    data.Rules.Add(unitHold);
                    data.Rules.Add(unitAIrole);
                    data.Rules.Add(unitPrereq);
                    data.Rules.Add(unitFlags);
                }

                // Read TERRAIN RULES
                if (line == "@TERRAIN")
                {
                    // First read normal terrain
                    string[] terrainName = new string[11];
                    string[] terrainMovecost = new string[11];
                    string[] terrainDefense = new string[11];
                    string[] terrainFood = new string[11];
                    string[] terrainShields = new string[11];
                    string[] terrainTrade = new string[11];
                    string[] terrainCanIrrigate = new string[11];
                    string[] terrainIrrigateBonus = new string[11];
                    string[] terrainIrrigateTurns = new string[11];
                    string[] terrainIrrigateAI = new string[11];
                    string[] terrainCanMine = new string[11];
                    string[] terrainMineBonus = new string[11];
                    string[] terrainMineTurns = new string[11];
                    string[] terrainMineAI = new string[11];
                    string[] terrainTransform = new string[11];
                    string[] terrainShortName = new string[11];

                    for (int row = 0; row < 11; row++)
                    {
                        line = file.ReadLine();
                        text = line.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        terrainName[row] = text[0].Trim();
                        terrainMovecost[row] = text[1].Trim();
                        terrainDefense[row] = text[2].Trim();
                        terrainFood[row] = text[3].Trim();
                        terrainShields[row] = text[4].Trim();
                        terrainTrade[row] = text[5].Trim();
                        terrainCanIrrigate[row] = text[6].Trim();
                        terrainIrrigateBonus[row] = text[7].Trim();
                        terrainIrrigateTurns[row] = text[8].Trim();
                        terrainIrrigateAI[row] = text[9].Trim();
                        terrainCanMine[row] = text[10].Trim();
                        terrainMineBonus[row] = text[11].Trim();
                        terrainMineTurns[row] = text[12].Trim();
                        terrainMineAI[row] = text[13].Trim();
                        terrainTransform[row] = text[14].Trim();
                        terrainShortName[row] = text[16].Trim();
                    }
                    data.Rules.Add(terrainName);
                    data.Rules.Add(terrainMovecost);
                    data.Rules.Add(terrainDefense);
                    data.Rules.Add(terrainFood);
                    data.Rules.Add(terrainShields);
                    data.Rules.Add(terrainTrade);
                    data.Rules.Add(terrainCanIrrigate);
                    data.Rules.Add(terrainIrrigateBonus);
                    data.Rules.Add(terrainIrrigateTurns);
                    data.Rules.Add(terrainIrrigateAI);
                    data.Rules.Add(terrainCanMine);
                    data.Rules.Add(terrainMineBonus);
                    data.Rules.Add(terrainMineTurns);
                    data.Rules.Add(terrainMineAI);
                    data.Rules.Add(terrainTransform);
                    data.Rules.Add(terrainShortName);

                    // Next read special terrain
                    string[] terrainSpecName = new string[22];
                    string[] terrainSpecMovecost = new string[22];
                    string[] terrainSpecDefense = new string[22];
                    string[] terrainSpecFood = new string[22];
                    string[] terrainSpecShields = new string[22];
                    string[] terrainSpecTrade = new string[22];
                    for (int row = 0; row < 22; row++)
                    {
                        line = file.ReadLine();
                        text = line.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        terrainSpecName[row] = text[0];
                        terrainSpecMovecost[row] = text[1].Trim();
                        terrainSpecDefense[row] = text[2].Trim();
                        terrainSpecFood[row] = text[3].Trim();
                        terrainSpecShields[row] = text[4].Trim();
                        terrainSpecTrade[row] = text[5].Trim();
                    }
                    data.Rules.Add(terrainSpecName);
                    data.Rules.Add(terrainSpecMovecost);
                    data.Rules.Add(terrainSpecDefense);
                    data.Rules.Add(terrainSpecFood);
                    data.Rules.Add(terrainSpecShields);
                    data.Rules.Add(terrainSpecTrade);
                }

                // Read GOVERNMENTS
                if (line == "@GOVERNMENTS")
                {
                    string[] governmentName = new string[7];
                    string[] governmentTitleHIS = new string[7];
                    string[] governmentTitleHER = new string[7];
                    for (int row = 0; row < 7; row++)
                    {
                        line = file.ReadLine();
                        text = line.Split(',').ToList();
                        governmentName[row] = text[0].Trim();
                        governmentTitleHIS[row] = text[1].Trim();
                        governmentTitleHER[row] = text[2].Trim();
                    }
                    data.Rules.Add(governmentName);
                    data.Rules.Add(governmentTitleHIS);
                    data.Rules.Add(governmentTitleHER);
                }

                // Read LEADERS
                if (line == "@LEADERS")
                {
                    // TODO: also read govrnment type for name replacement + male & female titles (leaderFemale determines what will the title be)
                    string[] leaderNameHIS = new string[21];
                    string[] leaderNameHER = new string[21];
                    string[] leaderFemale = new string[21];
                    string[] leaderColor = new string[21];
                    string[] leaderCityStyle = new string[21];
                    string[] leaderPlural = new string[21];
                    string[] leaderAdjective = new string[21];
                    string[] leaderAttack = new string[21];
                    string[] leaderExpand = new string[21];
                    string[] leaderCivilize = new string[21];
                    for (int row = 0; row < 21; row++)
                    {
                        line = file.ReadLine();
                        text = line.Split(',').ToList();
                        leaderNameHIS[row] = text[0].Trim();
                        leaderNameHER[row] = text[1].Trim();
                        leaderFemale[row] = text[2].Trim();
                        leaderColor[row] = text[3].Trim();
                        leaderCityStyle[row] = text[4].Trim();
                        leaderPlural[row] = text[5].Trim();
                        leaderAdjective[row] = text[6].Trim();
                        leaderAttack[row] = text[7].Trim();
                        leaderExpand[row] = text[8].Trim();
                        leaderCivilize[row] = text[9].Trim();
                    }
                    data.Rules.Add(leaderNameHIS);
                    data.Rules.Add(leaderNameHER);
                    data.Rules.Add(leaderFemale);
                    data.Rules.Add(leaderColor);
                    data.Rules.Add(leaderCityStyle);
                    data.Rules.Add(leaderPlural);
                    data.Rules.Add(leaderAdjective);
                    data.Rules.Add(leaderAttack);
                    data.Rules.Add(leaderExpand);
                    data.Rules.Add(leaderCivilize);
                }

                // Read CARAVAN TRADING COMMODITIES
                if (line == "@CARAVAN")
                {
                    string[] caravanCommoditie = new string[16];
                    for (int row = 0; row < 16; row++)
                    {
                        line = file.ReadLine();
                        text = line.Split(',').ToList();
                        caravanCommoditie[row] = text[0].Trim();
                    }
                    data.Rules.Add(caravanCommoditie);
                }

                // Read ORDERS
                if (line == "@ORDERS")
                {
                    string[] orderName = new string[11];
                    string[] orderShortcut = new string[11];
                    for (int row = 0; row < 11; row++)
                    {
                        line = file.ReadLine();
                        text = line.Split(',').ToList();
                        orderName[row] = text[0];
                        orderShortcut[row] = text[1].Trim();
                    }
                    data.Rules.Add(orderName);
                    data.Rules.Add(orderShortcut);
                }

                // Read DIFFICULTY
                if (line == "@DIFFICULTY")
                {
                    string[] difficulty = new string[6];
                    for (int row = 0; row < 6; row++)
                    {
                        line = file.ReadLine();
                        difficulty[row] = line;
                    }
                    data.Rules.Add(difficulty);
                }

                // Read ATTITUDES
                if (line == "@ATTITUDES")
                {
                    string[] attitude = new string[9];
                    for (int row = 0; row < 9; row++)
                    {
                        line = file.ReadLine();
                        attitude[row] = line;
                    }
                    data.Rules.Add(attitude);
                }
            }

            file.Close();
        }

        // READ SAV GAME
        public void ReadSAV(string savPath, string savName)
        {
            int intVal1, intVal2, intVal3, intVal4;

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
            data.Options = new bool[35];
            data.Options[0] = GetBit(bytes[12], 4);     // Simplified combat on/off
            data.Options[1] = GetBit(bytes[12], 7);     // Bloodlust on/off            
            data.Options[2] = GetBit(bytes[13], 0);     // Don't restart if eliminated
            data.Options[3] = GetBit(bytes[13], 7);     // Flat earth
            data.Options[4] = GetBit(bytes[14], 3);     // Music on/off
            data.Options[5] = GetBit(bytes[14], 4);     // Sound effects on/off
            data.Options[6] = GetBit(bytes[14], 5);     // Grid on/off
            data.Options[7] = GetBit(bytes[14], 6);     // Enter closes city screen     
            data.Options[8] = GetBit(bytes[14], 7);     // Move units without mouse
            data.Options[9] = GetBit(bytes[15], 0);     // Tutorial help on/off
            data.Options[10] = GetBit(bytes[15], 1);    // Instant advice on/off
            data.Options[11] = GetBit(bytes[15], 2);    // Fast piece slide on/off
            data.Options[12] = GetBit(bytes[15], 3);    // No pause after enemy moves on/off
            data.Options[13] = GetBit(bytes[15], 4);    // Show enemy moves on/off
            data.Options[14] = GetBit(bytes[15], 5);    // Autosave each turn on/off
            data.Options[15] = GetBit(bytes[15], 6);    // Always wait at end of turn on/off
            data.Options[16] = GetBit(bytes[15], 7);    // Cheat menu on/off
            data.Options[17] = GetBit(bytes[16], 0);    // Wonder movies on/off
            data.Options[18] = GetBit(bytes[16], 1);    // Diplomacy screen graphics on/off
            data.Options[19] = GetBit(bytes[16], 2);    // Throne room graphics on/off
            data.Options[20] = GetBit(bytes[16], 3);    // Civilopedia for advances on/off
            data.Options[21] = GetBit(bytes[16], 4);    // High council on/off
            data.Options[22] = GetBit(bytes[16], 5);    // Animated heralds on/off
            data.Options[23] = GetBit(bytes[20], 4);    // Cheat penalty/warning on/off
            data.Options[24] = GetBit(bytes[22], 0);    // Show city improvements built on/off
            data.Options[25] = GetBit(bytes[22], 1);    // Warn when city growth halted on/off
            data.Options[26] = GetBit(bytes[22], 2);    // Show invalid build instructions on/off
            data.Options[27] = GetBit(bytes[22], 3);    // Show non combat units build on/off
            data.Options[28] = GetBit(bytes[22], 4);    // Announce order restored in cities on/off
            data.Options[29] = GetBit(bytes[22], 5);    // Announce cities in disorder on/off
            data.Options[30] = GetBit(bytes[22], 6);    // Warn when food dangerously low on/off
            data.Options[31] = GetBit(bytes[22], 7);    // Announce we love king day on/off
            data.Options[32] = GetBit(bytes[23], 0);    // Warn when changing production will cost shileds on/off
            data.Options[33] = GetBit(bytes[23], 1);    // Warn when pollution occurs on/off
            data.Options[34] = GetBit(bytes[23], 2);    // Zoom to city not default action on/off

            // Number of turns passed
            data.TurnNumber = short.Parse(string.Concat(bytes[29].ToString("X"), bytes[28].ToString("X")), System.Globalization.NumberStyles.HexNumber);    //convert hex value 2 & 1 (in that order) together to int

            // Number of turns passed for game year calculation
            data.TurnNumberForGameYear = short.Parse(string.Concat(bytes[31].ToString("X"), bytes[30].ToString("X")), System.Globalization.NumberStyles.HexNumber);

            // Which unit is selected at start of game (return -1 if no unit is selected (FFFFhex=65535dec))
            int _selectedIndex = short.Parse(string.Concat(bytes[35].ToString("X"), bytes[34].ToString("X")), System.Globalization.NumberStyles.HexNumber);
            data.SelectedUnitIndex = (_selectedIndex == 65535) ? -1 : _selectedIndex;

            // Which human player is used
            data.PlayersCivIndex = bytes[39];    // TODO: how is this different from bytes[41]???

            // Players map currently shown
            data.WhichCivsMapShown = bytes[40];

            // Players civ number used
            data.PlayersCivilizationNumberUsed = bytes[41];

            // Map revealed
            data.MapRevealed = bytes[43] == 1;

            // Difficulty level
            data.DifficultyLevel = (DifficultyType)bytes[44];

            // Barbarian activity
            data.BarbarianActivity = (BarbarianActivityType)bytes[45];

            // Civs in play
            data.CivsInPlay = new bool[8] { false, false, false, false, false, false, false, false };
            for (int i = 0; i < 8; i++)
                data.CivsInPlay[i] = GetBit(bytes[46], i);

            // Civs with human player playing (multiplayer)
            //string humanPlayerPlayed = Convert.ToString(bytes[47], 2).PadLeft(8, '0');

            // Amount of pollution
            data.PollutionAmount = bytes[50];

            // Global temp rising times occured
            data.GlobalTempRiseOccured = bytes[51];

            //Number of turns of peace
            data.NoOfTurnsOfPeace = bytes[56];

            // Number of units
            data.NumberOfUnits = short.Parse(string.Concat(bytes[59].ToString("X"), bytes[58].ToString("X")), System.Globalization.NumberStyles.HexNumber);

            // Number of cities
            data.NumberOfCities = short.Parse(string.Concat(bytes[61].ToString("X"), bytes[60].ToString("X")), System.Globalization.NumberStyles.HexNumber);

            #endregion
            #region Wonders
            //=========================
            //WONDERS
            //=========================
            data.WonderCity = new int[28];        // city with wonder
            data.WonderBuilt = new bool[28];      // has the wonder been built?
            data.WonderDestroyed = new bool[28];  // has the wonder been destroyed?
            for (int i = 0; i < 28; i++)
            {
                // City number with the wonder
                intVal1 = bytes[266 + 2 * i];
                intVal2 = bytes[266 + 2 * i + 1];
                data.WonderCity[i] = short.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

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
            #region Civs
            //=========================
            //CIVS
            //=========================
            char[] asciich = new char[23];
            data.CivCityStyle = new int[8];
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
                // City style
                data.CivCityStyle[i + 1] = bytes[584 + 242 * i];

                // Leader names (if empty, get the name from RULES.TXT)
                for (int j = 0; j < 23; j++) asciich[j] = Convert.ToChar(bytes[584 + 2 + 242 * i + j]);
                data.CivLeaderName[i + 1] = new string(asciich);
                data.CivLeaderName[i + 1] = data.CivLeaderName[i + 1].Replace("\0", string.Empty);  // remove null characters

                // Tribe name (if empty, get the name from RULES.TXT)
                for (int j = 0; j < 23; j++) asciich[j] = Convert.ToChar(bytes[584 + 2 + 23 + 242 * i + j]);
                data.CivTribeName[i + 1] = new string(asciich);
                data.CivTribeName[i + 1] = data.CivTribeName[i + 1].Replace("\0", string.Empty);

                // Adjective (if empty, get the name from RULES.TXT)
                for (int j = 0; j < 23; j++) asciich[j] = Convert.ToChar(bytes[584 + 2 + 23 + 23 + 242 * i + j]);
                data.CivAdjective[i + 1] = new string(asciich);
                data.CivAdjective[i + 1] = data.CivAdjective[i + 1].Replace("\0", string.Empty);

                //Leader titles (Anarchy, Despotism, ...)
                // .... TO-DO ....
            }
            #endregion
            #region Tech & money
            //=========================
            //TECH & MONEY
            //=========================
            data.RulerGender = new int[8];
            data.CivMoney = new int[8];
            data.CivNumber = new int[8];
            data.CivResearchProgress = new int[8];
            data.CivResearchingTech = new int[8];
            data.CivSciRate = new int[8];
            data.CivTaxRate = new int[8];
            data.CivGovernment = new int[8];
            data.CivReputation = new int[8];
            data.CivTechs = new bool[89];
            // starting offset = 8E6(hex) = 2278(10), each block has 1427(10) bytes
            for (int i = 0; i < 8; i++) // for each civ
            {
                // Gender (0=male, 2=female)
                data.RulerGender[i] = bytes[2278 + 1428 * i + 1]; // 2nd byte in tribe block

                // Money
                intVal1 = bytes[2278 + 1428 * i + 2];    // 3rd byte in tribe block
                intVal2 = bytes[2278 + 1428 * i + 3];    // 4th byte in tribe block
                data.CivMoney[i] = short.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                // Tribe number as per @Leaders table in RULES.TXT
                data.CivNumber[i] = bytes[2278 + 1428 * i + 6];    // 7th byte in tribe block

                // Research progress
                intVal1 = bytes[2278 + 1428 * i + 8];    // 9th byte in tribe block
                intVal2 = bytes[2278 + 1428 * i + 9];    // 10th byte in tribe block
                data.CivResearchProgress[i] = short.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                // Tech currently being researched
                data.CivResearchingTech[i] = bytes[2278 + 1428 * i + 10]; // 11th byte in tribe block (FF(hex) = no goal)

                // Science rate (%/10)
                data.CivSciRate[i] = bytes[2278 + 1428 * i + 19]; // 20th byte in tribe block

                // Tax rate (%/10)
                data.CivTaxRate[i] = bytes[2278 + 1428 * i + 20]; // 21st byte in tribe block

                // Government
                data.CivGovernment[i] = bytes[2278 + 1428 * i + 21]; // 22nd byte in tribe block (0=anarchy, ...)

                // Reputation
                data.CivReputation[i] = bytes[2278 + 1428 * i + 30]; // 31st byte in tribe block

                // Treaties
                // ..... TO-DO .....

                // Attitudes
                // ..... TO-DO .....

                // Technologies
                string civTechs1 = Convert.ToString(bytes[2278 + 1428 * i + 88], 2).PadLeft(8, '0');    //89th byte
                string civTechs2 = Convert.ToString(bytes[2278 + 1428 * i + 89], 2).PadLeft(8, '0');
                string civTechs3 = Convert.ToString(bytes[2278 + 1428 * i + 90], 2).PadLeft(8, '0');
                string civTechs4 = Convert.ToString(bytes[2278 + 1428 * i + 91], 2).PadLeft(8, '0');
                string civTechs5 = Convert.ToString(bytes[2278 + 1428 * i + 92], 2).PadLeft(8, '0');
                string civTechs6 = Convert.ToString(bytes[2278 + 1428 * i + 93], 2).PadLeft(8, '0');
                string civTechs7 = Convert.ToString(bytes[2278 + 1428 * i + 94], 2).PadLeft(8, '0');
                string civTechs8 = Convert.ToString(bytes[2278 + 1428 * i + 95], 2).PadLeft(8, '0');
                string civTechs9 = Convert.ToString(bytes[2278 + 1428 * i + 96], 2).PadLeft(8, '0');
                string civTechs10 = Convert.ToString(bytes[2278 + 1428 * i + 97], 2).PadLeft(8, '0');
                string civTechs11 = Convert.ToString(bytes[2278 + 1428 * i + 98], 2).PadLeft(8, '0');
                string civTechs12 = Convert.ToString(bytes[2278 + 1428 * i + 99], 2).PadLeft(8, '0');
                string civTechs13 = Convert.ToString(bytes[2278 + 1428 * i + 100], 2).PadLeft(8, '0');   //101st byte
                civTechs13 = civTechs13.Remove(civTechs13.Length - 4); //remove last 4 bits, they are not important
                // Put all techs into one large string, where bit0=1st tech, bit1=2nd tech, ..., bit99=100th tech
                // First reverse bit order in all strings
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
                // Merge all strings into a large string
                string civTechs_ = String.Concat(civTechs1, civTechs2, civTechs3, civTechs4, civTechs5, civTechs6, civTechs7, civTechs8, civTechs9, civTechs10, civTechs11, civTechs12, civTechs13);
                // true = tech researched, false = not researched
                for (int no = 0; no < 89; no++)
                    data.CivTechs[no] = civTechs_[no] == '1';
            }
            #endregion
            #region Map
            //=========================
            //MAP
            //=========================
            // Map header ofset
            int ofset;
            if (bytes[10] > 39) ofset = 13702;  // FW and later (offset=3586hex)
            else ofset = 13432;                 // Conflicts (offset=3478hex)

            // Map X dimension
            intVal1 = bytes[ofset + 0];
            intVal2 = bytes[ofset + 1];
            data.MapXdim = short.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber) / 2; //map 150x120 is really 75x120

            // Map Y dimension
            intVal1 = bytes[ofset + 2];
            intVal2 = bytes[ofset + 3];
            data.MapYdim = short.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

            // Map area:
            intVal1 = bytes[ofset + 4];
            intVal2 = bytes[ofset + 5];
            data.MapArea = short.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

            //// Flat Earth flag (info already given before!!)
            //intVal1 = bytes[ofset + 6];
            //intVal2 = bytes[ofset + 7];
            //flatEarth = short.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

            // Map resource seed
            intVal1 = bytes[ofset + 8];
            intVal2 = bytes[ofset + 9];
            data.MapResourceSeed = short.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

            // Locator map X dimension
            intVal1 = bytes[ofset + 10];
            intVal2 = bytes[ofset + 11];
            data.MapLocatorXdim = short.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);  //TODO: what does this do?

            // Locator map Y dimension
            int intValue11 = bytes[ofset + 12];
            int intValue12 = bytes[ofset + 13];
            data.MapLocatorYdim = short.Parse(string.Concat(intValue12.ToString("X"), intValue11.ToString("X")), System.Globalization.NumberStyles.HexNumber);

            // Initialize Terrain array now that you know its size
            //TerrainTile = new ITerrain[Data.MapXdim, Data.MapYdim];   //TODO: where to put this?

            // block 1 - terrain improvements (for individual civs)
            int ofsetB1 = ofset + 14; //offset for block 2 values
            //...........
            // block 2 - terrain type
            int ofsetB2 = ofsetB1 + 7 * data.MapArea; //offset for block 2 values
            data.MapTerrainType = new TerrainType[data.MapXdim, data.MapYdim];
            data.MapVisibilityCivs = new bool[data.MapXdim, data.MapYdim][];
            data.MapRiverPresent = new bool[data.MapXdim, data.MapYdim];
            data.MapResourcePresent = new bool[data.MapXdim, data.MapYdim];
            data.MapUnitPresent = new bool[data.MapXdim, data.MapYdim];
            data.MapCityPresent = new bool[data.MapXdim, data.MapYdim];
            data.MapIrrigationPresent = new bool[data.MapXdim, data.MapYdim];
            data.MapMiningPresent = new bool[data.MapXdim, data.MapYdim];
            data.MapRoadPresent = new bool[data.MapXdim, data.MapYdim];
            data.MapRailroadPresent = new bool[data.MapXdim, data.MapYdim];
            data.MapFortressPresent = new bool[data.MapXdim, data.MapYdim];
            data.MapPollutionPresent = new bool[data.MapXdim, data.MapYdim];
            data.MapFarmlandPresent = new bool[data.MapXdim, data.MapYdim];
            data.MapAirbasePresent = new bool[data.MapXdim, data.MapYdim];
            data.MapIslandNo = new int[data.MapXdim, data.MapYdim];
            data.MapSpecialType = new SpecialType[data.MapXdim, data.MapYdim];
            for (int i = 0; i < data.MapArea; i++)
            {
                int x = i % data.MapXdim;
                int y = i / data.MapXdim;

                // Terrain type
                int terrB = ofsetB2 + i * 6 + 0;
                if (!GetBit(bytes[terrB], 0) && !GetBit(bytes[terrB], 1) && !GetBit(bytes[terrB], 2) && !GetBit(bytes[terrB], 3)) data.MapTerrainType[x, y] = TerrainType.Desert;   // xxxx 0000
                if (GetBit(bytes[terrB], 0) && !GetBit(bytes[terrB], 1) && !GetBit(bytes[terrB], 2) && !GetBit(bytes[terrB], 3)) data.MapTerrainType[x, y] = TerrainType.Plains;  // xxxx 0001
                if (!GetBit(bytes[terrB], 0) && GetBit(bytes[terrB], 1) && !GetBit(bytes[terrB], 2) && !GetBit(bytes[terrB], 3)) data.MapTerrainType[x, y] = TerrainType.Grassland;  // xxxx 0010
                if (GetBit(bytes[terrB], 0) && GetBit(bytes[terrB], 1) && !GetBit(bytes[terrB], 2) && !GetBit(bytes[terrB], 3)) data.MapTerrainType[x, y] = TerrainType.Forest;   // xxxx 0011
                if (!GetBit(bytes[terrB], 0) && !GetBit(bytes[terrB], 1) && GetBit(bytes[terrB], 2) && !GetBit(bytes[terrB], 3)) data.MapTerrainType[x, y] = TerrainType.Hills;  // xxxx 0100
                if (GetBit(bytes[terrB], 0) && !GetBit(bytes[terrB], 1) && GetBit(bytes[terrB], 2) && !GetBit(bytes[terrB], 3)) data.MapTerrainType[x, y] = TerrainType.Mountains;  // xxxx 0101
                if (!GetBit(bytes[terrB], 0) && GetBit(bytes[terrB], 1) && GetBit(bytes[terrB], 2) && !GetBit(bytes[terrB], 3)) data.MapTerrainType[x, y] = TerrainType.Tundra;  // xxxx 0110
                if (GetBit(bytes[terrB], 0) && GetBit(bytes[terrB], 1) && GetBit(bytes[terrB], 2) && !GetBit(bytes[terrB], 3)) data.MapTerrainType[x, y] = TerrainType.Glacier;  // xxxx 0111
                if (!GetBit(bytes[terrB], 0) && !GetBit(bytes[terrB], 1) && !GetBit(bytes[terrB], 2) && GetBit(bytes[terrB], 3)) data.MapTerrainType[x, y] = TerrainType.Swamp;  // xxxx 1000
                if (GetBit(bytes[terrB], 0) && !GetBit(bytes[terrB], 1) && !GetBit(bytes[terrB], 2) && GetBit(bytes[terrB], 3)) data.MapTerrainType[x, y] = TerrainType.Jungle;  // xxxx 1001
                if (!GetBit(bytes[terrB], 0) && GetBit(bytes[terrB], 1) && !GetBit(bytes[terrB], 2) && GetBit(bytes[terrB], 3)) data.MapTerrainType[x, y] = TerrainType.Ocean;  // xxxx 1010
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

            data.UnitXloc = new int[data.NumberOfUnits];
            data.UnitYloc = new int[data.NumberOfUnits];
            data.UnitDead = new bool[data.NumberOfUnits];
            data.UnitFirstMove = new bool[data.NumberOfUnits];
            data.UnitGreyStarShield = new bool[data.NumberOfUnits];
            data.UnitVeteran = new bool[data.NumberOfUnits];
            data.UnitType = new UnitType[data.NumberOfUnits];
            data.UnitCiv = new int[data.NumberOfUnits];
            data.UnitMovePointsLost = new int[data.NumberOfUnits];
            data.UnitHitPointsLost = new int[data.NumberOfUnits];
            data.UnitPrevXloc = new int[data.NumberOfUnits];
            data.UnitPrevYloc = new int[data.NumberOfUnits];
            data.UnitCaravanCommodity = new CommodityType[data.NumberOfUnits];
            data.UnitOrders = new OrderType[data.NumberOfUnits];
            data.UnitHomeCity = new int[data.NumberOfUnits];
            data.UnitGotoX = new int[data.NumberOfUnits];
            data.UnitGotoY = new int[data.NumberOfUnits];
            data.UnitLinkOtherUnitsOnTop = new int[data.NumberOfUnits];
            data.UnitLinkOtherUnitsUnder = new int[data.NumberOfUnits];
            for (int i = 0; i < data.NumberOfUnits; i++)
            {
                // Unit X location
                intVal1 = bytes[ofsetU + multipl * i + 0];
                intVal2 = bytes[ofsetU + multipl * i + 1];
                data.UnitXloc[i] = short.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                data.UnitDead[i] = GetBit(bytes[ofsetU + multipl * i + 1], 0);    // Unit is inactive (dead) if the value of X-Y is negative (1st bit = 1)

                // Unit Y location
                intVal1 = bytes[ofsetU + multipl * i + 2];
                intVal2 = bytes[ofsetU + multipl * i + 3];
                data.UnitYloc[i] = short.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                data.UnitFirstMove[i] = GetBit(bytes[ofsetU + multipl * i + 4], 1);         // If this is the unit's first move
                data.UnitGreyStarShield[i] = GetBit(bytes[ofsetU + multipl * i + 5], 0);    // Grey star to the shield
                data.UnitVeteran[i] = GetBit(bytes[ofsetU + multipl * i + 5], 2);           // Veteran status
                data.UnitType[i] = (UnitType)bytes[ofsetU + multipl * i + 6];               // Unit type
                data.UnitCiv[i] = bytes[ofsetU + multipl * i + 7];                          // Unit civ, 00 = barbarians                
                data.UnitMovePointsLost[i] = bytes[ofsetU + multipl * i + 8];               // Unit move points expended                
                data.UnitHitPointsLost[i] = bytes[ofsetU + multipl * i + 10];               // Unit hitpoints lost
                switch (bytes[ofsetU + multipl * i + 11])                                   // Unit previous move (00=up-right, 01=right, ..., 07=up, FF=no movement)   
                {
                    case 0:
                        data.UnitPrevXloc[i] = data.UnitXloc[i] - 1;
                        data.UnitPrevYloc[i] = data.UnitYloc[i] + 1;
                        break;
                    case 1:
                        data.UnitPrevXloc[i] = data.UnitXloc[i] - 2;
                        data.UnitPrevYloc[i] = data.UnitYloc[i];
                        break;
                    case 2:
                        data.UnitPrevXloc[i] = data.UnitXloc[i] - 1;
                        data.UnitPrevYloc[i] = data.UnitYloc[i] - 1;
                        break;
                    case 3:
                        data.UnitPrevXloc[i] = data.UnitXloc[i];
                        data.UnitPrevYloc[i] = data.UnitYloc[i] - 2;
                        break;
                    case 4:
                        data.UnitPrevXloc[i] = data.UnitXloc[i] + 1;
                        data.UnitPrevYloc[i] = data.UnitYloc[i] - 1;
                        break;
                    case 5:
                        data.UnitPrevXloc[i] = data.UnitXloc[i] + 2;
                        data.UnitPrevYloc[i] = data.UnitYloc[i];
                        break;
                    case 6:
                        data.UnitPrevXloc[i] = data.UnitXloc[i] + 1;
                        data.UnitPrevYloc[i] = data.UnitYloc[i] + 1;
                        break;
                    case 7:
                        data.UnitPrevXloc[i] = data.UnitXloc[i];
                        data.UnitPrevYloc[i] = data.UnitYloc[i] + 2;
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

                // Unit go-to X
                intVal1 = bytes[ofsetU + multipl * i + 18];
                intVal2 = bytes[ofsetU + multipl * i + 19];
                data.UnitGotoX[i] = short.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                // Unit go-to Y
                intVal1 = bytes[ofsetU + multipl * i + 20];
                intVal2 = bytes[ofsetU + multipl * i + 21];
                data.UnitGotoY[i] = short.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                // Unit link to other units on top of it
                intVal1 = bytes[ofsetU + multipl * i + 22];
                intVal2 = bytes[ofsetU + multipl * i + 23];
                data.UnitLinkOtherUnitsOnTop[i] = short.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                // Unit link to other units under it
                intVal1 = bytes[ofsetU + multipl * i + 24];
                intVal2 = bytes[ofsetU + multipl * i + 25];
                data.UnitLinkOtherUnitsUnder[i] = short.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);
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

            char[] asciichar = new char[15];
            data.CityXloc = new int[data.NumberOfCities];
            data.CityYloc = new int[data.NumberOfCities];
            data.CityCanBuildCoastal = new bool[data.NumberOfCities];
            data.CityAutobuildMilitaryRule = new bool[data.NumberOfCities];
            data.CityStolenTech = new bool[data.NumberOfCities];
            data.CityImprovementSold = new bool[data.NumberOfCities];
            data.CityWeLoveKingDay = new bool[data.NumberOfCities];
            data.CityCivilDisorder = new bool[data.NumberOfCities];
            data.CityCanBuildShips = new bool[data.NumberOfCities];
            data.CityObjectivex3 = new bool[data.NumberOfCities];
            data.CityObjectivex1 = new bool[data.NumberOfCities];
            data.CityOwner = new int[data.NumberOfCities];
            data.CitySize = new int[data.NumberOfCities];
            data.CityWhoBuiltIt = new int[data.NumberOfCities];
            data.CityFoodInStorage = new int[data.NumberOfCities];
            data.CityShieldsProgress = new int[data.NumberOfCities];
            data.CityNetTrade = new int[data.NumberOfCities];
            data.CityName = new string[data.NumberOfCities];
            data.CityDistributionWorkers = new bool[data.NumberOfCities][];
            data.CityNoOfSpecialistsx4 = new int[data.NumberOfCities];
            data.CityImprovements = new bool[data.NumberOfCities][];
            data.CityItemInProduction = new int[data.NumberOfCities];
            data.CityActiveTradeRoutes = new int[data.NumberOfCities];
            data.CityCommoditySupplied = new CommodityType[data.NumberOfCities][];
            data.CityCommodityDemanded = new CommodityType[data.NumberOfCities][];
            data.CityCommodityInRoute = new CommodityType[data.NumberOfCities][];
            data.CityTradeRoutePartnerCity = new int[data.NumberOfCities][];
            data.CityScience = new int[data.NumberOfCities];
            data.CityTax = new int[data.NumberOfCities];
            data.CityNoOfTradeIcons = new int[data.NumberOfCities];
            data.CityTotalFoodProduction = new int[data.NumberOfCities];
            data.CityTotalShieldProduction = new int[data.NumberOfCities];
            data.CityHappyCitizens = new int[data.NumberOfCities];
            data.CityUnhappyCitizens = new int[data.NumberOfCities];
            for (int i = 0; i < data.NumberOfCities; i++)
            {
                // City X location
                intVal1 = bytes[ofsetC + multipl * i + 0];
                intVal2 = bytes[ofsetC + multipl * i + 1];
                data.CityXloc[i] = short.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                // City Y location
                intVal1 = bytes[ofsetC + multipl * i + 2];
                intVal2 = bytes[ofsetC + multipl * i + 3];
                data.CityYloc[i] = short.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                data.CityCanBuildCoastal[i] = GetBit(bytes[ofsetC + multipl * i + 4], 0);    // Can build coastal improvements
                data.CityAutobuildMilitaryRule[i] = GetBit(bytes[ofsetC + multipl * i + 4], 3);    // Auto build under military rule
                data.CityStolenTech[i] = GetBit(bytes[ofsetC + multipl * i + 4], 4);         // Stolen tech
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

                // Food in storage
                intVal1 = bytes[ofsetC + multipl * i + 26];
                intVal2 = bytes[ofsetC + multipl * i + 27];
                data.CityFoodInStorage[i] = short.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                // Shield progress
                intVal1 = bytes[ofsetC + multipl * i + 28];
                intVal2 = bytes[ofsetC + multipl * i + 29];
                data.CityShieldsProgress[i] = short.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                // Net trade
                intVal1 = bytes[ofsetC + multipl * i + 30];
                intVal2 = bytes[ofsetC + multipl * i + 31];
                data.CityNetTrade[i] = short.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                // Name        
                for (int j = 0; j < 15; j++) asciichar[j] = Convert.ToChar(bytes[ofsetC + multipl * i + j + 32]);
                string cityName = new string(asciichar);
                data.CityName[i] = cityName.Replace("\0", string.Empty);

                // Distribution of workers on map in city view
                string cityWorkDistr1 = Convert.ToString(bytes[ofsetC + multipl * i + 48], 2).PadLeft(8, '0');  // inner circle (starting from N, going in counter-clokwise direction)                
                string cityWorkDistr2 = Convert.ToString(bytes[ofsetC + multipl * i + 49], 2).PadLeft(8, '0');  // on 8 of the outer circle    
                string cityWorkDistr3 = Convert.ToString(bytes[ofsetC + multipl * i + 50], 2).PadLeft(5, '0');  // on 4 of the outer circle
                string _cityDistributionWorkers = string.Format("{0}{1}{2}", cityWorkDistr3, cityWorkDistr2, cityWorkDistr1);
                data.CityDistributionWorkers[i] = new bool[21];
                for (int distNo = 0; distNo < 21; distNo++)
                    data.CityDistributionWorkers[i][distNo] = _cityDistributionWorkers[distNo] == '1';

                data.CityNoOfSpecialistsx4[i] = bytes[ofsetC + multipl * i + 51];   // Number of specialists x4

                // Improvements
                string cityImprovements1 = Convert.ToString(bytes[ofsetC + multipl * i + 52], 2).PadLeft(8, '0');   // bit6=palace (1st improvement), bit7=not important
                cityImprovements1 = cityImprovements1.Remove(cityImprovements1.Length - 1);                         // remove last bit, it is not important
                string cityImprovements2 = Convert.ToString(bytes[ofsetC + multipl * i + 53], 2).PadLeft(8, '0');
                string cityImprovements3 = Convert.ToString(bytes[ofsetC + multipl * i + 54], 2).PadLeft(8, '0');
                string cityImprovements4 = Convert.ToString(bytes[ofsetC + multipl * i + 55], 2).PadLeft(8, '0');
                string cityImprovements5 = Convert.ToString(bytes[ofsetC + multipl * i + 56], 2).PadLeft(8, '0');   // bit0-bit4=not important, bit5=port facility (last improvement)
                // Put all improvements into one large string, where bit0=palace, bit1=barracks, ..., bit33=port facility
                // First reverse bit order in all strings
                cityImprovements1 = Reverse(cityImprovements1);
                cityImprovements2 = Reverse(cityImprovements2);
                cityImprovements3 = Reverse(cityImprovements3);
                cityImprovements4 = Reverse(cityImprovements4);
                cityImprovements5 = Reverse(cityImprovements5);
                cityImprovements5 = cityImprovements5.Remove(cityImprovements5.Length - 5); // remove last 5 bits, they are not important
                // Merge all strings into a large string
                string cityImprovements_ = string.Format("{0}{1}{2}{3}{4}", cityImprovements1, cityImprovements2, cityImprovements3, cityImprovements4, cityImprovements5);
                // Convert string array to bool array
                data.CityImprovements[i] = new bool[34];
                for (int impNo = 0; impNo < 34; impNo++)
                    data.CityImprovements[i][impNo] = cityImprovements_[impNo] == '1';

                // Item in production
                // 0(dec)/0(hex) ... 61(dec)/3D(hex) are units, improvements are inversed (FF(hex)=1st, FE(hex)=2nd, ...)
                // convert this notation of improvements, so that 62(dec) is 1st improvement, 63(dec) is 2nd, ...
                data.CityItemInProduction[i] = bytes[ofsetC + multipl * i + 57];
                if (data.CityItemInProduction[i] > 70)  //if it is improvement
                    data.CityItemInProduction[i] = 255 - data.CityItemInProduction[i] + 62; // 62 because 0...61 are units

                data.CityActiveTradeRoutes[i] = bytes[ofsetC + multipl * i + 58];   // No of active trade routes

                // 1st, 2nd, 3rd trade commodities supplied
                data.CityCommoditySupplied[i] = new CommodityType[] { (CommodityType)bytes[ofsetC + multipl * i + 59], (CommodityType)bytes[ofsetC + multipl * i + 60], (CommodityType)bytes[ofsetC + multipl * i + 61] };

                // 1st, 2nd, 3rd trade commodities demanded
                data.CityCommodityDemanded[i] = new CommodityType[] { (CommodityType)bytes[ofsetC + multipl * i + 62], (CommodityType)bytes[ofsetC + multipl * i + 63], (CommodityType)bytes[ofsetC + multipl * i + 64] };

                // 1st, 2nd, 3rd trade commodities in route
                data.CityCommodityInRoute[i] = new CommodityType[] { (CommodityType)bytes[ofsetC + multipl * i + 65], (CommodityType)bytes[ofsetC + multipl * i + 66], (CommodityType)bytes[ofsetC + multipl * i + 67] };

                // 1st, 2nd, 3rd trade route partner city number
                data.CityTradeRoutePartnerCity[i] = new int[] { bytes[ofsetC + multipl * i + 68], bytes[ofsetC + multipl * i + 69], bytes[ofsetC + multipl * i + 70] };

                // Science
                intVal1 = bytes[ofsetC + multipl * i + 74];
                intVal2 = bytes[ofsetC + multipl * i + 75];
                data.CityScience[i] = short.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                // Tax
                intVal1 = bytes[ofsetC + multipl * i + 76];
                intVal2 = bytes[ofsetC + multipl * i + 77];
                data.CityTax[i] = short.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

                // No of trade icons
                intVal1 = bytes[ofsetC + multipl * i + 78];
                intVal2 = bytes[ofsetC + multipl * i + 79];
                data.CityNoOfTradeIcons[i] = short.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

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
            intVal1 = bytes[ofsetO + 63];
            intVal2 = bytes[ofsetO + 64];
            intVal3 = bytes[ofsetO + 65];
            intVal4 = bytes[ofsetO + 66];
            data.ActiveCursorXY = new int[] { short.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber),
                                              short.Parse(string.Concat(intVal4.ToString("X"), intVal3.ToString("X")), System.Globalization.NumberStyles.HexNumber) };

            // Clicked tile with your mouse XY position (does not count if you clicked on a city)
            intVal1 = bytes[ofsetO + 1425];
            intVal2 = bytes[ofsetO + 1426];
            intVal3 = bytes[ofsetO + 1427];
            intVal4 = bytes[ofsetO + 1428];
            data.ClickedXY = new int[] { short.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber),
                                         short.Parse(string.Concat(intVal4.ToString("X"), intVal3.ToString("X")), System.Globalization.NumberStyles.HexNumber) };

            // Zoom (=-7(min)...+8(max), 0=std.)
            intVal1 = bytes[ofsetO + 1429];
            intVal2 = bytes[ofsetO + 1430];
            data.Zoom = short.Parse(string.Concat(intVal2.ToString("X"), intVal1.ToString("X")), System.Globalization.NumberStyles.HexNumber);

            #endregion
        }

        // Helper function
        private bool GetBit(byte b, int bitNumber)
        {
            return (b & (1 << bitNumber)) != 0;
        }

        // Helper function
        private string Reverse(string s)   //Reverse a string
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}
