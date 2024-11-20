using Model.Core;

namespace Civ2engine.SaveLoad;

/// <summary>
/// Encapsulate and map the fields of the game object that need to be serialized
/// </summary>
public class JsonGameData : IGameData
{
    /// <summary>
    /// This constructor is to deserialize data
    /// </summary>
    public JsonGameData()
    {
        
    }

    /// <summary>
    /// This constructor provides mapping from the game object for serialization
    /// </summary>
    /// <param name="game"></param>
    public JsonGameData(IGame game)
    {
        DifficultyLevel = game.DifficultyLevel;
        TurnNumber = game.TurnNumber;
        StartingYear = game.Date.StartingYear == -4000 ? 0 : game.Date.StartingYear;
        TurnYearIncrement = game.Date.TurnYearIncrement;
        BarbarianActivity = game.BarbarianActivity;
        NoPollutionSkulls = game.PollutionSkulls;
        GlobalTempRiseOccured = game.GlobalTempRiseOccured;
        NoOfTurnsOfPeace = game.NoOfTurnsOfPeace;
    }
    public int DifficultyLevel { set; get; }
    public int TurnNumber { set; get; }
    public int StartingYear { set; get; }
    public int TurnYearIncrement { set; get; }
    public int BarbarianActivity { set; get; }
    public int NoPollutionSkulls { set; get; }
    public int GlobalTempRiseOccured { set; get; }
    public int NoOfTurnsOfPeace { set; get; }
}