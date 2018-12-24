using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class Carrier : BaseUnit
    {
        public Carrier() : base(160, 1, 9, 4, 2, 5)
        {
            Type = UnitType.Carrier;
            GAS = UnitGAS.Air;
            Name = "Carrier";
        }
    }
}
