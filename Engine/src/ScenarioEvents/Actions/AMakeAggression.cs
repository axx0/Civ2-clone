using System.Collections.Generic;

namespace Civ2engine;

public class AMakeAggression : IAction
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
