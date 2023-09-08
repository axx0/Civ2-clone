using System.Collections.Generic;
using System.Linq;
using Civ2engine.Advances;
using Civ2engine.Enums;
using Civ2engine.Improvements;
using Civ2engine.Units;

namespace Civ2engine
{
    public class Civilization
    {
        public int Id { get; set; }
        public bool Alive { get; set; }
        public int CityStyle { get; set; }

        private EpochType _epoch;
        public EpochType Epoch
        {
            get
            {
                _epoch = EpochType.Ancient;

                if (Advances[(int)AdvanceType.Invention] && Advances[(int)AdvanceType.Philosophy])
                {
                    _epoch = EpochType.Renaissance;
                }

                if (Advances[(int)AdvanceType.Industrializ])
                {
                    _epoch = EpochType.Industrial;
                }

                if (Advances[(int)AdvanceType.Automobile] && Advances[(int)AdvanceType.Electronics])
                {
                    _epoch = EpochType.Modern;
                }

                return _epoch;
            }
        }
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

        public GovernmentType Government { get; set; }

        public bool AnyUnitsAwaitingOrders => Units.Any(unit => unit.AwaitingOrders);

        private int _luxRate;

        public int LuxRate
        {
            get
            {
                _luxRate = 100 - TaxRate - ScienceRate;
                return _luxRate;
            }
            set
            {
                _luxRate = value;
            }
        }

        public int Population => Cities.Sum(c => c.Population);

        public string[] Titles { get; set; }

        public List<Unit> Units { get; } = new();
        public UnitType[] CasualtiesPerUnitType { get; set; }

        public List<City> Cities { get; } = new();
        
        public PlayerType PlayerType { get; set; }
        public int PowerRating { get; set; }
        public int PowerRank { get; set; }
        public int NormalColour { get; set; }
        
        public AdvanceGroupAccess[] AllowedAdvanceGroups { get; set; }
        public Dictionary<Effects, int> GlobalEffects { get; } = new();
    }

    public enum PlayerType
    {
        AI,
        Local,
        Remote,
        Barbarians
    }
}
