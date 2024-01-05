using System.Collections.Generic;

namespace Civ2engine;

public class AChangeMoney : IAction
{
    /// <summary>
    /// 0xFC=TRIGGERDEFENDER/TRIGGERRECEIVER, 0xFD=TRIGGERATTACKER
    /// </summary>
    public int ReceiverCivId { get; set; }
    public int Amount { get; set; }
    public List<string> Strings { get; set; }
}
