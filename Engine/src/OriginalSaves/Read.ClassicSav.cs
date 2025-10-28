using Civ2engine.Advances;
using Civ2engine.Enums;
using Civ2engine.IO;
using Civ2engine.MapObjects;
using Civ2engine.Production;
using Civ2engine.Statistics;
using Civ2engine.Terrains;
using Civ2engine.Units;
using Model;
using Model.Core;
using Model.Core.Advances;
using Model.Core.Cities;
using Model.Core.Mapping;
using Raylib_CSharp.Geometry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Model.Constants;

namespace Civ2engine.OriginalSaves;

// Read game data from SAV and RULES.txt
public class Read
{
    static string[] scnNames = ["Original", "SciFi", "Fantasy"];
    // READ SAV GAME
    public static IGame ClassicSav(byte[] bytes, Ruleset activeRuleSet, Rules rules,
        Dictionary<string, string?> viewData)
    {
        ClassicSaveObjects objects = new();
      
        #region Game version
        //=========================
        //GAME VERSION
        //=========================
        var gameVersion = bytes[10];
        #endregion
        #region Unit transport settings (TOT)
        //=========================
        //UNIT TRANSPORT SETTINGS
        //=========================
        if (gameVersion > 44)  // TOT only
        {
            var unitTransportRelationship = new short[80];
            var unitTransportBuildTransportSiteMask = new short[80];
            var unitTransportUseTransportSiteMask = new short[80];
            var unitTransportNativeTransportAbilityMask = new short[80];
            for (int unitId = 0; unitId < 80; unitId++) // 80=max units
            {
                unitTransportRelationship[unitId] = BitConverter.ToInt16(bytes, 12 + 8 * unitId + 0);
                unitTransportBuildTransportSiteMask[unitId] = BitConverter.ToInt16(bytes, 12 + 8 * unitId + 2);
                unitTransportUseTransportSiteMask[unitId] = BitConverter.ToInt16(bytes, 12 + 8 * unitId + 4);
                unitTransportNativeTransportAbilityMask[unitId] = BitConverter.ToInt16(bytes, 12 + 8 * unitId + 6);
            }
        }
        #endregion
        #region Options
        //=========================
        //GAME OPTIONS
        //=========================
        int optionsOffset;
        if (gameVersion <= 44) optionsOffset = 12;
        else optionsOffset = 652;    // TOT

        var options = new Options([
            !GetBit(bytes[optionsOffset + 0], 4),    // Simplified combat on/off 
            GetBit(bytes[optionsOffset + 0], 7),     // Bloodlust on/off            
            GetBit(bytes[optionsOffset + 1], 0),     // Don't restart if eliminated
            GetBit(bytes[optionsOffset + 1], 7),     // Flat earth
            GetBit(bytes[optionsOffset + 2], 3),     // Music on/off
            GetBit(bytes[optionsOffset + 2], 4),     // Sound effects on/off
            GetBit(bytes[optionsOffset + 2], 5),     // Grid on/off
            GetBit(bytes[optionsOffset + 2], 6),     // Enter closes city screen
            GetBit(bytes[optionsOffset + 2], 7),     // Move units without mouse
            GetBit(bytes[optionsOffset + 3], 0),     // Tutorial help on/off
            GetBit(bytes[optionsOffset + 3], 1),    // Instant advice on/off
            GetBit(bytes[optionsOffset + 3], 2),    // Fast piece slide on/off
            GetBit(bytes[optionsOffset + 3], 3),    // No pause after enemy moves on/off
            GetBit(bytes[optionsOffset + 3], 4),    // Show enemy moves on/off
            GetBit(bytes[optionsOffset + 3], 5),    // Autosave each turn on/off
            GetBit(bytes[optionsOffset + 3], 6),    // Always wait at end of turn on/off
            GetBit(bytes[optionsOffset + 3], 7),    // Cheat menu on/off
            GetBit(bytes[optionsOffset + 4], 0),    // Wonder movies on/off (for <= MGE)
            GetBit(bytes[optionsOffset + 4], 1),    // Throne room graphics on/off (for <= MGE)
            GetBit(bytes[optionsOffset + 4], 2),    // Diplomacy screen graphics on/off
            GetBit(bytes[optionsOffset + 4], 3),    // Civilopedia for advances on/off
            GetBit(bytes[optionsOffset + 4], 4),    // High council on/off (for <= MGE)
            GetBit(bytes[optionsOffset + 4], 5),    // Animated heralds on/off (for <= MGE)
            GetBit(bytes[optionsOffset + 8], 4),    // Cheat penalty/warning disabled?
            GetBit(bytes[optionsOffset + 8], 5),    // Scoring complete?
            GetBit(bytes[optionsOffset + 8], 6),    // Scenario (.scn) file?
            GetBit(bytes[optionsOffset + 8], 7),    // Scenario flag toggled?
            !GetBit(bytes[optionsOffset + 10], 0),  // Warn when city growth halted on/off
            !GetBit(bytes[optionsOffset + 10], 1),  // Show city improvements built on/off
            !GetBit(bytes[optionsOffset + 10], 2),  // Show non combat units build on/off
            !GetBit(bytes[optionsOffset + 10], 3),  // Show invalid build instructions on/off
            !GetBit(bytes[optionsOffset + 10], 4),  // Announce cities in disorder on/off
            !GetBit(bytes[optionsOffset + 10], 5),  // Announce order restored in cities on/off
            !GetBit(bytes[optionsOffset + 10], 6),  // Announce we love king day on/off
            !GetBit(bytes[optionsOffset + 10], 7),  // Warn when food dangerously low on/off
            !GetBit(bytes[optionsOffset + 11], 0),  // Warn when pollution occurs on/off
            !GetBit(bytes[optionsOffset + 11], 1),  // Warn when changing production will cost shileds on/off
            !GetBit(bytes[optionsOffset + 11], 2)   // Zoom to city not default action on/off
        ]);
        
        #endregion
        #region Parameters
        //=========================
        //GAME PARAMETERS
        //=========================
        int paramsOffset = gameVersion <= 44 ? 24 : 664;
        
        var firstAirUnitBuilt = GetBit(bytes[paramsOffset + 0], 1);
        var firstNavalUnitBuilt = GetBit(bytes[paramsOffset + 0], 2);
        var firstCaravanBuilt = GetBit(bytes[paramsOffset + 0], 4);
        var wasRepublicDemocracyAdopted = GetBit(bytes[paramsOffset + 0], 5);
        var firstSignificantlyDamagedUnit = GetBit(bytes[paramsOffset + 0], 6) && GetBit(bytes[paramsOffset + 0], 7);
        var turnNumber = BitConverter.ToInt16(bytes, paramsOffset + 4);
        var turnNumberForGameYear = BitConverter.ToInt16(bytes, paramsOffset + 6);
        var selectedUnitIndex = BitConverter.ToInt16(bytes, paramsOffset + 10);   // Unit selected at start of game (-1 if no unit)
        var playersCivIndex = bytes[paramsOffset + 15];   // Human player
        var whichCivsMapShown = bytes[paramsOffset + 16];
        var playersCivilizationNumberUsed = bytes[paramsOffset + 17]; // Players civ number used
        var mapRevealed = bytes[paramsOffset + 19] == 1;
        var difficultyLevel = bytes[paramsOffset + 20];
        var barbarianActivity = bytes[paramsOffset + 21];

        var civsInPlay = new bool[8];
        for (int i = 0; i < 8; i++)
            civsInPlay[i] = GetBit(bytes[paramsOffset + 22], i);

        // Civs with human player playing (multiplayer)
        var humanPlayers = new bool[8];
        for (int i = 0; i < 8; i++)
            humanPlayers[i] = GetBit(bytes[paramsOffset + 23], i);

        var globalTempRiseOccured = bytes[paramsOffset + 27];
        var noPollutionSkulls = BitConverter.ToInt16(bytes, paramsOffset + 30);
        var noOfTurnsOfPeace = bytes[paramsOffset + 32];
        var numberOfUnits = BitConverter.ToInt16(bytes, paramsOffset + 34);
        var numberOfCities = BitConverter.ToInt16(bytes, paramsOffset + 36);

        #endregion
        #region Technologies
        //=========================
        //TECHNOLOGIES
        //=========================
        int techsOffset = gameVersion <= 44 ? 66 : 706;

        int techs = gameVersion <= 39 ? 93 : 100;  // total no of techs from rules.txt (CiC has less)

        var civFirstDiscoveredTech = new byte[techs];
        for (int techNo = 0; techNo < techs; techNo++)
            civFirstDiscoveredTech[techNo] = bytes[techsOffset + techNo];

        var civsDiscoveredTechs = new bool[techs, 8];
        for (int techNo = 0; techNo < techs; techNo++)
            for (int civId = 0; civId < 8; civId++)
                civsDiscoveredTechs[techNo, civId] = GetBit(bytes[techsOffset + techs + techNo], civId);

        #endregion
        #region Wonders
        //=========================
        //WONDERS
        //=========================
        int offsetW;
        if (gameVersion <= 39) offsetW = 252;  // <= CiC
        else if (gameVersion <= 44) offsetW = 266;  // FW, MGE
        else offsetW = 906;  // TOT

        var wonderCity = new short[28];      // city Id with wonder
        var wonderBuilt = new bool[28];      // has the wonder been built?
        var wonderDestroyed = new bool[28];  // has the wonder been destroyed?
        for (int i = 0; i < 28; i++)
        {
            wonderCity[i] = BitConverter.ToInt16(bytes, offsetW + 2 * i);

            // Determine if wonder is built/destroyed
            if (wonderCity[i] == -1)   // 0xFFFF
            {
                wonderBuilt[i] = false;
            }
            else if (wonderCity[i] == -2)    // 0xFEFF
            {
                wonderDestroyed[i] = true;
            }
            else
            {
                wonderBuilt[i] = true;
                wonderDestroyed[i] = false;
            }
        }
        #endregion
        #region Unknown block
        //=========================
        //Unknown block
        //=========================
        int offsetUb = 962;
        if (gameVersion > 44)  // TOT
        {
            //data.ExtendedMetadata.Add("TOT-Scenario", scnNames[bytes[offsetUb + 20]]);   // Original, fantasy, Scifi

            // Pollution data (same as before so no use in reading it)
            // ...
        }
        #endregion
        #region Civ names
        //=========================
        //Civ names (without barbarians)
        //=========================
        int offsetN = gameVersion <= 44 ? offsetW + 318 : 1250;

        var cityStyles = new int[8];
        var leaderNames = new string[8];
        var tribeNames = new string[8];
        var adjectives = new string[8];
        var anarchyTitles = new string[8];
        var despotismTitles = new string[8];
        var monarchyTitles = new string[8];
        var communismTitles = new string[8];
        var fundamentalismTitles = new string[8];
        var republicTitles = new string[8];
        var democracyTitles = new string[8];
        // Manually add data for barbarians
        cityStyles[0] = 0;
        leaderNames[0] = "NULL";
        tribeNames[0] = "Barbarians";
        adjectives[0] = "Barbarian";
        // Add data for other 7 civs
        for (int i = 0; i < 7; i++)
        {
            cityStyles[i + 1] = bytes[offsetN + 242 * i];
            // Various names (if empty, get the names from RULES.TXT)
            leaderNames[i + 1] = ReadString(bytes, 24, offsetN + 242 * i + 2);
            tribeNames[i + 1] = ReadString(bytes, 24, offsetN + 242 * i + 26);
            adjectives[i + 1] = ReadString(bytes, 24, offsetN + 242 * i + 50);
            anarchyTitles[i + 1] = ReadString(bytes, 24, offsetN + 242 * i + 74);
            despotismTitles[i + 1] = ReadString(bytes, 24, offsetN + 242 * i + 98);
            monarchyTitles[i + 1] = ReadString(bytes, 24, offsetN + 242 * i + 122);
            communismTitles[i + 1] = ReadString(bytes, 24, offsetN + 242 * i + 146);
            fundamentalismTitles[i + 1] = ReadString(bytes, 24, offsetN + 242 * i + 170);
            republicTitles[i + 1] = ReadString(bytes, 24, offsetN + 242 * i + 194);
            democracyTitles[i + 1] = ReadString(bytes, 24, offsetN + 242 * i + 218);
        }
        #endregion
        #region Civ tech, money, etc.
        //=========================
        //Civ tech, money (with barbarians)
        //=========================
        int offsetT, sizeT;
        if (gameVersion <= 39) // <= CiC
        {
            offsetT = 2264;
            sizeT = 1396;
        }
        else if (gameVersion <= 44) // <= MGE
        {
            offsetT = 2278;
            sizeT = 1428;
        }
        else    // TOT
        {
            offsetT = 2945;
            sizeT = 3348;
        }

        objects.Civilizations = new List<Civilization>();

        int offsetExtra;
        for (int civId = 0; civId < 8; civId++) // for each civ including barbarians
        {
            // Define offsets
            if (gameVersion <= 39) offsetExtra = 0;    // CiC
            else if (gameVersion <= 44) offsetExtra = 0;    // FW, MGE
            else offsetExtra = 7;  // TOT

            var gender = bytes[offsetT + sizeT * civId + 1]; // 0=male, 2=female
            var money = BitConverter.ToInt16(bytes, offsetT + sizeT * civId + offsetExtra + 2);
            var tribeId = bytes[offsetT + sizeT * civId + offsetExtra + 6]; // Civ number as per @Leaders table in RULES.TXT
            var science = BitConverter.ToInt16(bytes, offsetT + sizeT * civId + offsetExtra + 8);
            var researchingAdvance = bytes[offsetT + sizeT * civId + offsetExtra + 10]; // Advance currently being researched (FF(hex) = no goal)
            var numberAdvancesResearched = bytes[offsetT + sizeT * civId + offsetExtra + 16];
            var numberFutureTechsResearched = bytes[offsetT + sizeT * civId + offsetExtra + 17];
            var scienceRate = bytes[offsetT + sizeT * civId + offsetExtra + 19]; // (%/10)
            var taxRate = bytes[offsetT + sizeT * civId + offsetExtra + 20]; // (%/10)
            var governmentId = bytes[offsetT + sizeT * civId + offsetExtra + 21]; // 0=anarchy, ...
            var reputation = bytes[offsetT + sizeT * civId + offsetExtra + 30];
            var patience = bytes[offsetT + sizeT * civId + offsetExtra + 31];

            // Treaties
            var treatyContact = new bool[8];
            var treatyCeaseFire = new bool[8];
            var treatyPeace = new bool[8];
            var treatyAlliance = new bool[8];
            var treatyVendetta = new bool[8];
            var treatyEmbassy = new bool[8];
            var treatyWar = new bool[8];
            for (int civ2Id = 0; civ2Id < 8; civ2Id++)
            {
                treatyContact[civ2Id] = GetBit(bytes[offsetT + sizeT * civId + offsetExtra + 32 + 4 * civ2Id + 0], 0);
                treatyCeaseFire[civ2Id] = GetBit(bytes[offsetT + sizeT * civId + offsetExtra + 32 + 4 * civ2Id + 0], 1);
                treatyPeace[civ2Id] = GetBit(bytes[offsetT + sizeT * civId + offsetExtra + 32 + 4 * civ2Id + 0], 2);
                treatyAlliance[civ2Id] = GetBit(bytes[offsetT + sizeT * civId + offsetExtra + 32 + 4 * civ2Id + 0], 3);
                treatyVendetta[civ2Id] = GetBit(bytes[offsetT + sizeT * civId + offsetExtra + 32 + 4 * civ2Id + 0], 4);
                treatyEmbassy[civ2Id] = GetBit(bytes[offsetT + sizeT * civId + offsetExtra + 32 + 4 * civ2Id + 0], 7);
                treatyWar[civ2Id] = GetBit(bytes[offsetT + sizeT * civId + offsetExtra + 32 + 4 * civ2Id + 1], 5);
            }

            // Attitudes
            var attitudes = new int[8];
            for (int civ2Id = 0; civ2Id < 8; civ2Id++)
                attitudes[civ2Id] = bytes[offsetT + sizeT * civId + offsetExtra + 64 + civ2Id];

            // Advances
            int noAdvances = gameVersion <= 39 ? 93 : 100;    // CiC has fewer techs in rules.txt
            var advances = new bool[noAdvances];
            for (int block = 0; block < 13; block++)
            {
                for (int bit = 0; bit < 8; bit++)
                {
                    if (8 * block + bit >= noAdvances - 1) break;

                    advances[block * 8 + bit] = GetBit(bytes[offsetT + sizeT * civId + offsetExtra + 88 + block], bit);
                }
            }

            var numberMilitaryUnits = BitConverter.ToInt16(bytes, offsetT + sizeT * civId + offsetExtra + 102);
            var numberCities = BitConverter.ToInt16(bytes, offsetT + sizeT * civId + offsetExtra + 104);
            var sumCitySizes = BitConverter.ToInt16(bytes, offsetT + sizeT * civId + offsetExtra + 108);

            // No. of units per type (from Rules.txt)
            int noUnitsType = gameVersion switch
            {
                <= 39 => 54,    // CiC
                <= 44 => 62,    // FW, MGE
                _ => 80         // TOT
            };

            var activeUnitsPerType = new byte[noUnitsType];
            var casualtiesPerUnitType = new int[noUnitsType];
            var unitsInProductionPerType = new byte[noUnitsType];
            for (int type = 0; type < noUnitsType; type++)
            {
                activeUnitsPerType[type] = bytes[offsetT + sizeT * civId + offsetExtra + 216 + type];
                casualtiesPerUnitType[type] = bytes[offsetT + sizeT * civId + offsetExtra + 216 + noUnitsType + type];
                unitsInProductionPerType[type] = bytes[offsetT + sizeT * civId + offsetExtra + 216 + 2 * noUnitsType + type];
            }

            // Correct offsets
            offsetExtra = gameVersion switch
            {
                <= 44 => 216 + 3 * noUnitsType + 592,    // CiC, FW, MGE
                _ => 223 + 3 * noUnitsType + 2320        // TOT
            };

            // Last contact with other civs
            var lastContact = new int[8];
            for (int civ2Id = 0; civ2Id < 8; civ2Id++)
                lastContact[civ2Id] = BitConverter.ToInt16(bytes, offsetT + sizeT * civId + offsetExtra + 2 * civ2Id);

            // Spaceships
            var hasSpaceship = GetBit(bytes[offsetT + sizeT * civId + offsetExtra + 30], 0);
            var spaceshipEstimatedArrival = BitConverter.ToInt16(bytes, offsetT + sizeT * civId + offsetExtra + 32);
            var spaceshipLaunchYear = BitConverter.ToInt16(bytes, offsetT + sizeT * civId + offsetExtra + 34);
            var spaceshipStructural = BitConverter.ToInt16(bytes, offsetT + sizeT * civId + offsetExtra + 38);
            var spaceshipComponentsPropulsion = BitConverter.ToInt16(bytes, offsetT + sizeT * civId + offsetExtra + 40);
            var spaceshipComponentsFuel = BitConverter.ToInt16(bytes, offsetT + sizeT * civId + offsetExtra + 42);
            var spaceshipModulesHabitation = BitConverter.ToInt16(bytes, offsetT + sizeT * civId + offsetExtra + 44);
            var spaceshipModulesLifeSupport = BitConverter.ToInt16(bytes, offsetT + sizeT * civId + offsetExtra + 46);
            var spaceshipModulesSolarPanel = BitConverter.ToInt16(bytes, offsetT + sizeT * civId + offsetExtra + 48);

            var tribe = rules.Leaders[tribeId];
            // If leader name string is empty (no manual input), find the name in RULES.TXT (don't search for barbarians)
            var leaderName = leaderNames[civId];
            if (civId != 0 && leaderName.Length == 0) leaderName = (gender == 0) ? tribe.NameMale : tribe.NameFemale;

            // If tribe name string is empty (no manual input), find the name in RULES.TXT (don't search for barbarians)
            var tribeName = tribeNames[civId];
            if (civId != 0 && tribeName.Length == 0) tribeName = tribe.Plural;

            // If adjective string is empty (no manual input), find adjective in RULES.TXT (don't search for barbarians)
            var adjective = adjectives[civId];
            if (civId != 0 && adjective.Length == 0) adjective = tribe.Adjective;

            // Set citystyle from input only for player civ. Other civs (AI) have set citystyle from RULES.TXT
            var cityStyle = cityStyles[civId];
            if (civId != 0 && civId != playersCivIndex) cityStyle = tribe.CityStyle;

            var gov = rules.Governments[governmentId];
            objects.Civilizations.Add(new Civilization
            {
                TribeId = tribeId,
                Id = civId,
                Alive = civsInPlay[civId],
                CityStyle = cityStyle,
                LeaderName = leaderName,
                LeaderGender = gender,
                LeaderTitle = (gender == 0) ? gov.TitleMale : gov.TitleFemale,
                TribeName = tribeName,
                Adjective = adjective,
                Money = money,
                ReseachingAdvance = researchingAdvance,
                Advances = advances,
                ScienceRate = scienceRate * 10,
                TaxRate = taxRate * 10,
                Government = governmentId,
                AllowedAdvanceGroups = tribe.AdvanceGroups ?? new[] { AdvanceGroupAccess.CanResearch }
            });
        }

        objects.Civilizations[0].PlayerType = PlayerType.Barbarians;
        objects.Civilizations[playersCivIndex].PlayerType = PlayerType.Local;
        #endregion
        #region Civs relations to advances groups (TOT only)
        //=========================
        //Relations to groups (from @LEADERS2 in rules.txt)
        // 0=can research, can own
        // 1=can’t research, can own
        // 2=can’t research, can’t own
        //=========================
        if (gameVersion > 44)
        {
            int offsetGrps = 29728;

            var civsRelationsToAdvancesGroups = new byte[21][];
            for (int civId = 0; civId < 21; civId++)
            {
                civsRelationsToAdvancesGroups[civId] = new byte[8];
                for (int group = 0; group < 8; group++)
                    civsRelationsToAdvancesGroups[civId][group] = bytes[offsetGrps + 8 * civId + group];
            }
        }
        #endregion
        #region Transporters (TOT only)
        //=========================
        //Transporters
        //=========================
        var noTransporters = 0;
        if (gameVersion > 44)
        {
            int offsetTr = 29896;

            objects.Transporters = new();

            noTransporters = BitConverter.ToInt16(bytes, offsetTr + 0);
            for (int transpId = 0; transpId < noTransporters; transpId++)
            {
                objects.Transporters.Add(new()
                {
                    X1 = BitConverter.ToInt16(bytes, offsetTr + 4 + 14 * transpId + 0),
                    Y1 = BitConverter.ToInt16(bytes, offsetTr + 4 + 14 * transpId + 2),
                    MapId1 = bytes[offsetTr + 4 + 14 * transpId + 4],
                    X2 = BitConverter.ToInt16(bytes, offsetTr + 4 + 14 * transpId + 6),
                    Y2 = BitConverter.ToInt16(bytes, offsetTr + 4 + 14 * transpId + 8),
                    MapId2 = bytes[offsetTr + 4 + 14 * transpId + 10],
                    Look = bytes[offsetTr + 4 + 14 * transpId + 11]
                });
            }
        }
        #endregion
        #region Map
        //=========================
        //MAP
        //=========================
        int offsetM = gameVersion switch
        {
            <=39 => 13432, // CiC
            <=44 => 13702,  // FW, MGE
            _ => 29900 + 14 * noTransporters   // TOT
        };

        var mapXdimX2 = BitConverter.ToInt16(bytes, offsetM + 0);  // Map X dimension x2
        var mapYdim = BitConverter.ToInt16(bytes, offsetM + 2);
        var mapArea = BitConverter.ToInt16(bytes, offsetM + 4);  // Xdim*Ydim/2
        //var flatEarth = BitConverter.ToInt16(bytes, offsetM + 6);   // Flat Earth flag (info already given before!!)
        var mapResourceSeed = BitConverter.ToInt16(bytes, offsetM + 8);    // not used in game?
        var mapLocatorXdim = BitConverter.ToInt16(bytes, offsetM + 10);  // Minimap width (=MapXdim/2 rounded up), important for getting offset of unit block!!
        var mapLocatorYdim = BitConverter.ToInt16(bytes, offsetM + 12);  // Minimap height (=MapYdim/4 rounded up), important for getting offset of unit block!!
        var noSecondaryMaps = gameVersion > 44 ? BitConverter.ToInt16(bytes, offsetM + 14) : (short)0;    // Secondary maps only in TOT

        int ofsetB1 = gameVersion <= 44 ? offsetM + 14 : offsetM + 16; //offset for block 2 values

        objects.Maps = new List<Map>();

        for (int mapNo = 0; mapNo < noSecondaryMaps + 1; mapNo++)
        {
            var map = new Map(options.FlatEarth, mapNo)
            {
                MapRevealed = mapRevealed,
                WhichCivsMapShown = whichCivsMapShown,
                XDim = mapXdimX2 / 2,
                YDim = mapYdim,
                ResourceSeed = mapResourceSeed,
                LocatorXdim = mapLocatorXdim,
                LocatorYdim = mapLocatorYdim
            };
            
            var tile = new Tile[map.XDim, map.YDim];

            // block 1 - terrain improvements that each civ sees (for 7 civs, ignore barbs)
            var unitVisibility = new bool[mapXdimX2 / 2, mapYdim, 8];
            var cityVisibility = new bool[mapXdimX2 / 2, mapYdim, 8];
            var irrigationVisibility = new bool[mapXdimX2 / 2, mapYdim, 8];
            var miningVisibility = new bool[mapXdimX2 / 2, mapYdim, 8];
            var roadVisibility = new bool[mapXdimX2 / 2, mapYdim, 8];
            var railroadVisibility = new bool[mapXdimX2 / 2, mapYdim, 8];
            var fortressVisibility = new bool[mapXdimX2 / 2, mapYdim, 8];
            var pollutionVisibility = new bool[mapXdimX2 / 2, mapYdim, 8];
            var airbaseVisibility = new bool[mapXdimX2 / 2, mapYdim, 8];
            var farmlandVisibility = new bool[mapXdimX2 / 2, mapYdim, 8];
            var transporterVisibility = new bool[mapXdimX2 / 2, mapYdim, 8];
            for (int civNo = 0; civNo < 7; civNo++)
            {
                for (int i = 0; i < mapArea; i++)
                {
                    int x = i % (mapXdimX2 / 2);
                    int y = i / (mapXdimX2 / 2);

                    int terrA = ofsetB1 + civNo * mapXdimX2 / 2 * mapYdim + i;
                    unitVisibility[x, y, civNo + 1] = GetBit(bytes[terrA], 0);
                    cityVisibility[x, y, civNo + 1] = GetBit(bytes[terrA], 1);
                    irrigationVisibility[x, y, civNo + 1] = GetBit(bytes[terrA], 2);
                    miningVisibility[x, y, civNo + 1] = GetBit(bytes[terrA], 3) && !GetBit(bytes[terrA], 2); ;
                    roadVisibility[x, y, civNo + 1] = GetBit(bytes[terrA], 4);
                    railroadVisibility[x, y, civNo + 1] = GetBit(bytes[terrA], 4) && GetBit(bytes[terrA], 5);
                    fortressVisibility[x, y, civNo + 1] = GetBit(bytes[terrA], 6) && !GetBit(bytes[terrA], 1);
                    pollutionVisibility[x, y, civNo + 1] = GetBit(bytes[terrA], 7);
                    airbaseVisibility[x, y, civNo + 1] = GetBit(bytes[terrA], 1) && GetBit(bytes[terrA], 6);
                    farmlandVisibility[x, y, civNo + 1] = GetBit(bytes[terrA], 2) && GetBit(bytes[terrA], 3);
                    transporterVisibility[x, y, civNo + 1] = GetBit(bytes[terrA], 1) && GetBit(bytes[terrA], 7);
                }
            }

            // block 2 - terrain type
            int ofsetB2 = ofsetB1 + 7 * mapArea;
            var terrainType = new int[mapXdimX2 / 2, mapYdim];
            var riverPresent = new bool[mapXdimX2 / 2, mapYdim];
            var resourcePresent = new bool[mapXdimX2 / 2, mapYdim];
            var unitPresent = new bool[mapXdimX2 / 2, mapYdim];
            var cityPresent = new bool[mapXdimX2 / 2, mapYdim];
            var irrigationPresent = new bool[mapXdimX2 / 2, mapYdim];
            var miningPresent = new bool[mapXdimX2 / 2, mapYdim];
            var roadPresent = new bool[mapXdimX2 / 2, mapYdim];
            var railroadPresent = new bool[mapXdimX2 / 2, mapYdim];
            var fortressPresent = new bool[mapXdimX2 / 2, mapYdim];
            var pollutionPresent = new bool[mapXdimX2 / 2, mapYdim];
            var farmlandPresent = new bool[mapXdimX2 / 2, mapYdim];
            var airbasePresent = new bool[mapXdimX2 / 2, mapYdim];
            var transporterPresent = new bool[mapXdimX2 / 2, mapYdim];
            var tileWithinCityRadiusOwner = new byte[mapXdimX2 / 2, mapYdim];
            var landSeaIndex = new byte[mapXdimX2 / 2, mapYdim];
            var tileVisibility = new bool[mapXdimX2 / 2, mapYdim, 8];
            var tileFertility = new int[mapXdimX2 / 2, mapYdim];
            var tileOwnership = new int[mapXdimX2 / 2, mapYdim];
            var specialType = new int[mapXdimX2 / 2, mapYdim];
            for (var x = 0; x < map.XDim; x++)
            {
                for (var y = 0; y < map.YDim; y++)
                {
                    int i = y * map.XDim + x;

                    // Terrain type
                    int terrB = ofsetB2 + i * 6 + 0;
                    terrainType[x, y] = bytes[terrB] & 0xF;
                    resourcePresent[x, y] = GetBit(bytes[terrB], 6); // not good. Locations of resources are determined by a separate formula
                    riverPresent[x, y] = GetBit(bytes[terrB], 7);

                    // Tile improvements (for all civs! In block 1 it's for indivudual civs)
                    terrB = ofsetB2 + i * 6 + 1;
                    unitPresent[x, y] = GetBit(bytes[terrB], 0);
                    cityPresent[x, y] = GetBit(bytes[terrB], 1);
                    irrigationPresent[x, y] = GetBit(bytes[terrB], 2);
                    miningPresent[x, y] = GetBit(bytes[terrB], 3) && !GetBit(bytes[terrB], 2);
                    roadPresent[x, y] = GetBit(bytes[terrB], 4);
                    railroadPresent[x, y] = GetBit(bytes[terrB], 4) && GetBit(bytes[terrB], 5);
                    fortressPresent[x, y] = GetBit(bytes[terrB], 6) && !GetBit(bytes[terrB], 1);
                    pollutionPresent[x, y] = GetBit(bytes[terrB], 7);
                    farmlandPresent[x, y] = GetBit(bytes[terrB], 2) && GetBit(bytes[terrB], 3);
                    airbasePresent[x, y] = GetBit(bytes[terrB], 1) && GetBit(bytes[terrB], 6);
                    transporterPresent[x, y] = GetBit(bytes[terrB], 1) && GetBit(bytes[terrB], 7);

                    // Owner of city which has tile in its radius (gives only one civ even if there are
                    // mutliple cities of different civs around tile, so kinda useless parameter)
                    tileWithinCityRadiusOwner[x, y] = bytes[ofsetB2 + i * 6 + 2] switch
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

                    landSeaIndex[x, y] = bytes[ofsetB2 + i * 6 + 3];

                    // Visibility of squares for all civs (0...red (barbarian), 1...white, 2...green, etc.)
                    for (int civ = 0; civ < 8; civ++)
                        tileVisibility[x, y, civ] = GetBit(bytes[ofsetB2 + i * 6 + 4], civ);

                    tileFertility[x, y] = bytes[ofsetB2 + i * 6 + 5] & 0xF;  // as determined by AI

                    // Tile ownership
                    tileOwnership[x, y] = (bytes[ofsetB2 + i * 6 + 5] & 0xF0) switch
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

                    var terrain = terrainType[x, y];
                    List<ConstructedImprovement> improvements = GetImprovementsFrom(farmlandPresent, irrigationPresent,
                        miningPresent, railroadPresent, roadPresent, fortressPresent, airbasePresent, pollutionPresent, 
                        transporterPresent, x, y);
                    var playerKnowledge = GetPlayerKnowledgeFrom(irrigationVisibility, miningVisibility, roadVisibility,
                        railroadVisibility, fortressVisibility, pollutionVisibility, airbaseVisibility, farmlandVisibility,
                        transporterVisibility, x, y);
                    tile[x, y] = new Tile(2 * x + (y % 2), y, rules.Terrains[mapNo][terrain], map.ResourceSeed, map, x,
                        Enumerable.Range(0, 8).Select(i => tileVisibility[x, y, i]).ToArray())
                    {
                        River = riverPresent[x, y],
                        Resource = resourcePresent[x, y],
                        PlayerKnowledge = playerKnowledge,
                        //UnitPresent = unitPresent[x, y],  // you can find this out yourself
                        //CityPresent = cityPresent[x, y],  // you can find this out yourself
                        Island = landSeaIndex[x, y],
                        Improvements = improvements
                    };
                }
            }

            map.Tile = tile;
            objects.Maps.Add(map);

            var mapSeed = gameVersion > 44 ? BitConverter.ToInt16(bytes, ofsetB2 + 6 * mapArea) : (byte)0;    // only in TOT

            int mapSeedLength = gameVersion > 44 ? 2 : 0;
            ofsetB1 += 13 * mapArea + mapSeedLength;
        }

        // Unknown block 1 (length = MapLocatorXdim*MapLocatorYdim/4)
        int ofsetUb1 = ofsetB1;

        // Unknown block 2 (length = 1024 for <=MGE, = 10240 for TOT)
        int ofsetUb2 = ofsetUb1 + 2 * mapLocatorXdim * mapLocatorYdim;

        #endregion
        #region Units
        //=========================
        //UNIT INFO
        //=========================
        int multipl, ofsetU;
        if (gameVersion <= 40)     // <= FW
        {
            ofsetU = ofsetUb2 + 1024;
            multipl = 26;
        }
        else if (gameVersion == 44)    // MGE
        {
            ofsetU = ofsetUb2 + 1024;
            multipl = 32;
        }
        else    // TOT
        {
            ofsetU = ofsetUb2 + 10240;
            multipl = 40; 
        }

        var unitHomeCity = new byte[numberOfUnits];
        int totOffset;
        for (int i = 0; i < numberOfUnits; i++)
        {
            totOffset = 0;

            var id = i;
            var x = BitConverter.ToInt16(bytes, ofsetU + multipl * i + totOffset);  // <0 for dead unit
            var y = BitConverter.ToInt16(bytes, ofsetU + multipl * i + 2 + totOffset);
            var mapIndex = 0;
            if (gameVersion > 44)  // TOT
            {
                mapIndex = BitConverter.ToInt16(bytes, ofsetU + multipl * i + 4);
                totOffset += 2;
            }
            var madeFirstMove = GetBit(bytes[ofsetU + multipl * i + 4 + totOffset], 6);
            var veteran = GetBit(bytes[ofsetU + multipl * i + 5 + totOffset], 5);
            var waitOrder = GetBit(bytes[ofsetU + multipl * i + 5 + totOffset], 6);
            var type = bytes[ofsetU + multipl * i + 6 + totOffset];
            var civId = bytes[ofsetU + multipl * i + 7 + totOffset];
            var movePointsLost = bytes[ofsetU + multipl * i + 8 + totOffset];

            // Unit visibility to other civs
            var visibility = new bool[8];
            for (int civ = 0; civ < 8; civ++)
                visibility[civ] = GetBit(bytes[ofsetU + multipl * i + 9 + totOffset], civ);

            var hitPointsLost = bytes[ofsetU + multipl * i + 10 + totOffset];
            int[] prevXy = bytes[ofsetU + multipl * i + 11 + totOffset] switch   // Previous move (00=up-right, 01=right, ..., 07=up, FF=no movement)   
            {
                0 => [x - 1, y + 1],
                1 => [x - 2, y],
                2 => [x - 1, y - 1],
                3 => [x, y - 2],
                4 => [x + 1, y - 1],
                5 => [x + 2, y],
                6 => [x + 1, y + 1],
                7 => [x, y + 2],
                255 => [x, y],
                _ => throw new ArgumentOutOfRangeException("Unknown previous move.")
            };

            // Counter/role parameter, can be either:
            // - work counter for settlers/engineers
            // - for air units =1 indicates it ran out of fuel
            // - for sea units =1 indicates it was lost at sea
            // - it represents Id of caravan commodity for trade units (from rules.txt)
            var counterRoleParameter = bytes[ofsetU + multipl * i + 13 + totOffset];

            var order = bytes[ofsetU + multipl * i + 15 + totOffset];
            //if (bytes[ofsetU + multipl * i + 15] == 27) data.UnitOrders[i] = OrderType.NoOrders;    // TODO: (this is temp) find out what 0x1B means in unit orders
            unitHomeCity[i] = bytes[ofsetU + multipl * i + 16 + totOffset];
            var goToX = BitConverter.ToInt16(bytes, ofsetU + multipl * i + 18 + totOffset);
            var goToY = BitConverter.ToInt16(bytes, ofsetU + multipl * i + 20 + totOffset);
            var goToMapIndex = 0;
            if (gameVersion > 44)  // TOT
            {
                goToMapIndex = BitConverter.ToInt16(bytes, ofsetU + multipl * i + 24);
                totOffset += 2;
            }
            var linkOtherUnitsOnTop = BitConverter.ToInt16(bytes, ofsetU + multipl * i + 22 + totOffset);
            var linkOtherUnitsUnder = BitConverter.ToInt16(bytes, ofsetU + multipl * i + 24 + totOffset);
            if (gameVersion > 44)  // TOT
            {
                var animation = BitConverter.ToInt16(bytes, ofsetU + multipl * i + 30 + totOffset);
                var orientation = BitConverter.ToInt16(bytes, ofsetU + multipl * i + 32 + totOffset);
            }

            if (mapIndex < 0) mapIndex = 0;   // avoid dead unit errors
            var validTile = objects.Maps[mapIndex].IsValidTileC2(x, y);

            var civilization = objects.Civilizations[civId];
            var unit = new Unit
            {
                Id = id,
                TypeDefinition = rules.UnitTypes[type],
                Dead = x < 0 || !validTile,
                CurrentLocation = validTile ? objects.Maps[mapIndex].TileC2(x, y) : null,
                X = x,
                Y = y,
                MapIndex = mapIndex,
                MovePointsLost = movePointsLost,
                HitPointsLost = hitPointsLost,
                MadeFirstMove = madeFirstMove,
                Veteran = veteran,
                Owner = civilization,
                PrevXy = prevXy,
                Order = order,
                GoToX = goToX,
                GoToY = goToY,
                LinkOtherUnitsOnTop = linkOtherUnitsOnTop,
                LinkOtherUnitsUnder = linkOtherUnitsUnder
            };

            switch (unit.AiRole)
            {
                case AiRoleType.Trade:
                    unit.CaravanCommodity = counterRoleParameter;
                    break;
                case AiRoleType.Settle:
                    unit.Counter = counterRoleParameter;
                    break;
            }

            objects.Civilizations[civId].Units.Add(unit);

            if (i == selectedUnitIndex)
                objects.ActiveUnit = unit;
        }
        #endregion
        #region Cities
        //=========================
        //CITIES
        //=========================
        int ofsetC = ofsetU + multipl * numberOfUnits;

        multipl = gameVersion switch
        {
            <= 40 => 84,    // <= FW
            44 => 88,       // MGE
            _ => 92         // ToT
        };

        objects.Cities = new();
        var productionItems = ProductionOrder.GetAll(rules);
        var totalUnitOrders = productionItems.Count(o => o is UnitProductionOrder);
        for (int i = 0; i < numberOfCities; i++)
        {
            totOffset = 0;
            var x = BitConverter.ToInt16(bytes, ofsetC + multipl * i + 0);
            var y = BitConverter.ToInt16(bytes, ofsetC + multipl * i + 2);
            var mapIndex = 0;
            if (gameVersion > 44)  // TOT
            {
                mapIndex = BitConverter.ToInt16(bytes, ofsetC + multipl * i + 4);
                totOffset += 2;
            }
            var civilDisorder = GetBit(bytes[ofsetC + multipl * i + 4 + totOffset], 0);
            var weLoveKingDay = GetBit(bytes[ofsetC + multipl * i + 4 + totOffset], 1);
            var improvementSold = GetBit(bytes[ofsetC + multipl * i + 4 + totOffset], 2);
            var stolenTech = GetBit(bytes[ofsetC + multipl * i + 4 + totOffset], 3);
            var autobuildMilitaryRule = GetBit(bytes[ofsetC + multipl * i + 4 + totOffset], 4);
            var canBuildCoastal = GetBit(bytes[ofsetC + multipl * i + 4 + totOffset], 7);
            var canBuildHydro = GetBit(bytes[ofsetC + multipl * i + 5 + totOffset], 3);
            var canBuildShips = GetBit(bytes[ofsetC + multipl * i + 6 + totOffset], 5);
            var autobuildMilitaryAdvisor = GetBit(bytes[ofsetC + multipl * i + 7 + totOffset], 0);
            var autobuildDomesticAdvisor = GetBit(bytes[ofsetC + multipl * i + 7 + totOffset], 1);
            var objectivex1 = GetBit(bytes[ofsetC + multipl * i + 7 + totOffset], 2);
            var objectivex3 = GetBit(bytes[ofsetC + multipl * i + 7 + totOffset], 4);
            var ownerIndex = bytes[ofsetC + multipl * i + 8 + totOffset];
            var size = bytes[ofsetC + multipl * i + 9 + totOffset];
            var whoBuiltIt = bytes[ofsetC + multipl * i + 10 + totOffset];
            var turnsExpiredSinceCaptured = bytes[ofsetC + multipl * i + 11 + totOffset];
            var whoKnowsAboutIt = new bool[8];
            var lastSizeRevealedToCivs = new int[8];
            for (int civId = 0; civId < 8; civId++)
            {
                whoKnowsAboutIt[civId] = GetBit(bytes[ofsetC + multipl * i + 12 + totOffset], civId);
            }
            for (int civId = 0; civId < 8; civId++)
            {
                lastSizeRevealedToCivs[civId] = bytes[ofsetC + multipl * i + 13 + civId + totOffset];
            }

            // Specialists (limited to reading 16 for each city)
            var specialists = new byte[16];
            for (int j = 0; j < 4; j++)
            {
                var bt = bytes[ofsetC + multipl * i + 22 + j + totOffset];
                for (int bitPair = 0; bitPair < 4; bitPair++)
                {
                    bool bit1 = GetBit(bt, 2 * bitPair + 0);
                    bool bit2 = GetBit(bt, 2 * bitPair + 1);

                    if (!bit1 && !bit2) specialists[4 * j + bitPair] = 0; // No specialists (0-0)
                    else if (bit1 && !bit2) specialists[4 * j + bitPair] = 1; // Entertainer (0-1)
                    else if (!bit1 && bit2) specialists[4 * j + bitPair] = 2; // Taxman (1-0)
                    else specialists[4 * j + bitPair] = 3; // Scientist (1-1)
                }
            }

            var foodInStorage = BitConverter.ToInt16(bytes, ofsetC + multipl * i + 26 + totOffset);
            var shieldsProgress = BitConverter.ToInt16(bytes, ofsetC + multipl * i + 28 + totOffset);
            var netTrade = BitConverter.ToInt16(bytes, ofsetC + multipl * i + 30 + totOffset);
            var name = ReadString(bytes, 16, ofsetC + multipl * i + 32 + totOffset);

            // Distribution of workers on map in city view
            // 1st byte - inner circle (starting from N, going in counter-clokwise direction)
            // 2nd byte - 8 on outer circle
            // 3nd byte - 4 on outer circle + city square
            var distributionWorkers = new bool[21];
            for (int bit = 0; bit < 8; bit++)
            {
                distributionWorkers[0 * 8 + bit] = GetBit(bytes[ofsetC + multipl * i + 48 + totOffset + 0], 7 - bit);
                distributionWorkers[1 * 8 + bit] = GetBit(bytes[ofsetC + multipl * i + 48 + totOffset + 1], 7 - bit);
                if (bit > 2)
                {
                    distributionWorkers[2 * 8 + bit - 3] = GetBit(bytes[ofsetC + multipl * i + 48 + totOffset + 2], 7 - bit);
                }
            }

            var noOfSpecialistsx4 = bytes[ofsetC + multipl * i + 51 + totOffset];   // e.g. 8 = 2 specialists

            // Improvements
            int noImprovements = gameVersion <= 44 ? 34 : 35;  // TOT has additional transporter
            var improvements = new bool[noImprovements];
            int count = 0;
            for (int block = 0; block < 5; block++)
            {
                for (int bit = 0; bit < 8; bit++)
                {
                    if (block == 0 && bit == 0) // skip 'nothing'
                        continue;

                    improvements[count] = GetBit(bytes[ofsetC + multipl * i + 52 + totOffset + block], bit);
                    count++;

                    if (count == noImprovements)
                        break;
                }
            }

            // Item in production
            // 0,1,2...61(MGE)/79(TOT) are units, improvements are inversed (FF(hex)=1st, FE(hex)=2nd, ...)
            var itemInProduction = (sbyte)bytes[ofsetC + multipl * i + 57 + totOffset];

            var activeTradeRoutes = bytes[ofsetC + multipl * i + 58 + totOffset];

            // 1st, 2nd, 3rd trade commodities supplied
            var commoditySupplied = new int[] { bytes[ofsetC + multipl * i + 59 + totOffset], bytes[ofsetC + multipl * i + 60 + totOffset], bytes[ofsetC + multipl * i + 61 + totOffset] };

            // 1st, 2nd, 3rd trade commodities demanded
            var commodityDemanded = new int[] { bytes[ofsetC + multipl * i + 62 + totOffset], bytes[ofsetC + multipl * i + 63 + totOffset], bytes[ofsetC + multipl * i + 64 + totOffset] };

            // 1st, 2nd, 3rd trade commodities in route
            var commodityInRoute = new int[] { bytes[ofsetC + multipl * i + 65 + totOffset], bytes[ofsetC + multipl * i + 66 + totOffset], bytes[ofsetC + multipl * i + 67 + totOffset] };

            // 1st, 2nd, 3rd trade route partner city number
            var tradeRoutePartnerCity = new int[]
            {
                BitConverter.ToInt16(bytes, ofsetC + multipl * i + 68 + totOffset),
                BitConverter.ToInt16(bytes, ofsetC + multipl * i + 70 + totOffset),
                BitConverter.ToInt16(bytes, ofsetC + multipl * i + 72 + totOffset)
            };

            var science = BitConverter.ToInt16(bytes, ofsetC + multipl * i + 74 + totOffset);
            var tax = BitConverter.ToInt16(bytes, ofsetC + multipl * i + 76 + totOffset);
            var noOfTradeIcons = BitConverter.ToInt16(bytes, ofsetC + multipl * i + 78 + totOffset);
            var totalFoodProduction = bytes[ofsetC + multipl * i + 80 + totOffset];
            var totalShieldProduction = bytes[ofsetC + multipl * i + 81 + totOffset];
            var happyCitizens = bytes[ofsetC + multipl * i + 82 + totOffset];
            var unhappyCitizens = bytes[ofsetC + multipl * i + 83 + totOffset];

            var tile = objects.Maps[mapIndex].TileC2(x, y);
            var owner = objects.Civilizations[ownerIndex]; 
            var city = new City
            {
                X = x,
                Y = y,
                MapIndex = mapIndex,
                CanBuildCoastal = canBuildCoastal,
                AutobuildMilitaryRule = autobuildMilitaryRule,
                StolenTech = stolenTech,
                ImprovementSold = improvementSold,
                WeLoveKingDay = weLoveKingDay,
                CivilDisorder = civilDisorder,
                CanBuildHydro = canBuildHydro,
                CanBuildShips = canBuildShips,
                AutobuildMilitaryAdvisor = autobuildMilitaryRule,
                AutobuildDomesticAdvisor = autobuildDomesticAdvisor,
                Objective = (objectivex1 ? 1 : 0) + (objectivex3 ? 3 : 0),
                Owner = owner,
                Size = size,
                WhoBuiltIt = objects.Civilizations[whoBuiltIt],
                WhoKnowsAboutIt = whoKnowsAboutIt,
                LastSizeRevealedToCivs = lastSizeRevealedToCivs,
                FoodInStorage = foodInStorage,
                ShieldsProgress = shieldsProgress,
                NetTrade = netTrade,
                Name = name,
                NoOfSpecialistsx4 = noOfSpecialistsx4,
                ItemInProduction = itemInProduction >= 0 ? productionItems[itemInProduction] :
                        productionItems[totalUnitOrders - itemInProduction - 1],
                ActiveTradeRoutes = activeTradeRoutes,
                CommoditySupplied = commoditySupplied.Where(c => c < rules.CaravanCommoditie.Length).Select(c => rules.CaravanCommoditie[c]).ToArray(),
                CommodityDemanded = commodityDemanded.Where(c => c < rules.CaravanCommoditie.Length).Select(c => rules.CaravanCommoditie[c]).ToArray(),
                TradeRoutes = commodityInRoute.Zip(tradeRoutePartnerCity).Select((tuple) => new TradeRoute { Commodity = rules.CaravanCommoditie[tuple.First % rules.CaravanCommoditie.Length], Destination = tuple.Second }).ToArray(),
                //CommodityInRoute = commodityInRoute.Select(c => (CommodityType)c).ToArray(),
                TradeRoutePartnerCity = tradeRoutePartnerCity,
                //Science = science,    //what does this mean???
                //Tax = tax,
                //NoOfTradeIcons = noOfTradeIcons,
                //TotalFoodProduction = totalFoodProduction,    // No need to import this, it's calculated
                //TotalShieldProduction = totalShieldProduction,    // No need to import this, it's calculated
                HappyCitizens = happyCitizens,
                UnhappyCitizens = unhappyCitizens,
                Location = tile
            };

            for (int j = 1; j < 8; j++)
            {
                if (whoKnowsAboutIt[j])
                {
                    tile.PlayerKnowledge[j].CityHere = new CityInfo
                    {
                        Name = name,
                        OwnerId = ownerIndex,
                        Size = lastSizeRevealedToCivs[j]
                    };
                }
            }

            owner.Cities.Add(city);

            foreach (var (first, second) in objects.Maps[mapIndex].CityRadius(tile, true).Zip(distributionWorkers))
            {
                if (first != null && second)
                {
                    first.WorkedBy = city;
                }
            }

            tile.CityHere = city;

            for (var improvementNo = 0; improvementNo < noImprovements; improvementNo++)
                if (improvements[improvementNo]) city.AddImprovement(rules.Improvements[improvementNo + 1]);

            objects.Cities.Add(city);
        }

        // Set home cities of units
        foreach (var civ in objects.Civilizations)
            foreach (var unit in civ.Units)
                unit.HomeCity = unitHomeCity[unit.Id] == 255 ? null : objects.Cities[unitHomeCity[unit.Id]];
        #endregion
        #region Data for finding next city name
        //=========================
        //DATA FOR FINDING NEXT CITY NAME
        //=========================
        int ofsetTc = ofsetC + multipl * numberOfCities;
        foreach (Civilization civ in objects.Civilizations)
        {
            if (civ.PlayerType != PlayerType.Barbarians)
            {
                int ofsetTcThisCiv = ofsetTc + 3 * civ.TribeId + 1;
            }
        }
        #endregion
        #region Other info
        //=========================
        //OTHER INFO
        //=========================
        int ofsetO = ofsetTc + 63;
        
        var activeCursorXy = new short[] { BitConverter.ToInt16(bytes, ofsetO + 0), 
                                            BitConverter.ToInt16(bytes, ofsetO + 2) };

        int noHumanPlayers = 0;
        for (int i = 0; i < 8; i++) 
        { 
            var stat = humanPlayers[i] ? 1 : 0;
            noHumanPlayers += stat;
        }
        int blockOo = gameVersion <= 44 ? 60 * noHumanPlayers + 1302 : 60 * noHumanPlayers + 1314;

        // Clicked tile with your mouse XY position (does not count if you clicked on a city)
        var clickedXy = new int[] { BitConverter.ToInt16(bytes, ofsetO + blockOo + 0), 
                                     BitConverter.ToInt16(bytes, ofsetO + blockOo + 2) };

        // Zoom (=-7(min)...+8(max), 0=std.)
        var zoom = BitConverter.ToInt16(bytes, ofsetO + blockOo + 4);
        
        viewData.Add("Zoom", zoom.ToString());

        // Update map data
        for (int mapNo = 0; mapNo < noSecondaryMaps + 1; mapNo++)
        {
            objects.Maps[mapNo].StartingClickedXy = clickedXy;
        }
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

        bool totalWar, objectiveVictory, countWondersAsObjectives, forbidGovernmentSwitching, 
            forbidTechFromConquests, elliminatePollution, terrainAnimationLockout, unitAnimationLockout,
            spRfileOverride, specialWwiIonlyAi;
        totalWar = objectiveVictory = countWondersAsObjectives = forbidGovernmentSwitching = 
            forbidTechFromConquests = elliminatePollution = terrainAnimationLockout = unitAnimationLockout =
            spRfileOverride = specialWwiIonlyAi = false;
        int techParadigm, turnYearIncrement, startingYear, maxTurns, objectiveProtagonist, 
            noObjectivesDecisiveVictory, noObjectivesMarginalVictory, noObjectivesMarginalDefeat, 
            noObjectivesDecisiveDefeat;
        techParadigm = turnYearIncrement = startingYear = maxTurns = objectiveProtagonist = 
            noObjectivesDecisiveVictory = noObjectivesMarginalVictory = noObjectivesMarginalDefeat =
            noObjectivesDecisiveDefeat = 0;
        string scenarioName = string.Empty;
        if (scnFileOrDerived)
        {
            totalWar = GetBit(bytes[ofsetS + 0], 0);
            objectiveVictory = GetBit(bytes[ofsetS + 0], 1);
            countWondersAsObjectives = GetBit(bytes[ofsetS + 0], 2);
            forbidGovernmentSwitching = GetBit(bytes[ofsetS + 0], 4);
            forbidTechFromConquests = GetBit(bytes[ofsetS + 0], 5);
            elliminatePollution = GetBit(bytes[ofsetS + 0], 6);
            terrainAnimationLockout = GetBit(bytes[ofsetS + 0], 7);    // TOT only
            unitAnimationLockout = GetBit(bytes[ofsetS + 1], 0);    // TOT only
            spRfileOverride = GetBit(bytes[ofsetS + 1], 1);
            specialWwiIonlyAi = GetBit(bytes[ofsetS + 1], 7);

            // Scenario name (read till first null character)
            int step = 0;
            List<char> chars = new();
            while (bytes[ofsetS + 2 + step] != 0x0)
            {
                chars.Add((char)bytes[ofsetS + 2 + step]);
                step++;
            }
            scenarioName = String.Concat(chars);

            techParadigm = BitConverter.ToInt16(bytes, ofsetS + 82);
            turnYearIncrement = BitConverter.ToInt16(bytes, ofsetS + 84);
            startingYear = BitConverter.ToInt16(bytes, ofsetS + 86);
            maxTurns = BitConverter.ToInt16(bytes, ofsetS + 88);
            objectiveProtagonist = bytes[ofsetS + 90];
            noObjectivesDecisiveVictory = BitConverter.ToInt16(bytes, ofsetS + 92);
            noObjectivesMarginalVictory = BitConverter.ToInt16(bytes, ofsetS + 94);
            noObjectivesMarginalDefeat = BitConverter.ToInt16(bytes, ofsetS + 96);
            noObjectivesDecisiveDefeat = BitConverter.ToInt16(bytes, ofsetS + 98);

            // Correct offset
            ofsetS += 100;
        }
        #endregion
        #region Destroyed civs
        //=========================
        //DESTROYED CIVS
        //=========================
        var turnCivDestroyed = new short[12];
        var civIdThatDestroyedOtherCiv = new byte[12];
        var civIdThatWasDestroyed = new byte[12];
        var nameOfDestroyedCiv = new string[12];
        var noCivsDestroyed = BitConverter.ToInt16(bytes, ofsetS + 0);
        for (int i = 0; i < 12; i++)
        {
            turnCivDestroyed[i] = BitConverter.ToInt16(bytes, ofsetS + 2 + 2 * i);
        }
        for (int i = 0; i < 12; i++)
        {
            civIdThatDestroyedOtherCiv[i] = bytes[ofsetS + 26 + i];
        }
        for (int i = 0; i < 12; i++)
        {
            civIdThatWasDestroyed[i] = bytes[ofsetS + 38 + i];
        }
        for (int i = 0; i < 12; i++)
        {
            nameOfDestroyedCiv[i] = ReadString(bytes, 24, ofsetS + 50 + 24 * i);
        }
        #endregion
        #region Events
        //=========================
        //EVENTS
        //=========================
        List<ScenarioEvent> events = new();
        ScenarioEvent @event = new();
        int[]? flags = null;
        var offsetE = IndexofStringInByteArray(bytes, "EVNT");
        if (offsetE != -1)
        {
            offsetE += 4;
            var numberOfEvents = BitConverter.ToInt16(bytes, offsetE);

            offsetE += 4;
            multipl = gameVersion <= 44 ? 444 : 276;  // no of bytes for each event

            // Read strings
            var eventStrings = ReadEventStrings(bytes, offsetE + multipl * numberOfEvents);

            for (int i = 0; i < numberOfEvents; i++)
            {
                var eventTriggerIds = BitConverter.ToInt32(bytes, offsetE + multipl * i + 0);
                var eventActionIds = FindPositionOfBits(BitConverter.ToInt32(bytes, offsetE + multipl * i + 4)).ToArray();
                bool[] eventModifiers;
                byte[] eventTriggerParam, eventActionParam;

                if (gameVersion <= 44) // <= MGE
                {
                    eventModifiers = new bool[32];

                    // Trigger parameters
                    eventTriggerParam = new byte[48];
                    for (int j = 0; j < 48; j++)
                        eventTriggerParam[j] = bytes[offsetE + multipl * i + 8 + j];

                    // Action parameters
                    eventActionParam = new byte[388];
                    for (int j = 0; j < 388; j++)
                        eventActionParam[j] = bytes[offsetE + multipl * i + 56 + j];
                }
                else    // TOT
                {
                    // Event modifiers
                    eventModifiers = new bool[32];
                    for (int j = 0; j < 4; j++)
                        for (int b = 0; b < 8; b++)
                            eventModifiers[8 * j + b] = GetBit(bytes[offsetE + multipl * i + 8 + j], b);

                    // Trigger parameters
                    eventTriggerParam = new byte[39];
                    for (int j = 0; j < 39; j++)
                        eventTriggerParam[j] = bytes[offsetE + multipl * i + 12 + j];

                    // Action parameters
                    eventActionParam = new byte[225];
                    for (int j = 0; j < 225; j++)
                        eventActionParam[j] = bytes[offsetE + multipl * i + 51 + j];
                }

                // Make events from read data
                // @INITFLAG
                if (eventModifiers[11])
                {
                    flags = new int[9];
                    for (int j = 0; j < 8; j++)
                    {
                        flags[j] = BitConverter.ToInt32(new byte[4] {
                            eventActionParam[85 + 4 * j], eventActionParam[86 + 4 * j],
                            eventActionParam[87 + 4 * j], eventActionParam[88 + 4 * j] });
                    }
                    flags[8] = BitConverter.ToInt32(new byte[4] {
                        eventActionParam[129], eventActionParam[130],
                        eventActionParam[131], eventActionParam[132] });
                }
                else
                {
                    // Second trigger with @AND modifier
                    if (eventModifiers[30])
                    {
                        @event.Trigger2 = CreateScenarioTrigger(gameVersion, eventTriggerIds,
                                eventModifiers, eventTriggerParam, eventStrings, objects);
                        events.Add(@event);
                    }
                    else
                    {
                        @event = new ScenarioEvent
                        {
                            Trigger = CreateScenarioTrigger(gameVersion, eventTriggerIds,
                                eventModifiers, eventTriggerParam, eventStrings, objects),
                            Actions = CreateScenarioActions(gameVersion, eventActionIds,
                                eventModifiers, eventActionParam, eventStrings, objects),
                            Delay = eventModifiers[1] ?
                                BitConverter.ToInt16(new byte[2] { eventActionParam[195], eventActionParam[196] }) : null,
                            JustOnce = eventActionIds.Contains(6),
                            Continuous = eventModifiers[7]
                        };

                        // Add event to list unless it's the first trigger with @AND modifier
                        if (!eventModifiers[28])
                        {
                            events.Add(@event);
                        }
                    }
                }
            }
        }

        objects.Scenario = new Scenario
        {
            Events = events,
            Flags = flags == null ? null : flags,
            TotalWar = totalWar,
            ObjectiveVictory = objectiveVictory,
            CountWondersAsObjectives = countWondersAsObjectives,
            ForbidGovernmentSwitching = forbidGovernmentSwitching,
            ForbidTechFromConquests = forbidTechFromConquests,
            ElliminatePollution = elliminatePollution,
            SpecialWwiIonlyAi = specialWwiIonlyAi,
            Name = scenarioName,
            TechParadigm = techParadigm,
            TurnYearIncrement = turnYearIncrement,
            StartingYear = startingYear,
            MaxTurns = maxTurns,
            ObjectiveProtagonist = objectiveProtagonist,
            NoObjectivesDecisiveVictory = noObjectivesDecisiveVictory,
            NoObjectivesMarginalVictory = noObjectivesMarginalVictory,
            NoObjectivesMarginalDefeat = noObjectivesMarginalDefeat,
            NoObjectivesDecisiveDefeat = noObjectivesDecisiveDefeat
        };

        // If there are no events in .sav read them from EVENTS.TXT (if it exists)
        if (events.Count == 0 && Directory.EnumerateFiles(activeRuleSet.FolderPath, "events.txt", 
            new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive }).FirstOrDefault() != null)
        {
            objects.Scenario.Events = EventsLoader.LoadEvents(new string[] { activeRuleSet.FolderPath }, rules, objects);
        }
        #endregion

