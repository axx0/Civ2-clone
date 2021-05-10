using System;
using System.Collections.Generic;
using Civ2engine.Advances;
using Civ2engine.Improvements;
using Civ2engine.Terrains;
using Civ2engine.Units;

namespace Civ2engine
{
    public class Rules
    {
        // Game rules from RULES.txt

        // Cosmic rules
        public CosmicRules Cosmic { get; } = new();

        // Units
        public UnitDefinition[] UnitTypes { get;  internal set; }

        // Advances
        public Advance[] Advances { get;  internal set; }

        public readonly Dictionary<string, int> AdvanceMappings = new()
        {
            {"nil", -1},
            {"no", -2}
        };
        
        // Trading commodities
        public string[] CaravanCommoditie { get; internal set; }
        
        // Difficulty
        public string[] Difficulty { get; internal set; }

        // Attitudes
        public string[] Attitude { get; internal set; }
        
        public Improvement[] Improvements { get; internal set; }
        
        
        public Terrain[] Terrains { get; internal set; }
        public Government[] Governments { get; internal set; }
        public LeaderDefaults[] Leaders { get; internal set; }
        public Order[] Orders { get; internal set; }
    }
}
