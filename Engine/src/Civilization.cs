using System.Linq;
using Civ2engine.Enums;

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
        public bool AnyUnitsAwaitingOrders => Game.AllUnits.Any(unit => unit.Owner == this && unit.AwaitingOrders);

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

        private int _population;
        public int Population
        {
            get
            {
                _population = 0;
                foreach (City city in Game.GetCities.Where(n => n.Owner == this))
                {
                    _population += city.Population;
                }
                return _population;
            }
        }

        public string[] Titles { get; set; }
    }
}
