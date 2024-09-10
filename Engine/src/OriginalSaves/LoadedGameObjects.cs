using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Civ2engine.Advances;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.Production;
using Civ2engine.Terrains;
using Civ2engine.Units;
using Model.Core.Cities;
using Model.Core.Mapping;
using Tile = Civ2engine.MapObjects.Tile;

namespace Civ2engine.OriginalSaves
{
    public class LoadedGameObjects
    {
        public Unit ActiveUnit { get; }
        private Rules Rules { get; }
        public Scenario Scenario { get; }

        public LoadedGameObjects(Rules rules, GameData gameData)
        {
            Rules = rules;
            var maps = new List<Map>();
            for (int mapNo = 0; mapNo < gameData.MapNoSecondaryMaps + 1; mapNo++)
            {
                var map = new Map(gameData.OptionsArray[3], mapNo)
                {
                    MapRevealed = gameData.MapRevealed,
                    WhichCivsMapShown = gameData.WhichCivsMapShown,
                    Zoom = gameData.Zoom,
                    StartingClickedXy = gameData.ClickedXy,
                    XDim = gameData.MapXdimX2 / 2,
                    YDim = gameData.MapYdim,
                    ResourceSeed = gameData.MapResourceSeed,
                    LocatorXdim = gameData.MapLocatorXdim,
                    LocatorYdim = gameData.MapLocatorYdim
                };
                map.Tile = PopulateTilesFromGameData(gameData, Rules, map);
                maps.Add(map);
            }
            this.Maps = maps;
            
            // Create all 8 civs (tribes)
            var civs = new List<Civilization>();
            for (var i = 0; i < 8; i++)
            {
                civs.Add(CreateCiv(i, gameData.PlayersCivIndex, gameData.CivsInPlay[i], gameData.CivCityStyle[i],
                    gameData.CivLeaderName[i], gameData.CivTribeName[i], gameData.CivAdjective[i],
                    gameData.RulerGender[i], gameData.CivMoney[i], gameData.CivNumber[i],
                    gameData.CivResearchProgress[i], gameData.CivResearchingAdvance[i], gameData.CivSciRate[i],
                    gameData.CivTaxRate[i], gameData.CivGovernment[i], gameData.CivReputation[i],
                    gameData.CivPatience[i], gameData.CivTreatyContact, gameData.CivTreatyCeaseFire,
                    gameData.CivTreatyPeace, gameData.CivTreatyAlliance, gameData.CivTreatyVendetta,
                    gameData.CivTreatyEmbassy, gameData.CivTreatyWar, gameData.CivAttitudes, gameData.CivAdvances,
                    gameData.CivLastContact, gameData.CivHasSpaceship[i], gameData.CivSpaceshipEstimatedArrival[i],
                    gameData.CivSpaceshipLaunchYear[i], gameData.CivSpaceshipStructural[i],
                    gameData.CivSpaceshipComponentsPropulsion[i], gameData.CivSpaceshipComponentsFuel[i],
                    gameData.CivSpaceshipModulesHabitation[i], gameData.CivSpaceshipModulesLifeSupport[i],
                    gameData.CivSpaceshipModulesSolarPanel[i]));
            }

            civs[0].PlayerType = PlayerType.Barbarians;
            civs[gameData.PlayersCivIndex].PlayerType = PlayerType.Local;

            this.Civilizations = civs;

            // Create cities
            var cities = new List<City>();
            var productionOrders = ProductionOrder.GetAll(rules);
            var totalUnitOrders = productionOrders.Count(o => o is UnitProductionOrder);
            for (var i = 0; i < gameData.NumberOfCities; i++)
            {
                // Xloc<0 or Yloc<0 => destroyed cities
                //if (gameData.CityYloc[i] < 1)
                //{
                //    Debug.WriteLine($"Bad city data pos{i} ({gameData.CityXloc[i]},{gameData.CityYloc[i]}: {gameData.CityName[i]}");
                //    continue;
                //}
                cities.Add(CreateCity(gameData.CityXloc[i], gameData.CityYloc[i], gameData.CityMapNo[i], 
                    gameData.CityCanBuildCoastal[i], gameData.CityAutobuildMilitaryRule[i], gameData.CityStolenAdvance[i],
                    gameData.CityImprovementSold[i], gameData.CityWeLoveKingDay[i], gameData.CityCivilDisorder[i], 
                    gameData.CityCanBuildHydro[i], gameData.CityCanBuildShips[i], gameData.CityAutobuildMilitaryAdvisor[i],
                    gameData.CityAutobuildDomesticAdvisor[i], gameData.CityObjectivex1[i], gameData.CityObjectivex3[i],
                    gameData.CityOwner[i], gameData.CitySize[i], gameData.CityWhoBuiltIt[i], gameData.CityTurnsExpiredSinceCaptured[i],
                    gameData.CityWhoKnowsAboutIt[i], gameData.CityLastSizeRevealedToCivs[i], gameData.CityFoodInStorage[i],
                    gameData.CityShieldsProgress[i], gameData.CityNetTrade[i], gameData.CityName[i], gameData.CityDistributionWorkers[i],
                    gameData.CityNoOfSpecialistsx4[i], gameData.CityImprovements[i], gameData.CityItemInProduction[i], 
                    gameData.CityActiveTradeRoutes[i], gameData.CityCommoditySupplied[i], gameData.CityCommodityDemanded[i],
                    gameData.CityCommodityInRoute[i], gameData.CityTradeRoutePartnerCity[i], gameData.CityScience[i], 
                    gameData.CityTax[i], gameData.CityNoOfTradeIcons[i], gameData.CityTotalFoodProduction[i], 
                    gameData.CityTotalShieldProduction[i], gameData.CityHappyCitizens[i], gameData.CityUnhappyCitizens[i], 
                    productionOrders, totalUnitOrders));
            }

            Cities = cities;

            // Create units
            for (int i = 0; i < gameData.NumberOfUnits; i++)
            {
                var unit = CreateUnit(gameData.UnitType[i], gameData.UnitXloc[i], gameData.UnitYloc[i], 
                    gameData.UnitMap[i], gameData.UnitMadeMoveThisTurn[i], gameData.UnitVeteran[i], 
                    gameData.UnitWaitOrder[i], gameData.UnitCiv[i], gameData.UnitMovePointsLost[i],
                    gameData.UnitHitPointsLost[i], gameData.UnitPrevXloc[i], gameData.UnitPrevYloc[i],
                    gameData.UnitCounterRoleParameter[i], gameData.UnitOrders[i], gameData.UnitHomeCity[i],
                    gameData.UnitGotoX[i], gameData.UnitGotoY[i], gameData.UnitMapNoOfGoto[i], 
                    gameData.UnitLinkOtherUnitsOnTop[i], gameData.UnitLinkOtherUnitsUnder[i],
                    gameData.UnitAnimation[i], gameData.UnitOrientation[i]);
                if (i == gameData.SelectedUnitIndex)
                {
                    ActiveUnit = unit;
                }
            }

            // Transporters
            var transporters = new List<Transporter>();
            for (int i = 0; i < gameData.NoTransporters; i++)
            {
                transporters.Add(new Transporter
                {
                    X1 = gameData.Transporter1X[i],
                    Y1 = gameData.Transporter1Y[i],
                    MapId1 = gameData.Transporter1MapNo[i],
                    X2 = gameData.Transporter2X[i],
                    Y2 = gameData.Transporter2Y[i],
                    MapId2 = gameData.Transporter2MapNo[i],
                    Look = gameData.TransporterLook[i]
                });
            }

            Transporters = transporters;

            // Scenario
            List<ScenarioEvent> events = new ();
            ScenarioEvent @event = new();
            int[]? flags = null;
            for (int i = 0; i < gameData.NumberOfEvents; i++)
            {
                // @INITFLAG
                if (gameData.EventModifiers[i][11])
                {
                    flags = new int[9];
                    for (int j = 0; j <8; j++)
                    {
                        flags[j] = BitConverter.ToInt32(new byte[4] { 
                            gameData.EventActionParam[i][85 + 4 * j], gameData.EventActionParam[i][86 + 4 * j],
                            gameData.EventActionParam[i][87 + 4 * j], gameData.EventActionParam[i][88 + 4 * j] });
                    }
                    flags[8] = BitConverter.ToInt32(new byte[4] {
                        gameData.EventActionParam[i][129], gameData.EventActionParam[i][130],
                        gameData.EventActionParam[i][131], gameData.EventActionParam[i][132] });
                }
                else
                {
                    // Second trigger with @AND modifier
                    if (gameData.EventModifiers[i][30])
                    {
                        @event.Trigger2 = CreateScenarioTrigger(gameData.GameVersion, gameData.EventTriggerIds[i],
                                gameData.EventModifiers[i], gameData.EventTriggerParam[i], gameData.EventStrings);
                        events.Add(@event);
                    }
                    else
                    {
                        @event = new ScenarioEvent
                        {
                            Trigger = CreateScenarioTrigger(gameData.GameVersion, gameData.EventTriggerIds[i],
                                gameData.EventModifiers[i], gameData.EventTriggerParam[i], gameData.EventStrings),
                            Actions = CreateScenarioActions(gameData.GameVersion, gameData.EventActionIds[i],
                                gameData.EventModifiers[i], gameData.EventActionParam[i], gameData.EventStrings),
                            Delay = gameData.EventModifiers[i][1] ? 
                                BitConverter.ToInt16(new byte[2] { gameData.EventActionParam[i][195], gameData.EventActionParam[i][196] }) : null,
                            JustOnce = gameData.EventActionIds[i].Contains(6),
                            Continuous = gameData.EventModifiers[i][7]
                        };

                        // Add event to list unless it's the first trigger with @AND modifier
                        if (!gameData.EventModifiers[i][28])
                        {
                            events.Add(@event);
                        }
                    }
                }
            }
            Scenario = new Scenario
            {
                Events = events,
                Flags = flags == null ? null : flags,
                TotalWar = gameData.TotalWar,
                ObjectiveVictory = gameData.ObjectiveVictory,
                CountWondersAsObjectives = gameData.CountWondersAsObjectives,
                ForbidGovernmentSwitching = gameData.ForbidGovernmentSwitching,
                ForbidTechFromConquests = gameData.ForbidTechFromConquests,
                ElliminatePollution = gameData.ElliminatePollution,
                SpecialWwiIonlyAi = gameData.SpecialWwiIonlyAi,
                Name = gameData.ScenarioName,
                TechParadigm = gameData.TechParadigm,
                TurnYearIncrement = gameData.TurnYearIncrement,
                StartingYear = gameData.StartingYear,
                MaxTurns = gameData.MaxTurns,
                ObjectiveProtagonist = gameData.ObjectiveProtagonist,
                NoObjectivesDecisiveVictory = gameData.NoObjectivesDecisiveVictory,
                NoObjectivesMarginalVictory = gameData.NoObjectivesMarginalVictory,
                NoObjectivesMarginalDefeat = gameData.NoObjectivesMarginalDefeat,
                NoObjectivesDecisiveDefeat = gameData.NoObjectivesDecisiveDefeat
            };
        }

