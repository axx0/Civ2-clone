using System.Collections.Generic;

namespace Model;

public class EndGame : IScenarioAction
{
    public bool EndScreens { get; set; }
    public List<string> Strings { get; set; }
}
