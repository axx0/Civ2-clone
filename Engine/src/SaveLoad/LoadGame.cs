using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Civ2engine.IO;
using Civ2engine.OriginalSaves;
using Model;
using Model.InterfaceActions;

namespace Civ2engine.SaveLoad;

public static class LoadGame
{
    public static IInterfaceAction LoadFrom(string path, IMain mainApp)
    {
        var savDirectory = Path.GetDirectoryName(path);
        var root = Settings.SearchPaths.FirstOrDefault(p => savDirectory.StartsWith(p)) ?? Settings.SearchPaths[0];

        //TODO: File.ReadAllBytes will throw a FileNotFoundException if this file doesn't exist.
        //So the fileData.Length check is not checking the right thing and the throw statement at the end is not quite right here.
        var fileData = File.ReadAllBytes(path);

        if (fileData.Length > 0)
        {
            if (fileData[0] == 67) // Classic saves start with the word CIVILIZE so if we see a C treat it as old 
            {
                GameData gameData = Read.ReadSavFile(fileData);

                return mainApp.SetActiveRulesetFromFile(root, savDirectory, gameData.ExtendedMetadata).HandleLoadClassicGame(gameData);
            }

            // We're in new territory... 
            var jsonDocument = JsonDocument.Parse(fileData);

            var metaData = jsonDocument.RootElement.GetProperty("extendedMetadata");
            var extendedMetadata = new Dictionary<string, string>();
            foreach (var meta in metaData.EnumerateObject())
            {
                extendedMetadata[meta.Name] = meta.Value.GetString() ?? string.Empty;
            }
            
            var activeInterface = mainApp.SetActiveRulesetFromFile(root, savDirectory, extendedMetadata);
            
            var rules = RulesParser.ParseRules(activeInterface.MainApp.ActiveRuleSet);
            
            var game = GameSerializer.Read(jsonDocument.RootElement.GetProperty("game"), activeInterface.MainApp.ActiveRuleSet, rules);
            

            return activeInterface.HandleLoadGame(game, rules, activeInterface.MainApp.ActiveRuleSet);
        }
        throw new FileNotFoundException($"File {path} not found");
    }
}