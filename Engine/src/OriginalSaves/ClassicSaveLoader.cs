namespace Civ2engine
{
    public static class ClassicSaveLoader
    {
        public static void LoadSave(Ruleset ruleset, string saveFileName, Rules rules, IPlayer localPlayer)
        {
            GameData gameData = Read.ReadSAVFile(ruleset.FolderPath, saveFileName);

            var hydrator = new LoadedGameObjects(rules, gameData);
            
            // Make an instance of a new game
            Game.Create(rules, gameData, hydrator, ruleset, localPlayer);
        }
    }
}