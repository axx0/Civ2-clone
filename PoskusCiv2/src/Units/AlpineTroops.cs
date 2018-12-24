using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class AlpineTroops : BaseUnit
    {
        public AlpineTroops() : base(50, 5, 5, 2, 1, 1)
        {
            Type = UnitType.AlpineTroops;
            GAS = UnitGAS.Air;
            Name = "Alpine Troops";
        }
    }
}
