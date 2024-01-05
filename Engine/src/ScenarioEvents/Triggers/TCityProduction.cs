using System.Collections.Generic;

namespace Civ2engine;

public class TCityProduction : ITrigger
{
    /// <summary>
    /// 0xFE = ANYBODY
    /// </summary>
    public int BuilderCivId { get; set; }

    /// <summary>
    /// Improvements start @FF down
    /// </summary>
    public int ImprovementUnitId { get; set; }
    public List<string> Strings { get; set; }
}
