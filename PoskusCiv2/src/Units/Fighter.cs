using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class Fighter : BaseUnit
    {
        public Fighter() : base(60, 4, 3, 2, 2, 10)
        {
            Type = UnitType.Fighter;
            LSA = UnitLSA.Air;
            Name = "Fighter";
        }
    }
}
