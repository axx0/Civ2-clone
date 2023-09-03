using System;
using System.Collections.Generic;
using RaylibUI.ImageLoader;
using Civ2engine.IO;
using Civ2engine;
using Civ2engine.Units;
using RaylibUI.Initialization;
using RaylibUI.RunGame;

namespace RaylibUI
{
    public partial class Main
    {
        private Unit activeUnit;
        

        private void LocateStartingFiles(string savName, Func<Ruleset, string, bool> initializer)
        {
            var ruleSet = new Ruleset
            {
                FolderPath = Settings.Civ2Path,
                Root = Settings.Civ2Path
            };
            if (initializer(ruleSet, savName))
            {
                //Sounds.LoadSounds(ruleSet.Paths);
                Playgame();
                return;
            }
        }

        public bool LoadGameInitialization(Ruleset ruleset, string saveFileName)
        {
            var rules = RulesParser.ParseRules(ruleset);

            GameData gameData = Read.ReadSAVFile(ruleset.FolderPath, saveFileName);
            var hydrator = new LoadedGameObjects(rules, gameData);
            map = hydrator.Map;
            activeUnit = hydrator.ActiveUnit;

            Images.LoadGraphicsAssetsFromFiles(ruleset, rules);
            return true;
        }

        public void Playgame()
        {
            //Sounds.Stop();
            //Sounds.PlaySound(GameSounds.MenuOk);

            //var playerCiv = Game.GetPlayerCiv;

            //StartGame();
            //Sounds.PlaySound(GameSounds.MenuOk);
        }

        
        public void StartGame(Game game)
        {
            _activeScreen = new GameScreen(game, Soundman);
        }

    }
}
