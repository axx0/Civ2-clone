
using Civ2engine.IO;
using Civ2engine;
using Civ2engine.Units;
using Model;
using Model.Core;
using RaylibUI.RunGame;

namespace RaylibUI
{
    public partial class Main
    {
        private Unit _activeUnit;

        
        public void StartGame(IGame game, IDictionary<string, string?>? viewData)
        {
            game.UpdatePlayerViewData();
            
            _activeScreen = new GameScreen(this, game, Soundman, viewData);

            if (game.TurnNumber == 0)
            {
                game.StartNextTurn();
            }
            else
            {
                // If we're not on turn one start with active player
                game.StartPlayerTurn(game.ActivePlayer);
            }
        }

        public IUserInterface SetActiveRulesetFromFile(string root, string subDirectory,
            Dictionary<string, string> extendedMetadata)
        {
            var maxScore = -1;
            Ruleset selected = AllRuleSets.First();
            foreach (var set in AllRuleSets)
            {
                var score = extendedMetadata
                    .Where(thing => set.Metadata.ContainsKey(thing.Key) && set.Metadata[thing.Key] == thing.Value)
                    .Sum(thing => 1000);

                if (set.Paths.Contains(subDirectory))
                {
                    score += 100;
                }

                if (set.Root == root)
                {
                    score += 10;
                }

                if (score > maxScore)
                {
                    maxScore = score;
                    selected = set;
                }
            }

            ActiveRuleSet = !selected.Paths.Contains(subDirectory) ? new Ruleset(selected, subDirectory) : selected;
            if (selected.InterfaceIndex != Interfaces.IndexOf(ActiveInterface))
            {
                ActiveInterface = Interfaces[selected.InterfaceIndex];
            }

            TextureCache.Clear();
            ImageUtils.SetLook(ActiveInterface);
            return ActiveInterface;
        }

        public Ruleset[] AllRuleSets { get; set; }
        public Ruleset? ActiveRuleSet { get; private set; }

        public IUserInterface SetActiveRuleSet(int ruleSetIndex)
        {
            ActiveRuleSet = AllRuleSets[ruleSetIndex];
            if (ActiveRuleSet.InterfaceIndex != Interfaces.IndexOf(ActiveInterface))
            {
                ActiveInterface = Interfaces[ActiveRuleSet.InterfaceIndex];
            }

            return ActiveInterface;
        }
    }
}
