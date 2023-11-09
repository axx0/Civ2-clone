using Civ2engine.Enums;
using System.Collections.Generic;

namespace Civ2engine;

public class TUnitKilled : ITrigger
{
    public UnitType UnitKilled { get; set; }
    public int AttackerCivId { get; set; }
    public int DefenderCivId { get; set; }
    public List<string> Strings { get; set; }
}
