using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class CruiseMsl : BaseUnit
    {
        public CruiseMsl() : base(60, 18, 0, 1, 3, 12)
        {
            Type = UnitType.CruiseMsl;
            GAS = UnitGAS.Air;
            Name = "Cruise Msl.";
        }
    }
}
