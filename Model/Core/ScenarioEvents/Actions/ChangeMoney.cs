using System.Collections.Generic;

namespace Model;

public class ChangeMoney : IScenarioAction
{
    /// <summary>
    /// 0xFC=TRIGGERDEFENDER/TRIGGERRECEIVER, 0xFD=TRIGGERATTACKER
    /// </summary>
    public int ReceiverCivId { get; set; }
    public int Amount { get; set; }
    public List<string> Strings { get; set; }
}
