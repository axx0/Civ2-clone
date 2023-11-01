using System.Collections.Generic;

namespace Civ2engine;

public class TReceivedTechnology : ITrigger
{
    public int TechnologyId { get; set; }
    public int ReceiverCivId { get; set; }
    public List<string> Strings { get; set; }
}
