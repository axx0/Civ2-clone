using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class Chariot : BaseUnit
    {
        public Chariot() : base(30, 3, 1, 1, 1, 2)
        {
            Type = UnitType.Chariot;
            LSA = UnitLSA.Land;
            Name = "Chariot";
        }
    }
}
