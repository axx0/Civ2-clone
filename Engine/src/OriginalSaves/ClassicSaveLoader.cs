using Civ2engine.IO;
using System.IO;
using System.Linq;

namespace Civ2engine
{
    public static class ClassicSaveLoader
    {
        public static Game LoadSave(Ruleset ruleset, string saveFileName, Rules rules)
        {
            GameData gameData = Read.ReadSAVFile(ruleset.FolderPath, saveFileName);

            var hydrator = new LoadedGameObjects(rules, gameData);

            // If there are no events in .sav read them from EVENTS.TXT (if it exists)
            if (gameData.GameVersion <= 44) // TODO: read events for TOT also
            {
                if (hydrator.Scenario.Events.Count == 0 &&
                    Directory.EnumerateFiles(ruleset.FolderPath, "events.txt", new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive }).FirstOrDefault() != null)
                {
                    hydrator.Scenario.Events = EventsLoader.LoadEvents(new string[] { ruleset.FolderPath }, rules, hydrator);
                }
            }

            // Make an instance of a new game
            return Game.Create(rules, gameData, hydrator, ruleset);
        }
    }
}