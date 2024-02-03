using System.Collections.Generic;

namespace Civ2engine;

public class CityTaken : ITrigger
{
    public City? City { get; set; }

    /// <summary>
    /// 0xFE = ANYBODY
    /// </summary>
    public int AttackerCivId { get; set; }

    /// <summary>
    /// 0xFE = ANYBODY
    /// </summary>
    public int DefenderCivId { get; set; }
    public bool IsUnitSpy { get; set; }
    public List<string> Strings { get; set; }
}
