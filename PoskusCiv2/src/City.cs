using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;
using PoskusCiv2.Improvements;

namespace PoskusCiv2
{
    public class City
    {
        public int X { get; set; }
        public int Y { get; set; }

        public int X2   //Civ2 style
        {
            get { return 2 * X + (Y % 2); }
        }

        public int Y2   //Civ2 style
        {
            get { return Y; }
        }

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

        public IImprovement[] Improvements => _improvements.OrderBy(i => i.Id).ToArray();
        public IImprovement[] Wonders => _wonders.OrderBy(i => i.WId).ToArray();

        private List<IImprovement> _improvements = new List<IImprovement>();
        private List<IImprovement> _wonders = new List<IImprovement>();

        public void AddImprovement(IImprovement improvement) => _improvements.Add(improvement);
        public void AddWonder(IImprovement wonder) => _wonders.Add(wonder);
    }
}
