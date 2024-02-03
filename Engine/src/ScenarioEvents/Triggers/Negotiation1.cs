using System.Collections.Generic;

namespace Civ2engine;

public class Negotiation1 : ITrigger
{
    /// <summary>
    /// 0xFE = ANYBODY
    /// </summary>
    public int TalkerCivId { get; set; }

    /// <summary>
    /// 0xFE = ANYBODY
    /// </summary>
    public int ListenerCivId { get; set; }
    public int TalkerType { get; set; }
    public int ListenerType { get; set; }
    public List<string> Strings { get; set; }
}
