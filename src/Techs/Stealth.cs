using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Stealth : BaseTech
    {
        public Stealth() : base(3, -2, TechType.Supercond, TechType.Robotics, 3, 0)
        {
            Type = TechType.Stealth;
            Name = "Stealth";
        }
    }
}
