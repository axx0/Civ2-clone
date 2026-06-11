namespace Model.Core.ScenarioEvents.Actions;

public class EndGame : IScenarioAction
{
    public bool EndScreens { get; set; }
    public List<string> Strings { get; set; } = [];
}
