namespace Civ2engine;

public interface IGameData
{
    int DifficultyLevel { get; }
    int TurnNumber { get; }
    int StartingYear { get; set;  }
    int TurnYearIncrement { get; set; }
    int BarbarianActivity { get; }
    int NoPollutionSkulls { get; }
    int GlobalTempRiseOccured { get; }
    int NoOfTurnsOfPeace { get; }
    int[] CitiesBuiltSoFar { get; set; }
}