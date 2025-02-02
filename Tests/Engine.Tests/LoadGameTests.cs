using Civ2engine;
using Civ2engine.IO;
using Civ2engine.SaveLoad;
using Model;

namespace Engine.Tests;

public class LoadGameTests
{

    public LoadGameTests()
    {
        // Need to hard code SearchPaths somehow to get the LoadFrom() method to work.
        // This happens to be where I have the game installed on my local.
        Settings.AddPath("C:\\code\\Civilization_2_2");
    }

    [Fact]
    public void TestLoadFromThrowsExceptionIfPathNotFound()
    {
        // Arrange
        var path = "C:\\code\\Civilization_2_2\\pathtonowhere.sav";
        var mainApp = new MockMainApp();

        // Act
        // Assert
        Assert.Throws<FileNotFoundException>(() => LoadGame.LoadFrom(path, mainApp));
            
    }
}

internal class MockMainApp : IMain
{
    public MockMainApp()
    {
    }

    public Ruleset[] AllRuleSets { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public Ruleset ActiveRuleSet => throw new NotImplementedException();

    public IUserInterface SetActiveRuleSet(int ruleSetIndex)
    {
        throw new NotImplementedException();
    }

    public IUserInterface SetActiveRulesetFromFile(string root, string subDirectory, Dictionary<string, string> extendedMetadata)
    {
        throw new NotImplementedException();
    }
}