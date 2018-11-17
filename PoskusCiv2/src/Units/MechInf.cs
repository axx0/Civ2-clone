using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class MechInf : BaseUnit
    {
        public MechInf() : base(50, 6, 6, 3, 1, 3)
        {
            Type = UnitType.MechInf;
            LSA = UnitLSA.Land;
            Name = "Mech. Inf.";
        }
    }
}
