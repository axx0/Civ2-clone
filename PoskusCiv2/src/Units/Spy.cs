using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class Spy : BaseUnit
    {
        public Spy() : base(30, 0, 0, 1, 1, 3)
        {
            Type = UnitType.Spy;
            LSA = UnitLSA.Land;
            Name = "Spy";
        }
    }
}
