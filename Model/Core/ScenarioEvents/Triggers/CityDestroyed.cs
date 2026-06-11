namespace Model.Core.ScenarioEvents.Triggers;

public class CityDestroyed : ITrigger
{
    public int CityId { get; set; }

    /// <summary>
    /// 0xFE = ANYBODY
    /// </summary>
    public int OwnerId { get; set; }
    public List<string> Strings { get; set; } = [];
}
