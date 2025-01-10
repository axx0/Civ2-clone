using Civ2engine;
using System.Collections.Generic;

namespace Model;

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
