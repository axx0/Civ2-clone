using System.Collections.Generic;

namespace Model;

public class EnableTechnology : IScenarioAction
{
    /// <summary>
    /// 0xFC=TRIGGERDEFENDER/TRIGGERRECEIVER, 0xFD=TRIGGERATTACKER
    /// </summary>
    public int WhomId { get; set; }
    public int TechnologyId { get; set; }
    public int Value { get; set; }
    public List<string> Strings { get; set; }
}
