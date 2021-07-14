using System.Collections.Generic;
using System.Linq;
using Civ2engine.Enums;
using Civ2engine.Units;

namespace Civ2engine
{
    public class Civilization : BaseInstance
    {
        public int Id { get; set; }
        public bool Alive { get; set; }
        public CityStyleType CityStyle { get; set; }
        public string LeaderName { get; set; }
        public int LeaderGender { get; set; }
        public string LeaderTitle { get; set; }
        public string TribeName { get; set; }
        public string Adjective { get; set; }
        public int Money { get; set; }
        public int ReseachingAdvance { get; set; }
        public bool[] Advances { get; set; }
        public int ScienceRate { get; set; }
        public int TaxRate { get; set; }
        public GovernmentType Government { get; set; }
        public bool AnyUnitsAwaitingOrders => Units.Any(unit => unit.Owner == this && unit.AwaitingOrders);

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

        public List<City> Cities { get; } = new();
    }
}
