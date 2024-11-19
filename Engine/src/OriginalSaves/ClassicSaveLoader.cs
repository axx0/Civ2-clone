using System.IO;
using System.Linq;
using Civ2engine.IO;

namespace Civ2engine.OriginalSaves
{
    public static class ClassicSaveLoader
    {
        public static Game LoadSave(GameData gameData, Ruleset ruleset, Rules rules)
        {
            // Make an instance of a new game
            return Game.Create(rules, gameData, LoadGameObjects(gameData, ruleset, rules), ruleset, new Options(gameData.OptionsArray));
        }

        public static Game LoadScn(GameData gameData, Ruleset ruleset, Rules rules)
        {
            return Game.Create(rules, gameData, LoadGameObjects(gameData, ruleset, rules), ruleset, new Options(gameData.OptionsArray));
        }

        /// <summary>
        /// Make Game objects from .sav/.scn data
        /// </summary>
        private static ILoadedGameObjects LoadGameObjects(GameData gameData, Ruleset ruleset, Rules rules)
        {
            var objects = new ClassicSaveObjects(rules, gameData);

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