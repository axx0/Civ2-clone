using System.Collections.Generic;

namespace Model;

public class BribeUnit : ITrigger
{
    /// <summary>
    /// 0xFE = ANYBODY
    /// </summary>
    public int WhoCivId { get; set; }

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
