using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Electricity : BaseTech
    {
        public Electricity() : base(4, 0, TechType.Metallurgy, TechType.Magnetism, 2, 4)
        {
            Type = TechType.Electricity;
            Name = "Electricity";
        }
    }
}
