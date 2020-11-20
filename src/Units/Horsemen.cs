using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class Horsemen : BaseUnit
    {
        public Horsemen() : base(20, 2, 1, 1, 1, 2)
        {
            Type = UnitType.Horsemen;
            GAS = UnitGAS.Air;
            Name = "Horsemen";
        }
    }
}
