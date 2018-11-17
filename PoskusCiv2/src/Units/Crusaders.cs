using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class Crusaders : BaseUnit
    {
        public Crusaders() : base(40, 5, 1, 1, 1, 2)
        {
            Type = UnitType.Crusaders;
            LSA = UnitLSA.Land;
            Name = "Crusaders";
        }
    }
}
