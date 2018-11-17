using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class Catapult : BaseUnit
    {
        public Catapult() : base(40, 6, 1, 1, 1, 1)
        {
            Type = UnitType.Catapult;
            LSA = UnitLSA.Land;
            Name = "Catapult";
        }
    }
}
