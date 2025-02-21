using System.Collections.Generic;

namespace Model;

public class PlayWav : IScenarioAction
{
    public string File { get; set; }
    public List<string> Strings { get; set; }
}
