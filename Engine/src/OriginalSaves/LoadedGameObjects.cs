using System.Collections.Generic;
using System.Linq;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.Production;
using Civ2engine.Units;

namespace Civ2engine
{
    public class LoadedGameObjects
    {
        public Unit ActiveUnit { get; }
        private Rules Rules { get; }

        public LoadedGameObjects(Rules rules, GameData gameData)
        {
            Rules = rules;
            var map = new Map(gameData.OptionsArray[3])
            {
                MapRevealed = gameData.MapRevealed,
                WhichCivsMapShown = gameData.WhichCivsMapShown,
                Zoom = gameData.Zoom,
                StartingClickedXY = gameData.ClickedXY
            };

            map.PopulateTilesFromGameData(gameData, Rules);
            this.Map = map;
            
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
                cities.Add(CreateCity(gameData.CityXloc[i], gameData.CityYloc[i], gameData.CityCanBuildCoastal[i],
                    gameData.CityAutobuildMilitaryRule[i], gameData.CityStolenAdvance[i],
                    gameData.CityImprovementSold[i], gameData.CityWeLoveKingDay[i],
                    gameData.CityCivilDisorder[i], gameData.CityCanBuildShips[i], gameData.CityObjectivex3[i],
                    gameData.CityObjectivex1[i], gameData.CityOwner[i], gameData.CitySize[i],
                    gameData.CityWhoBuiltIt[i], gameData.CityFoodInStorage[i], gameData.CityShieldsProgress[i],
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
                    gameData.UnitDead[i], gameData.UnitFirstMove[i], gameData.UnitGreyStarShield[i],
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
        }

        public List<City> Cities { get; set; }

        public List<Civilization> Civilizations { get; set; }

        public Map Map { get; set; }

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
                CityStyle = (CityStyleType)style,
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
                Government = (GovernmentType)government
            };
        }
        
        public City CreateCity(int x, int y, bool canBuildCoastal, bool autobuildMilitaryRule, bool stolenTech,
            bool improvementSold, bool weLoveKingDay, bool civilDisorder, bool canBuildShips, bool objectivex3,
            bool objectivex1, int ownerIndex, int size, int whoBuiltIt, int foodInStorage, int shieldsProgress, int netTrade,
            string name, bool[] distributionWorkers, int noOfSpecialistsx4, bool[] improvements, int itemInProduction,
            int activeTradeRoutes, CommodityType[] commoditySupplied, CommodityType[] commodityDemanded,
            CommodityType[] commodityInRoute, int[] tradeRoutePartnerCity, int science, int tax, int noOfTradeIcons,
            int totalFoodProduction, int totalShieldProduction, int happyCitizens, int unhappyCitizens, ProductionOrder[] productionItems)
        {
            var tile = Map.TileC2(x, y);
            var owner = Civilizations[ownerIndex];
            var city = new City
            {
                X = x,
                Y = y,
                CanBuildCoastal = canBuildCoastal,
                AutobuildMilitaryRule = autobuildMilitaryRule,
                StolenTech = stolenTech,
                ImprovementSold = improvementSold,
                WeLoveKingDay = weLoveKingDay,
                CivilDisorder = civilDisorder,
                CanBuildShips = canBuildShips,
                Objectivex3 = objectivex3,
                Objectivex1 = objectivex1,
                Owner = owner,
                Size = size,
                WhoBuiltIt = Civilizations[whoBuiltIt],
                FoodInStorage = foodInStorage,
                ShieldsProgress = shieldsProgress,
                NetTrade = netTrade,
                Name = name,
                NoOfSpecialistsx4 = noOfSpecialistsx4,
                ItemInProduction = productionItems[itemInProduction],
                ActiveTradeRoutes = activeTradeRoutes,
                CommoditySupplied = commoditySupplied.Where(c=> (int)c < Rules.CaravanCommoditie.Length ).ToArray(),
                CommodityDemanded = commodityDemanded.Where(c=> (int)c < Rules.CaravanCommoditie.Length ).ToArray(),
                CommodityInRoute = commodityInRoute,
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
            
            foreach (var (first, second) in Map.CityRadius(tile,true).Zip(distributionWorkers.Reverse()))
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
        
        public Unit CreateUnit (UnitType type, int x, int y, bool dead, bool firstMove, bool greyStarShield, bool veteran, int civId,
            int movePointsLost, int hitPointsLost, int prevX, int prevY, CommodityType caravanCommodity, OrderType orders,
            int homeCity, int goToX, int goToY, int linkOtherUnitsOnTop, int linkOtherUnitsUnder)
        {
            var validTile = Map.IsValidTileC2(x, y);

            var civilization = Civilizations[civId];
            var unit = new Unit
            {
                Id = civilization.Units.Count,
                TypeDefinition = Rules.UnitTypes[(int)type],
                Dead = dead || !validTile,
                CurrentLocation = validTile ? Map.TileC2(x,y) : null,
                X = x,
                Y = y,
                MovePointsLost = movePointsLost,
                HitPointsLost = hitPointsLost,
                FirstMove = firstMove,
                GreyStarShield = greyStarShield,
                Veteran = veteran,
                Owner = civilization,
                PrevXY = new[] { prevX, prevY },
                CaravanCommodity = caravanCommodity,
                Order = orders,
                HomeCity = homeCity == 255 ? null : Cities[homeCity],
                GoToX = goToX,
                GoToY = goToY,
                LinkOtherUnitsOnTop = linkOtherUnitsOnTop,
                LinkOtherUnitsUnder = linkOtherUnitsUnder
            };

            civilization.Units.Add(unit);
            return unit;
        }

    }
}