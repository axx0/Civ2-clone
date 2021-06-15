using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.Enums;
using Civ2engine.IO;
using Civ2engine.Terrains;
using Civ2engine.Units;

namespace Civ2engine
{
    public partial class Game : BaseInstance
    {
        public static void Create(Rules rules, GameData gameData)
        {
            _instance = new Game(rules, gameData);
        }

        public Game(Rules rules, GameData gameData)
        {
            _rules = rules;

            //_civsInPlay = SAVgameData.CivsInPlay;
            _gameVersion = gameData.GameVersion;

            _options = new Options();
            _options.Set(gameData.OptionsArray);

            _turnNumber = gameData.TurnNumber;
            TurnNumberForGameYear = gameData.TurnNumberForGameYear;
            _difficultyLevel = gameData.DifficultyLevel;
            _barbarianActivity = gameData.BarbarianActivity;
            PollutionAmount = gameData.PollutionAmount;
            GlobalTempRiseOccured = gameData.GlobalTempRiseOccured;
            NoOfTurnsOfPeace = gameData.NoOfTurnsOfPeace;
            NumberOfUnits = gameData.NumberOfUnits;
            NumberOfCities = gameData.NumberOfCities;
            
            // Create all 8 civs (tribes)
            for (var i = 0; i < 8; i++)
            {
                CreateCiv(i, gameData.PlayersCivIndex, gameData.CivsInPlay[i], gameData.CivCityStyle[i],
                    gameData.CivLeaderName[i], gameData.CivTribeName[i], gameData.CivAdjective[i],
                    gameData.RulerGender[i], gameData.CivMoney[i], gameData.CivNumber[i],
                    gameData.CivResearchProgress[i], gameData.CivResearchingTech[i], gameData.CivSciRate[i],
                    gameData.CivTaxRate[i], gameData.CivGovernment[i], gameData.CivReputation[i],
                    gameData.CivTechs);
            }

            // Create cities
            for (var i = 0; i < gameData.NumberOfCities; i++)
            {
                CreateCity(gameData.CityXloc[i], gameData.CityYloc[i], gameData.CityCanBuildCoastal[i],
                    gameData.CityAutobuildMilitaryRule[i], gameData.CityStolenTech[i],
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
                    gameData.CityHappyCitizens[i], gameData.CityUnhappyCitizens[i]);
            }

            // Create units
            for (int i = 0; i < gameData.NumberOfUnits; i++)
            {
                CreateUnit(gameData.UnitType[i], gameData.UnitXloc[i], gameData.UnitYloc[i],
                    gameData.UnitDead[i], gameData.UnitFirstMove[i], gameData.UnitGreyStarShield[i],
                    gameData.UnitVeteran[i], gameData.UnitCiv[i], gameData.UnitMovePointsLost[i],
                    gameData.UnitHitPointsLost[i], gameData.UnitPrevXloc[i], gameData.UnitPrevYloc[i],
                    gameData.UnitCaravanCommodity[i], gameData.UnitOrders[i], gameData.UnitHomeCity[i],
                    gameData.UnitGotoX[i], gameData.UnitGotoY[i], gameData.UnitLinkOtherUnitsOnTop[i],
                    gameData.UnitLinkOtherUnitsUnder[i]);
            }

            //_activeXY = SAVgameData.ActiveCursorXY; // Active unit or view piece coords (if it's active unit, you really don't need this)

            _activeUnit = gameData.SelectedUnitIndex == -1
                ? null
                : AllUnits.Find(unit => unit.Id == gameData.SelectedUnitIndex); // null means all units have ended turn
            _playerCiv = GetCivs[gameData.PlayersCivIndex];
            _activeCiv = _playerCiv;
            
            _maps = new[] {new Map()};
        }

