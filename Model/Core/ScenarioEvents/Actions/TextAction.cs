namespace Model.Core.ScenarioEvents.Actions;

public class TextAction : IScenarioAction
{
    public bool NoBroadcast { get; set; }
    public List<string> Strings { get; set; } = [];
}
