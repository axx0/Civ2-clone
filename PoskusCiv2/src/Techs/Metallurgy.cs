using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Metallurgy : BaseTech
    {
        public Metallurgy() : base(6, -2, TechType.Gunpowder, TechType.University, 1, 0)
        {
            Type = TechType.Metallurgy;
            Name = "Metallurgy";
        }
    }
}
