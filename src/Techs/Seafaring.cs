using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Seafaring : BaseTech
    {
        public Seafaring() : base(4, 1, TechType.MapMaking, TechType.Pottery, 0, 1)
        {
            Type = TechType.Seafaring;
            Name = "Seafaring";
        }
    }
}
