using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Miniaturiz : BaseTech
    {
        public Miniaturiz() : base(4, 1, TechType.MachineTools, TechType.Electronics, 3, 4)
        {
            Type = TechType.Miniaturiz;
            Name = "Miniaturization";
        }
    }
}
