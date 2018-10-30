using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoskusCiv2
{
    public class City
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool CanBuildCoastal { get; set; }
        public bool AutobuildMilitaryRule { get; set; }
        public bool StolenTech { get; set; }
        public bool ImprovementSold { get; set; }
        public bool WeLoveKingDay { get; set; }
        public bool CivilDisorder { get; set; }
        public bool CanBuildShips { get; set; }
        public bool Objectivex3 { get; set; }
        public bool Objectivex1 { get; set; }
        public int Owner { get; set; }
        public int Size { get; set; }
        public int WhoBuiltIt { get; set; }
        public int FoodBox { get; set; }
        public int ShieldBox { get; set; }
        public int NetTrade { get; set; }
        public string Name { get; set; }
        public int WorkersInnerCircle { get; set; }
        public int WorkersOn8 { get; set; }
        public int WorkersOn4 { get; set; }
        public int NoOfSpecialistsx4 { get; set; }

    }
}
