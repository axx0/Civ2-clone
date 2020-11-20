using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class Pikemen : BaseUnit
    {
        public Pikemen() : base(20, 1, 2, 1, 1, 1)
        {
            Type = UnitType.Pikemen;
            GAS = UnitGAS.Air;
            Name = "Pikemen";
        }
    }
}
