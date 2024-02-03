using System.Collections.Generic;

namespace Civ2engine;

public class TurnInterval : ITrigger
{
    public int Interval { get; set; }
    public List<string> Strings { get; set; }
}