        public static void NewGame(GameInitializationConfig config, Map[] maps, IList<Civilization> civilizations)
        {
            var settlerType = config.Rules.UnitTypes[(int) UnitType.Settlers];
            var playingCivs = civilizations.Skip(1).ToList();
            
            var units = civilizations.Skip(1).Select(c=> new { Civ = c, DefaultStart = config.StartPositions != null ? GetDefaultStart(config, c, maps[0]) : null })
                .OrderBy( c => c.DefaultStart != null)
                .Select(c => new
            {
                c.Civ, StartLocation = c.DefaultStart ?? GetStartLoc(c.Civ, config, maps[0])
            }).Select((c, id) => new Unit
            {
                Counter = 0,
                Dead = false,
                Id = id,
                Order = OrderType.NoOrders,
                Owner = c.Civ,
                Type = UnitType.Settlers,
                Veteran = false,
                X = c.StartLocation.X,
                Y = c.StartLocation.Y,
                TypeDefinition = settlerType
            }).ToList();

            maps[0].WhichCivsMapShown = config.PlayerCiv.Id;
            
            _instance = new Game(maps, config.Rules, civilizations, units) {_playerCiv = config.PlayerCiv};

        }

        private static Tile GetDefaultStart(GameInitializationConfig config, Civilization civilization, Map map)
        {
            var index = Array.FindIndex(config.Rules.Leaders, l => l.Adjective == civilization.Adjective);
            if (index > -1 && index < config.StartPositions.Length)
            {
                var pos = config.StartPositions[index];
                if (pos[0] != -1 && pos[1] != -1)
                {
                    var tile = map.TileC2(pos[0], pos[1]);
                    if (tile.Fertility > -1)
                    {
                        map.SetAsStartingLocation(tile, civilization.Id);
                        config.StartTiles.Add(tile);
                        return tile;
                    }
                }
            }

            return null;
        }


        private static Tile GetStartLoc(Civilization civilization, GameInitializationConfig config, Map map)
        {
            var maxFertility = 0m;
            var tiles = new HashSet<Tile>();
            for (int y = 0; y < map.Tile.GetLength(1); y++)
            {
                for (int x = 0; x < map.Tile.GetLength(0); x++)
                {
                    var tile = map.Tile[x, y];
                    if(tile.Fertility < maxFertility) continue;
                    if (tile.Fertility > maxFertility)
                    {
                        tiles.Clear();
                        maxFertility = tile.Fertility;
                    }

                    tiles.Add(tile);
                }
            }

            var selectedTile = tiles.OrderByDescending(t=> DistanceToNearestStart(config, t)).First();
            
            config.StartTiles.Add(selectedTile);
            map.SetAsStartingLocation(selectedTile, civilization.Id);
            return selectedTile;
        }

        private static double DistanceToNearestStart(GameInitializationConfig config, Tile tile)
        {
            if (config.StartTiles.Count == 0)
            {
                return config.Random.Next();
            }

            
            var minDist = DistanceTo(config.StartTiles[0], tile);
            for (int i = 1; i < config.StartTiles.Count; i++)
            {
                var dist = DistanceTo(config.StartTiles[i], tile);
                if (dist < minDist)
                {
                    minDist = dist;
                }
            }

            return minDist;
        }

        /// <summary>
        /// Compute the square euclidian distance between to tiles
        /// </summary>
        /// <param name="startTile"></param>
        /// <param name="tile"></param>
        /// <returns></returns>
        private static double DistanceTo(Tile startTile, Tile tile)
        {
             return Math.Pow(startTile.X - tile.X,2) + Math.Pow(startTile.Y - tile.Y, 2);
        }

        private Game(Map[] maps, Rules configRules, IList<Civilization> civilizations, List<Unit> units)
        {
            _options = new Options();
            _maps = maps;
            GetCivs.AddRange(civilizations);
            AllUnits.AddRange(units);
            _rules = configRules;
            _activeUnit = units[0];
            _activeCiv = civilizations[1];
            CurrentMap.ActiveXY = _activeUnit.XY;
            CurrentMap.StartingClickedXY = _activeUnit.XY;
        }

    }
}
