using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Civ2engine.OriginalSaves
{
    // Read game data from SAV and RULES.txt
    public class Read
    {
        
        static string[] scnNames = new []{"Original","SciFi","Fantasy"};
        // READ SAV GAME
        public static GameData ReadSavFile(Byte[] bytes)
        {
            var data = new GameData();
          
            #region Game version
            //=========================
            //GAME VERSION
            //=========================
            data.GameVersion = bytes[10];
            #endregion
            #region Unit transport settings (TOT)
            //=========================
            //UNIT TRANSPORT SETTINGS
            //=========================
            if (data.GameVersion > 44)  // TOT only
            {
                data.UnitTransportRelationship = new short[80];
                data.UnitTransportBuildTransportSiteMask = new short[80];
                data.UnitTransportUseTransportSiteMask = new short[80];
                data.UnitTransportNativeTransportAbilityMask = new short[80];
                for (int unitId = 0; unitId < 80; unitId++) // 80=max units
                {
                    data.UnitTransportRelationship[unitId] = BitConverter.ToInt16(bytes, 12 + 8 * unitId + 0);
                    data.UnitTransportBuildTransportSiteMask[unitId] = BitConverter.ToInt16(bytes, 12 + 8 * unitId + 2);
                    data.UnitTransportUseTransportSiteMask[unitId] = BitConverter.ToInt16(bytes, 12 + 8 * unitId + 4);
                    data.UnitTransportNativeTransportAbilityMask[unitId] = BitConverter.ToInt16(bytes, 12 + 8 * unitId + 6);
                }
            }
            #endregion
            #region Options
            //=========================
            //GAME OPTIONS
            //=========================
            int optionsOffset;
            if (data.GameVersion <= 44) optionsOffset = 12;
            else optionsOffset = 652;    // TOT

            data.OptionsArray = new bool[38];
            data.OptionsArray[0] = !GetBit(bytes[optionsOffset + 0], 4);    // Simplified combat on/off 
            data.OptionsArray[1] = GetBit(bytes[optionsOffset + 0], 7);     // Bloodlust on/off            
            data.OptionsArray[2] = GetBit(bytes[optionsOffset + 1], 0);     // Don't restart if eliminated
            data.OptionsArray[3] = GetBit(bytes[optionsOffset + 1], 7);     // Flat earth
            data.OptionsArray[4] = GetBit(bytes[optionsOffset + 2], 3);     // Music on/off
            data.OptionsArray[5] = GetBit(bytes[optionsOffset + 2], 4);     // Sound effects on/off
            data.OptionsArray[6] = GetBit(bytes[optionsOffset + 2], 5);     // Grid on/off
            data.OptionsArray[7] = GetBit(bytes[optionsOffset + 2], 6);     // Enter closes city screen
            data.OptionsArray[8] = GetBit(bytes[optionsOffset + 2], 7);     // Move units without mouse
            data.OptionsArray[9] = GetBit(bytes[optionsOffset + 3], 0);     // Tutorial help on/off
            data.OptionsArray[10] = GetBit(bytes[optionsOffset + 3], 1);    // Instant advice on/off
            data.OptionsArray[11] = GetBit(bytes[optionsOffset + 3], 2);    // Fast piece slide on/off
            data.OptionsArray[12] = GetBit(bytes[optionsOffset + 3], 3);    // No pause after enemy moves on/off
            data.OptionsArray[13] = GetBit(bytes[optionsOffset + 3], 4);    // Show enemy moves on/off
            data.OptionsArray[14] = GetBit(bytes[optionsOffset + 3], 5);    // Autosave each turn on/off
            data.OptionsArray[15] = GetBit(bytes[optionsOffset + 3], 6);    // Always wait at end of turn on/off
            data.OptionsArray[16] = GetBit(bytes[optionsOffset + 3], 7);    // Cheat menu on/off
            data.OptionsArray[17] = GetBit(bytes[optionsOffset + 4], 0);    // Wonder movies on/off (for <= MGE)
            data.OptionsArray[18] = GetBit(bytes[optionsOffset + 4], 1);    // Throne room graphics on/off (for <= MGE)
            data.OptionsArray[19] = GetBit(bytes[optionsOffset + 4], 2);    // Diplomacy screen graphics on/off
            data.OptionsArray[20] = GetBit(bytes[optionsOffset + 4], 3);    // Civilopedia for advances on/off
            data.OptionsArray[21] = GetBit(bytes[optionsOffset + 4], 4);    // High council on/off (for <= MGE)
            data.OptionsArray[22] = GetBit(bytes[optionsOffset + 4], 5);    // Animated heralds on/off (for <= MGE)
            data.OptionsArray[23] = GetBit(bytes[optionsOffset + 8], 4);    // Cheat penalty/warning disabled?
            data.OptionsArray[24] = GetBit(bytes[optionsOffset + 8], 5);    // Scoring complete?
            data.OptionsArray[25] = GetBit(bytes[optionsOffset + 8], 6);    // Scenario (.scn) file?
            data.OptionsArray[26] = GetBit(bytes[optionsOffset + 8], 7);    // Scenario flag toggled?
            data.OptionsArray[27] = !GetBit(bytes[optionsOffset + 10], 0);  // Warn when city growth halted on/off
            data.OptionsArray[28] = !GetBit(bytes[optionsOffset + 10], 1);  // Show city improvements built on/off
            data.OptionsArray[29] = !GetBit(bytes[optionsOffset + 10], 2);  // Show non combat units build on/off
            data.OptionsArray[30] = !GetBit(bytes[optionsOffset + 10], 3);  // Show invalid build instructions on/off
            data.OptionsArray[31] = !GetBit(bytes[optionsOffset + 10], 4);  // Announce cities in disorder on/off
            data.OptionsArray[32] = !GetBit(bytes[optionsOffset + 10], 5);  // Announce order restored in cities on/off
            data.OptionsArray[33] = !GetBit(bytes[optionsOffset + 10], 6);  // Announce we love king day on/off
            data.OptionsArray[34] = !GetBit(bytes[optionsOffset + 10], 7);  // Warn when food dangerously low on/off
            data.OptionsArray[35] = !GetBit(bytes[optionsOffset + 11], 0);  // Warn when pollution occurs on/off
            data.OptionsArray[36] = !GetBit(bytes[optionsOffset + 11], 1);  // Warn when changing production will cost shileds on/off
            data.OptionsArray[37] = !GetBit(bytes[optionsOffset + 11], 2);  // Zoom to city not default action on/off
            #endregion
            #region Parameters
            //=========================
            //GAME PARAMETERS
            //=========================
            int paramsOffset = data.GameVersion <= 44 ? 24 : 664;
            
            data.FirstAirUnitBuilt = GetBit(bytes[paramsOffset + 0], 1);
            data.FirstNavalUnitBuilt = GetBit(bytes[paramsOffset + 0], 2);
            data.FirstCaravanBuilt = GetBit(bytes[paramsOffset + 0], 4);
            data.WasRepublicDemocracyAdopted = GetBit(bytes[paramsOffset + 0], 5);
            data.FirstSignificantlyDamagedUnit = GetBit(bytes[paramsOffset + 0], 6) && GetBit(bytes[paramsOffset + 0], 7);
            data.TurnNumber = BitConverter.ToInt16(bytes, paramsOffset + 4);
            data.TurnNumberForGameYear = BitConverter.ToInt16(bytes, paramsOffset + 6);
            data.SelectedUnitIndex = BitConverter.ToInt16(bytes, paramsOffset + 10);   // Unit selected at start of game (-1 if no unit)
            data.PlayersCivIndex = bytes[paramsOffset + 15];   // Human player
            data.WhichCivsMapShown = bytes[paramsOffset + 16];
            data.PlayersCivilizationNumberUsed = bytes[paramsOffset + 17]; // Players civ number used
            data.MapRevealed = bytes[paramsOffset + 19] == 1;
            data.DifficultyLevel = bytes[paramsOffset + 20];
            data.BarbarianActivity = bytes[paramsOffset + 21];

            data.CivsInPlay = new bool[8];
            for (int i = 0; i < 8; i++)
                data.CivsInPlay[i] = GetBit(bytes[paramsOffset + 22], i);

            // Civs with human player playing (multiplayer)
            data.HumanPlayers = new bool[8];
            for (int i = 0; i < 8; i++)
                data.HumanPlayers[i] = GetBit(bytes[paramsOffset + 23], i);

            data.GlobalTempRiseOccured = bytes[paramsOffset + 27];
            data.NoPollutionSkulls = BitConverter.ToInt16(bytes, paramsOffset + 30);
            data.NoOfTurnsOfPeace = bytes[paramsOffset + 32];
            data.NumberOfUnits = BitConverter.ToInt16(bytes, paramsOffset + 34);
            data.NumberOfCities = BitConverter.ToInt16(bytes, paramsOffset + 36);

            #endregion
            #region Technologies
            //=========================
            //TECHNOLOGIES
            //=========================
            int techsOffset = data.GameVersion <= 44 ? 66 : 706;

            int techs = data.GameVersion <= 39 ? 93 : 100;  // total no of techs from rules.txt (CiC has less)

            data.CivFirstDiscoveredTech = new byte[techs];
            for (int techNo = 0; techNo < techs; techNo++)
                data.CivFirstDiscoveredTech[techNo] = bytes[techsOffset + techNo];

            data.CivsDiscoveredTechs = new bool[techs, 8];
            for (int techNo = 0; techNo < techs; techNo++)
                for (int civId = 0; civId < 8; civId++)
                    data.CivsDiscoveredTechs[techNo, civId] = GetBit(bytes[techsOffset + techs + techNo], civId);

            #endregion
            #region Wonders
            //=========================
            //WONDERS
            //=========================
            int offsetW;
            if (data.GameVersion <= 39) offsetW = 252;  // <= CiC
            else if (data.GameVersion <= 44) offsetW = 266;  // FW, MGE
            else offsetW = 906;  // TOT

            data.WonderCity = new short[28];      // city Id with wonder
            data.WonderBuilt = new bool[28];      // has the wonder been built?
            data.WonderDestroyed = new bool[28];  // has the wonder been destroyed?
            for (int i = 0; i < 28; i++)
            {
                data.WonderCity[i] = BitConverter.ToInt16(bytes, offsetW + 2 * i);

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
            #region Unknown block
            //=========================
            //Unknown block
            //=========================
            int offsetUb = 962;
            if (data.GameVersion > 44)  // TOT
            {
                data.ExtendedMetadata.Add("TOT-Scenario", scnNames[bytes[offsetUb + 20]]);   // Original, fantasy, Scifi

                // Pollution data (same as before so no use in reading it)
                // ...
            }
            #endregion
            #region Civ names
            //=========================
            //Civ names (without barbarians)
            //=========================
            int offsetN = data.GameVersion <= 44 ? offsetW + 318 : 1250;

            data.CivCityStyle = new byte[8];
            data.CivLeaderName = new string[8];
            data.CivTribeName = new string[8];
            data.CivAdjective = new string[8];
            data.CivAnarchyTitle = new string[8];
            data.CivDespotismTitle = new string[8];
            data.CivMonarchyTitle = new string[8];
            data.CivCommunismTitle = new string[8];
            data.CivFundamentalismTitle = new string[8];
            data.CivRepublicTitle = new string[8];
            data.CivDemocracyTitle = new string[8];
            // Manually add data for barbarians
            data.CivCityStyle[0] = 0;
            data.CivLeaderName[0] = "NULL";
            data.CivTribeName[0] = "Barbarians";
            data.CivAdjective[0] = "Barbarian";
            // Add data for other 7 civs
            for (int i = 0; i < 7; i++)
            {
                data.CivCityStyle[i + 1] = bytes[offsetN + 242 * i];
                // Various names (if empty, get the names from RULES.TXT)
                data.CivLeaderName[i + 1] = ReadString(bytes, 24, offsetN + 242 * i + 2);
                data.CivTribeName[i + 1] = ReadString(bytes, 24, offsetN + 242 * i + 26);
                data.CivAdjective[i + 1] = ReadString(bytes, 24, offsetN + 242 * i + 50);
                data.CivAnarchyTitle[i + 1] = ReadString(bytes, 24, offsetN + 242 * i + 74);
                data.CivDespotismTitle[i + 1] = ReadString(bytes, 24, offsetN + 242 * i + 98);
                data.CivMonarchyTitle[i + 1] = ReadString(bytes, 24, offsetN + 242 * i + 122);
                data.CivCommunismTitle[i + 1] = ReadString(bytes, 24, offsetN + 242 * i + 146);
                data.CivFundamentalismTitle[i + 1] = ReadString(bytes, 24, offsetN + 242 * i + 170);
                data.CivRepublicTitle[i + 1] = ReadString(bytes, 24, offsetN + 242 * i + 194);
                data.CivDemocracyTitle[i + 1] = ReadString(bytes, 24, offsetN + 242 * i + 218);
            }
            #endregion
            #region Civ tech, money, etc.
            //=========================
            //Civ tech, money (with barbarians)
            //=========================
            int offsetT, sizeT;
            if (data.GameVersion <= 39) // <= CiC
            {
                offsetT = 2264;
                sizeT = 1396;
            }
            else if (data.GameVersion <= 44) // <= MGE
            {
                offsetT = 2278;
                sizeT = 1428;
            }
            else    // TOT
            {
                offsetT = 2945;
                sizeT = 3348;
            }

            data.RulerGender = new byte[8];
            data.CivMoney = new short[8];
            data.CivNumber = new byte[8];
            data.CivResearchProgress = new short[8];
            data.CivResearchingAdvance = new byte[8];
            data.CivNumberAdvancesResearched = new byte[8];
            data.CivNumberFutureTechsResearched = new byte[8];
            data.CivSciRate = new byte[8];
            data.CivTaxRate = new byte[8];
            data.CivGovernment = new byte[8];
            data.CivReputation = new byte[8];
            data.CivPatience = new byte[8];
            data.CivTreatyContact = new bool[8][];
            data.CivTreatyCeaseFire = new bool[8][];
            data.CivTreatyPeace = new bool[8][];
            data.CivTreatyAlliance = new bool[8][];
            data.CivTreatyVendetta = new bool[8][];
            data.CivTreatyEmbassy = new bool[8][];
            data.CivTreatyWar = new bool[8][];
            data.CivAttitudes = new int[8][];
            data.CivAdvances = new bool[8][];
            data.CivNumberMilitaryUnits = new short[8];
            data.CivNumberCities = new short[8];
            data.CivSumCitySizes = new short[8];
            data.CivActiveUnitsPerType = new byte[8][];
            data.CivCasualtiesPerType = new byte[8][];
            data.CivUnitsInProductionPerType = new byte[8][];
            data.CivLastContact = new int[8][];
            data.CivHasSpaceship = new bool[8];
            data.CivSpaceshipEstimatedArrival = new short[8];
            data.CivSpaceshipLaunchYear = new short[8];
            data.CivSpaceshipStructural = new short[8];
            data.CivSpaceshipComponentsPropulsion = new short[8];
            data.CivSpaceshipComponentsFuel = new short[8];
            data.CivSpaceshipModulesHabitation = new short[8];
            data.CivSpaceshipModulesLifeSupport = new short[8];
            data.CivSpaceshipModulesSolarPanel = new short[8];
            int offsetExtra;
            for (int civId = 0; civId < 8; civId++) // for each civ including barbarians
            {
                // Define offsets
                if (data.GameVersion <= 39) offsetExtra = 0;    // CiC
                else if (data.GameVersion <= 44) offsetExtra = 0;    // FW, MGE
                else offsetExtra = 7;  // TOT

                data.RulerGender[civId] = bytes[offsetT + sizeT * civId + 1]; // 0=male, 2=female
                data.CivMoney[civId] = BitConverter.ToInt16(bytes, offsetT + sizeT * civId + offsetExtra + 2);
                data.CivNumber[civId] = bytes[offsetT + sizeT * civId + offsetExtra + 6]; // Civ number as per @Leaders table in RULES.TXT
                data.CivResearchProgress[civId] = BitConverter.ToInt16(bytes, offsetT + sizeT * civId + offsetExtra + 8);
                data.CivResearchingAdvance[civId] = bytes[offsetT + sizeT * civId + offsetExtra + 10]; // Advance currently being researched (FF(hex) = no goal)
                data.CivNumberAdvancesResearched[civId] = bytes[offsetT + sizeT * civId + offsetExtra + 16];
                data.CivNumberFutureTechsResearched[civId] = bytes[offsetT + sizeT * civId + offsetExtra + 17];
                data.CivSciRate[civId] = bytes[offsetT + sizeT * civId + offsetExtra + 19]; // (%/10)
                data.CivTaxRate[civId] = bytes[offsetT + sizeT * civId + offsetExtra + 20]; // (%/10)
                data.CivGovernment[civId] = bytes[offsetT + sizeT * civId + offsetExtra + 21]; // 0=anarchy, ...
                data.CivReputation[civId] = bytes[offsetT + sizeT * civId + offsetExtra + 30];
                data.CivPatience[civId] = bytes[offsetT + sizeT * civId + offsetExtra + 31];

                // Treaties
                data.CivTreatyContact[civId] = new bool[8];
                data.CivTreatyCeaseFire[civId] = new bool[8];
                data.CivTreatyPeace[civId] = new bool[8];
                data.CivTreatyAlliance[civId] = new bool[8];
                data.CivTreatyVendetta[civId] = new bool[8];
                data.CivTreatyEmbassy[civId] = new bool[8];
                data.CivTreatyWar[civId] = new bool[8];
                for (int civ2Id = 0; civ2Id < 8; civ2Id++)
                {
                    data.CivTreatyContact[civId][civ2Id] = GetBit(bytes[offsetT + sizeT * civId + offsetExtra + 32 + 4 * civ2Id + 0], 0);
                    data.CivTreatyCeaseFire[civId][civ2Id] = GetBit(bytes[offsetT + sizeT * civId + offsetExtra + 32 + 4 * civ2Id + 0], 1);
                    data.CivTreatyPeace[civId][civ2Id] = GetBit(bytes[offsetT + sizeT * civId + offsetExtra + 32 + 4 * civ2Id + 0], 2);
                    data.CivTreatyAlliance[civId][civ2Id] = GetBit(bytes[offsetT + sizeT * civId + offsetExtra + 32 + 4 * civ2Id + 0], 3);
                    data.CivTreatyVendetta[civId][civ2Id] = GetBit(bytes[offsetT + sizeT * civId + offsetExtra + 32 + 4 * civ2Id + 0], 4);
                    data.CivTreatyEmbassy[civId][civ2Id] = GetBit(bytes[offsetT + sizeT * civId + offsetExtra + 32 + 4 * civ2Id + 0], 7);
                    data.CivTreatyWar[civId][civ2Id] = GetBit(bytes[offsetT + sizeT * civId + offsetExtra + 32 + 4 * civ2Id + 1], 5);
                }

                // Attitudes
                data.CivAttitudes[civId] = new int[8];
                for (int civ2Id = 0; civ2Id < 8; civ2Id++)
                    data.CivAttitudes[civId][civ2Id] = bytes[offsetT + sizeT * civId + offsetExtra + 64 + civ2Id];

                // Technologies
                int noTechs = data.GameVersion <= 39 ? 93 : 100;    // CiC has fewer techs in rules.txt
                data.CivAdvances[civId] = new bool[noTechs];
                for (int block = 0; block < 13; block++)
                {
                    for (int bit = 0; bit < 8; bit++)
                    {
                        if (8 * block + bit >= noTechs - 1) break;

                        data.CivAdvances[civId][block * 8 + bit] = GetBit(bytes[offsetT + sizeT * civId + offsetExtra + 88 + block], bit);
                    }
                }

                data.CivNumberMilitaryUnits[civId] = BitConverter.ToInt16(bytes, offsetT + sizeT * civId + offsetExtra + 102);
                data.CivNumberCities[civId] = BitConverter.ToInt16(bytes, offsetT + sizeT * civId + offsetExtra + 104);
                data.CivSumCitySizes[civId] = BitConverter.ToInt16(bytes, offsetT + sizeT * civId + offsetExtra + 108);

                // No. of units per type (from Rules.txt)
                int noUnitsType;
                if (data.GameVersion <= 39) noUnitsType = 54;    // CiC
                else if (data.GameVersion <= 44) noUnitsType = 62;    // FW, MGE
                else noUnitsType = 80;  // TOT

                data.CivActiveUnitsPerType[civId] = new byte[noUnitsType];
                data.CivCasualtiesPerType[civId] = new byte[noUnitsType];
                data.CivUnitsInProductionPerType[civId] = new byte[noUnitsType];
                for (int type = 0; type < noUnitsType; type++)
                {
                    data.CivActiveUnitsPerType[civId][type] = bytes[offsetT + sizeT * civId + offsetExtra + 216 + type];
                    data.CivCasualtiesPerType[civId][type] = bytes[offsetT + sizeT * civId + offsetExtra + 216 + noUnitsType + type];
                    data.CivUnitsInProductionPerType[civId][type] = bytes[offsetT + sizeT * civId + offsetExtra + 216 + 2 * noUnitsType + type];
                }

                // Correct offsets
                if (data.GameVersion <= 39) offsetExtra = 216 + 3 * noUnitsType + 592;    // CiC
                else if (data.GameVersion <= 44) offsetExtra = 216 + 3 * noUnitsType + 592;    // FW, MGE
                else offsetExtra = 223 + 3 * noUnitsType + 2320;  // TOT

                // Last contact with other civs
                data.CivLastContact[civId] = new int[8];
                for (int civ2Id = 0; civ2Id < 8; civ2Id++)
                    data.CivLastContact[civId][civ2Id] = BitConverter.ToInt16(bytes, offsetT + sizeT * civId + offsetExtra + 2 * civ2Id);

                // Spaceships
                data.CivHasSpaceship[civId] = GetBit(bytes[offsetT + sizeT * civId + offsetExtra + 30], 0);
                data.CivSpaceshipEstimatedArrival[civId] = BitConverter.ToInt16(bytes, offsetT + sizeT * civId + offsetExtra + 32);
                data.CivSpaceshipLaunchYear[civId] = BitConverter.ToInt16(bytes, offsetT + sizeT * civId + offsetExtra + 34);
                data.CivSpaceshipStructural[civId] = BitConverter.ToInt16(bytes, offsetT + sizeT * civId + offsetExtra + 38);
                data.CivSpaceshipComponentsPropulsion[civId] = BitConverter.ToInt16(bytes, offsetT + sizeT * civId + offsetExtra + 40);
                data.CivSpaceshipComponentsFuel[civId] = BitConverter.ToInt16(bytes, offsetT + sizeT * civId + offsetExtra + 42);
                data.CivSpaceshipModulesHabitation[civId] = BitConverter.ToInt16(bytes, offsetT + sizeT * civId + offsetExtra + 44);
                data.CivSpaceshipModulesLifeSupport[civId] = BitConverter.ToInt16(bytes, offsetT + sizeT * civId + offsetExtra + 46);
                data.CivSpaceshipModulesSolarPanel[civId] = BitConverter.ToInt16(bytes, offsetT + sizeT * civId + offsetExtra + 48);
            }
            #endregion
            #region Civs relations to advances groups (TOT only)
            //=========================
            //Relations to groups (from @LEADERS2 in rules.txt)
            // 0=can research, can own
            // 1=can’t research, can own
            // 2=can’t research, can’t own
            //=========================
            if (data.GameVersion > 44)
            {
                int offsetGrps = 29728;

                data.CivsRelationsToAdvancesGroups = new byte[21][];
                for (int civId = 0; civId < 21; civId++)
                {
                    data.CivsRelationsToAdvancesGroups[civId] = new byte[8];
                    for (int group = 0; group < 8; group++)
                        data.CivsRelationsToAdvancesGroups[civId][group] = bytes[offsetGrps + 8 * civId + group];
                }
            }
            #endregion
            #region Transporters (TOT only)
            //=========================
            //Transporters
            //=========================
            if (data.GameVersion > 44)
            {
                int offsetTr = 29896;

                data.NoTransporters = BitConverter.ToInt16(bytes, offsetTr + 0);
                data.Transporter1X = new short[data.NoTransporters];
                data.Transporter1Y = new short[data.NoTransporters];
                data.Transporter1MapNo = new byte[data.NoTransporters];
                data.Transporter2X = new short[data.NoTransporters];
                data.Transporter2Y = new short[data.NoTransporters];
                data.Transporter2MapNo = new byte[data.NoTransporters];
                data.TransporterLook = new byte[data.NoTransporters];
                for (int transpId = 0; transpId < data.NoTransporters; transpId++)
                {
                    data.Transporter1X[transpId] = BitConverter.ToInt16(bytes, offsetTr + 4 + 14 * transpId + 0);
                    data.Transporter1Y[transpId] = BitConverter.ToInt16(bytes, offsetTr + 4 + 14 * transpId + 2);
                    data.Transporter1MapNo[transpId] = bytes[offsetTr + 4 + 14 * transpId + 4];
                    data.Transporter2X[transpId] = BitConverter.ToInt16(bytes, offsetTr + 4 + 14 * transpId + 6);
                    data.Transporter2Y[transpId] = BitConverter.ToInt16(bytes, offsetTr + 4 + 14 * transpId + 8);
                    data.Transporter2MapNo[transpId] = bytes[offsetTr + 4 + 14 * transpId + 10];
                    data.TransporterLook[transpId] = bytes[offsetTr + 4 + 14 * transpId + 11];
                }
            }
            #endregion
            #region Map
            //=========================
            //MAP
            //=========================
            int offsetM;
            if (data.GameVersion <= 39) offsetM = 13432;    // <= CiC
            else if (data.GameVersion <= 44) offsetM = 13702;   // <= MGE
            else offsetM = 29900 + 14 * data.NoTransporters; // TOT

            data.MapXdimX2 = BitConverter.ToInt16(bytes, offsetM + 0);  // Map X dimension x2
            data.MapYdim = BitConverter.ToInt16(bytes, offsetM + 2);
            data.MapArea = BitConverter.ToInt16(bytes, offsetM + 4);  // Xdim*Ydim/2
            //flatEarth = BitConverter.ToInt16(bytes, offsetM + 6);   // Flat Earth flag (info already given before!!)
            data.MapResourceSeed = BitConverter.ToInt16(bytes, offsetM + 8);    // not used in game?
            data.MapLocatorXdim = BitConverter.ToInt16(bytes, offsetM + 10);  // Minimap width (=MapXdim/2 rounded up), important for getting offset of unit block!!
            data.MapLocatorYdim = BitConverter.ToInt16(bytes, offsetM + 12);  // Minimap height (=MapYdim/4 rounded up), important for getting offset of unit block!!
            data.MapNoSecondaryMaps = data.GameVersion > 44 ? BitConverter.ToInt16(bytes, offsetM + 14) : (short)0;    // Secondary maps only in TOT

            int ofsetB1 = data.GameVersion <= 44 ? offsetM + 14 : offsetM + 16; //offset for block 2 values

            data.MapUnitVisibility = new bool[data.MapNoSecondaryMaps + 1][,,];
            data.MapCityVisibility = new bool[data.MapNoSecondaryMaps + 1][,,];
            data.MapIrrigationVisibility = new bool[data.MapNoSecondaryMaps + 1][,,];
            data.MapMiningVisibility = new bool[data.MapNoSecondaryMaps + 1][,,];
            data.MapRoadVisibility = new bool[data.MapNoSecondaryMaps + 1][,,];
            data.MapRailroadVisibility = new bool[data.MapNoSecondaryMaps + 1][,,];
            data.MapFortressVisibility = new bool[data.MapNoSecondaryMaps + 1][,,];
            data.MapPollutionVisibility = new bool[data.MapNoSecondaryMaps + 1][,,];
            data.MapAirbaseVisibility = new bool[data.MapNoSecondaryMaps + 1][,,];
            data.MapFarmlandVisibility = new bool[data.MapNoSecondaryMaps + 1][,,];
            data.MapTransporterVisibility = new bool[data.MapNoSecondaryMaps + 1][,,];
            data.MapTerrainType = new int[data.MapNoSecondaryMaps + 1][,];
            data.MapRiverPresent = new bool[data.MapNoSecondaryMaps + 1][,];
            data.MapResourcePresent = new bool[data.MapNoSecondaryMaps + 1][,];
            data.MapUnitPresent = new bool[data.MapNoSecondaryMaps + 1][,];
            data.MapCityPresent = new bool[data.MapNoSecondaryMaps + 1][,];
            data.MapIrrigationPresent = new bool[data.MapNoSecondaryMaps + 1][,];
            data.MapMiningPresent = new bool[data.MapNoSecondaryMaps + 1][,];
            data.MapRoadPresent = new bool[data.MapNoSecondaryMaps + 1][,];
            data.MapRailroadPresent = new bool[data.MapNoSecondaryMaps + 1][,];
            data.MapFortressPresent = new bool[data.MapNoSecondaryMaps + 1][,];
            data.MapPollutionPresent = new bool[data.MapNoSecondaryMaps + 1][,];
            data.MapFarmlandPresent = new bool[data.MapNoSecondaryMaps + 1][,];
            data.MapAirbasePresent = new bool[data.MapNoSecondaryMaps + 1][,];
            data.MapTransporterPresent = new bool[data.MapNoSecondaryMaps + 1][,];
            data.MapTileWithinCityRadiusOwner = new byte[data.MapNoSecondaryMaps + 1][,];
            data.LandSeaIndex = new byte[data.MapNoSecondaryMaps + 1][,];
            data.MapTileVisibility = new bool[data.MapNoSecondaryMaps + 1][,,];
            data.MapTileFertility = new int[data.MapNoSecondaryMaps + 1][,];
            data.MapTileOwnership = new int[data.MapNoSecondaryMaps + 1][,];
            data.MapSpecialType = new int[data.MapNoSecondaryMaps + 1][,];
            data.MapSeed = new short[data.MapNoSecondaryMaps + 1];
            for (int mapNo = 0; mapNo < data.MapNoSecondaryMaps + 1; mapNo++)
            {
                // block 1 - terrain improvements that each civ sees (for 7 civs, ignore barbs)
                data.MapUnitVisibility[mapNo] = new bool[data.MapXdimX2 / 2, data.MapYdim, 8];
                data.MapCityVisibility[mapNo] = new bool[data.MapXdimX2 / 2, data.MapYdim, 8];
                data.MapIrrigationVisibility[mapNo] = new bool[data.MapXdimX2 / 2, data.MapYdim, 8];
                data.MapMiningVisibility[mapNo] = new bool[data.MapXdimX2 / 2, data.MapYdim, 8];
                data.MapRoadVisibility[mapNo] = new bool[data.MapXdimX2 / 2, data.MapYdim, 8];
                data.MapRailroadVisibility[mapNo] = new bool[data.MapXdimX2 / 2, data.MapYdim, 8];
                data.MapFortressVisibility[mapNo] = new bool[data.MapXdimX2 / 2, data.MapYdim, 8];
                data.MapPollutionVisibility[mapNo] = new bool[data.MapXdimX2 / 2, data.MapYdim, 8];
                data.MapAirbaseVisibility[mapNo] = new bool[data.MapXdimX2 / 2, data.MapYdim, 8];
                data.MapFarmlandVisibility[mapNo] = new bool[data.MapXdimX2 / 2, data.MapYdim, 8];
                data.MapTransporterVisibility[mapNo] = new bool[data.MapXdimX2 / 2, data.MapYdim, 8];
                for (int civNo = 0; civNo < 7; civNo++)
                {
                    for (int i = 0; i < data.MapArea; i++)
                    {
                        int x = i % (data.MapXdimX2 / 2);
                        int y = i / (data.MapXdimX2 / 2);

                        int terrA = ofsetB1 + civNo * data.MapXdimX2 / 2 * data.MapYdim + i;
                        data.MapUnitVisibility[mapNo][x, y, civNo + 1] = GetBit(bytes[terrA], 0);
                        data.MapCityVisibility[mapNo][x, y, civNo + 1] = GetBit(bytes[terrA], 1);
                        data.MapIrrigationVisibility[mapNo][x, y, civNo + 1] = GetBit(bytes[terrA], 2);
                        data.MapMiningVisibility[mapNo][x, y, civNo + 1] = GetBit(bytes[terrA], 3) && !GetBit(bytes[terrA], 2); ;
                        data.MapRoadVisibility[mapNo][x, y, civNo + 1] = GetBit(bytes[terrA], 4);
                        data.MapRailroadVisibility[mapNo][x, y, civNo + 1] = GetBit(bytes[terrA], 4) && GetBit(bytes[terrA], 5);
                        data.MapFortressVisibility[mapNo][x, y, civNo + 1] = GetBit(bytes[terrA], 6) && !GetBit(bytes[terrA], 1);
                        data.MapPollutionVisibility[mapNo][x, y, civNo + 1] = GetBit(bytes[terrA], 7);
                        data.MapAirbaseVisibility[mapNo][x, y, civNo + 1] = GetBit(bytes[terrA], 1) && GetBit(bytes[terrA], 6);
                        data.MapFarmlandVisibility[mapNo][x, y, civNo + 1] = GetBit(bytes[terrA], 2) && GetBit(bytes[terrA], 3);
                        data.MapTransporterVisibility[mapNo][x, y, civNo + 1] = GetBit(bytes[terrA], 1) && GetBit(bytes[terrA], 7);
                    }
                }

                // block 2 - terrain type
                int ofsetB2 = ofsetB1 + 7 * data.MapArea;
                data.MapTerrainType[mapNo] = new int[data.MapXdimX2 / 2, data.MapYdim];
                data.MapRiverPresent[mapNo] = new bool[data.MapXdimX2 / 2, data.MapYdim];
                data.MapResourcePresent[mapNo] = new bool[data.MapXdimX2 / 2, data.MapYdim];
                data.MapUnitPresent[mapNo] = new bool[data.MapXdimX2 / 2, data.MapYdim];
                data.MapCityPresent[mapNo] = new bool[data.MapXdimX2 / 2, data.MapYdim];
                data.MapIrrigationPresent[mapNo] = new bool[data.MapXdimX2 / 2, data.MapYdim];
                data.MapMiningPresent[mapNo] = new bool[data.MapXdimX2 / 2, data.MapYdim];
                data.MapRoadPresent[mapNo] = new bool[data.MapXdimX2 / 2, data.MapYdim];
                data.MapRailroadPresent[mapNo] = new bool[data.MapXdimX2 / 2, data.MapYdim];
                data.MapFortressPresent[mapNo] = new bool[data.MapXdimX2 / 2, data.MapYdim];
                data.MapPollutionPresent[mapNo] = new bool[data.MapXdimX2 / 2, data.MapYdim];
                data.MapFarmlandPresent[mapNo] = new bool[data.MapXdimX2 / 2, data.MapYdim];
                data.MapAirbasePresent[mapNo] = new bool[data.MapXdimX2 / 2, data.MapYdim];
                data.MapTransporterPresent[mapNo] = new bool[data.MapXdimX2 / 2, data.MapYdim];
                data.MapTileWithinCityRadiusOwner[mapNo] = new byte[data.MapXdimX2 / 2, data.MapYdim];
                data.LandSeaIndex[mapNo] = new byte[data.MapXdimX2 / 2, data.MapYdim];
                data.MapTileVisibility[mapNo] = new bool[data.MapXdimX2 / 2, data.MapYdim, 8];
                data.MapTileFertility[mapNo] = new int[data.MapXdimX2 / 2, data.MapYdim];
                data.MapTileOwnership[mapNo] = new int[data.MapXdimX2 / 2, data.MapYdim];
                data.MapSpecialType[mapNo] = new int[data.MapXdimX2 / 2, data.MapYdim];
                for (int i = 0; i < data.MapArea; i++)
                {
                    int x = i % (data.MapXdimX2 / 2);
                    int y = i / (data.MapXdimX2 / 2);

                    // Terrain type
                    int terrB = ofsetB2 + i * 6 + 0;
                    data.MapTerrainType[mapNo][x, y] = bytes[terrB] & 0xF;
                    data.MapResourcePresent[mapNo][x, y] = GetBit(bytes[terrB], 6); // not good. Locations of resources are determined by a separate formula
                    data.MapRiverPresent[mapNo][x, y] = GetBit(bytes[terrB], 7);

                    // Tile improvements (for all civs! In block 1 it's for indivudual civs)
                    terrB = ofsetB2 + i * 6 + 1;
                    data.MapUnitPresent[mapNo][x, y] = GetBit(bytes[terrB], 0);
                    data.MapCityPresent[mapNo][x, y] = GetBit(bytes[terrB], 1);
                    data.MapIrrigationPresent[mapNo][x, y] = GetBit(bytes[terrB], 2);
                    data.MapMiningPresent[mapNo][x, y] = GetBit(bytes[terrB], 3) && !GetBit(bytes[terrB], 2);
                    data.MapRoadPresent[mapNo][x, y] = GetBit(bytes[terrB], 4);
                    data.MapRailroadPresent[mapNo][x, y] = GetBit(bytes[terrB], 4) && GetBit(bytes[terrB], 5);
                    data.MapFortressPresent[mapNo][x, y] = GetBit(bytes[terrB], 6) && !GetBit(bytes[terrB], 1);
                    data.MapPollutionPresent[mapNo][x, y] = GetBit(bytes[terrB], 7);
                    data.MapFarmlandPresent[mapNo][x, y] = GetBit(bytes[terrB], 2) && GetBit(bytes[terrB], 3);
                    data.MapAirbasePresent[mapNo][x, y] = GetBit(bytes[terrB], 1) && GetBit(bytes[terrB], 6);
                    data.MapTransporterPresent[mapNo][x, y] = GetBit(bytes[terrB], 1) && GetBit(bytes[terrB], 7);

                    // Owner of city which has tile in its radius (gives only one civ even if there are
                    // mutliple cities of different civs around tile, so kinda useless parameter)
                    data.MapTileWithinCityRadiusOwner[mapNo][x, y] = bytes[ofsetB2 + i * 6 + 2] switch
                    {
                        0x20 => 1,
                        0x40 => 2,
                        0x60 => 3,
                        0x80 => 4,
                        0xA0 => 5,
                        0xC0 => 6,
                        0xE0 => 7,
                        _ => 0      // no cities around
                    };

                    data.LandSeaIndex[mapNo][x, y] = bytes[ofsetB2 + i * 6 + 3];

                    // Visibility of squares for all civs (0...red (barbarian), 1...white, 2...green, etc.)
                    for (int civ = 0; civ < 8; civ++)
                        data.MapTileVisibility[mapNo][x, y, civ] = GetBit(bytes[ofsetB2 + i * 6 + 4], civ);

                    data.MapTileFertility[mapNo][x, y] = bytes[ofsetB2 + i * 6 + 5] & 0xF;  // as determined by AI

                    // Tile ownership
                    data.MapTileOwnership[mapNo][x, y] = (bytes[ofsetB2 + i * 6 + 5] & 0xF0) switch
                    {
                        0x10 => 1,
                        0x20 => 2,
                        0x30 => 3,
                        0x40 => 4,
                        0x50 => 5,
                        0x60 => 6,
                        0x70 => 7,
                        _ => 0      // no owner or barbs
                    };
                }

                data.MapSeed[mapNo] = data.GameVersion > 44 ? BitConverter.ToInt16(bytes, ofsetB2 + 6 * data.MapArea) : (byte)0;    // only in TOT

                int mapSeedLength = data.GameVersion > 44 ? 2 : 0;
                ofsetB1 += 13 * data.MapArea + mapSeedLength;
            }
            

            // Unknown block 1 (length = MapLocatorXdim*MapLocatorYdim/4)
            int ofsetUb1 = ofsetB1;

            // Unknown block 2 (length = 1024 for <=MGE, = 10240 for TOT)
            int ofsetUb2 = ofsetUb1 + 2 * data.MapLocatorXdim * data.MapLocatorYdim;

            #endregion
            #region Units
            //=========================
            //UNIT INFO
            //=========================
            int multipl, ofsetU;
            if (data.GameVersion <= 40)     // <= FW
            {
                ofsetU = ofsetUb2 + 1024;
                multipl = 26;
            }
            else if (data.GameVersion == 44)    // MGE
            {
                ofsetU = ofsetUb2 + 1024;
                multipl = 32;
            }
            else    // TOT
            {
                ofsetU = ofsetUb2 + 10240;
                multipl = 40; 
            }

            data.UnitXloc = new short[data.NumberOfUnits];
            data.UnitYloc = new short[data.NumberOfUnits];
            data.UnitMap = new short[data.NumberOfUnits];
            data.UnitMadeMoveThisTurn = new bool[data.NumberOfUnits];
            data.UnitVeteran = new bool[data.NumberOfUnits];
            data.UnitWaitOrder = new bool[data.NumberOfUnits];
            data.UnitType = new byte[data.NumberOfUnits];
            data.UnitCiv = new byte[data.NumberOfUnits];
            data.UnitMovePointsLost = new short[data.NumberOfUnits];
            data.UnitVisibility = new bool[data.NumberOfUnits][];
            data.UnitHitPointsLost = new byte[data.NumberOfUnits];
            data.UnitPrevXloc = new short[data.NumberOfUnits];
            data.UnitPrevYloc = new short[data.NumberOfUnits];
            data.UnitCounterRoleParameter = new byte[data.NumberOfUnits];
            data.UnitOrders = new byte[data.NumberOfUnits];
            data.UnitHomeCity = new byte[data.NumberOfUnits];
            data.UnitGotoX = new short[data.NumberOfUnits];
            data.UnitGotoY = new short[data.NumberOfUnits];
            data.UnitMapNoOfGoto = new short[data.NumberOfUnits];
            data.UnitLinkOtherUnitsOnTop = new short[data.NumberOfUnits];
            data.UnitLinkOtherUnitsUnder = new short[data.NumberOfUnits];
            data.UnitAnimation = new short[data.NumberOfUnits];
            data.UnitOrientation = new short[data.NumberOfUnits];
            int totOffset;
            for (int i = 0; i < data.NumberOfUnits; i++)
            {
                totOffset = 0;
                data.UnitXloc[i] = BitConverter.ToInt16(bytes, ofsetU + multipl * i + totOffset);   // <0 for dead unit
                data.UnitYloc[i] = BitConverter.ToInt16(bytes, ofsetU + multipl * i + 2 + totOffset);
                if (data.GameVersion > 44)  // TOT
                {
                    data.UnitMap[i] = BitConverter.ToInt16(bytes, ofsetU + multipl * i + 4);
                    totOffset += 2;
                }
                data.UnitMadeMoveThisTurn[i] = GetBit(bytes[ofsetU + multipl * i + 4 + totOffset], 6);
                data.UnitVeteran[i] = GetBit(bytes[ofsetU + multipl * i + 5 + totOffset], 5);
                data.UnitWaitOrder[i] = GetBit(bytes[ofsetU + multipl * i + 5 + totOffset], 6);
                data.UnitType[i] = bytes[ofsetU + multipl * i + 6 + totOffset];
                data.UnitCiv[i] = bytes[ofsetU + multipl * i + 7 + totOffset];
                data.UnitMovePointsLost[i] = bytes[ofsetU + multipl * i + 8 + totOffset];

                // Unit visibility to other civs
                data.UnitVisibility[i] = new bool[8];
                for (int civId = 0; civId < 8; civId++)
                    data.UnitVisibility[i][civId] = GetBit(bytes[ofsetU + multipl * i + 9 + totOffset], civId);

                data.UnitHitPointsLost[i] = bytes[ofsetU + multipl * i + 10 + totOffset];
                switch (bytes[ofsetU + multipl * i + 11 + totOffset])   // Previous move (00=up-right, 01=right, ..., 07=up, FF=no movement)   
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

                // Counter/role parameter, can be either:
                // - work counter for settlers/engineers
                // - for air units =1 indicates it ran out of fuel
                // - for sea units =1 indicates it was lost at sea
                // - it represents Id of caravan commodity for trade units (from rules.txt)
                data.UnitCounterRoleParameter[i] = bytes[ofsetU + multipl * i + 13 + totOffset];

                data.UnitOrders[i] = bytes[ofsetU + multipl * i + 15 + totOffset];
                //if (bytes[ofsetU + multipl * i + 15] == 27) data.UnitOrders[i] = OrderType.NoOrders;    // TODO: (this is temp) find out what 0x1B means in unit orders
                data.UnitHomeCity[i] = bytes[ofsetU + multipl * i + 16 + totOffset];
                data.UnitGotoX[i] = BitConverter.ToInt16(bytes, ofsetU + multipl * i + 18 + totOffset);
                data.UnitGotoY[i] = BitConverter.ToInt16(bytes, ofsetU + multipl * i + 20 + totOffset);
                if (data.GameVersion > 44)  // TOT
                {
                    data.UnitMapNoOfGoto[i] = BitConverter.ToInt16(bytes, ofsetU + multipl * i + 24);
                    totOffset += 2;
                }
                data.UnitLinkOtherUnitsOnTop[i] = BitConverter.ToInt16(bytes, ofsetU + multipl * i + 22 + totOffset);
                data.UnitLinkOtherUnitsUnder[i] = BitConverter.ToInt16(bytes, ofsetU + multipl * i + 24 + totOffset);
                if (data.GameVersion > 44)  // TOT
                {
                    data.UnitAnimation[i] = BitConverter.ToInt16(bytes, ofsetU + multipl * i + 30 + totOffset);
                    data.UnitOrientation[i] = BitConverter.ToInt16(bytes, ofsetU + multipl * i + 32 + totOffset);
                }
            }
            #endregion
            #region Cities
            //=========================
            //CITIES
            //=========================
            int ofsetC = ofsetU + multipl * data.NumberOfUnits;

            if (data.GameVersion <= 40) // <= FW
            {
                multipl = 84;
            }
            else if (data.GameVersion == 44)    // MGE
            {
                multipl = 88;
            }
            else
            {
                multipl = 92;
            }
            
            data.CityXloc = new short[data.NumberOfCities];
            data.CityYloc = new short[data.NumberOfCities];
            data.CityMapNo = new short[data.NumberOfCities];
            data.CityCanBuildCoastal = new bool[data.NumberOfCities];
            data.CityCanBuildHydro = new bool[data.NumberOfCities];
            data.CityAutobuildMilitaryRule = new bool[data.NumberOfCities];
            data.CityStolenAdvance = new bool[data.NumberOfCities];
            data.CityImprovementSold = new bool[data.NumberOfCities];
            data.CityWeLoveKingDay = new bool[data.NumberOfCities];
            data.CityCivilDisorder = new bool[data.NumberOfCities];
            data.CityCanBuildShips = new bool[data.NumberOfCities];
            data.CityAutobuildMilitaryAdvisor = new bool[data.NumberOfCities];
            data.CityAutobuildDomesticAdvisor = new bool[data.NumberOfCities];
            data.CityObjectivex3 = new bool[data.NumberOfCities];
            data.CityObjectivex1 = new bool[data.NumberOfCities];
            data.CityOwner = new byte[data.NumberOfCities];
            data.CitySize = new byte[data.NumberOfCities];
            data.CityWhoBuiltIt = new byte[data.NumberOfCities];
            data.CityTurnsExpiredSinceCaptured = new byte[data.NumberOfCities];
            data.CityWhoKnowsAboutIt = new bool[data.NumberOfCities][];
            data.CityLastSizeRevealedToCivs = new int[data.NumberOfCities][];
            data.CitySpecialists = new byte[data.NumberOfCities][];
            data.CityFoodInStorage = new short[data.NumberOfCities];
            data.CityShieldsProgress = new short[data.NumberOfCities];
            data.CityNetTrade = new short[data.NumberOfCities];
            data.CityName = new string[data.NumberOfCities];
            data.CityDistributionWorkers = new bool[data.NumberOfCities][];
            data.CityNoOfSpecialistsx4 = new byte[data.NumberOfCities];
            data.CityImprovements = new bool[data.NumberOfCities][];
            data.CityItemInProduction = new sbyte[data.NumberOfCities];
            data.CityActiveTradeRoutes = new int[data.NumberOfCities];
            data.CityCommoditySupplied = new int[data.NumberOfCities][];
            data.CityCommodityDemanded = new int[data.NumberOfCities][];
            data.CityCommodityInRoute = new int[data.NumberOfCities][];
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
                totOffset = 0;
                data.CityXloc[i] = BitConverter.ToInt16(bytes, ofsetC + multipl * i + 0);
                data.CityYloc[i] = BitConverter.ToInt16(bytes, ofsetC + multipl * i + 2);
                if (data.GameVersion > 44)  // TOT
                {
                    data.CityMapNo[i] = BitConverter.ToInt16(bytes, ofsetC + multipl * i + 4);
                    totOffset += 2;
                }
                data.CityCivilDisorder[i] = GetBit(bytes[ofsetC + multipl * i + 4 + totOffset], 0);
                data.CityWeLoveKingDay[i] = GetBit(bytes[ofsetC + multipl * i + 4 + totOffset], 1);
                data.CityImprovementSold[i] = GetBit(bytes[ofsetC + multipl * i + 4 + totOffset], 2);
                data.CityStolenAdvance[i] = GetBit(bytes[ofsetC + multipl * i + 4 + totOffset], 3);
                data.CityAutobuildMilitaryRule[i] = GetBit(bytes[ofsetC + multipl * i + 4 + totOffset], 4);
                data.CityCanBuildCoastal[i] = GetBit(bytes[ofsetC + multipl * i + 4 + totOffset], 7);
                data.CityCanBuildHydro[i] = GetBit(bytes[ofsetC + multipl * i + 5 + totOffset], 3);
                data.CityCanBuildShips[i] = GetBit(bytes[ofsetC + multipl * i + 6 + totOffset], 5);
                data.CityAutobuildMilitaryAdvisor[i] = GetBit(bytes[ofsetC + multipl * i + 7 + totOffset], 0);
                data.CityAutobuildDomesticAdvisor[i] = GetBit(bytes[ofsetC + multipl * i + 7 + totOffset], 1);
                data.CityObjectivex1[i] = GetBit(bytes[ofsetC + multipl * i + 7 + totOffset], 2);
                data.CityObjectivex3[i] = GetBit(bytes[ofsetC + multipl * i + 7 + totOffset], 4);
                data.CityOwner[i] = bytes[ofsetC + multipl * i + 8 + totOffset];
                data.CitySize[i] = bytes[ofsetC + multipl * i + 9 + totOffset];
                data.CityWhoBuiltIt[i] = bytes[ofsetC + multipl * i + 10 + totOffset];
                data.CityTurnsExpiredSinceCaptured[i] = bytes[ofsetC + multipl * i + 11 + totOffset];
                data.CityWhoKnowsAboutIt[i] = new bool[8];
                data.CityLastSizeRevealedToCivs[i] = new int[8];
                for (int civId = 0; civId < 8; civId++)
                {
                    data.CityWhoKnowsAboutIt[i][civId] = GetBit(bytes[ofsetC + multipl * i + 12 + totOffset], civId);
                }
                for (int civId = 0; civId < 8; civId++)
                {
                    data.CityLastSizeRevealedToCivs[i][civId] = bytes[ofsetC + multipl * i + 13 + civId + totOffset];
                }

                // Specialists (limited to reading 16 for each city)
                data.CitySpecialists[i] = new byte[16];
                for (int j = 0; j < 4; j++)
                {
                    var bt = bytes[ofsetC + multipl * i + 22 + j + totOffset];
                    for (int bitPair = 0; bitPair < 4; bitPair++)
                    {
                        bool bit1 = GetBit(bt, 2 * bitPair + 0);
                        bool bit2 = GetBit(bt, 2 * bitPair + 1);

                        if (!bit1 && !bit2) data.CitySpecialists[i][4 * j + bitPair] = 0; // No specialists (0-0)
                        else if (bit1 && !bit2) data.CitySpecialists[i][4 * j + bitPair] = 1; // Entertainer (0-1)
                        else if (!bit1 && bit2) data.CitySpecialists[i][4 * j + bitPair] = 2; // Taxman (1-0)
                        else data.CitySpecialists[i][4 * j + bitPair] = 3; // Scientist (1-1)
                    }
                }

                data.CityFoodInStorage[i] = BitConverter.ToInt16(bytes, ofsetC + multipl * i + 26 + totOffset);
                data.CityShieldsProgress[i] = BitConverter.ToInt16(bytes, ofsetC + multipl * i + 28 + totOffset);
                data.CityNetTrade[i] = BitConverter.ToInt16(bytes, ofsetC + multipl * i + 30 + totOffset);
                data.CityName[i] = ReadString(bytes, 16, ofsetC + multipl * i + 32 + totOffset);

                // Distribution of workers on map in city view
                // 1st byte - inner circle (starting from N, going in counter-clokwise direction)
                // 2nd byte - 8 on outer circle
                // 3nd byte - 4 on outer circle + city square
                data.CityDistributionWorkers[i] = new bool[21];
                for (int bit = 0; bit < 8; bit++)
                {
                    data.CityDistributionWorkers[i][0 * 8 + bit] = GetBit(bytes[ofsetC + multipl * i + 48 + totOffset + 0], 7 - bit);
                    data.CityDistributionWorkers[i][1 * 8 + bit] = GetBit(bytes[ofsetC + multipl * i + 48 + totOffset + 1], 7 - bit);
                    if (bit > 2)
                    {
                        data.CityDistributionWorkers[i][2 * 8 + bit - 3] = GetBit(bytes[ofsetC + multipl * i + 48 + totOffset + 2], 7 - bit);
                    }
                }

                data.CityNoOfSpecialistsx4[i] = bytes[ofsetC + multipl * i + 51 + totOffset];   // e.g. 8 = 2 specialists

                // Improvements
                int noImprovements = data.GameVersion <= 44 ? 34 : 35;  // TOT has additional transporter
                data.CityImprovements[i] = new bool[noImprovements];
                int count = 0;
                for (int block = 0; block < 5; block++)
                {
                    for (int bit = 0; bit < 8; bit++)
                    {
                        if (block == 0 && bit == 0) // skip 'nothing'
                            continue;
                        
                        data.CityImprovements[i][count] = GetBit(bytes[ofsetC + multipl * i + 52 + totOffset + block], bit);
                        count++;

                        if (count == noImprovements)
                            break;
                    }
                }
                
                // Item in production
                // 0,1,2...61(MGE)/79(TOT) are units, improvements are inversed (FF(hex)=1st, FE(hex)=2nd, ...)
                data.CityItemInProduction[i] = (sbyte)bytes[ofsetC + multipl * i + 57 + totOffset];

                data.CityActiveTradeRoutes[i] = bytes[ofsetC + multipl * i + 58 + totOffset];

                // 1st, 2nd, 3rd trade commodities supplied
                data.CityCommoditySupplied[i] = new int[] { bytes[ofsetC + multipl * i + 59 + totOffset], bytes[ofsetC + multipl * i + 60 + totOffset], bytes[ofsetC + multipl * i + 61 + totOffset] };

                // 1st, 2nd, 3rd trade commodities demanded
                data.CityCommodityDemanded[i] = new int[] { bytes[ofsetC + multipl * i + 62 + totOffset], bytes[ofsetC + multipl * i + 63 + totOffset], bytes[ofsetC + multipl * i + 64 + totOffset] };

                // 1st, 2nd, 3rd trade commodities in route
                data.CityCommodityInRoute[i] = new int[] { bytes[ofsetC + multipl * i + 65 + totOffset], bytes[ofsetC + multipl * i + 66 + totOffset], bytes[ofsetC + multipl * i + 67 + totOffset] };

                // 1st, 2nd, 3rd trade route partner city number
                data.CityTradeRoutePartnerCity[i] = new int[]
                {
                    BitConverter.ToInt16(bytes, ofsetC + multipl * i + 68 + totOffset),
                    BitConverter.ToInt16(bytes, ofsetC + multipl * i + 70 + totOffset),
                    BitConverter.ToInt16(bytes, ofsetC + multipl * i + 72 + totOffset)
                };

                data.CityScience[i] = BitConverter.ToInt16(bytes, ofsetC + multipl * i + 74 + totOffset);
                data.CityTax[i] = BitConverter.ToInt16(bytes, ofsetC + multipl * i + 76 + totOffset);
                data.CityNoOfTradeIcons[i] = BitConverter.ToInt16(bytes, ofsetC + multipl * i + 78 + totOffset);
                data.CityTotalFoodProduction[i] = bytes[ofsetC + multipl * i + 80 + totOffset];
                data.CityTotalShieldProduction[i] = bytes[ofsetC + multipl * i + 81 + totOffset];
                data.CityHappyCitizens[i] = bytes[ofsetC + multipl * i + 82 + totOffset];
                data.CityUnhappyCitizens[i] = bytes[ofsetC + multipl * i + 83 + totOffset];
            }
            #endregion
            #region Data for finding next city name
            //=========================
            //DATA FOR FINDING NEXT CITY NAME
            //=========================
            int ofsetTc = ofsetC + multipl * data.NumberOfCities;

            data.CitiesBuiltSofar = new byte[21];
            for (int civId = 0; civId < 21; civId++)
            {
                data.CitiesBuiltSofar[civId] = bytes[ofsetTc + 3 * civId + 1];
            }
            #endregion
            #region Other info
            //=========================
            //OTHER INFO
            //=========================
            int ofsetO = ofsetTc + 63;
            
            data.ActiveCursorXy = new short[] { BitConverter.ToInt16(bytes, ofsetO + 0), 
                                                BitConverter.ToInt16(bytes, ofsetO + 2) };

            int noHumanPlayers = 0;
            for (int i = 0; i < 8; i++) 
            { 
                var stat = data.HumanPlayers[i] ? 1 : 0;
                noHumanPlayers += stat;
            }
            int blockOo = data.GameVersion <= 44 ? 60 * noHumanPlayers + 1302 : 60 * noHumanPlayers + 1314;

            // Clicked tile with your mouse XY position (does not count if you clicked on a city)
            data.ClickedXy = new int[] { BitConverter.ToInt16(bytes, ofsetO + blockOo + 0), 
                                         BitConverter.ToInt16(bytes, ofsetO + blockOo + 2) };

            // Zoom (=-7(min)...+8(max), 0=std.)
            data.Zoom = BitConverter.ToInt16(bytes, ofsetO + blockOo + 4);
            #endregion
            #region Scenario parameters (optional)
            //=========================
            //SCENARIO PARAMS (only present in .scn or derived .sav files)
            //=========================
            int ofsetS = ofsetO + blockOo + 76;

            // How to determine if the file has scenario parameters?
            // More than 340 bytes (length of Destroyed civs block in TOT) have to appear
            // between end of "Other info" block and end of file/start of EVNT block, as scenario
            // params is exactly 100 bytes long
            bool scnFileOrDerived = false;
            var offsetEvnt = IndexofStringInByteArray(bytes, "EVNT");
            if ((offsetEvnt == -1 && (bytes.Length - ofsetS > 340)) ||
                (offsetEvnt != -1 && (offsetEvnt - ofsetS > 340)))
            {
                scnFileOrDerived = true;
            }

            if (scnFileOrDerived)
            {
                data.TotalWar = GetBit(bytes[ofsetS + 0], 0);
                data.ObjectiveVictory = GetBit(bytes[ofsetS + 0], 1);
                data.CountWondersAsObjectives = GetBit(bytes[ofsetS + 0], 2);
                data.ForbidGovernmentSwitching = GetBit(bytes[ofsetS + 0], 4);
                data.ForbidTechFromConquests = GetBit(bytes[ofsetS + 0], 5);
                data.ElliminatePollution = GetBit(bytes[ofsetS + 0], 6);
                data.TerrainAnimationLockout = GetBit(bytes[ofsetS + 0], 7);    // TOT only
                data.UnitAnimationLockout = GetBit(bytes[ofsetS + 1], 0);    // TOT only
                data.SpRfileOverride = GetBit(bytes[ofsetS + 1], 1);
                data.SpecialWwiIonlyAi = GetBit(bytes[ofsetS + 1], 7);

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
                data.ObjectiveProtagonist = bytes[ofsetS + 90];
                data.NoObjectivesDecisiveVictory = BitConverter.ToInt16(bytes, ofsetS + 92);
                data.NoObjectivesMarginalVictory = BitConverter.ToInt16(bytes, ofsetS + 94);
                data.NoObjectivesMarginalDefeat = BitConverter.ToInt16(bytes, ofsetS + 96);
                data.NoObjectivesDecisiveDefeat = BitConverter.ToInt16(bytes, ofsetS + 98);

                // Correct offset
                ofsetS += 100;
            }

            #endregion
            #region Destroyed civs
            //=========================
            //DESTROYED CIVS
            //=========================
            data.NoCivsDestroyed = BitConverter.ToInt16(bytes, ofsetS + 0);
            data.TurnCivDestroyed = new short[12];
            data.CivIdThatDestroyedOtherCiv = new byte[12];
            data.CivIdThatWasDestroyed = new byte[12];
            data.NameOfDestroyedCiv = new string[12];
            for (int i = 0; i < 12; i++)
            {
                data.TurnCivDestroyed[i] = BitConverter.ToInt16(bytes, ofsetS + 2 + 2 * i);
            }
            for (int i = 0; i < 12; i++)
            {
                data.CivIdThatDestroyedOtherCiv[i] = bytes[ofsetS + 26 + i];
            }
            for (int i = 0; i < 12; i++)
            {
                data.CivIdThatWasDestroyed[i] = bytes[ofsetS + 38 + i];
            }
            for (int i = 0; i < 12; i++)
            {
                data.NameOfDestroyedCiv[i] = ReadString(bytes, 24, ofsetS + 50 + 24 * i);
            }
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
                data.EventModifiers = new bool[data.NumberOfEvents][];
                data.EventTriggerParam = new byte[data.NumberOfEvents][];
                data.EventActionParam = new byte[data.NumberOfEvents][];

                offsetE += 4;
                multipl = data.GameVersion <= 44 ? 444 : 276;  // no of bytes for each event

                // Read strings
                data.EventStrings = ReadEventStrings(bytes, offsetE + multipl * data.NumberOfEvents);

                for (int i = 0; i < data.NumberOfEvents; i++)
                {
                    data.EventTriggerIds[i] = BitConverter.ToInt32(bytes, offsetE + multipl * i + 0);
                    data.EventActionIds[i] = FindPositionOfBits(BitConverter.ToInt32(bytes, offsetE + multipl * i + 4)).ToArray();

                    if (data.GameVersion <= 44) // <= MGE
                    {
                        data.EventModifiers[i] = new bool[32];

                        // Trigger parameters
                        data.EventTriggerParam[i] = new byte[48];
                        for (int j = 0; j < 48; j++)
                            data.EventTriggerParam[i][j] = bytes[offsetE + multipl * i + 8 + j];

                        // Action parameters
                        data.EventActionParam[i] = new byte[388];
                        for (int j = 0; j < 388; j++)
                            data.EventActionParam[i][j] = bytes[offsetE + multipl * i + 56 + j];
                    }
                    else    // TOT
                    {
                        // Event modifiers
                        data.EventModifiers[i] = new bool[32];
                        for (int j = 0; j < 4; j++)
                            for (int b = 0; b < 8; b++)
                                data.EventModifiers[i][8 * j + b] = GetBit(bytes[offsetE + multipl * i + 8 + j], b);

                        // Trigger parameters
                        data.EventTriggerParam[i] = new byte[39];
                        for (int j = 0; j < 39; j++)
                            data.EventTriggerParam[i][j] = bytes[offsetE + multipl * i + 12 + j];

                        // Action parameters
                        data.EventActionParam[i] = new byte[225];
                        for (int j = 0; j < 225; j++)
                            data.EventActionParam[i][j] = bytes[offsetE + multipl * i + 51 + j];
                    }
                }
            }
            #endregion

            return data;
        }

        // Get bit value in byte
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
            for (int bitToTest = 0; bitToTest < 32; bitToTest++)
            {
                if ((n & (1 << bitToTest)) != 0)
                {
                    positions.Add(bitToTest);
                }
            }

            return positions;
        }

        // Read string from byte array. Terminate reading when 0x0 encountered.
        private static string ReadString(byte[] bytes, int strLength, int offset)
        {
            char[] asciich = new char[strLength];
            for (int j = 0; j < strLength; j++)
            {
                asciich[j] = Convert.ToChar(bytes[offset + j]);
                if (asciich[j] == 0x0) break;
            }
            var str = new string(asciich);
            return str.Replace("\0", string.Empty); // remove null characters
        }
    }
}
