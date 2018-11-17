using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class Diplomat : BaseUnit
    {
        public Diplomat() : base(30, 0, 0, 1, 1, 2)
        {
            Type = UnitType.Diplomat;
            LSA = UnitLSA.Land;
            Name = "Diplomat";
        }
    }
}
