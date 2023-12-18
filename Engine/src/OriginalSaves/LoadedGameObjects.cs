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
using Tile = Civ2engine.MapObjects.Tile;

namespace Civ2engine
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
                var _map = new Map(gameData.OptionsArray[3], mapNo)
                {
                    MapRevealed = gameData.MapRevealed,
                    WhichCivsMapShown = gameData.WhichCivsMapShown,
                    Zoom = gameData.Zoom,
                    StartingClickedXY = gameData.ClickedXY,
                    XDim = gameData.MapXdim_x2 / 2,
                    YDim = gameData.MapYdim,
                    ResourceSeed = gameData.MapResourceSeed,
                    LocatorXdim = gameData.MapLocatorXdim,
                    LocatorYdim = gameData.MapLocatorYdim
                };
                _map.Tile = PopulateTilesFromGameData(gameData, Rules, _map);
                maps.Add(_map);
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
                    gameData.CivAdvances));
            }

            civs[0].PlayerType = PlayerType.Barbarians;
            civs[gameData.PlayersCivIndex].PlayerType = PlayerType.Local;

            this.Civilizations = civs;

            // Create cities
            var cities = new List<City>();
            for (var i = 0; i < gameData.NumberOfCities; i++)
            {
                cities.Add(CreateCity(gameData.CityXloc[i], gameData.CityYloc[i], gameData.CityMapNo[i], 
                    gameData.CityCanBuildCoastal[i], gameData.CityAutobuildMilitaryRule[i], gameData.CityStolenAdvance[i],
                    gameData.CityImprovementSold[i], gameData.CityWeLoveKingDay[i], gameData.CityCivilDisorder[i], 
                    gameData.CityCanBuildHydro[i], gameData.CityCanBuildShips[i], gameData.CityAutobuildMilitaryAdvisor[i],
                    gameData.CityAutobuildDomesticAdvisor[i], gameData.CityObjectivex1[i], gameData.CityObjectivex3[i],
                    gameData.CityOwner[i], gameData.CitySize[i],
                    gameData.CityWhoBuiltIt[i], gameData.CityWhoKnowsAboutIt[i], gameData.CityLastSizeRevealedToCivs[i],
                    gameData.CityFoodInStorage[i], gameData.CityShieldsProgress[i],
                    gameData.CityNetTrade[i], gameData.CityName[i], gameData.CityDistributionWorkers[i],
                    gameData.CityNoOfSpecialistsx4[i], gameData.CityImprovements[i],
                    gameData.CityItemInProduction[i], gameData.CityActiveTradeRoutes[i],
                    gameData.CityCommoditySupplied[i], gameData.CityCommodityDemanded[i],
                    gameData.CityCommodityInRoute[i], gameData.CityTradeRoutePartnerCity[i],
                    gameData.CityScience[i], gameData.CityTax[i], gameData.CityNoOfTradeIcons[i],
                    gameData.CityTotalFoodProduction[i], gameData.CityTotalShieldProduction[i],
                    gameData.CityHappyCitizens[i], gameData.CityUnhappyCitizens[i], rules.ProductionItems));
            }

            Cities = cities;

            // Create units
            for (int i = 0; i < gameData.NumberOfUnits; i++)
            {
                var unit = CreateUnit(gameData.UnitType[i], gameData.UnitXloc[i], gameData.UnitYloc[i], 
                    gameData.UnitMap[i], gameData.UnitDead[i], gameData.UnitFirstMove[i], gameData.UnitGreyStarShield[i],
                    gameData.UnitVeteran[i], gameData.UnitCiv[i], gameData.UnitMovePointsLost[i],
                    gameData.UnitHitPointsLost[i], gameData.UnitPrevXloc[i], gameData.UnitPrevYloc[i],
                    gameData.UnitCaravanCommodity[i], gameData.UnitOrders[i], gameData.UnitHomeCity[i],
                    gameData.UnitGotoX[i], gameData.UnitGotoY[i], gameData.UnitLinkOtherUnitsOnTop[i],
                    gameData.UnitLinkOtherUnitsUnder[i]);
                if (i == gameData.SelectedUnitIndex)
                {
                    ActiveUnit = unit;
                }
            }

            // Scenario
            List<ScenarioEvent> events = new ();
            for (int i = 0; i < gameData.NumberOfEvents; i++)
            {
                events.Add(CreateEvent(gameData.EventTriggerIds[i], gameData.EventActionIds[i],
                    gameData.EventTriggerParam[i], gameData.EventActionParam[i], gameData.EventStrings));
            }
            Scenario = new Scenario
            {
                Events = events,
                TotalWar = gameData.TotalWar,
                ObjectiveVictory = gameData.ObjectiveVictory,
                CountWondersAsObjectives = gameData.CountWondersAsObjectives,
                ForbidGovernmentSwitching = gameData.ForbidGovernmentSwitching,
                ForbidTechFromConquests = gameData.ForbidTechFromConquests,
                ElliminatePollution = gameData.ElliminatePollution,
                SpecialWWIIonlyAI = gameData.SpecialWWIIonlyAI,
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
                        //UnitPresent = data.MapUnitPresent[map.MapIndex][col, row],  // you can find this out yourself
                        //CityPresent = data.MapCityPresent[map.MapIndex][col, row],  // you can find this out yourself


                        Island = data.MapIslandNo[map.MapIndex][col, row],
                        Improvements = improvements 
                    };
                }
            }

            return tile;
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

            return improvements;
        }

        public List<City> Cities { get; set; }

        public List<Civilization> Civilizations { get; set; }

        public List<Map> Maps { get; set; }

        public Civilization CreateCiv(int id, int whichHumanPlayerIsUsed, bool alive, int style, string leaderName, string tribeName, string adjective,
            int gender, int money, int tribeNumber, int researchProgress, int researchingAdvance, int sciRate, int taxRate,
            int government, int reputation, bool[][] advances)
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
                Government = (GovernmentType)government,
                AllowedAdvanceGroups = new [] { AdvanceGroupAccess.CanResearch } // Default for MPG < TOT will need to read from file
            };
        }
        
        public City CreateCity(int x, int y, int mapNo, bool canBuildCoastal, bool autobuildMilitaryRule, bool stolenTech,
            bool improvementSold, bool weLoveKingDay, bool civilDisorder, bool canBuildHydro, bool canBuildShips,
            bool autoBuildMilitary, bool autoBuildDomestic, bool objectivex1, bool objectivex3,
            int ownerIndex, int size, int whoBuiltIt, bool[] whoKnowsAboutIt, int[] lastSizeRevealedToCivs, 
            int foodInStorage, int shieldsProgress, int netTrade, string name, bool[] distributionWorkers, 
            int noOfSpecialistsx4, bool[] improvements, int itemInProduction,
            int activeTradeRoutes, int[] commoditySupplied, int[] commodityDemanded,
            int[] commodityInRoute, int[] tradeRoutePartnerCity, int science, int tax, int noOfTradeIcons,
            int totalFoodProduction, int totalShieldProduction, int happyCitizens, int unhappyCitizens, ProductionOrder[] productionItems)
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
                        productionItems[Rules.ProductionItems.Where(i => i is UnitProductionOrder).Count() - itemInProduction - 1],
                ActiveTradeRoutes = activeTradeRoutes,
                CommoditySupplied = commoditySupplied.Where(c => c < Rules.CaravanCommoditie.Length).Select(c => (CommodityType)c).ToArray(),
                CommodityDemanded = commodityDemanded.Where(c => c < Rules.CaravanCommoditie.Length).Select(c => (CommodityType)c).ToArray(),
                CommodityInRoute = commodityInRoute.Select(c => (CommodityType)c).ToArray(),
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
        
        public Unit CreateUnit (int type, int x, int y, int mapNo, bool dead, bool firstMove, bool greyStarShield, 
            bool veteran, int civId, int movePointsLost, int hitPointsLost, int prevX, int prevY, int caravanCommodity, 
            int orders, int homeCity, int goToX, int goToY, int linkOtherUnitsOnTop, int linkOtherUnitsUnder)
        {
            if (mapNo < 0) mapNo = 0;   // avoid dead unit errors
            var validTile = Maps[mapNo].IsValidTileC2(x, y);

            var civilization = Civilizations[civId];
            var unit = new Unit
            {
                Id = civilization.Units.Count,
                TypeDefinition = Rules.UnitTypes[type],
                Dead = dead || !validTile,
                CurrentLocation = validTile ? Maps[mapNo].TileC2(x,y) : null,
                X = x,
                Y = y,
                MapIndex = mapNo,
                MovePointsLost = movePointsLost,
                HitPointsLost = hitPointsLost,
                FirstMove = firstMove,
                GreyStarShield = greyStarShield,
                Veteran = veteran,
                Owner = civilization,
                PrevXY = new[] { prevX, prevY },
                CaravanCommodity = (CommodityType)caravanCommodity,
                Order = (OrderType)orders,
                HomeCity = homeCity == 255 ? null : Cities[homeCity],
                GoToX = goToX,
                GoToY = goToY,
                LinkOtherUnitsOnTop = linkOtherUnitsOnTop,
                LinkOtherUnitsUnder = linkOtherUnitsUnder
            };

            civilization.Units.Add(unit);
            return unit;
        }

        public ScenarioEvent CreateEvent(int triggerId, int[] actionIds, 
            int[] triggerParam, int[] actionParam, List<string> strings)
        {
            ITrigger? trigger = default;
            switch (triggerId)
            {
                case 0:
                    trigger = new TUnitKilled
                    {
                        UnitKilled = (UnitType)triggerParam[1],
                        AttackerCivId = triggerParam[4],
                        DefenderCivId = triggerParam[7],
                        Strings = strings.GetRange(0, 3),
                    };
                    strings.RemoveRange(0, 3);
                    break;
                case 1:
                    trigger = new TCityTaken
                    {
                        City = Cities.Find(c => c.Name == strings[0]),
                        AttackerCivId = triggerParam[4],
                        DefenderCivId = triggerParam[7],
                        Strings = strings.GetRange(0, 3),
                    };
                    strings.RemoveRange(0, 3);
                    break;
                case 2:
                    trigger = new TTurn
                    {
                        Turn = triggerParam[9]
                    };
                    break;
                case 3:
                    trigger = new TTurnInterval
                    {
                        Interval = triggerParam[9]
                    };
                    break;
                case 4:
                    trigger = new TNegotiation
                    {
                        TalkerCivId = triggerParam[4],
                        TalkerType = triggerParam[5],
                        ListenerCivId = triggerParam[7],
                        ListenerType = triggerParam[8],
                        Strings = strings.GetRange(0, 2),
                    };
                    strings.RemoveRange(0, 2);
                    break;
                case 5:
                    trigger = new TScenarioLoaded { };
                    break;
                case 6:
                    trigger = new TRandomTurn
                    {
                        Denominator = triggerParam[10]
                    };
                    break;
                case 7:
                    trigger = new TNoSchism
                    {
                        CivId = triggerParam[7],
                        Strings = strings.GetRange(0, 1),
                    };
                    strings.RemoveRange(0, 1);
                    break;
                case 8:
                    trigger = new TReceivedTechnology
                    {
                        TechnologyId = triggerParam[11],
                        ReceiverCivId = triggerParam[7],
                        Strings = strings.GetRange(0, 1)
                    };
                    strings.RemoveRange(0, 1);
                    break;
                default:
                    break;
            }

            var actions = new List<IAction>();
            for (int i = 0; i < actionIds.Length; i++)
            {
                switch (actionIds[i])
                {
                    case 0:
                        List<string> _texts = new();
                        for (int j = 0; j < 20; j++)
                        {
                            if (actionParam[j] != 0)
                            {
                                _texts.Add(strings[0]);
                                strings.RemoveRange(0, 1);
                            }
                        }
                        actions.Add(new AText
                        {
                            Strings = new List<string>(_texts)
                        });
                        break;
                    case 1:
                        actions.Add(new AMoveUnit
                        {
                            OwnerCivId = actionParam[21],
                            UnitMovedId = actionParam[23],
                            NumberToMove = actionParam[24],
                            MapCoords = new int[8] { actionParam[25], actionParam[26],
                                actionParam[27], actionParam[28], actionParam[29], 
                                actionParam[30], actionParam[31], actionParam[32] },
                            MapDest = new int[2] { actionParam[33], actionParam[34] },
                            Strings = strings.GetRange(0, 2),
                        });
                        strings.RemoveRange(0, 2);
                        break;
                    case 2:
                        actions.Add(new ACreateUnit
                        {
                            OwnerCivId = actionParam[40],
                            CreatedUnit = (UnitType)actionParam[42],
                            Locations = new int[10, 2] { { actionParam[43], actionParam[44] }, { actionParam[45], actionParam[46] }, { actionParam[47], actionParam[48] }, { actionParam[49], actionParam[50] }, { actionParam[51], actionParam[52] }, { actionParam[53], actionParam[54] }, { actionParam[55], actionParam[56] }, { actionParam[57], actionParam[58] }, { actionParam[59], actionParam[60] }, { actionParam[61], actionParam[62] } },
                            Veteran = actionParam[64] == 1,
                            HomeCity = Cities.Find(c => c.Name == strings[2]),
                            Strings = strings.GetRange(0, 3)
                        });
                        strings.RemoveRange(0, 3);
                        break;
                    case 3:
                        actions.Add(new AChangeMoney
                        {
                            ReceiverCivId = actionParam[80],
                            Amount = actionParam[81],
                            Strings = strings.GetRange(0, 1)
                        });
                        strings.RemoveRange(0, 1);
                        break;
                    case 4:
                        actions.Add(new APlayWAV
                        {
                            File = strings.GetRange(0, 1).FirstOrDefault(),
                            Strings = strings.GetRange(0, 1)
                        });
                        strings.RemoveRange(0, 1);
                        break;
                    case 5:
                        actions.Add(new AMakeAggression
                        {
                            WhomCivId = actionParam[36],
                            WhoCivId = actionParam[38],
                            Strings = strings.GetRange(0, 2)
                        });
                        strings.RemoveRange(0, 2);
                        break;
                    case 6:
                        actions.Add(new AJustOnce { });
                        break;
                    case 7:
                        actions.Add(new APlayCDtrack
                        {
                            TrackNo = actionParam[84]
                        });
                        break;
                    case 8:
                        actions.Add(new ADontplayWonders { });
                        break;
                    case 9:
                        actions.Add(new AChangeTerrain
                        {
                            TerrainTypeId = actionParam[85],
                            MapCoords = new int[8] { actionParam[86], actionParam[87],
                                actionParam[88], actionParam[89], actionParam[90],
                                actionParam[91], actionParam[92], actionParam[93] }
                        });
                        break;
                    case 10:
                        actions.Add(new ADestroyCiv
                        {
                            CivId = actionParam[94],
                        });
                        break;
                    case 11:
                        actions.Add(new AGiveTech
                        {
                            TechId = actionParam[95],
                            CivId = actionParam[96],
                        });
                        break;
                    default:
                        break;
                }
            }

            var scenEvent = new ScenarioEvent
            {
                Trigger = trigger,
                Actions = actions,
            };

            return scenEvent;
        }
    }
}