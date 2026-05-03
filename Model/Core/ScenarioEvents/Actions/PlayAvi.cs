namespace Model.Core.ScenarioEvents.Actions;

public class PlayAvi : IScenarioAction
{
    public string File { get; set; }
    public List<string> Strings { get; set; }
}
