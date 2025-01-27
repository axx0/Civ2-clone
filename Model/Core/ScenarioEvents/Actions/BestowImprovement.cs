using System.Collections.Generic;

namespace Model;

public class BestowImprovement : IScenarioAction
{
    /// <summary>
    /// 0xFC=TRIGGERDEFENDER/TRIGGERRECEIVER, 0xFD=TRIGGERATTACKER
    /// </summary>
    public int RaceId { get; set; }
    public int ImprovementId { get; set; }
    public bool Randomize { get; set; }
    public bool Capital { get; set; }
    public bool Wonders { get; set; }
    public List<string> Strings { get; set; }
}
