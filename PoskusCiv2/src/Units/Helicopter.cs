using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class Helicopter : BaseUnit
    {
        public Helicopter() : base(100, 10, 3, 2, 2, 6)
        {
            Type = UnitType.Helicopter;
            LSA = UnitLSA.Air;
            Name = "Helicopter";
        }
    }
}
