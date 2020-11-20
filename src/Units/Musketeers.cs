using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class Musketeers : BaseUnit
    {
        public Musketeers() : base(30, 3, 3, 2, 1, 1)
        {
            Type = UnitType.Musketeers;
            GAS = UnitGAS.Air;
            Name = "Musketeers";
        }
    }
}
