using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.IO;
using Civ2engine.MapObjects;
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

internal class MockGame : IGame
{
    public FastRandom Random => throw new NotImplementedException();
    public Civilization GetPlayerCiv => throw new NotImplementedException();
    public IDictionary<int, TerrainImprovement> TerrainImprovements => throw new NotImplementedException();
    public Rules Rules => throw new NotImplementedException();
    public Civilization GetActiveCiv => throw new NotImplementedException();
    public Options Options => throw new NotImplementedException();
    public Scenario ScenarioData => throw new NotImplementedException();
    public IPlayer ActivePlayer => throw new NotImplementedException();
    public IScriptEngine Script => throw new NotImplementedException();
    public IList<Map> Maps => throw new NotImplementedException();
    public IHistory History => throw new NotImplementedException();
    public Dictionary<string, List<string>?> CityNames => throw new NotImplementedException();

    public event EventHandler<PlayerEventArgs> OnPlayerEvent;
    public event EventHandler<UnitEventArgs> OnUnitEvent;

    public void ConnectPlayer(IPlayer player) => throw new NotImplementedException();
    public string Order2String(int unitOrder) => throw new NotImplementedException();
    public void ChooseNextUnit() => throw new NotImplementedException();
    public bool ProcessEndOfTurn() => throw new NotImplementedException();
    public void ChoseNextCiv() => throw new NotImplementedException();
    public void TriggerMapEvent(MapEventType updateMap, List<Tile> tiles) => throw new NotImplementedException();
    public double MaxDistance => throw new NotImplementedException();
    public int DifficultyLevel { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public IGameDate Date => throw new NotImplementedException();
    public int TurnNumber => throw new NotImplementedException();
    public List<City> AllCities => throw new NotImplementedException();
    public IPlayer[] Players => throw new NotImplementedException();
    public int PollutionSkulls => throw new NotImplementedException();
    public int GlobalTempRiseOccured => throw new NotImplementedException();
    public int NoOfTurnsOfPeace => throw new NotImplementedException();
    public int BarbarianActivity => throw new NotImplementedException();
    public int NoMaps => throw new NotImplementedException();
    public List<Civilization> AllCivilizations => throw new NotImplementedException();
    public void TriggerUnitEvent(UnitEventType eventType, IUnit triggerUnit, BlockedReason reason = BlockedReason.NotBlocked) => throw new NotImplementedException();
    public void TriggerUnitEvent(UnitEventArgs combatEventArgs) => throw new NotImplementedException();
    public void SetHumanPlayer(int playerCivId) => throw new NotImplementedException();
    public void StartPlayerTurn(IPlayer activePlayer) => throw new NotImplementedException();
    public void StartNextTurn() => throw new NotImplementedException();
}
