namespace Model.Core.ScenarioEvents.Triggers;

public class RandomTurn : ITrigger
{
    public int Denominator { get; set; }
    public List<string> Strings { get; set; }
}
