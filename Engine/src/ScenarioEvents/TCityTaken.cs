using System.Collections.Generic;

namespace Civ2engine;

public class TCityTaken : ITrigger
{
    public City? City { get; set; }
    public int AttackerCivId { get; set; }
    public int DefenderCivId { get; set; }
    public List<string> Strings { get; set; }
}
