using System.Collections.Generic;

namespace Civ2engine;

public class AMoveUnit : IAction
{
    public int OwnerCivId { get; set; }
    public int UnitMovedId { get; set; }
    public int[] MapCoords { get; set; }
    public int[] MapDest { get; set; }
    public int NumberToMove { get; set; }
    public List<string> Strings { get; set; }
}
