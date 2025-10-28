using System.Collections.Generic;
using System.Linq;
using Civ2engine.Advances;
using Civ2engine.Units;
using Model.Constants;

namespace Civ2engine
{
    public class Civilization
    {
        public int TribeId { get; set; }
        public int Id { get; set; }
        public bool Alive { get; set; }
        public int CityStyle { get; set; }

        public int Epoch { get; set; }
        public string LeaderName { get; set; }
        public int LeaderGender { get; set; }
        public string LeaderTitle { get; set; }
        public string TribeName { get; set; }
        public string Adjective { get; set; }
        public int Money { get; set; }
        
        public int Science { get; set; }
        public int ReseachingAdvance { get; set; } = -1;
        public bool[] Advances { get; set; }
        public int ScienceRate { get; set; }
        public int TaxRate { get; set; }

        public int Government { get; set; }

        public bool AnyUnitsAwaitingOrders => Units.Any(unit => unit.AwaitingOrders);
        
        public int LuxRate => 100 - TaxRate - ScienceRate;

        public string[] Titles { get; set; }

        public List<Unit> Units { get; } = new();
        
        public int[] CasualtiesPerUnitType { get; set; }

        public List<City> Cities { get; } = new();

        public PlayerType PlayerType { get; set; }
        public int PowerRating { get; set; }
        public int PowerRank { get; set; }
        public int NormalColour { get; set; }
        
        public AdvanceGroupAccess[] AllowedAdvanceGroups { get; set; }
        public Dictionary<Effects, int> GlobalEffects { get; } = new();
    }
}
