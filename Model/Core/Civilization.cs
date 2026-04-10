using System.Collections.Generic;
using System.Linq;
using Civ2engine.Advances;
using Civ2engine.Units;
using Model.Constants;
using Model.Core;
using Model.Core.Units;

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

        /// <summary>
        /// Recorded every 2 turns
        /// </summary>
        public List<int> PowerRating { get; set; } = new();
        public int PowerRank { get; set; }
        public int NormalColour { get; set; }
        
        public AdvanceGroupAccess[] AllowedAdvanceGroups { get; set; }
        public Dictionary<Effects, int> GlobalEffects { get; } = new();
        public ThroneRoom ThroneRoom { get; set; } = new();
        public int Betrayals { get; set; }
        public int FutureTechCount { get; set; }
        public int Patience { get; set; }

        public int[] Attitude { get; set; } = [];
        public int[] Reputation { get; set; } = [];
        public Relation?[] Relations { get; set; } = [];
    }

    public class Relation
    {
        public int Summary
        {
            get
            {
                var status = 0;
                if (Contact) status |= 1;
                if (CeaseFire) status |= 2;
                if (Peace) status |= 4;
                if (Alliance) status |= 8;
                if (Vendetta) status |= 16;
                if (Embassy) status |= 32;
                if (War) status |= 64;
                return status;
            }
        }

        public void UpdateFrom(int value)
        {
            Contact = (value & 1) != 0;
            CeaseFire = (value & 2) != 0;
            Peace = (value & 4) != 0;
            Alliance = (value & 8) != 0;
            Vendetta = (value & 16) != 0;
            Embassy = (value & 32) != 0;
            War = (value & 64) != 0;
        }

        public bool War { get; set; }

        public bool Embassy { get; set; }

        public bool Vendetta { get; set; }

        public bool Alliance { get; set; }

        public bool Peace { get; set; }

        public bool CeaseFire { get; set; }

        public bool Contact { get; set; }
    }
}
