using Civ2engine;
using Civ2engine.IO;
using Civ2engine.MapObjects;
using Civ2engine.SaveLoad.SavFile;
using Civ2engine.Scripting.ScriptObjects;
using Core.Tests.TestFiles;
using Model.Constants;
using Model.Core.Units;
using Neo.IronLua;

namespace Core.Tests.Ai;

public class AiScriptHarnessTests
{
    [Fact]
    public void DefenderFortifies_WhenCityUnderDefended()
    {
        var (game, aiPlayer, civ) = CreateGameAndAi();
        var tile = FindEmptyTile(game);
        CreateCity(game, civ, tile, size: 1);

        var defenderType = game.Rules.UnitTypes.First(t => t.AIrole == AiRoleType.Defend);
        var unit = CreateUnit(civ, defenderType, tile, veteran: false);

        var result = aiPlayer.Ai.Call(AiEvent.UnitOrdersNeeded, new LuaTable { { "Unit", new UnitApi(unit, game) } });

        Assert.Equal("F", UnwrapLuaResult(result));
    }

    [Fact]
    public void DefenderFortifies_WhenVeteranOutranksWeakestDefender()
    {
        var (game, aiPlayer, civ) = CreateGameAndAi();
        var tile = FindEmptyTile(game);
        CreateCity(game, civ, tile, size: 1);

        var defenderType = game.Rules.UnitTypes.First(t => t.AIrole == AiRoleType.Defend);
        CreateUnit(civ, defenderType, tile, veteran: false);
        CreateUnit(civ, defenderType, tile, veteran: false);
        var unit = CreateUnit(civ, defenderType, tile, veteran: true);

        var result = aiPlayer.Ai.Call(AiEvent.UnitOrdersNeeded, new LuaTable { { "Unit", new UnitApi(unit, game) } });

        Assert.Equal("F", UnwrapLuaResult(result));
    }

    [Fact]
    public void ResearchComplete_ReturnsBestTech()
    {
        var (game, aiPlayer, civ) = CreateGameAndAi();

        var tech1 = new Tech(game.Rules.Advances, 0) { aiValue = 10, name = "Low Value", epoch = 0 };
        var tech2 = new Tech(game.Rules.Advances, 1) { aiValue = 50, name = "High Value", epoch = 1 };
        var tech3 = new Tech(game.Rules.Advances, 2) { aiValue = 30, name = "Medium Value", epoch = 0 };

        var possibilities = new LuaTable
        {
            [1] = tech1,
            [2] = tech2,
            [3] = tech3
        };

        var result = aiPlayer.Ai.Call(AiEvent.ResearchComplete,
            new LuaTable { { "researchPossibilities", possibilities } });

        var selectedTech = Assert.IsType<Tech>(UnwrapLuaResult(result));
        Assert.Equal(50, selectedTech.aiValue);
        Assert.Equal("High Value", selectedTech.name);
    }

    [Fact]
    public void ResearchComplete_BiasesTowardsLowerEpochs()
    {
        var (game, aiPlayer, civ) = CreateGameAndAi();

        // tech1 has higher aiValue but higher epoch
        // tech2 has lower aiValue but lower epoch
        var tech1 = new Tech(game.Rules.Advances, 0) { aiValue = 100, name = "Modern Tech", epoch = 5 };
        var tech2 = new Tech(game.Rules.Advances, 1) { aiValue = 60, name = "Ancient Tech", epoch = 0 };

        var possibilities = new LuaTable
        {
            [1] = tech1,
            [2] = tech2
        };

        var result = aiPlayer.Ai.Call(AiEvent.ResearchComplete,
            new LuaTable { { "researchPossibilities", possibilities } });

        var selectedTech = Assert.IsType<Tech>(UnwrapLuaResult(result));
        
        // With biasing towards lower epochs, Ancient Tech (epoch 0) should be preferred even with lower aiValue
        Assert.Equal("Ancient Tech", selectedTech.name);
    }

    private static (Game game, AiPlayer aiPlayer, Civilization civ) CreateGameAndAi()
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
        var aiPlayer = (AiPlayer)game.Players[civ.Id];

        // Ensure the tribe objects have access to the game if they are used in scripts
        // But for these tests, we are mostly calling Ai.Call directly.

        return (game, aiPlayer, civ);
    }

    private static Tile FindEmptyTile(Game game)
    {
        foreach (var tile in game.Maps[0].Tile)
        {
            if (tile.CityHere == null && tile.UnitsHere.Count == 0)
            {
                return tile;
            }
        }

        throw new InvalidOperationException("No empty tile found for AI script harness.");
    }

    private static City CreateCity(Game game, Civilization civ, Tile tile, int size)
    {
        var city = new City
        {
            Owner = civ,
            WhoBuiltIt = civ,
            Name = "Testopolis",
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

    private static Unit CreateUnit(Civilization civ, UnitDefinition type, Tile tile, bool veteran)
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
        return unit;
    }

    private static string FindScriptsDirectory()
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

        throw new DirectoryNotFoundException("Could not locate Engine/Scripts for AI script tests.");
    }

    private static object? UnwrapLuaResult(object? result)
    {
        if (result is LuaResult { Count: > 0 } luaResult)
        {
            return luaResult[0];
        }

        return result;
    }
}