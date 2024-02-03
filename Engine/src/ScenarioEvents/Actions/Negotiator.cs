using System.Collections.Generic;

namespace Civ2engine;

public class Negotiator : IScenarioAction
{
    public bool TypeTalker { get; set; }
    public bool StateSet { get; set; }

    /// <summary>
    /// 0xFC=TRIGGERDEFENDER/TRIGGERRECEIVER, 0xFD=TRIGGERATTACKER
    /// </summary>
    public int WhoId { get; set; }
    public List<string> Strings { get; set; }
}
