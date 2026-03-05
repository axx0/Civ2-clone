using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.IO;
using Civ2engine.MapObjects;
using Civ2engine.SaveLoad.SavFile;
using Civ2engine.Scripting;
using Civ2engine.Scripting.ScriptObjects;
using Engine.Tests.TestFiles;
using Model.Constants;
using Model.Core.Units;
using Neo.IronLua;

namespace Engine.Tests;

public static class ApiTestHarness
{
    public static (Game game, AiPlayer aiPlayer, Civilization civ) CreateGameAndAi()
    {
        var testFilesDirectory = TestFileUtils.GetTestFileDirectory();
        var scriptsDirectory = FindScriptsDirectory();
        var ruleset = new Ruleset("mock", new Dictionary<string, string>(), testFilesDirectory, scriptsDirectory);
        Labels.UpdateLabels(ruleset);

        var rules = RulesParser.ParseRules(ruleset);
        var savFile = new JsonSavFile();
        var savePath = TestFileUtils.GetTestFilePath("test_json.sav");
        var game = (Game)savFile.LoadGame(File.ReadAllBytes(savePath), ruleset, rules);

        var civ = game.AllCivilizations.First(c => c.PlayerType != PlayerType.Barbarians);
        
        // Ensure the player is an AiPlayer
        if (game.Players[civ.Id] is not AiPlayer aiPlayer)
        {
             aiPlayer = new AiPlayer(0, civ, game.Maps[0].Tile[0,0], game);
             game.Players[civ.Id] = aiPlayer;
        }

        return (game, aiPlayer, civ);
    }

    public static Tile FindEmptyTile(Game game)
    {
        foreach (var tile in game.Maps[0].Tile)
        {
            if (tile.CityHere == null && tile.UnitsHere.Count == 0)
            {
                return tile;
            }
        }

        throw new InvalidOperationException("No empty tile found for tests.");
    }

    public static Unit CreateUnit(Civilization civ, UnitDefinition type, Tile tile, bool veteran = false)
    {
        var unit = new Unit
        {
            Id = civ.Units.Count,
            Owner = civ,
            TypeDefinition = type,
            CurrentLocation = tile,
            X = tile.X,
            Y = tile.Y,
            MapIndex = tile.Z,
            Veteran = veteran
        };

        civ.Units.Add(unit);
        // tile.UnitsHere.Add(unit); // Already added in CreateUnit? Check if necessary
        return unit;
    }

    public static City CreateCity(Game game, Civilization civ, Tile tile, string name = "Testopolis", int size = 1)
    {
        var city = new City
        {
            Owner = civ,
            WhoBuiltIt = civ,
            Name = name,
            Size = size,
            Location = tile,
            X = tile.X,
            Y = tile.Y,
            MapIndex = tile.Z
        };

        tile.CityHere = city;
        civ.Cities.Add(city);
        game.AllCities.Add(city);

        return city;
    }

    public static string FindScriptsDirectory()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory != null)
        {
            var candidate = Path.Combine(directory.FullName, "Engine", "Scripts");
            if (Directory.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate Engine/Scripts for tests.");
    }
}
