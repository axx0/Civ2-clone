namespace Model.Core.ScenarioEvents.Actions;

public class DestroyCiv : IScenarioAction
{
    /// <summary>
    /// 0xFC=TRIGGERDEFENDER/TRIGGERRECEIVER, 0xFD=TRIGGERATTACKER
    /// </summary>
    public int CivId { get; set; }
	public List<string> Strings { get; set; }
}
