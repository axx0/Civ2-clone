using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class LaborUnion : BaseTech
    {
        public LaborUnion() : base(4, -1, TechType.MassProd, TechType.GuerillaWar, 3, 2)
        {
            Type = TechType.LaborUnion;
            Name = "Labor Union";
        }
    }
}
