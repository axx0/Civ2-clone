using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class Explorer : BaseUnit
    {
        public Explorer() : base(30, 0, 1, 1, 1, 1)
        {
            Type = UnitType.Explorer;
            GAS = UnitGAS.Air;
            Name = "Explorer";
        }
    }
}
