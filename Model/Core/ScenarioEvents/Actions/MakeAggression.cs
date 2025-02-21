using System.Collections.Generic;

namespace Model;

public class MakeAggression : IScenarioAction
{
    /// <summary>
    /// 0xFC=TRIGGERRECEIVER/TRIGGERDEFENDER, 0xFD=TRIGGERATTACKER
    /// </summary>
    public int WhomCivId { get; set; }

    /// <summary>
    /// 0xFC=TRIGGERRECEIVER/TRIGGERDEFENDER, 0xFD=TRIGGERATTACKER
    /// </summary>
    public int WhoCivId { get; set; }
    public List<string> Strings { get; set; }
}
