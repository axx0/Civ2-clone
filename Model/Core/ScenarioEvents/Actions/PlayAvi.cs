using System.Collections.Generic;

namespace Model;

public class PlayAvi : IScenarioAction
{
    public string File { get; set; }
    public List<string> Strings { get; set; }
}