        GameData data = new()
        {
            TurnNumber = turnNumber,
            StartingYear = startingYear,
            TurnYearIncrement = turnYearIncrement,
            DifficultyLevel = difficultyLevel,
            BarbarianActivity = barbarianActivity,
            NoPollutionSkulls = noPollutionSkulls,
            GlobalTempRiseOccured = globalTempRiseOccured,
            NoOfTurnsOfPeace = noOfTurnsOfPeace,
        };

        return Game.Create(rules, data, objects, activeRuleSet, options);
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

    private static List<ConstructedImprovement> GetImprovementsFrom(bool[,] farmlandPresent, bool[,] irrigationPresent,
        bool[,] miningPresent, bool[,] railroadPresent, bool[,] roadPresent, bool[,] fortressPresent,
        bool[,] airbasePresent, bool[,] pollutionPresent, bool[,] transporterPresent, int col, int row)
    {
        var improvements = new List<ConstructedImprovement>();

        if (farmlandPresent[col, row])
        {
            improvements.Add(new ConstructedImprovement
            { Improvement = ImprovementTypes.Irrigation, Group = ImprovementTypes.ProductionGroup, Level = 1 });
        }
        else if (irrigationPresent[col, row])
        {
            improvements.Add(new ConstructedImprovement
            { Improvement = ImprovementTypes.Irrigation, Group = ImprovementTypes.ProductionGroup, Level = 0 });
        }
        else if (miningPresent[col, row])
        {
            improvements.Add(new ConstructedImprovement
            { Improvement = ImprovementTypes.Mining, Group = ImprovementTypes.ProductionGroup, Level = 0 });
        }

        if (railroadPresent[col, row])
        {
            improvements.Add(new ConstructedImprovement { Improvement = ImprovementTypes.Road, Level = 1 });
        }
        else if (roadPresent[col, row])
        {
            improvements.Add(new ConstructedImprovement { Improvement = ImprovementTypes.Road, Level = 0 });
        }

        if (fortressPresent[col, row])
        {
            improvements.Add(new ConstructedImprovement
            { Improvement = ImprovementTypes.Fortress, Group = ImprovementTypes.DefenceGroup, Level = 0 });
        }
        else if (airbasePresent[col, row])
        {
            improvements.Add(new ConstructedImprovement
            { Improvement = ImprovementTypes.Airbase, Group = ImprovementTypes.DefenceGroup, Level = 0 });
        }

        if (pollutionPresent[col, row])
        {
            improvements.Add(new ConstructedImprovement
            {
                Improvement = ImprovementTypes.Pollution,
                Level = 0
            });
        }

        if (transporterPresent[col, row])
        {
            improvements.Add(new ConstructedImprovement
            {
                Improvement = ImprovementTypes.Transporter,
                Level = 0
            });
        }
        return improvements;
    }

