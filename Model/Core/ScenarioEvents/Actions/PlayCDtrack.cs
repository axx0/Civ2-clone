using System.Collections.Generic;

namespace Model;

public class PlayCDtrack : IScenarioAction
{
    public int TrackNo { get; set; }
    public List<string> Strings { get; set; }
}