        /// <summary>
        /// Generate first instance of terrain tiles by importing game data.
        /// </summary>
        /// <param name="data">Game data.</param>
        /// <param name="rules">Game rules.</param>
        /// <param name="map">The map these tiles are for</param>
        private static Tile[,] PopulateTilesFromGameData(GameData data, Rules rules, Map map)
        {
            var tile = new Tile[map.XDim, map.YDim];
            for (var col = 0; col < map.XDim; col++)
            {
                for (var row = 0; row < map.YDim; row++)
                {
                    var terrain = data.MapTerrainType[map.MapIndex][col, row];
                    List<ConstructedImprovement> improvements = GetImprovementsFrom(data, col, row);
                    var playerKnowledge = GetPlayerKnowledgeFrom(data, col, row);
                    tile[col, row] = new Tile(2 * col + (row % 2), row, rules.Terrains[map.MapIndex][terrain], map.ResourceSeed, map, col,
                        Enumerable.Range(0, 8).Select(i => data.MapTileVisibility[0][col, row, i]).ToArray(),
                        Enumerable.Range(0, 8).Select(i => data.MapUnitVisibility[0][col, row, i]).ToArray(),
                        Enumerable.Range(0, 8).Select(i => data.MapIrrigationVisibility[0][col, row, i]).ToArray(),
                        Enumerable.Range(0, 8).Select(i => data.MapMiningVisibility[0][col, row, i]).ToArray(),
                        Enumerable.Range(0, 8).Select(i => data.MapRoadVisibility[0][col, row, i]).ToArray(),
                        Enumerable.Range(0, 8).Select(i => data.MapRailroadVisibility[0][col, row, i]).ToArray(),
                        Enumerable.Range(0, 8).Select(i => data.MapFortressVisibility[0][col, row, i]).ToArray(),
                        Enumerable.Range(0, 8).Select(i => data.MapPollutionVisibility[0][col, row, i]).ToArray(),
                        Enumerable.Range(0, 8).Select(i => data.MapAirbaseVisibility[0][col, row, i]).ToArray(),
                        Enumerable.Range(0, 8).Select(i => data.MapFarmlandVisibility[0][col, row, i]).ToArray(),
                        Enumerable.Range(0, 8).Select(i => data.MapTransporterVisibility[0][col, row, i]).ToArray())
                    {
                        River = data.MapRiverPresent[map.MapIndex][col, row],
                        Resource = data.MapResourcePresent[map.MapIndex][col, row],
                        PlayerKnowledge = playerKnowledge,
                        //UnitPresent = data.MapUnitPresent[map.MapIndex][col, row],  // you can find this out yourself
                        //CityPresent = data.MapCityPresent[map.MapIndex][col, row],  // you can find this out yourself


                        Island = data.LandSeaIndex[map.MapIndex][col, row],
                        Improvements = improvements 
                    };
                }
            }

            return tile;
        }

