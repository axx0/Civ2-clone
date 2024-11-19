namespace Model.Core;

public interface IGameDate
{
    string GameYearString(int turnNo, string separator = " ");
    int TurnYearIncrement { get; }
    int StartingYear { get; }
}