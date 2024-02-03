using System.Collections.Generic;

namespace Civ2engine;

public class NoSchism : ITrigger
{
    /// <summary>
    /// 0xFE = ANYBODY
    /// </summary>
    public int CivId { get; set; }
    public List<string> Strings { get; set; }
}
