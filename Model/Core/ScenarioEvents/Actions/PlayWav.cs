namespace Model.Core.ScenarioEvents.Actions;

public class PlayWav : IScenarioAction
{
    public string File { get; set; } = string.Empty;
    public List<string> Strings { get; set; } = [];
}
