using System.Collections.Generic;

namespace Civ2engine;

public class TNegotiation : ITrigger
{
    public int TalkerCivId { get; set; }
    public int ListenerCivId { get; set; }
    public int TalkerType { get; set; }
    public int ListenerType { get; set; }
    public List<string> Strings { get; set; }
}
