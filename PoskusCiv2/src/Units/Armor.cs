using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class Armor : BaseUnit
    {
        public Armor() : base(80, 10, 5, 3, 1, 3)
        {
            Type = UnitType.Armor;
            LSA = UnitLSA.Land;
            Name = "Armor";
        }
    }
}
