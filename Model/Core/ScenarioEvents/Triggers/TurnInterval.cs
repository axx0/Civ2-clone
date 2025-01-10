using System.Collections.Generic;

namespace Model;

public class TurnInterval : ITrigger
{
    public int Interval { get; set; }
    public List<string> Strings { get; set; }
}
