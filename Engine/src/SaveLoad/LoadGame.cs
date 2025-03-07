using System.Collections.Generic;
using System.IO;
using System.Linq;
using Civ2engine.IO;
using Civ2engine.SaveLoad.SavFile;
using Model;
using Model.InterfaceActions;

namespace Civ2engine.SaveLoad;

public static class LoadGame
{
    public static IInterfaceAction LoadFrom(string path, IMain mainApp)
    {
        if (File.Exists(path))
        {
            return LoadFromInternal(path, mainApp);
        }
        else
        {
            //TODO: Keeping this here so we can handle and show this as an error in the UI.
            throw new FileNotFoundException($"File {path} not found. Check the filename and try again.");
        }
    }

    private static IInterfaceAction LoadFromInternal(string path, IMain mainApp)
    {
        var fileData = File.ReadAllBytes(path);
        bool classicSave = fileData[0] == 67;   // Classic saves start with the word CIVILIZE so if we see a C treat it as old 

        var extendedMetadata = new Dictionary<string, string>();

        var savDirectory = Path.GetDirectoryName(path);
        var root = Settings.SearchPaths.FirstOrDefault(savDirectory.StartsWith) ?? Settings.SearchPaths[0];
        var activeInterface = mainApp.SetActiveRulesetFromFile(root, savDirectory, extendedMetadata);
        var activeRuleSet = activeInterface.MainApp.ActiveRuleSet;
        var rules = RulesParser.ParseRules(activeRuleSet);        

        if (classicSave)
        {
            var classicSavFile = new ClassicSavFile();
            var gameLoader = new GameLoader(path, savDirectory, rules, activeRuleSet, classicSavFile);
            var game = classicSavFile.LoadGame(fileData, activeRuleSet, rules);
            return gameLoader.LoadGame(game, activeInterface);
        }
        else
        {
            // We're in new territory... 
            var jsonSavFile = new JsonSavFile();
            var gameLoader = new GameLoader(path, savDirectory, rules, activeRuleSet, jsonSavFile);
            var game = jsonSavFile.LoadGame(fileData, activeRuleSet, rules);
            return gameLoader.LoadGame(game, activeInterface);
        }
    }
}