using System.Collections.Generic;

namespace Civ2engine;

public class TNoSchism : ITrigger
{
    public int CivId { get; set; }
    public List<string> Strings { get; set; }
}
