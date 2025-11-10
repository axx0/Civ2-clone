using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.MapObjects;
using Model.Core;
using Model.Core.Units;

namespace Engine.Tests;

internal class MockGame : IGame
{
    public FastRandom Random => throw new NotImplementedException();
    public Civilization GetPlayerCiv => throw new NotImplementedException();
    public IDictionary<int, TerrainImprovement> TerrainImprovements => throw new NotImplementedException();
    public IImprovementEncoder ImprovementEncoder { get; }
    public Rules Rules { get; set; }
    public Civilization GetActiveCiv => throw new NotImplementedException();
    public Options Options => throw new NotImplementedException();
    public Scenario ScenarioData => throw new NotImplementedException();
    public IPlayer ActivePlayer => Players[0];
    public IScriptEngine Script => throw new NotImplementedException();
    public IList<Map> Maps { get; init; }

    public IHistory History => throw new NotImplementedException();
    public Dictionary<string, List<string>?> CityNames => throw new NotImplementedException();

    public void ConnectPlayer(IPlayer player) => throw new NotImplementedException();
    public string Order2String(int unitOrder) => throw new NotImplementedException();
    public void ChooseNextUnit() => throw new NotImplementedException();
    public bool ProcessEndOfTurn() => throw new NotImplementedException();
    public void ChoseNextCiv() => throw new NotImplementedException();
    public void TriggerMapEvent(MapEventType updateMap, List<Tile> tiles) {
        
    }
    
    public double MaxDistance => throw new NotImplementedException();
    public int DifficultyLevel { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public IGameDate Date => throw new NotImplementedException();
    public int TurnNumber => throw new NotImplementedException();
    public List<City> AllCities => throw new NotImplementedException();
    public IPlayer[] Players { get; set; }

    public int PollutionSkulls => throw new NotImplementedException();
    public int GlobalTempRiseOccured => throw new NotImplementedException();
    public int NoOfTurnsOfPeace => throw new NotImplementedException();
    public int BarbarianActivity => throw new NotImplementedException();
    public int NoMaps => throw new NotImplementedException();
    public List<Civilization> AllCivilizations { get; set; }

    public void TriggerUnitEvent(UnitEventType eventType, IUnit triggerUnit, BlockedReason reason = BlockedReason.NotBlocked) => throw new NotImplementedException();
    public void SetHumanPlayer(int playerCivId) => throw new NotImplementedException();
    public void StartPlayerTurn(IPlayer activePlayer) => throw new NotImplementedException();
    public void StartNextTurn() => throw new NotImplementedException();
}