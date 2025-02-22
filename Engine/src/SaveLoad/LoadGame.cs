using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Civ2engine.IO;
using Civ2engine.OriginalSaves;
using Model;
using Model.Core;
using Model.InterfaceActions;

namespace Civ2engine.SaveLoad;

public static class LoadGame
{
    public static IInterfaceAction LoadFrom(string path, IMain mainApp)
    {
        var savDirectory = Path.GetDirectoryName(path);
        var root = Settings.SearchPaths.FirstOrDefault(savDirectory.StartsWith) ?? Settings.SearchPaths[0];
        var fileData = File.ReadAllBytes(path);
        bool classicSave = fileData[0] == 67;   // Classic saves start with the word CIVILIZE so if we see a C treat it as old 

        //TODO: File.ReadAllBytes above will throw a FileNotFoundException if the sav file doesn't exist.
        //So the fileData.Length check is not checking the right thing and the throw statement isn't really needed.
        if (fileData.Length == 0)
            throw new FileNotFoundException($"File {path} not found");

        var extendedMetadata = new Dictionary<string, string>();

        JsonDocument jsonDocument = null!;
        if (classicSave)
        {
            var scnNames = new string[] { "Original", "SciFi", "Fantasy" };
            if (fileData[10] > 44)
            {
                extendedMetadata.Add("TOT-Scenario", scnNames[fileData[982]]);
            }
        }
        else
        {
            // We're in new territory... 
            jsonDocument = JsonDocument.Parse(fileData);

            var metaData = jsonDocument.RootElement.GetProperty("extendedMetadata");
            foreach (var meta in metaData.EnumerateObject())
            {
                extendedMetadata[meta.Name] = meta.Value.GetString() ?? string.Empty;
            }
        }

        var activeInterface = mainApp.SetActiveRulesetFromFile(root, savDirectory, extendedMetadata);
        var rules = RulesParser.ParseRules(activeInterface.MainApp.ActiveRuleSet);

        var viewData = new Dictionary<string, string?>();
        IGame game;
        if (classicSave)
        {
            game = Read.ClassicSav(fileData, activeInterface.MainApp.ActiveRuleSet, rules, viewData);

            if (string.Equals(Path.GetExtension(path), ".scn", StringComparison.OrdinalIgnoreCase))
            {
                var scnName = Path.GetFileName(path);
                return activeInterface.HandleLoadScenario(game, scnName, savDirectory);
            }
        }
        else
        {
            
            if (jsonDocument.RootElement.TryGetProperty("viewData", out var viewDataElement))
            {
                foreach (var prop in viewDataElement.EnumerateObject())
                {
                    viewData[prop.Name] = prop.Value.GetString();
                }
            }
            game = GameSerializer.Read(jsonDocument.RootElement.GetProperty("game"), activeInterface.MainApp.ActiveRuleSet, rules);
        }

        return activeInterface.HandleLoadGame(game, rules, activeInterface.MainApp.ActiveRuleSet, viewData);
    }
}