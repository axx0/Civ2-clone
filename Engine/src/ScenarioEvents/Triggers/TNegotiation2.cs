﻿using System.Collections.Generic;

namespace Civ2engine;

public class TNegotiation2 : ITrigger
{
    public int TalkerMask { get; set; }
    public int ListenerMask { get; set; }
    public List<string> Strings { get; set; }
}
