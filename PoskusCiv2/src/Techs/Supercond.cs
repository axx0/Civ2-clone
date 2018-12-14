using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Supercond : BaseTech
    {
        public Supercond() : base(4, 1, TechType.Plastics, TechType.Laser, 3, 3)
        {
            Type = TechType.Supercond;
            Name = "Superconductor";
        }
    }
}
