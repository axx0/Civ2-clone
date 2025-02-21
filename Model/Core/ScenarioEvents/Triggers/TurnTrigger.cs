using System.Collections.Generic;

namespace Model;

public class TurnTrigger : ITrigger
{
    /// <summary>
    /// 0xFFFF = every turn
    /// </summary>
    public int Turn { get; set; }
    public List<string> Strings { get; set; }
}
