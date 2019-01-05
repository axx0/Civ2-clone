using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoskusCiv2
{
    public class Civilization
    {
        public int Id { get; set; }
        public int CityStyle { get; set; }
        public string LeaderName { get; set; }
        public string TribeName { get; set; }
        public string Adjective { get; set; }
        public int Money { get; set; }
        public int ReseachingTech { get; set; }
        public int[] Techs { get; set; }

        private int _population;
        public int Population
        {
            get
            {
                foreach (City city in Game.Cities.Where(n => n.Owner == Id))
                {
                    _population += city.Population;
                }
                return _population;
            }
        }
    }
}
