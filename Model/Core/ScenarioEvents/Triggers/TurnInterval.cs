namespace Model.Core.ScenarioEvents.Triggers;

public class TurnInterval : ITrigger
{
    public int Interval { get; set; }
    public List<string> Strings { get; set; }
}
