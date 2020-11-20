using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class Howitzer : BaseUnit
    {
        public Howitzer() : base(70, 12, 2, 3, 2, 2)
        {
            Type = UnitType.Howitzer;
            GAS = UnitGAS.Air;
            Name = "Howitzer";
        }
    }
}
