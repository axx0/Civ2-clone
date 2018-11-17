using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class Caravan : BaseUnit
    {
        public Caravan() : base(50, 0, 1, 1, 1, 1)
        {
            Type = UnitType.Caravan;
            LSA = UnitLSA.Land;
            Name = "Caravan";
        }
    }
}
