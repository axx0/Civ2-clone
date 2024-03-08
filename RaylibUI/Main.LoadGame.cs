
using Civ2engine.IO;
using Civ2engine;
using Civ2engine.Units;
using RaylibUI.RunGame;

namespace RaylibUI
{
    public partial class Main
    {
        private Unit _activeUnit;

        
        public void StartGame(Game game)
        {
            game.UpdatePlayerViewData();
            
            _activeScreen = new GameScreen(this, game, Soundman);
        }

        public Ruleset SetActiveRulesetFromFile(string root, string subDirectory,
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

            if (selected.InterfaceIndex != Interfaces.IndexOf(ActiveInterface))
            {
                ActiveInterface = Interfaces[selected.InterfaceIndex];
            }

            ActiveRuleSet = !selected.Paths.Contains(subDirectory) ? new Ruleset(selected, subDirectory) : selected;
            
            return selected;
        }

        public Ruleset[] AllRuleSets { get; set; }
        public Ruleset ActiveRuleSet { get; private set; }

        public void SetActiveRuleSet(int ruleSetIndex)
        {
            ActiveRuleSet = AllRuleSets[ruleSetIndex];
        }
    }
}