    private static PlayerTile[] GetPlayerKnowledgeFrom(bool[,,] irrigationVisibility, bool[,,] miningVisibility, bool[,,] roadVisibility,
        bool[,,] railroadVisibility, bool[,,] fortressVisibility, bool[,,] pollutionVisibility, bool[,,] airbaseVisibility,
        bool[,,] farmlandVisibility, bool[,,] transporterVisibility, int col, int row)
    {
        return Enumerable.Range(0, 8).Select(i => BuildPlayerTileKnowledge(
            irrigationVisibility[col, row, i],
            miningVisibility[col, row, i],
            roadVisibility[col, row, i],
            railroadVisibility[col, row, i],
            fortressVisibility[col, row, i],
            pollutionVisibility[col, row, i],
            airbaseVisibility[col, row, i],
            farmlandVisibility[col, row, i],
            transporterVisibility[col, row, i])).ToArray();
    }

    private static PlayerTile BuildPlayerTileKnowledge(
            bool irrigation, bool mining, bool road, bool railroad, bool fortress,
            bool pollution, bool airbase, bool farmland, bool transporter)
    {
        var tileKnowledge = new PlayerTile();

        if (farmland)
        {
            tileKnowledge.Improvements.Add(new ConstructedImprovement
            { Improvement = ImprovementTypes.Irrigation, Group = ImprovementTypes.ProductionGroup, Level = 1 });
        }
        else if (irrigation)
        {
            tileKnowledge.Improvements.Add(new ConstructedImprovement
            { Improvement = ImprovementTypes.Irrigation, Group = ImprovementTypes.ProductionGroup, Level = 0 });
        }
        else if (mining)
        {
            tileKnowledge.Improvements.Add(new ConstructedImprovement
            { Improvement = ImprovementTypes.Mining, Group = ImprovementTypes.ProductionGroup, Level = 0 });
        }

        if (railroad)
        {
            tileKnowledge.Improvements.Add(new ConstructedImprovement { Improvement = ImprovementTypes.Road, Level = 1 });
        }
        else if (road)
        {
            tileKnowledge.Improvements.Add(new ConstructedImprovement { Improvement = ImprovementTypes.Road, Level = 0 });
        }

        if (fortress)
        {
            tileKnowledge.Improvements.Add(new ConstructedImprovement
            { Improvement = ImprovementTypes.Fortress, Group = ImprovementTypes.DefenceGroup, Level = 0 });
        }
        else if (airbase)
        {
            tileKnowledge.Improvements.Add(new ConstructedImprovement
            { Improvement = ImprovementTypes.Airbase, Group = ImprovementTypes.DefenceGroup, Level = 0 });
        }

        if (pollution)
        {
            tileKnowledge.Improvements.Add(new ConstructedImprovement
            {
                Improvement = ImprovementTypes.Pollution,
                Level = 0
            });
        }

        if (transporter)
        {
            tileKnowledge.Improvements.Add(new ConstructedImprovement
            {
                Improvement = ImprovementTypes.Transporter,
                Level = 0
            });
        }

        return tileKnowledge;
    }

