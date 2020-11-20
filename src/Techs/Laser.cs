using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Laser : BaseTech
    {
        public Laser() : base(4, 0, TechType.NuclearPwr, TechType.MassProd, 3, 3)
        {
            Type = TechType.Laser;
            Name = "The Laser";
        }
    }
}
