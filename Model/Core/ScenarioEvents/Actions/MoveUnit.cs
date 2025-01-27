using System.Collections.Generic;

namespace Model;

public class MoveUnit : IScenarioAction
{
    /// <summary>
    /// 0xFD=TRIGGERATTACKER, 0xFC=TRIGGERRECEIVER
    /// </summary>
    public int OwnerCivId { get; set; }

    /// <summary>
    /// 0xFE=ANYUNIT
    /// </summary>
    public int UnitMovedId { get; set; }
    public int MapId { get; set; }
    public int[,] MapCoords { get; set; }
    public int[] MapDest { get; set; }

    /// <summary>
    /// 0xFE=ALLUNITS
    /// </summary>
    public int NumberToMove { get; set; }
    public List<string> Strings { get; set; }
}
