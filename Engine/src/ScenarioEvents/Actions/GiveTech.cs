using System.Collections.Generic;

namespace Civ2engine;

public class GiveTech : IScenarioAction
{
    /// <summary>
    /// 0xFC=TRIGGERDEFENDER/TRIGGERRECEIVER, 0xFD=TRIGGERATTACKER
    /// </summary>
    public int CivId { get; set; }
    public int TechId { get; set; }
	public List<string> Strings { get; set; }
}
