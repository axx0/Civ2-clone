using System.Collections.Generic;

namespace Model;

/// <summary>
/// (Who + Technology + Flag)
/// or
/// (Mask + Threshold)
/// or
/// (Count + State)
/// </summary>
public class FlagAction : IScenarioAction
{
    /// <summary>
    /// On, Off, Set or Clear
    /// </summary>
    public bool State { get; set; }
    public bool MaskUsed { get; set; }
    public bool Continuous { get; set; }
    public int Flag { get; set; }
    public int Mask { get; set; }

    /// <summary>
    /// 0xFA=OMITTED, 0xFC=TRIGGERDEFENDER/TRIGGERRECEIVER, 0xFD=TRIGGERATTACKER
    /// </summary>
    public int WhoId { get; set; }
    public List<string> Strings { get; set; }
}
