using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class Caravel : BaseUnit
    {
        public Caravel() : base(40, 2, 1, 1, 1, 3)
        {
            Type = UnitType.Caravel;
            GAS = UnitGAS.Air;
            Name = "Caravel";
        }
    }
}
