namespace Civ2engine;

public interface IGameData
{
    int DifficultyLevel { get; }
    int TurnNumber { get; }
    int StartingYear { get; }
    int TurnYearIncrement { get; }
    int BarbarianActivity { get; }
    int NoPollutionSkulls { get; }
    int GlobalTempRiseOccured { get; }
    int NoOfTurnsOfPeace { get; }
}