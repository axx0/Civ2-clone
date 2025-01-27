using System.Collections.Generic;

namespace Model;

public class NoSchism : ITrigger
{
    /// <summary>
    /// 0xFE = ANYBODY
    /// </summary>
    public int CivId { get; set; }
    public List<string> Strings { get; set; }
}
