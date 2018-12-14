using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Automobile : BaseTech
    {
        public Automobile() : base(6, -1, TechType.Combustion, TechType.Steel, 3, 4)
        {
            Type = TechType.Automobile;
            Name = "Automobile";
        }
    }
}
