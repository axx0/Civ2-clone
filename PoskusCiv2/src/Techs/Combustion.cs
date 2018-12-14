using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Combustion : BaseTech
    {
        public Combustion() : base(5, -1, TechType.Refining, TechType.Explosives, 2, 4)
        {
            Type = TechType.Combustion;
            Name = "Combustion";
        }
    }
}
