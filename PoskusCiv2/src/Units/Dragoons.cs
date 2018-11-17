using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class Dragoons : BaseUnit
    {
        public Dragoons() : base(50, 5, 2, 2, 1, 2)
        {
            Type = UnitType.Dragoons;
            LSA = UnitLSA.Land;
            Name = "Dragoons";
        }
    }
}
