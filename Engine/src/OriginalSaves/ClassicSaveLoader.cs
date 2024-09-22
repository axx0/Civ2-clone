using System.IO;
using System.Linq;
using Civ2engine.IO;

namespace Civ2engine.OriginalSaves
{
    public static class ClassicSaveLoader
    {
        public static Game LoadSave(GameData gameData, Ruleset ruleset, Rules rules)
        {
            var hydrator = LoadGameObjects(gameData, ruleset, rules);

            // Make an instance of a new game
            return Game.Create(rules, gameData, hydrator, ruleset);
        }

        public static Game LoadScn(GameData gameData, Ruleset ruleset, Rules rules)
        {
            var hydrator = LoadGameObjects(gameData, ruleset, rules);

            return Game.CreateScenario(rules, gameData, hydrator, ruleset);
        }

        /// <summary>
        /// Make Game objects from .sav/.scn data
        /// </summary>
        private static LoadedGameObjects LoadGameObjects(GameData gameData, Ruleset ruleset, Rules rules)
        {
            var objects = new LoadedGameObjects(rules, gameData);

            // If there are no events in .sav read them from EVENTS.TXT (if it exists)
            if (objects.Scenario.Events.Count == 0 &&
                Directory.EnumerateFiles(ruleset.FolderPath, "events.txt", new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive }).FirstOrDefault() != null)
            {
                objects.Scenario.Events = EventsLoader.LoadEvents(new string[] { ruleset.FolderPath }, rules, objects);
            }

            return objects;
        }
    }
}