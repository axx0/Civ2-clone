using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.Advances;
using Civ2engine.Enums;
using Civ2engine.IO;
using Civ2engine.Scripting;
using Civ2engine.Statistics;
using Neo.IronLua;

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

            AI = new AIPlayer(difficulty);
            Players = new Dictionary<PlayerType, IPlayer> { { PlayerType.AI, AI }, { PlayerType.Local, localPlayer } };

            Script.RunScript("advances.lua");
            Script.RunScript("improvements.lua");
            
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

            if (!objects.ActiveUnit.Dead)
            {
                _activeUnit = objects.ActiveUnit;
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
            foreach (var capital in AllCities.Where(c=> c.ImprovementExists(ImprovementType.Palace)))
            {
                capital.Owner.Capital = capital;
            }

            foreach (var city in AllCities)
            {
                city.SetUnitSupport(Rules.Cosmic);
                city.CalculateOutput(city.Owner.Capital, city.Owner.Government, this);
            }
        }
    }
}
