using System.Collections.Generic;

namespace Model;

public class CityProduction : ITrigger
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