        private static PlayerTile[] GetPlayerKnowledgeFrom(GameData data, int col, int row)
        {
            return Enumerable.Range(0, 8).Select(i => BuildPlayerTileKnowledge(
                data.MapIrrigationVisibility[0][col, row, i],
                data.MapMiningVisibility[0][col, row, i],
                data.MapRoadVisibility[0][col, row, i],
                data.MapRailroadVisibility[0][col, row, i],
                data.MapFortressVisibility[0][col, row, i],
                data.MapPollutionVisibility[0][col, row, i],
                data.MapAirbaseVisibility[0][col, row, i],
                data.MapFarmlandVisibility[0][col, row, i],
                data.MapTransporterVisibility[0][col, row, i])).ToArray();
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
            }else if (mining)
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
                    Improvement = ImprovementTypes.Pollution, Level = 0
                });
            }
            
            if (transporter)
            {
                tileKnowledge.Improvements.Add(new ConstructedImprovement
                {
                    Improvement = ImprovementTypes.Transporter, Level = 0
                });
            }

            return tileKnowledge;
        }

        private static List<ConstructedImprovement> GetImprovementsFrom(GameData data, int col, int row)
        {
            var improvements = new List<ConstructedImprovement>();

            if (data.MapFarmlandPresent[0][col, row])
            {
                improvements.Add(new ConstructedImprovement
                    { Improvement = ImprovementTypes.Irrigation, Group = ImprovementTypes.ProductionGroup, Level = 1 });
            }
            else if (data.MapIrrigationPresent[0][col, row])
            {
                improvements.Add(new ConstructedImprovement
                    { Improvement = ImprovementTypes.Irrigation, Group = ImprovementTypes.ProductionGroup, Level = 0 });
            }else if (data.MapMiningPresent[0][col, row])
            {
                improvements.Add(new ConstructedImprovement
                    { Improvement = ImprovementTypes.Mining, Group = ImprovementTypes.ProductionGroup, Level = 0 });
            }

            if (data.MapRailroadPresent[0][col, row])
            {
                improvements.Add(new ConstructedImprovement { Improvement = ImprovementTypes.Road, Level = 1 });
            }
            else if (data.MapRoadPresent[0][col, row])
            {
                improvements.Add(new ConstructedImprovement { Improvement = ImprovementTypes.Road, Level = 0 });
            }
            
            if (data.MapFortressPresent[0][col, row])
            {
                improvements.Add(new ConstructedImprovement
                    { Improvement = ImprovementTypes.Fortress, Group = ImprovementTypes.DefenceGroup, Level = 0 });
            }
            else if (data.MapAirbasePresent[0][col, row])
            {
                improvements.Add(new ConstructedImprovement
                    { Improvement = ImprovementTypes.Airbase, Group = ImprovementTypes.DefenceGroup, Level = 0 });
            }

            if (data.MapPollutionPresent[0][col, row])
            {
                improvements.Add(new ConstructedImprovement
                {
                    Improvement = ImprovementTypes.Pollution, Level = 0
                });
            }

            if (data.MapTransporterPresent[0][col, row])
            {
                improvements.Add(new ConstructedImprovement
                {
                    Improvement = ImprovementTypes.Transporter, Level = 0
                });
            }
            return improvements;
        }

        public List<City> Cities { get; set; }

        public List<Transporter> Transporters { get; set; }

        public List<Civilization> Civilizations { get; set; }

        public List<Map> Maps { get; set; }

        public Civilization CreateCiv(int id, int whichHumanPlayerIsUsed, bool alive, int style, string leaderName, string tribeName, 
            string adjective, int gender, int money, int tribeNumber, int researchProgress, int researchingAdvance, int sciRate, int taxRate,
            int government, int reputation, int patience, bool[][] contact, bool[][] ceaseFire, bool[][] peace, bool[][] alliance, 
            bool[][] vendetta, bool[][] embassy, bool[][] war, int[][] attitudes, bool[][] advances, int[][] lastContact, bool hasSpaceship,
            int ssEstArrival, int ssLaunchYear, int ssStructural, int ssPropulsion, int ssFuel, int ssHabitation, int ssLifeSupport,
            int ssSolarPanel)
        {
            var tribe = Rules.Leaders[tribeNumber];
            // If leader name string is empty (no manual input), find the name in RULES.TXT (don't search for barbarians)
            if (id != 0 && leaderName.Length == 0) leaderName = (gender == 0) ? tribe.NameMale : tribe.NameFemale;

            // If tribe name string is empty (no manual input), find the name in RULES.TXT (don't search for barbarians)
            if (id != 0 && tribeName.Length == 0) tribeName = tribe.Plural;

            // If adjective string is empty (no manual input), find adjective in RULES.TXT (don't search for barbarians)
            if (id != 0 && adjective.Length == 0) adjective = tribe.Adjective;

            // Set citystyle from input only for player civ. Other civs (AI) have set citystyle from RULES.TXT
            if (id != 0 && id != whichHumanPlayerIsUsed) style = tribe.CityStyle;

            var gov = Rules.Governments[government];
            return new Civilization
            {
                Id = id,
                Alive = alive,
                CityStyle = style,
                LeaderName = leaderName,
                LeaderGender = gender,
                LeaderTitle = (gender == 0) ? gov.TitleMale : gov.TitleFemale,
                TribeName = tribeName,
                Adjective = adjective,
                Money = money,
                ReseachingAdvance = researchingAdvance,
                Advances = advances[id],
                ScienceRate = sciRate * 10,
                TaxRate = taxRate * 10,
                Government = government,
                AllowedAdvanceGroups = new [] { AdvanceGroupAccess.CanResearch } // Default for MPG < TOT will need to read from file
            };
        }
        
        public City CreateCity(int x, int y, int mapNo, bool canBuildCoastal, bool autobuildMilitaryRule,
            bool stolenTech,
            bool improvementSold, bool weLoveKingDay, bool civilDisorder, bool canBuildHydro, bool canBuildShips,
            bool autoBuildMilitary, bool autoBuildDomestic, bool objectivex1, bool objectivex3,
            int ownerIndex, int size, int whoBuiltIt, int turnsExpiredSinceCaptured, bool[] whoKnowsAboutIt,
            int[] lastSizeRevealedToCivs,
            int foodInStorage, int shieldsProgress, int netTrade, string name, bool[] distributionWorkers,
            int noOfSpecialistsx4, bool[] improvements, int itemInProduction,
            int activeTradeRoutes, int[] commoditySupplied, int[] commodityDemanded,
            int[] commodityInRoute, int[] tradeRoutePartnerCity, int science, int tax, int noOfTradeIcons,
            int totalFoodProduction, int totalShieldProduction, int happyCitizens, int unhappyCitizens,
            IProductionOrder[] productionItems, int totalUnitOrders)
        {
            var tile = Maps[mapNo].TileC2(x, y);
            var owner = Civilizations[ownerIndex];
            var city = new City
            {
                X = x,
                Y = y,
                MapIndex = mapNo,
                CanBuildCoastal = canBuildCoastal,
                AutobuildMilitaryRule = autobuildMilitaryRule,
                StolenTech = stolenTech,
                ImprovementSold = improvementSold,
                WeLoveKingDay = weLoveKingDay,
                CivilDisorder = civilDisorder,
                CanBuildHydro = canBuildHydro,
                CanBuildShips = canBuildShips,
                AutobuildMilitaryAdvisor = autoBuildMilitary,
                AutobuildDomesticAdvisor = autoBuildDomestic,
                Objectivex1 = objectivex1,
                Objectivex3 = objectivex3,
                Owner = owner,
                Size = size,
                WhoBuiltIt = Civilizations[whoBuiltIt],
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
                CommoditySupplied = commoditySupplied.Where(c => c < Rules.CaravanCommoditie.Length).Select(c => Rules.CaravanCommoditie[c]).ToArray(),
                CommodityDemanded = commodityDemanded.Where(c => c < Rules.CaravanCommoditie.Length).Select(c => Rules.CaravanCommoditie[c]).ToArray(),
                TradeRoutes = commodityInRoute.Zip(tradeRoutePartnerCity).Select(((tuple) => new TradeRoute{ Commodity = Rules.CaravanCommoditie[tuple.First % Rules.CaravanCommoditie.Length], Destination = tuple.Second} )).ToArray(),
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

            owner.Cities.Add(city);

            foreach (var (first, second) in Maps[mapNo].CityRadius(tile,true).Zip(distributionWorkers))
            {
                if (first != null && second)
                {
                    first.WorkedBy = city;
                }
            }

            tile.CityHere = city;

            for (var improvementNo = 0; improvementNo < 34; improvementNo++)
                if (improvements[improvementNo]) city.AddImprovement(Rules.Improvements[improvementNo+1]);


            return city;
        }
        
        public Unit CreateUnit (int type, int x, int y, int mapNo, bool madeFirstMove, bool veteran, bool wait,
            int civId, int movePointsLost, int hitPointsLost, int prevX, int prevY, int counterRoleParam, 
            int orders, int homeCity, int goToX, int goToY, int goToMap, int linkOtherUnitsOnTop, int linkOtherUnitsUnder,
            int animation, int orientation)
        {
            if (mapNo < 0) mapNo = 0;   // avoid dead unit errors
            var validTile = Maps[mapNo].IsValidTileC2(x, y);

            var civilization = Civilizations[civId];
            var unit = new Unit
            {
                Id = civilization.Units.Count,
                TypeDefinition = Rules.UnitTypes[type],
                Dead = x < 0 || !validTile,
                CurrentLocation = validTile ? Maps[mapNo].TileC2(x,y) : null,
                X = x,
                Y = y,
                MapIndex = mapNo,
                MovePointsLost = movePointsLost,
                HitPointsLost = hitPointsLost,
                MadeFirstMove = madeFirstMove,
                Veteran = veteran,
                Owner = civilization,
                PrevXy = new[] { prevX, prevY },
                Order = orders,
                HomeCity = homeCity == 255 ? null : Cities[homeCity],
                GoToX = goToX,
                GoToY = goToY,
                LinkOtherUnitsOnTop = linkOtherUnitsOnTop,
                LinkOtherUnitsUnder = linkOtherUnitsUnder
            };

            switch (unit.AIrole)
            {
                case AIroleType.Trade:
                    unit.CaravanCommodity = counterRoleParam;
                    break;
                case AIroleType.Settle:
                    unit.Counter = counterRoleParam;
                    break;
            }

            civilization.Units.Add(unit);
            return unit;
        }

        public ITrigger CreateScenarioTrigger(int version, int triggerId, bool[] modifiers,
            byte[] triggerParam, List<string> strings)
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
                        City = Cities.Find(c => c.Name == strings[0]),
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

        public List<IScenarioAction> CreateScenarioActions(int version, int[] actionIds, bool[] modifiers,
            byte[] actionParam, List<string> strings)
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
                            HomeCity = Cities.Find(c => c.Name == strings[2]),
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
}