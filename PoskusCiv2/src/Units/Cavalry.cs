using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class Cavalry : BaseUnit
    {
        public Cavalry() : base(60, 8, 3, 2, 1, 2)
        {
            Type = UnitType.Cavalry;
            LSA = UnitLSA.Land;
            Name = "Cavalry";
        }
    }
}
