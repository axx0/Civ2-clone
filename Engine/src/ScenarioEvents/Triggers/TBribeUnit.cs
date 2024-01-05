using System.Collections.Generic;

namespace Civ2engine;

public class TBribeUnit : ITrigger
{
    /// <summary>
    /// 0xFE = ANYBODY
    /// </summary>
    public int WhoCivID { get; set; }

    /// <summary>
    /// 0xFE = ANYBODY
    /// </summary>
    public int WhomCivId { get; set; }

    /// <summary>
    /// 0xFE = ANYUNIT
    /// </summary>
    public int UnitTypeId { get; set; }
    public List<string> Strings { get; set; }
}
