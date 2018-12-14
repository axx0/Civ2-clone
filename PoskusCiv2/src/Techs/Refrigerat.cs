using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Refrigerat : BaseTech
    {
        public Refrigerat() : base(3, 1, TechType.Electricity, TechType.Sanitation, 3, 1)
        {
            Type = TechType.Refrigerat;
            Name = "Refrigeration";
        }
    }
}
