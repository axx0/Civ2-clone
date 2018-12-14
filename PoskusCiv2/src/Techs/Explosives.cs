using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Explosives : BaseTech
    {
        public Explosives() : base(5, 0, TechType.Gunpowder, TechType.Chemistry, 2, 4)
        {
            Type = TechType.Explosives;
            Name = "Explosives";
        }
    }
}
