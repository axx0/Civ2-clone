using Civ2engine;
using Civ2engine.IO;
using Civ2engine.SaveLoad;
using Civ2engine.SaveLoad.SavFile;
using Civ2engine.Units;
using Engine.Tests.TestFiles;
using Model;
using Model.Core;
using Model.InterfaceActions;

namespace Engine.Tests;

public class GameLoaderTests
{
    [Fact]
    public void TestLoadGameHandlesScenarioFile()
    {
        // Arrange
        var path = TestFileUtils.GetTestFilePath("test_scenario.scn");
        var savDirectory = TestFileUtils.GetTestFileDirectory();
        var rules = new Rules();
        var activeRuleSet = new Ruleset(
            "mock",
            new Dictionary<string, string>(),
            [savDirectory]);
        var savFile = new MockSavFile();
        var gameLoader = new GameLoader(path, savDirectory, rules, activeRuleSet, savFile);
        var game = new MockGame();
        var activeInterface = new MockInterface();

        // Act
        var result = gameLoader.LoadGame(game, activeInterface);

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IInterfaceAction>(result);
    }

    [Fact]
    public void TestLoadGameHandlesNormalFile()
    {
        // Arrange
        var path = TestFileUtils.GetTestFilePath("test_game.sav");
        var savDirectory = TestFileUtils.GetTestFileDirectory();
        var rules = new Rules();
        var activeRuleSet = new Ruleset(
            "mock",
            new Dictionary<string, string>(),
            [savDirectory]);
        var savFile = new MockSavFile();
        var gameLoader = new GameLoader(path, savDirectory, rules, activeRuleSet, savFile);
        var game = new MockGame();
        var activeInterface = new MockInterface();

        // Act
        var result = gameLoader.LoadGame(game, activeInterface);

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IInterfaceAction>(result);
    }
}

internal class MockSavFile : SavFileBase
{
    public override IGame LoadGame(byte[] fileData, Ruleset activeRuleSet, Rules rules)
    {
        return new MockGame();
    }
}
