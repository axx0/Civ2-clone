using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class Phalanx : BaseUnit
    {
        public Phalanx() : base(20, 1, 2, 1, 1, 1)
        {
            Type = UnitType.Phalanx;
            LSA = UnitLSA.Land;
            Name = "Phalanx";
        }
    }
}
