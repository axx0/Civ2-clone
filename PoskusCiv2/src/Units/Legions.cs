using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class Legions : BaseUnit
    {
        public Legions() : base(TechType.Gunpowder, 0, 1, 0, 4, 2, 1, 1, 4, 0, 1, TechType.IronWorking, "000000000000000")
        {
            Type = UnitType.Legions;
            Name = "Legion";
        }
    }
}
