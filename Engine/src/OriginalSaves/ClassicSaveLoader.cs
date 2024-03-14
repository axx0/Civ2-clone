using System.IO;
using System.Linq;
using Civ2engine.IO;

namespace Civ2engine.OriginalSaves
{
    public static class ClassicSaveLoader
    {
        public static Game LoadSave(GameData gameData, Ruleset ruleset, Rules rules)
        {
            var hydrator = new LoadedGameObjects(rules, gameData);

            // If there are no events in .sav read them from EVENTS.TXT (if it exists)
            if (hydrator.Scenario.Events.Count == 0 &&
                Directory.EnumerateFiles(ruleset.FolderPath, "events.txt", new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive }).FirstOrDefault() != null)
            {
                hydrator.Scenario.Events = EventsLoader.LoadEvents(new string[] { ruleset.FolderPath }, rules, hydrator);
            }

            // Make an instance of a new game
            return Game.Create(rules, gameData, hydrator, ruleset);
        }
    }
}