    private static ITrigger CreateScenarioTrigger(byte version, int triggerId, bool[] modifiers,
            byte[] triggerParam, List<string> strings, ClassicSaveObjects objects)
    {
        ITrigger? trigger = default;
        switch (triggerId)
        {
            case 0x1:
                trigger = new UnitKilled
                {
                    UnitKilledId = version <= 44 ? triggerParam[4] : triggerParam[29],
                    AttackerCivId = triggerParam[16],
                    DefenderCivId = version <= 44 ? triggerParam[28] : triggerParam[20],
                    DefenderOnly = modifiers[21],
                    MapId = modifiers[24] ? 0 :
                            modifiers[25] ? 1 :
                            modifiers[26] ? 2 :
                            modifiers[27] ? 3 : 0,
                    Strings = strings.GetRange(0, 3),
                };
                strings.RemoveRange(0, 3);
                break;

            case 0x2:
                trigger = new CityTaken
                {
                    City = objects.Cities.Find(c => c.Name == strings[0]),
                    AttackerCivId = triggerParam[16],
                    DefenderCivId = version <= 44 ? triggerParam[28] : triggerParam[20],
                    IsUnitSpy = modifiers[6],
                    Strings = strings.GetRange(0, 3),
                };
                strings.RemoveRange(0, 3);
                break;

            case 0x4:
                trigger = new TurnTrigger()
                {
                    Turn = version <= 44 ?
                            BitConverter.ToInt16(new byte[2] { triggerParam[36], triggerParam[37] }) :
                            BitConverter.ToInt16(new byte[2] { triggerParam[30], triggerParam[31] })
                };
                break;

            case 0x8:
                trigger = new TurnInterval
                {
                    Interval = version <= 44 ?
                            BitConverter.ToInt16(new byte[2] { triggerParam[36], triggerParam[37] }) :
                            BitConverter.ToInt16(new byte[2] { triggerParam[30], triggerParam[31] })
                };
                break;

            case 0x10:
                // If all string pointers are 0, it's the second type
                if (!(triggerParam[8] == 0 && triggerParam[9] == 0 && triggerParam[10] == 0 && triggerParam[11] == 0
                    && triggerParam[12] == 0 && triggerParam[13] == 0 && triggerParam[14] == 0 && triggerParam[15] == 0))
                {
                    trigger = new Negotiation1
                    {
                        TalkerCivId = triggerParam[16],
                        TalkerType = version <= 44 ? triggerParam[20] : triggerParam[8],
                        ListenerCivId = version <= 44 ? triggerParam[28] : triggerParam[20],
                        ListenerType = version <= 44 ? triggerParam[32] : triggerParam[12],
                        Strings = strings.GetRange(0, 2),
                    };
                    strings.RemoveRange(0, 2);
                }
                else
                {
                    trigger = new Negotiation2
                    {
                        TalkerMask = BitConverter.ToInt32(new byte[4] { triggerParam[16], triggerParam[17], triggerParam[18], triggerParam[19] }),
                        ListenerMask = BitConverter.ToInt32(new byte[4] { triggerParam[20], triggerParam[21], triggerParam[22], triggerParam[23] })
                    };
                }
                break;

            case 0x20:
                trigger = new ScenarioLoaded { };
                break;

            case 0x40:
                trigger = new RandomTurn
                {
                    Denominator = version <= 44 ? triggerParam[40] : triggerParam[32]
                };
                break;

            case 0x80:
                trigger = new NoSchism
                {
                    CivId = version <= 44 ? triggerParam[28] : triggerParam[20],
                    Strings = strings.GetRange(0, 1),
                };
                strings.RemoveRange(0, 1);
                break;

            case 0x100:
                trigger = new ReceivedTechnology
                {
                    TechnologyId = version <= 44 ? triggerParam[44] : triggerParam[36],
                    ReceiverCivId = version <= 44 ? triggerParam[28] : triggerParam[20],
                    IsFutureTech = modifiers[8],
                    Strings = strings.GetRange(0, 1)
                };
                strings.RemoveRange(0, 1);
                break;

            case 0x200:
                trigger = new CityProduction
                {
                    BuilderCivId = triggerParam[20],
                    ImprovementUnitId = triggerParam[37],
                    Strings = strings.GetRange(0, 1)
                };
                strings.RemoveRange(0, 1);
                break;

            case 0x400:
                trigger = new AlphaCentauriArrival
                {
                    RaceCivId = triggerParam[16],
                    Size = triggerParam[38],
                    Strings = strings.GetRange(0, 1)
                };
                strings.RemoveRange(0, 1);
                break;

            case 0x800:
                trigger = new CityDestroyed
                {
                    OwnerId = triggerParam[20],
                    CityId = triggerParam[28],
                    Strings = strings.GetRange(0, 2)
                };
                strings.RemoveRange(0, 2);
                break;

            case 0x1000:
                trigger = new BribeUnit
                {
                    WhoCivId = triggerParam[16],
                    WhomCivId = triggerParam[20],
                    UnitTypeId = triggerParam[28],
                    Strings = strings.GetRange(0, 2)
                };
                strings.RemoveRange(0, 2);
                break;

            case 0x2000:
                trigger = new CheckFlag
                {
                    State = modifiers[10],
                    CountUsed = modifiers[15],
                    TechnologyUsed = modifiers[18],
                    WhoId = triggerParam[16],
                    FlagMask = BitConverter.ToInt32(new byte[4] { triggerParam[24], triggerParam[25], triggerParam[26], triggerParam[27] }),
                    TechnologyId = triggerParam[36],
                    CountThreshold = triggerParam[38],
                    Strings = strings.GetRange(0, 1)
                };
                strings.RemoveRange(0, 1);
                break;

            default:
                break;
        }

        return trigger;
    }

