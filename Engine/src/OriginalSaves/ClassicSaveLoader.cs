namespace Civ2engine
{
    public static class ClassicSaveLoader
    {
        public static Game LoadSave(Ruleset ruleset, string saveFileName, Rules rules)
        {
            GameData gameData = Read.ReadSAVFile(ruleset.FolderPath, saveFileName);

            var hydrator = new LoadedGameObjects(rules, gameData);
            
            // Make an instance of a new game
            return Game.Create(rules, gameData, hydrator, ruleset);
        }
    }
}