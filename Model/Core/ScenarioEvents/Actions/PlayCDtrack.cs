namespace Model.Core.ScenarioEvents.Actions;

public class PlayCDtrack : IScenarioAction
{
    public int TrackNo { get; set; }
    public List<string> Strings { get; set; }
}
