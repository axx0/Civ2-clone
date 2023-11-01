using Civ2engine.Enums;
using System.Collections.Generic;

namespace Civ2engine;

public class ACreateUnit : IAction
{
    public int OwnerCivId { get; set; }
    public UnitType CreatedUnit { get; set; }
    public int[,] Locations { get; set; }
    public bool Veteran { get; set; }
    public City? HomeCity { get; set; }
    public List<string> Strings { get; set; }
}
