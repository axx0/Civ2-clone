using System.Collections.Generic;

namespace Civ2engine;

public class TTurnInterval : ITrigger
{
    public int Interval { get; set; }
    public List<string> Strings { get; set; }
}
