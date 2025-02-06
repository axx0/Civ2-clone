using System.Collections;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.MapObjects;
using Civ2engine.Units;

namespace Model.Core;

public interface IGame
{
    FastRandom Random { get;  }
    Civilization GetPlayerCiv { get; }
    IDictionary<int,TerrainImprovement> TerrainImprovements { get; }
    
    Rules Rules { get; }
    Civilization GetActiveCiv { get; }
    Options Options { get; }
    Scenario ScenarioData { get; }
    
    IPlayer ActivePlayer { get; }
    IScriptEngine Script { get; }
    
    IList<Map> Maps { get; }
    IHistory History { get; }
    Dictionary<string, List<string>?> CityNames { get; }
    void ConnectPlayer(IPlayer player);
    string Order2String(int unitOrder);
    
    event EventHandler<PlayerEventArgs> OnPlayerEvent;
    void ChooseNextUnit();
    bool ProcessEndOfTurn();
    void ChoseNextCiv();
    void TriggerMapEvent(MapEventType updateMap, List<Tile> tiles);
    double MaxDistance { get; }
    int DifficultyLevel { get; set; }
    IGameDate Date { get; }
    
    int TurnNumber { get; }
    List<City> AllCities { get; }
    IPlayer[] Players { get; }
    int PollutionSkulls { get; }
    int GlobalTempRiseOccured { get; }
    int NoOfTurnsOfPeace { get; }
    int BarbarianActivity { get; }
    int NoMaps { get; }
    List<Civilization> AllCivilizations { get; }

    event EventHandler<UnitEventArgs> OnUnitEvent;
    void TriggerUnitEvent(UnitEventType eventType, IUnit triggerUnit, BlockedReason reason = BlockedReason.NotBlocked);
    void TriggerUnitEvent(UnitEventArgs combatEventArgs);
    void SetHumanPlayer(int playerCivId);
    void StartPlayerTurn(IPlayer activePlayer);
    void StartNextTurn();
}   