    private static List<IScenarioAction> CreateScenarioActions(int version, int[] actionIds, bool[] modifiers,
        byte[] actionParam, List<string> strings, ClassicSaveObjects objects)
    {
        var actions = new List<IScenarioAction>();
        for (int i = 0; i < actionIds.Length; i++)
        {
            switch (actionIds[i])
            {
                case 0:
                    List<string> texts = new();
                    for (int j = 0; j < 10; j++)
                    {
                        // One string pointer exists = one line of text
                        if (actionParam[1 + 4 * j + 0] != 0 || actionParam[1 + 4 * j + 1] != 0 ||
                            actionParam[1 + 4 * j + 2] != 0 || actionParam[1 + 4 * j + 3] != 0)    // TODO: determine for MGE
                        {
                            texts.Add(strings[0]);
                            strings.RemoveRange(0, 1);
                        }
                    }
                    actions.Add(new TextAction
                    {
                        NoBroadcast = modifiers[22],
                        Strings = new List<string>(texts)
                    });
                    break;

                case 1:
                    actions.Add(new MoveUnit
                    {
                        OwnerCivId = version <= 44 ? actionParam[84] : actionParam[0],
                        UnitMovedId = version <= 44 ? actionParam[92] : actionParam[197],
                        MapId = version <= 44 ? 0 : actionParam[198],
                        NumberToMove = version <= 44 ? actionParam[96] : actionParam[129],
                        MapCoords = version <= 44 ?
                            new int[4, 2]
                            {
                                    { BitConverter.ToInt16(new byte[2] { actionParam[100], actionParam[101] }),
                                      BitConverter.ToInt16(new byte[2] { actionParam[104], actionParam[105] }) },
                                    { BitConverter.ToInt16(new byte[2] { actionParam[108], actionParam[109] }),
                                      BitConverter.ToInt16(new byte[2] { actionParam[112], actionParam[113] }) },
                                    { BitConverter.ToInt16(new byte[2] { actionParam[116], actionParam[117] }),
                                      BitConverter.ToInt16(new byte[2] { actionParam[120], actionParam[121] }) },
                                    { BitConverter.ToInt16(new byte[2] { actionParam[124], actionParam[125] }),
                                      BitConverter.ToInt16(new byte[2] { actionParam[128], actionParam[129] }) }
                            } : new int[4, 2]
                            {
                                    { BitConverter.ToInt16(new byte[2] { actionParam[93], actionParam[94] }),
                                      BitConverter.ToInt16(new byte[2] { actionParam[95], actionParam[96] }) },
                                    { BitConverter.ToInt16(new byte[2] { actionParam[97], actionParam[98] }),
                                      BitConverter.ToInt16(new byte[2] { actionParam[99], actionParam[100] }) },
                                    { BitConverter.ToInt16(new byte[2] { actionParam[101], actionParam[102] }),
                                      BitConverter.ToInt16(new byte[2] { actionParam[103], actionParam[104] }) },
                                    { BitConverter.ToInt16(new byte[2] { actionParam[105], actionParam[106] }),
                                      BitConverter.ToInt16(new byte[2] { actionParam[107], actionParam[108] }) }
                            },
                        MapDest = version <= 44 ?
                            new int[2]
                            {
                                    BitConverter.ToInt16(new byte[2] { actionParam[132], actionParam[133] }),
                                    BitConverter.ToInt16(new byte[2] { actionParam[136], actionParam[137] })
                            } : new int[2]
                            {
                                    BitConverter.ToInt16(new byte[2] { actionParam[125], actionParam[126] }),
                                    BitConverter.ToInt16(new byte[2] { actionParam[127], actionParam[128] })
                            },
                        Strings = strings.GetRange(0, 2),
                    });
                    strings.RemoveRange(0, 2);
                    break;

                case 2:
                    actions.Add(new CreateUnit
                    {
                        OwnerCivId = version <= 44 ? actionParam[160] : actionParam[201],
                        CreatedUnitId = version <= 44 ? actionParam[168] : actionParam[202],
                        Randomize = modifiers[0],
                        InCapital = modifiers[2],
                        Locations = version <= 44 ?
                        new int[10, 3]
                        {
                                { BitConverter.ToInt16(new byte[2] { actionParam[172], actionParam[173] }),
                                  BitConverter.ToInt16(new byte[2] { actionParam[176], actionParam[177] }),
                                  0 },
                                { BitConverter.ToInt16(new byte[2] { actionParam[180], actionParam[181] }),
                                  BitConverter.ToInt16(new byte[2] { actionParam[184], actionParam[185] }),
                                  0 },
                                { BitConverter.ToInt16(new byte[2] { actionParam[188], actionParam[189] }),
                                  BitConverter.ToInt16(new byte[2] { actionParam[192], actionParam[193] }),
                                  0 },
                                { BitConverter.ToInt16(new byte[2] { actionParam[196], actionParam[197] }),
                                  BitConverter.ToInt16(new byte[2] { actionParam[200], actionParam[201] }),
                                  0 },
                                { BitConverter.ToInt16(new byte[2] { actionParam[204], actionParam[205] }),
                                  BitConverter.ToInt16(new byte[2] { actionParam[208], actionParam[209] }),
                                  0 },
                                { BitConverter.ToInt16(new byte[2] { actionParam[212], actionParam[213] }),
                                  BitConverter.ToInt16(new byte[2] { actionParam[216], actionParam[217] }),
                                  0 },
                                { BitConverter.ToInt16(new byte[2] { actionParam[220], actionParam[221] }),
                                  BitConverter.ToInt16(new byte[2] { actionParam[224], actionParam[225] }),
                                  0 },
                                { BitConverter.ToInt16(new byte[2] { actionParam[228], actionParam[229] }),
                                  BitConverter.ToInt16(new byte[2] { actionParam[232], actionParam[233] }),
                                  0 },
                                { BitConverter.ToInt16(new byte[2] { actionParam[236], actionParam[237] }),
                                  BitConverter.ToInt16(new byte[2] { actionParam[240], actionParam[241] }),
                                  0 },
                                { BitConverter.ToInt16(new byte[2] { actionParam[244], actionParam[245] }),
                                  BitConverter.ToInt16(new byte[2] { actionParam[248], actionParam[249] }),
                                  0 },
                        } : new int[10, 3]
                        {
                                { BitConverter.ToInt16(new byte[2] { actionParam[133], actionParam[134] }),
                                  BitConverter.ToInt16(new byte[2] { actionParam[135], actionParam[136] }),
                                  BitConverter.ToInt16(new byte[2] { actionParam[137], actionParam[138] }) },
                                { BitConverter.ToInt16(new byte[2] { actionParam[139], actionParam[140] }),
                                  BitConverter.ToInt16(new byte[2] { actionParam[141], actionParam[142] }),
                                  BitConverter.ToInt16(new byte[2] { actionParam[143], actionParam[144] }) },
                                { BitConverter.ToInt16(new byte[2] { actionParam[145], actionParam[146] }),
                                  BitConverter.ToInt16(new byte[2] { actionParam[147], actionParam[148] }),
                                  BitConverter.ToInt16(new byte[2] { actionParam[149], actionParam[150] }) },
                                { BitConverter.ToInt16(new byte[2] { actionParam[151], actionParam[152] }),
                                  BitConverter.ToInt16(new byte[2] { actionParam[153], actionParam[154] }),
                                  BitConverter.ToInt16(new byte[2] { actionParam[155], actionParam[156] }) },
                                { BitConverter.ToInt16(new byte[2] { actionParam[157], actionParam[158] }),
                                  BitConverter.ToInt16(new byte[2] { actionParam[159], actionParam[160] }),
                                  BitConverter.ToInt16(new byte[2] { actionParam[161], actionParam[162] }) },
                                { BitConverter.ToInt16(new byte[2] { actionParam[163], actionParam[164] }),
                                  BitConverter.ToInt16(new byte[2] { actionParam[165], actionParam[166] }),
                                  BitConverter.ToInt16(new byte[2] { actionParam[167], actionParam[168] }) },
                                { BitConverter.ToInt16(new byte[2] { actionParam[169], actionParam[170] }),
                                  BitConverter.ToInt16(new byte[2] { actionParam[171], actionParam[172] }),
                                  BitConverter.ToInt16(new byte[2] { actionParam[173], actionParam[174] }) },
                                { BitConverter.ToInt16(new byte[2] { actionParam[175], actionParam[176] }),
                                  BitConverter.ToInt16(new byte[2] { actionParam[177], actionParam[178] }),
                                  BitConverter.ToInt16(new byte[2] { actionParam[179], actionParam[180] }) },
                                { BitConverter.ToInt16(new byte[2] { actionParam[181], actionParam[182] }),
                                  BitConverter.ToInt16(new byte[2] { actionParam[183], actionParam[184] }),
                                  BitConverter.ToInt16(new byte[2] { actionParam[185], actionParam[186] }) },
                                { BitConverter.ToInt16(new byte[2] { actionParam[187], actionParam[188] }),
                                  BitConverter.ToInt16(new byte[2] { actionParam[189], actionParam[190] }),
                                  BitConverter.ToInt16(new byte[2] { actionParam[191], actionParam[192] }) }
                        },
                        NoLocations = version <= 44 ? 10 : actionParam[203],    // TODO: where is this read for MGE?
                        Veteran = version <= 44 ? actionParam[256] == 1 : actionParam[204] == 1,
                        Count = version <= 44 ? 1 : actionParam[205],    // TODO: where is this read for MGE?
                        HomeCity = objects.Cities.Find(c => c.Name == strings[2]),
                        Strings = strings.GetRange(0, 3)
                    });
                    strings.RemoveRange(0, 3);
                    break;

                case 3:
                    actions.Add(new ChangeMoney
                    {
                        ReceiverCivId = version <= 44 ? actionParam[320] : actionParam[206],
                        Amount = version <= 44 ? actionParam[324] : actionParam[131],
                        Strings = strings.GetRange(0, 1)
                    });
                    strings.RemoveRange(0, 1);
                    break;

                case 4:
                    actions.Add(new PlayWav
                    {
                        File = strings.GetRange(0, 1).FirstOrDefault(),
                        Strings = strings.GetRange(0, 1)
                    });
                    strings.RemoveRange(0, 1);
                    break;

                case 5:
                    actions.Add(new MakeAggression
                    {
                        WhomCivId = version <= 44 ? actionParam[144] : actionParam[199],
                        WhoCivId = version <= 44 ? actionParam[152] : actionParam[200],
                        Strings = strings.GetRange(0, 2)
                    });
                    strings.RemoveRange(0, 2);
                    break;

                case 7:
                    actions.Add(new PlayCDtrack
                    {
                        TrackNo = version <= 44 ? actionParam[336] : actionParam[207]
                    });
                    break;

                case 8:
                    actions.Add(new DontplayWonders { });
                    break;

                case 9:
                    actions.Add(new ChangeTerrain
                    {
                        TerrainTypeId = version <= 44 ? actionParam[340] : actionParam[208],
                        MapCoords = version <= 44 ?
                            new int[4, 2]
                            {
                                    { BitConverter.ToInt16(new byte[2] { actionParam[344], actionParam[345] }),
                                      BitConverter.ToInt16(new byte[2] { actionParam[348], actionParam[349] }) },
                                    { BitConverter.ToInt16(new byte[2] { actionParam[352], actionParam[353] }),
                                      BitConverter.ToInt16(new byte[2] { actionParam[356], actionParam[357] }) },
                                    { BitConverter.ToInt16(new byte[2] { actionParam[360], actionParam[361] }),
                                      BitConverter.ToInt16(new byte[2] { actionParam[364], actionParam[365] }) },
                                    { BitConverter.ToInt16(new byte[2] { actionParam[368], actionParam[369] }),
                                      BitConverter.ToInt16(new byte[2] { actionParam[372], actionParam[373] }) }
                            } : new int[4, 2]
                            {
                                    { BitConverter.ToInt16(new byte[2] { actionParam[109], actionParam[110] }),
                                      BitConverter.ToInt16(new byte[2] { actionParam[111], actionParam[112] }) },
                                    { BitConverter.ToInt16(new byte[2] { actionParam[113], actionParam[114] }),
                                      BitConverter.ToInt16(new byte[2] { actionParam[115], actionParam[116] }) },
                                    { BitConverter.ToInt16(new byte[2] { actionParam[117], actionParam[118] }),
                                      BitConverter.ToInt16(new byte[2] { actionParam[119], actionParam[120] }) },
                                    { BitConverter.ToInt16(new byte[2] { actionParam[121], actionParam[122] }),
                                      BitConverter.ToInt16(new byte[2] { actionParam[123], actionParam[124] }) }
                            },
                        MapId = version <= 44 ? 0 : actionParam[209],
                        ExceptionMask = version <= 44 ? (short)0 :
                            BitConverter.ToInt16(new byte[2] { actionParam[193], actionParam[194] })
                    });
                    break;

                case 10:
                    actions.Add(new DestroyCiv
                    {
                        CivId = version <= 44 ? actionParam[376] : actionParam[210],
                    });
                    break;

                case 11:
                    actions.Add(new GiveTech
                    {
                        TechId = version <= 44 ? actionParam[280] : actionParam[211],
                        CivId = version <= 44 ? actionParam[384] : actionParam[212],
                    });
                    break;

                case 12:
                    actions.Add(new PlayAvi
                    {
                        File = strings.GetRange(0, 1).FirstOrDefault(),
                        Strings = strings.GetRange(0, 1)
                    });
                    strings.RemoveRange(0, 1);
                    break;

                case 13:
                    actions.Add(new EndGameOverride { });
                    break;

                case 14:
                    actions.Add(new EndGame
                    {
                        EndScreens = modifiers[3]
                    });
                    break;

                case 15:
                    actions.Add(new BestowImprovement
                    {
                        RaceId = actionParam[213],
                        ImprovementId = actionParam[214],
                        Randomize = modifiers[0],
                        Capital = modifiers[4],
                        Wonders = modifiers[5]
                    });
                    break;

                case 16:
                    actions.Add(new TransportAction
                    {
                        UnitId = actionParam[221],
                        TransportMask = BitConverter.ToInt16(new byte[2] { actionParam[89], actionParam[90] }),
                        TransportMode = actionParam[92],
                    });
                    break;

                case 17:
                    actions.Add(new TakeTechnology
                    {
                        TechId = actionParam[211],
                        WhomId = actionParam[212],
                        Collapse = actionParam[218] == 1
                    });
                    break;

                case 18:
                    actions.Add(new ModifyReputation
                    {
                        WhoId = actionParam[215],
                        WhomId = actionParam[216],
                        Betray = actionParam[217],
                        Modifier = actionParam[217],
                    });
                    break;

                case 19:
                    actions.Add(new EnableTechnology
                    {
                        TechnologyId = actionParam[211],
                        WhomId = actionParam[212],
                        Value = actionParam[219]
                    });
                    break;

                case 21:
                    actions.Add(new FlagAction
                    {
                        State = modifiers[9],
                        Continuous = modifiers[12],
                        MaskUsed = modifiers[16],
                        Flag = actionParam[85],
                        Mask = BitConverter.ToInt32(new byte[4] { actionParam[85], actionParam[86], actionParam[87], actionParam[88] }),
                        WhoId = actionParam[220],
                    });
                    break;

                case 22:
                    actions.Add(new Negotiator
                    {
                        TypeTalker = modifiers[13],
                        StateSet = modifiers[14],
                        WhoId = actionParam[222]
                    });
                    break;

                default:
                    break;
            }
        }

        return actions;
    }
}
