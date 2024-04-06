using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.Advances;
using Civ2engine.Enums;
using Civ2engine.IO;
using Civ2engine.MapObjects;
using Civ2engine.OriginalSaves;
using Civ2engine.Scripting;
using Civ2engine.Statistics;
using Civ2engine.Terrains;

namespace Civ2engine
{
    public partial class Game
    {
        public static Game Create(Rules rules, GameData gameData, LoadedGameObjects objects, Ruleset ruleset)
        {
            _instance = new Game(rules, gameData, objects, ruleset.Paths);
            return _instance;
        }

        public static Game StartNew(Map[] maps, GameInitializationConfig config, IList<Civilization> civilizations,
            string[] paths)
        {
            _instance = new Game(maps, config.Rules, civilizations, new Options(config), paths, (DifficultyType)config.DifficultyLevel);
            _instance.StartNextTurn();
            return _instance;
        }

        public static void CreateScenario(Rules rules, GameData gameData, LoadedGameObjects objects, Ruleset ruleset)
        {
            _instance = new Game(rules, gameData, objects, ruleset.Paths);
        }

        public static Game UpdateScenarioChoices(GameInitializationConfig config)
        {
            _instance.SetHumanPlayer(config.ScenPlayerCivId);
            _instance.DifficultyLevel = (DifficultyType)config.DifficultyLevel;
            _instance.GetPlayerCiv.LeaderGender = config.Gender;
            _instance.GetPlayerCiv.LeaderName = config.LeaderName;
            _instance.StartNextTurn();
            return _instance;
        }

        private Game(Map[] maps, Rules configRules, IList<Civilization> civilizations, Options options,
            string[] gamePaths, DifficultyType difficulty)
        {
            Script = new ScriptEngine(this, gamePaths);
            _options = options;
            _maps = maps;
            _rules = configRules;
            TurnNumber = 0;
            Date = new Date(0, 1, difficulty);
            _difficultyLevel = difficulty;
            
            AllCivilizations.AddRange(civilizations);

            CityNames = NameLoader.LoadCityNames(gamePaths);

            Players = civilizations.Select(c => new Player(_difficultyLevel, c)).Cast<IPlayer>().ToArray();

            TerrainImprovements = TerrainImprovementFunctions.GetStandardImprovements(Rules); 
            
            Script.RunScript("game_setup.lua");

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

        private Game(Rules rules, GameData gameData, LoadedGameObjects objects, string[] rulesetPaths) 
            : this(objects.Maps.ToArray(), rules,objects.Civilizations,new Options(gameData.OptionsArray), 
                  rulesetPaths, (DifficultyType)gameData.DifficultyLevel)
        {
            _scenarioData = objects.Scenario;

            TurnNumber = gameData.TurnNumber;
            Date = new Date(gameData.StartingYear, gameData.TurnYearIncrement, (DifficultyType)gameData.DifficultyLevel);

            _barbarianActivity = (BarbarianActivityType)gameData.BarbarianActivity;
            PollutionSkulls = gameData.NoPollutionSkulls;
            
            GlobalTempRiseOccured = gameData.GlobalTempRiseOccured;
            NoOfTurnsOfPeace = gameData.NoOfTurnsOfPeace;

            var playerCiv = GetPlayerCiv;
            var activePlayer = Players.FirstOrDefault(p => p.Civilization == playerCiv);

            if (objects.ActiveUnit is { Dead: false })
            {
                activePlayer.ActiveUnit = objects.ActiveUnit;
            }
            else
            {
                activePlayer.ActiveUnit = playerCiv.Units.FirstOrDefault(u => u.AwaitingOrders);
                if (activePlayer.ActiveUnit == null)
                {
                    activePlayer.ActiveTile = playerCiv.Cities[0].Location;
                }
            }

            _activeCiv = playerCiv;
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
