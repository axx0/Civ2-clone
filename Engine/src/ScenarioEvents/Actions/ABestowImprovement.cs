﻿using System.Collections.Generic;

namespace Civ2engine;

public class ABestowImprovement : IAction
{
    /// <summary>
    /// 0xFC=TRIGGERDEFENDER/TRIGGERRECEIVER, 0xFD=TRIGGERATTACKER
    /// </summary>
    public int RaceId { get; set; }
    public int ImprovementId { get; set; }
    public bool Randomize { get; set; }
    public bool Capital { get; set; }
    public bool Wonders { get; set; }
    public List<string> Strings { get; set; }
}
