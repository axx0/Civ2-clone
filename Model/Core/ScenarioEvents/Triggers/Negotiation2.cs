using System.Collections.Generic;

namespace Model;

public class Negotiation2 : ITrigger
{
    public int TalkerMask { get; set; }
    public int ListenerMask { get; set; }
    public List<string> Strings { get; set; }
}
