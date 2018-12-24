using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class Cannon : BaseUnit
    {
        public Cannon() : base(40, 8, 1, 2, 1, 1)
        {
            Type = UnitType.Cannon;
            GAS = UnitGAS.Air;
            Name = "Cannon";
        }
    }
}
