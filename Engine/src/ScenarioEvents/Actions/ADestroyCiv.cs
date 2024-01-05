using System.Collections.Generic;

namespace Civ2engine;

public class ADestroyCiv : IAction
{
    /// <summary>
    /// 0xFC=TRIGGERDEFENDER/TRIGGERRECEIVER, 0xFD=TRIGGERATTACKER
    /// </summary>
    public int CivId { get; set; }
	public List<string> Strings { get; set; }
}
