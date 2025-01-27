using System.Collections.Generic;

namespace Model;

public class TransportAction : IScenarioAction
{
    public int UnitId { get; set; }
    public short TransportMask { get; set; }

    /// <summary>
    /// 0x48=native, 0x50=build, 0x60=use
    /// </summary>
    public int TransportMode { get; set; }
	public List<string> Strings { get; set; }
}
