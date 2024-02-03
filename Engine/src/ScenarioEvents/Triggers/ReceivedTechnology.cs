using System.Collections.Generic;

namespace Civ2engine;

public class ReceivedTechnology : ITrigger
{
    public int TechnologyId { get; set; }
    public bool IsFutureTech { get; set; }

    /// <summary>
    /// 0xFE = ANYBODY
    /// </summary>
    public int ReceiverCivId { get; set; }
    public List<string> Strings { get; set; }
}
