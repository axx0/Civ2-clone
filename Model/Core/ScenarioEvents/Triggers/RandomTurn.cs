using System.Collections.Generic;

namespace Model;

public class RandomTurn : ITrigger
{
    public int Denominator { get; set; }
    public List<string> Strings { get; set; }
}
