using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class Freight : BaseUnit
    {
        public Freight() : base(50, 0, 1, 1, 1, 2)
        {
            Type = UnitType.Freight;
            LSA = UnitLSA.Land;
            Name = "Freight";
        }
    }
}
