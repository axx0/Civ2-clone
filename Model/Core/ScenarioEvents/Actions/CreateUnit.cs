using Civ2engine;
using Civ2engine.Enums;
using System.Collections.Generic;

namespace Model;

public class CreateUnit : IScenarioAction
{
    /// <summary>
    /// 0xFC=TRIGGERRECEIVER/TRIGGERDEFENDER, 0xFD=TRIGGERATTACKER
    /// </summary>
    public int OwnerCivId { get; set; }
    public int CreatedUnitId { get; set; }
    public int[,] Locations { get; set; }
    public int NoLocations { get; set; }
    public bool Veteran { get; set; }
    public int Count { get; set; }
    public City? HomeCity { get; set; }
    public bool Randomize { get; set; }
    public bool InCapital { get; set; }
    public List<string> Strings { get; set; }
}
