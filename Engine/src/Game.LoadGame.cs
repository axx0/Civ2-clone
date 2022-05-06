using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.Advances;
using Civ2engine.Enums;
using Civ2engine.IO;
using Civ2engine.MapObjects;
using Civ2engine.Scripting;
using Civ2engine.Statistics;
using Civ2engine.Terrains;

namespace Civ2engine
{
    public partial class Game
    {
        public static void Create(Rules rules, GameData gameData, LoadedGameObjects objects, Ruleset ruleset,
            IPlayer localPlayer)
        {
            _instance = new Game(rules, gameData, objects, ruleset.Paths, localPlayer);
        }

        public static void StartNew(Map[] maps, GameInitializationConfig config, IList<Civilization> civilizations,
            IPlayer localPlayer)
        {
            _instance = new Game(maps, config.Rules, civilizations, new Options(config), config.RuleSet.Paths, (DifficultyType)config.DifficultlyLevel, localPlayer);
            _instance.StartNextTurn();
        }

        private Game(Map[] maps, Rules configRules, IList<Civilization> civilizations, Options options,
            string[] gamePaths, DifficultyType difficulty, IPlayer localPlayer)
        {
            Script = new ScriptEngine(localPlayer.UI, this, gamePaths);
            _options = options;
            _maps = maps;
            _rules = configRules;
            TurnNumber = 0;
            _difficultyLevel = difficulty;
            
            AllCivilizations.AddRange(civilizations);

            CityNames = NameLoader.LoadCityNames(gamePaths);

            Players = civilizations.Select(c =>
            {
                var player = c.PlayerType switch
                {
                    PlayerType.AI => new AIPlayer(_difficultyLevel),
                    PlayerType.Local => localPlayer,
                    PlayerType.Remote => throw new NotSupportedException("Network play not implemented"),
                    PlayerType.Barbarians =>
                        //TODO: create seperate barbarian player 
                        new AIPlayer(_difficultyLevel),
                    _ => throw new ArgumentOutOfRangeException(nameof(c.PlayerType), c.PlayerType, null)
                };
                return player.SetCiv(c);
            }).ToArray();

            TerrainImprovements = TerrainImprovementFunctions.GetStandardImprovements(Rules); 

            Script.RunScript("tile_improvements.lua");
            
            
            
            Script.RunScript("improvements.lua");
            Script.RunScript("advances.lua");

            AllCivilizations.ForEach((civ) =>
            {
                OnCivEvent?.Invoke(this, new CivEventArgs(CivEventType.Created, civ));
            });
            

            this.SetupTech();
            
            Power.CalculatePowerRatings(this);
        }

        private Game(Rules rules, GameData gameData, LoadedGameObjects objects, string[] rulesetPaths,
            IPlayer localPlayer) 
            : this(new [] { objects.Map}, rules,objects.Civilizations,new Options(gameData.OptionsArray), rulesetPaths, gameData.DifficultyLevel, localPlayer)
        {
            //_civsInPlay = SAVgameData.CivsInPlay;
            _gameVersion = gameData.GameVersion;

            TurnNumber = gameData.TurnNumber;
            TurnNumberForGameYear = gameData.TurnNumberForGameYear;
            _barbarianActivity = gameData.BarbarianActivity;
            PollutionAmount = gameData.PollutionAmount;
            
            GlobalTempRiseOccured = gameData.GlobalTempRiseOccured;
            NoOfTurnsOfPeace = gameData.NoOfTurnsOfPeace;

            if (objects.ActiveUnit is { Dead: false })
            {
                localPlayer.ActiveUnit = objects.ActiveUnit;
            }
            else
            {
                var playerCiv = GetPlayerCiv;
                localPlayer.ActiveUnit = playerCiv.Units.FirstOrDefault(u => u.AwaitingOrders);
                if (localPlayer.ActiveUnit == null)
                {
                    localPlayer.ActiveTile = playerCiv.Cities[0].Location;
                }
            }

            _activeCiv = GetPlayerCiv;
            AllCities.AddRange(objects.Cities);
            
            for (var index = 0; index < _maps.Length; index++)
            {
                var map = _maps[index];
                map.NormalizeIslands();
                map.CalculateFertility(Rules.Terrains[index]);
                AllCities.ForEach(c =>
                {
                    map.AdjustFertilityForCity(c.Location);
                });
            }

            foreach (var civilization in AllCivilizations)
            {
                SetImprovementsForCities(civilization);
            }

            foreach (var map in _maps)
            {
                foreach (var tile in map.Tile)
                {
                    if (tile is { CityHere: null, Improvements.Count: > 0 })
                    {
                        foreach (var construct in tile.Improvements.Where(c=>TerrainImprovements.ContainsKey(c.Improvement)))
                        {
                            var improvement = TerrainImprovements[construct.Improvement];
                            var terrain = improvement.AllowedTerrains[tile.Z]
                                .FirstOrDefault(t => t.TerrainType == (int)tile.Type);
                            if (terrain is not null)
                            {
                                tile.BuildEffects(improvement, terrain, construct.Level);
                            }
                        }
                    }
                }
            }

            foreach (var city in AllCities)
            {
                city.SetUnitSupport(Rules.Cosmic);
                city.CalculateOutput(city.Owner.Government, this);
            }
        }
    }
}
