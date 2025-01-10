using System.Collections.Generic;

namespace Model;

public class TextAction : IScenarioAction
{
    public bool NoBroadcast { get; set; }
    public List<string> Strings { get; set; }
}
