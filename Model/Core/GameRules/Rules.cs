using System.Collections.Generic;
using System.Linq;
using Civ2engine.Advances;
using Civ2engine.Enums;
using Civ2engine.Production;
using Civ2engine.Terrains;
using Civ2engine.Units;
using Model.Core.Advances;
using Model.Core.Cities;

namespace Civ2engine
{
    public class Rules
    {
        // Game rules from RULES.txt

        // Cosmic rules
        public CosmicRules Cosmic { get; } = new();

        // Units
        public UnitDefinition[] UnitTypes { get; set; }

        // Advances
        public Advance[] Advances { get; set; }

        public readonly Dictionary<string, int> AdvanceMappings = new()
        {
            {"nil", AdvancesConstants.Nil},
            {"no", AdvancesConstants.No}
        };

        // Trading commodities
        public Commodity[] CaravanCommoditie { get; set; }

        // Difficulty
        public string[] Difficulty { get; set; }

        // Attitudes
        public string[] Attitude { get; set; }

        public Improvement[] Improvements { get; set; }


        public IList<Terrain[]> Terrains { get; set; }
        public Government[] Governments { get; set; }
        public LeaderDefaults[] Leaders { get; set; }
        public Orders[] Orders { get; set; }

        public MapParams[] Maps { get; set; } = {new() {Type = MapType.Standard}};

        private IProductionOrder[]? _items;

        public List<TerrainImprovement> TerrainImprovements { get; } = new List<TerrainImprovement>();

        public IProductionOrder[]? ProductionOrders
        {
            get => _items;
            set => _items = value;
        }

        public int FirstWonderIndex { get; set; }
    }
}
