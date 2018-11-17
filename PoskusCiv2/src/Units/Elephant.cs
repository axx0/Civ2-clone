using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class Elephant : BaseUnit
    {
        public Elephant() : base(40, 4, 1, 1, 1, 2)
        {
            Type = UnitType.Elephant;
            LSA = UnitLSA.Land;
            Name = "Elephant";
        }
    }
}
