using System;
using System.Collections.Generic;
using Civ2engine.IO;

namespace Civ2engine
{
    public partial class Game
    {
        public static void Create(Rules rules, GameData gameData, LoadedGameObjects objects, Ruleset ruleset)
        {
            _instance = new Game(rules, gameData, objects, ruleset.Paths);
        }

        public static void StartNew(Map[] maps, GameInitializationConfig config, IList<Civilization> civilizations)
        {
            _instance = new Game(maps, config.Rules, civilizations, new Options(config), config.RuleSet.Paths);
            _instance.StartNextTurn();
        }
        
        private Game(Map[] maps, Rules configRules, IList<Civilization> civilizations, Options options, string[] gamePaths)
        {
            _options = options;
            _maps = maps;
            AllCivilizations.AddRange(civilizations);
            
            CityNames = NameLoader.LoadCityNames(gamePaths);
            _rules = configRules;
            TurnNumber = 0;
        }

        private Game(Rules rules, GameData gameData, LoadedGameObjects objects, string[] rulesetPaths) : this(new [] { objects.Map}, rules,objects.Civilizations,new Options(gameData.OptionsArray), rulesetPaths )
        {
            //_civsInPlay = SAVgameData.CivsInPlay;
            _gameVersion = gameData.GameVersion;

            TurnNumber = gameData.TurnNumber;
            TurnNumberForGameYear = gameData.TurnNumberForGameYear;
            _difficultyLevel = gameData.DifficultyLevel;
            _barbarianActivity = gameData.BarbarianActivity;
            PollutionAmount = gameData.PollutionAmount;
            
            GlobalTempRiseOccured = gameData.GlobalTempRiseOccured;
            NoOfTurnsOfPeace = gameData.NoOfTurnsOfPeace;
            
            _activeUnit = objects.ActiveUnit;

            _activeCiv = GetPlayerCiv;
        }
    }
}
